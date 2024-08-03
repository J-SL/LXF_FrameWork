using LXF_Framework.MonoYield;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace LXF_Framework
{
    namespace DependencyInjection
    {
        [DefaultExecutionOrder(-1000)]
        public class LXF_Injector : LXF_Singleton<LXF_Injector>
        {
            const BindingFlags k_bindingFlags =  BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

            readonly Dictionary<Type, object> registry_Method = new();
            readonly Dictionary<string, GameObject> registry_targetObject = new();
            readonly Dictionary <Type, object> registry_singleMonoList = new();

            protected override void Awake()
            {
                base.Awake();

                //Find all modules implementing IDependencyProvider
                var prviders = FindMonoBehaviors().OfType<IDependencyProvider>();
                foreach (var provider in prviders)
                {
                    RegisterProvider(provider);
                }

                //ensure that there is only one instance of LXF_Provider in scene
                var providersCount = FindObjectsByType<LXF_Provider>(FindObjectsSortMode.None).Count();
                if (providersCount > 1 || providersCount == 0)
                {
                    throw new Exception("There should be only one instance of LXF_Provider in scene");
                }


                //Find all injectable objects and inject thier dependencies
                var injectables = FindMonoBehaviors().Where(IsInjectable);

                foreach (var injectable in injectables)
                {
                    Inject(injectable);
                }
            }

            public void Inject(LXF_MonoYield injectable)
            {
                var type = injectable.GetType();
                var gameObject = injectable.gameObject;

                //inject fields
                var injectableFields = type.GetFields(k_bindingFlags).
                    Where(m => Attribute.IsDefined(m, typeof(LXF_InjectAttribute)));

                foreach (var field in injectableFields)
                {
                    var fieldType = field.FieldType;

                    var attribute = field.GetCustomAttribute<LXF_InjectAttribute>();

                    var instance = GetInstance(fieldType, attribute ,gameObject)
                        ?? throw new Exception($"Cannot get instance dependency of type {fieldType.Name} for {type.Name}.{field.Name}");

                    field.SetValue(injectable, instance);

                    Debug.Log($"Field: Injected {fieldType.Name} to {type.Name}.{field.Name}");
                }

                // Inject properties
                var injectableProperties = type.GetProperties(k_bindingFlags)
                    .Where(p => Attribute.IsDefined(p, typeof(LXF_InjectAttribute)) && p.CanWrite);

                foreach (var property in injectableProperties)
                {
                    var propertyType = property.PropertyType;

                    var attribute = property.GetCustomAttribute<LXF_InjectAttribute>();

                    var instance = GetInstance(propertyType, attribute,gameObject)
                        ?? throw new Exception($"Cannot get instance dependency of type {propertyType.Name} for {type.Name}.{property.Name}");
                    
                    property.SetValue(injectable, instance);
                    
                    Debug.Log($"Property: Injected {propertyType.Name} to {type.Name}.{property.Name}");
                }

                //inject methods
                var injectableMethods = type.GetMethods(k_bindingFlags).
                    Where(m => Attribute.IsDefined(m, typeof(LXF_InjectAttribute)));

                foreach (var method in injectableMethods)
                {
                    var parameters = method.GetParameters().Select(p => p.ParameterType);

                    var attribute = method.GetCustomAttribute<LXF_InjectAttribute>();

                    var resolvedInstances = parameters.Select(t => GetInstance
                    (t, attribute,gameObject)).ToArray();

                    if (resolvedInstances.Any(i => i == null))
                    {
                        throw new Exception($"Cannot get instance dependencies of {type.Name}.{method.Name}");
                    }

                    method.Invoke(injectable, resolvedInstances);
                    Debug.Log($"Method: Injected dependencies to {type.Name}.{method.Name}");
                }
            }

            object GetInstance(Type type,LXF_InjectAttribute attribute ,GameObject gameObject)
            {
                switch (attribute.InjectionMode)
                {
                    case InjectionMode.NormalClass:
                        registry_Method.TryGetValue(type, out var instance);
                        return instance;
                    case InjectionMode.Self:
                        return gameObject.GetComponent(type);
                    case InjectionMode.TargetObject:
                        registry_targetObject.TryGetValue(attribute.ObjectName, out var targetObject);
                        var component = targetObject.GetComponent(type);
                        return component;
                    case InjectionMode.SingleMono:
                            registry_singleMonoList.TryGetValue(type, out var singleMono);
                            return singleMono;
                    default:
                        throw new Exception($"Injection mode {attribute.InjectionMode} is not supported");
                }
            }

            static bool IsInjectable(LXF_MonoYield obj)
            {
                var members = obj.GetType().GetMembers(k_bindingFlags);

                return members.Any(m => Attribute.IsDefined(m, typeof(LXF_InjectAttribute)));
            }

            private void RegisterProvider(IDependencyProvider provider)
            {
                var fields = provider.GetType().GetFields(k_bindingFlags);
                foreach (var field in fields)
                {
                    if (!Attribute.IsDefined(field, typeof(LXF_ProvideAttribute))) continue;

                    var attribute = field.GetCustomAttribute<LXF_ProvideAttribute>();

                    if(attribute.ProvideMode == ProvideMode.SingleMonoList)
                    {
                        var singleMonoList = field.GetValue(provider) as IEnumerable;
                        foreach(var s in singleMonoList)
                        {
                            registry_singleMonoList.Add(s.GetType(), s);
                            Debug.Log($"Provider {s.GetType().Name} from {provider.GetType().Name}");
                        }
                    }                  

                    if (attribute.ProvideMode == ProvideMode.GameObject)
                    {
                        var providedInstance = field.GetValue(provider);

                        if (providedInstance != null)
                        {
                            registry_targetObject.Add(field.Name, providedInstance as GameObject);
                            Debug.Log($"Provider {field.Name} from {provider.GetType().Name}");
                        }
                        else
                        {
                            throw new Exception($"Provider {provider.GetType().Name} return null for : {field.Name}");
                        }
                    }
                    
                }

                var methods = provider.GetType().GetMethods(k_bindingFlags);
                foreach (var method in methods)
                {
                    if(!Attribute.IsDefined(method, typeof(LXF_ProvideAttribute))) continue;
                    
                    var attribute = method.GetCustomAttribute<LXF_ProvideAttribute>();

                    if(!(attribute.ProvideMode == ProvideMode.Method))
                    {
                        throw new Exception($"Provider {provider.GetType().Name} provide mode {attribute.ProvideMode} is not supported");
                    }

                    var returnType = method.ReturnType;

                    var providedInstance = method.Invoke(provider, null);

                    if(providedInstance != null)
                    {
                        registry_Method.Add(returnType, providedInstance);
                        Debug.Log($"Provider {returnType.Name} from {provider.GetType().Name}");
                    }
                    else
                    {
                        throw new Exception($"Provider {provider.GetType().Name} return null for : {returnType.Name}");
                    }
                }            
            }

            static LXF_MonoYield[] FindMonoBehaviors()
            {
                return FindObjectsByType<LXF_MonoYield>(FindObjectsSortMode.InstanceID);
            }
        }
    }   
}



