using Cysharp.Threading.Tasks;
using LXF_Framework.DependencyInjection;
using LXF_Framework.MonoYield;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;

namespace LXF_Framework
{
    namespace FrameworkCore
    {     
        public abstract class LXFMVC_Controller : LXF_MonoYield
        {
            [LXF_Inject(InjectionMode.NormalClass)]
            protected readonly LXF_DataWriter Writer;
        }

        public abstract class LXFMVC_View: LXF_MonoYield
        {
            [LXF_Inject(InjectionMode.NormalClass)]
            protected readonly LXF_DataReader Reader;
        }


        /// <summary>
        /// This class is a base class for all data container classes. It provides a way to access the data container singletons.
        /// You can create your own data container class by inheriting from this class and implementing the abstract methods.
        /// </summary>
        public abstract class LXF_DataContainer<T, TContainer>  where T : LXF_DataContainer<T, TContainer>
            where TContainer : LXF_DataContainer<T, TContainer>.BaseDataContainer, new()
        {
            public TContainer GetContainerSingleton() => Singleton<TContainer>.Instance;
            public TProvider GetDataProviderSingleton<TProvider>() where TProvider : BaseDataContainer.LXF_DataProvider, new() =>
                Singleton<TProvider>.Instance;
            public TController GetDataControllerSingleton<TController>() where TController : BaseDataContainer.LXF_DataController, new() =>
                Singleton<TController>.Instance;

            public abstract class BaseDataContainer
            {
                public abstract class LXF_DataProvider
                {
                    protected TContainer container;

                    public LXF_DataProvider() => container = SingletonWithParams<T>.Instance().GetContainerSingleton();
                }

                
                public abstract class LXF_DataController
                {
                    protected TContainer container;

                    public LXF_DataController() => container = SingletonWithParams<T>.Instance().GetContainerSingleton();
                
                }
                      
            }          
        }
                   

        public partial class LXF_DataReader
        {             
        
        }

        public partial class LXF_DataWriter
        {
             
        }

        public static class CoreUtility
        {
            public static (List<LXF_MonoYield>, List<IInjectable>) FindScriptsInScene()
            {
                List<LXF_MonoYield> monoYieldScripts = new List<LXF_MonoYield>();
                List<IInjectable> injectableScripts = new List<IInjectable>();

                GameObject[] rootGameObjects = SceneManager.GetActiveScene().GetRootGameObjects();

                foreach (GameObject root in rootGameObjects)
                {
                    FindScriptsInGameObject(root, monoYieldScripts, injectableScripts);
                }

                return (monoYieldScripts, injectableScripts);
            }

            private static void FindScriptsInGameObject(GameObject obj, List<LXF_MonoYield> monoYieldScripts, List<IInjectable> injectableScripts)
            {
                MonoBehaviour[] monos = obj.GetComponentsInChildren<MonoBehaviour>(true);

                foreach (var mono in monos)
                {                  
                    if (mono is LXF_MonoYield monoYield)
                    {
                        monoYieldScripts.Add(monoYield);
                    }

                    if (mono is IInjectable injectable)
                    {
                        injectableScripts.Add(injectable);
                    }
                }           
            }
        }
    }

    namespace TweenSystem
    {
        public enum TweenMode
        {
            Null,
            Fixed,
        }

        /// <summary>
        /// this class is base class apply to child script to added gameObject
        /// </summary>
        public class LXF_TimerManage : MonoBehaviour
        {
            public static LXF_TimerManage Instance { get; protected set; }

            private void Awake()
            {
                Instance ??= this;
                if (Instance != this)
                {
                    Destroy(Instance);
                    Instance = this;
                }

                DontDestroyOnLoad(gameObject);
            }

            public void StartCoroutineHelper(IEnumerator routine) => StartCoroutine(routine);


            //可以利用这个做一个按钮动画，当连点按钮时可以不断的开启/终止动画，不过今天太累了下次再做吧
            public void StopTargetCoroutine(Coroutine c) => StopCoroutine(c);

            public TweenQueueExecutor CreatTweenQueue(float waitTime) => new TweenQueueExecutor(waitTime);
            public void StartTweenQueue(TweenQueueExecutor tweenQueue) => tweenQueue.TweenCoroutine = StartCoroutine(tweenQueue.TweenQueue());

            public void StartTimerCoroutine(float targetTime, Action callback, Action<bool> isDone) =>
                StartCoroutine(LXF_Timer.Timer(targetTime, callback, isDone));
            public void StartTimerCoroutine<T>(float targetTime, Action<T> callback, Action<bool> isDone) =>
                StartCoroutine(LXF_Timer.Timer(targetTime, callback, isDone));
            public void StartTimerInRealTimeCoroutine(float targetTime, Action callback, Action<bool> isDone) =>
                StartCoroutine(LXF_Timer.TimerInRealTime(targetTime, callback, isDone));
            public void StartTweenListFloatTimerCoroutine_Standard(TweenListFloatTimer tween, Func<float, float, float, float> lerpType) =>
                StartCoroutine(tween.LerpValueStandard(lerpType));
            public void StartTweenListFloatTimerCoroutine(TweenListFloatTimer tween, Func<float, float, float, float> lerpType) =>
               StartCoroutine(tween.LerpValue(lerpType));
            public Coroutine StartTweenFloatTimerCoroutine(TweenFloatTimer tweener, Func<float, float, float, float> lerpType) =>
                StartCoroutine(tweener.LerpValue(lerpType));
            public void StartTweenIntTimerCoroutine(TweenIntTimer tweener, Func<int, int, float, float> lerpType) =>
               StartCoroutine(tweener.LerpValue(lerpType));
            public void StartTweenQuaternionTimerCoroutine(TweenQuaternionTimer tweener, Func<Quaternion, Quaternion, float, Quaternion> lerpType) =>
               StartCoroutine(tweener.LerpValue(lerpType));
            public void StartTweenVector2TimerCoroutine(TweenVector2Timer tweener, Func<Vector2, Vector2, float, Vector2> lerpType) =>
                StartCoroutine(tweener.LerpValue(lerpType));
            public void StartTweenVector2TimerCoroutine_Speed(TweenVector2Timer tweener, Func<Vector2, Vector2, float, Vector2> lerpType) =>
                StartCoroutine(tweener.LerpValue_Speed(lerpType));
            public void StartTweenVector3TimerCoroutine(TweenVector3Timer tweener, Func<Vector3, Vector3, float, Vector3> lerpType, TweenMode t = TweenMode.Null)
            {
                switch (t)
                {
                    case TweenMode.Null:
                        StartCoroutine(tweener.LerpValue(lerpType));
                        break;
                    case TweenMode.Fixed:
                        StartCoroutine(tweener.LerpValueFixed(lerpType));
                        break;
                    default:
                        StartCoroutine(tweener.LerpValue(lerpType));
                        break;
                }
            }

            public void StartCrossBoolenCoroutine(TweenBoolenTimer tween) => StartCoroutine(tween.CrossBoolen());

        }

        //这个类还待完善，可以考虑支持固定长度的队列，亦或是不同的时间间隔
        public class TweenQueueExecutor
        {
            float waitTime;
            private Queue<Action> Tweens = new();
            public Coroutine TweenCoroutine { get; set; }
            public TweenQueueExecutor(float waitTime)
            {
                this.waitTime = waitTime;
            }

            public void ExecuteQueue(Action action)
            {
                Tweens.Enqueue(action);
                if (TweenCoroutine is null)
                    LXF_TimerManage.Instance.StartTweenQueue(this);
            }


            public IEnumerator TweenQueue()
            {
                while (true)
                {
                    if (Tweens.Count == 0)
                    {
                        TweenCoroutine = null;
                        yield break;
                    }
                    Tweens.Dequeue()?.Invoke();
                    yield return new WaitForSeconds(waitTime);
                }
            }
        }


        #region TweenValue
        public static class TweenListFloat
        {
            public static void Tween(Action<List<float>> valueSetter, List<float> valueList, List<float> endValueList, float duration)
            {
                var c = new TweenListFloatTimer(valueSetter, valueList, endValueList, duration);
                LXF_TimerManage.Instance.StartTweenListFloatTimerCoroutine_Standard(c, (start, end, t) => Mathf.Lerp(start, end, t));
            }
            public static void Tween(Action<List<float>> valueSetter, List<float> valueList, float endValue, float duration)
            {
                var c = new TweenListFloatTimer(valueSetter, valueList, endValue, duration);
                LXF_TimerManage.Instance.StartTweenListFloatTimerCoroutine(c, (start, end, t) => Mathf.Lerp(start, end, t));
            }
            public static void Tween(Action<List<float>> valueSetter, List<float> valueList, List<float> endValueList, float duration, Action callback)
            {
                var c = new TweenListFloatTimer(valueSetter, valueList, endValueList, duration, callback);
                LXF_TimerManage.Instance.StartTweenListFloatTimerCoroutine_Standard(c, (start, end, t) => Mathf.Lerp(start, end, t));
            }
            public static void Tween(Action<List<float>> valueSetter, List<float> valueList, float endValue, float duration, Action callback)
            {
                var c = new TweenListFloatTimer(valueSetter, valueList, endValue, duration, callback);
                LXF_TimerManage.Instance.StartTweenListFloatTimerCoroutine(c, (start, end, t) => Mathf.Lerp(start, end, t));
            }
            public static void SmoothTween(Action<List<float>> valueSetter, List<float> valueList, List<float> endValueList, float duration)
            {
                var c = new TweenListFloatTimer(valueSetter, valueList, endValueList, duration);
                LXF_TimerManage.Instance.StartTweenListFloatTimerCoroutine_Standard(c, (start, end, t) => Mathf.SmoothStep(start, end, t));
            }
            public static void SmoothTween(Action<List<float>> valueSetter, List<float> valueList, List<float> endValueList, float duration, Action callback)
            {
                var c = new TweenListFloatTimer(valueSetter, valueList, endValueList, duration, callback);
                LXF_TimerManage.Instance.StartTweenListFloatTimerCoroutine_Standard(c, (start, end, t) => Mathf.SmoothStep(start, end, t));
            }
        }
        public class TweenListFloatTimer
        {
            private Action<List<float>> valueSetter;
            private List<float> valueList;
            private List<float> endValueList;
            private float endValue;
            private float duration;

            public bool IsDone { get; private set; }
            public event EventHandler OnEndTweening;
            public TweenListFloatTimer(Action<List<float>> valueSetter, List<float> valueList, float endValue, float duration)
            {
                this.valueSetter = valueSetter;
                this.valueList = valueList;
                this.endValue = endValue;
                this.duration = duration;

                IsDone = false;
            }
            public TweenListFloatTimer(Action<List<float>> valueSetter, List<float> valueList, List<float> endValueList, float duration)
            {
                this.valueSetter = valueSetter;
                this.valueList = valueList;
                this.endValueList = endValueList;
                this.duration = duration;

                IsDone = false;
            }
            public TweenListFloatTimer(Action<List<float>> valueSetter, List<float> valueList, float endValue, float duration, Action callback)
            {
                this.valueSetter = valueSetter;
                this.valueList = valueList;
                this.endValue = endValue;
                this.duration = duration;
                OnEndTweening += (sender, args) => callback.Invoke();

                IsDone = false;
            }
            public TweenListFloatTimer(Action<List<float>> valueSetter, List<float> valueList, List<float> endValueList, float duration, Action callback)
            {
                this.valueSetter = valueSetter;
                this.valueList = valueList;
                this.endValueList = endValueList;
                this.duration = duration;
                OnEndTweening += (sender, args) => callback.Invoke();

                IsDone = false;
            }

            public IEnumerator LerpValue(Func<float, float, float, float> lerpType)
            {
                var elapsedTime = 0f;

                while (elapsedTime < duration)
                {
                    var l = new List<float>();
                    foreach (var value in valueList)
                    {
                        var v = lerpType(value, endValue, elapsedTime / duration);
                        l.Add(v);
                    }
                    valueSetter(l);
                    elapsedTime += Time.deltaTime;
                    yield return null;
                }
                Debug.Log("elapsedTime: " + elapsedTime);

                valueSetter(endValueList);

                IsDone = true;
                OnEndTweening?.Invoke(this, EventArgs.Empty);
            }

            public IEnumerator LerpValueStandard(Func<float, float, float, float> lerpType)
            {
                var elapsedTime = 0f;
                if (valueList.Count != endValueList.Count)
                    throw new ArgumentException("the valueList count not equal endValueList!");
                while (elapsedTime < duration)
                {
                    var l = new List<float>();
                    var index = 0;
                    foreach (var value in valueList)
                    {
                        var v = lerpType(value, endValueList[index], elapsedTime / duration);
                        index++;
                        l.Add(v);
                    }
                    valueSetter(l);
                    elapsedTime += Time.deltaTime;
                    yield return null;
                }
                Debug.Log("elapsedTime: " + elapsedTime);

                valueSetter(endValueList);

                IsDone = true;
                OnEndTweening?.Invoke(this, EventArgs.Empty);
            }
        }

        public static class TweenFloat
        {
            public static Coroutine Tween(Action<float> valueSetter, float value, float endValue, float duration)
            {
                var c = new TweenFloatTimer(valueSetter, value, endValue, duration);
                return LXF_TimerManage.Instance.StartTweenFloatTimerCoroutine(c, (start, end, t) => Mathf.Lerp(start, end, t));
            }
            public static void Tween(Action<float> valueSetter, float value, float endValue, float duration, Action callback)
            {
                var c = new TweenFloatTimer(valueSetter, value, endValue, duration, callback);
                LXF_TimerManage.Instance.StartTweenFloatTimerCoroutine(c, (start, end, t) => Mathf.Lerp(start, end, t));
            }
            public static void SmoothTween(Action<float> valueSetter, float value, float endValue, float duration)
            {
                var c = new TweenFloatTimer(valueSetter, value, endValue, duration);
                LXF_TimerManage.Instance.StartTweenFloatTimerCoroutine(c, (start, end, t) => Mathf.SmoothStep(start, end, t));
            }
            public static void SmoothTween(Action<float> valueSetter, float value, float endValue, float duration, Action callback)
            {
                var c = new TweenFloatTimer(valueSetter, value, endValue, duration, callback);
                LXF_TimerManage.Instance.StartTweenFloatTimerCoroutine(c, (start, end, t) => Mathf.SmoothStep(start, end, t));
            }
        }
        public class TweenFloatTimer
        {
            private Action<float> valueSetter;
            private float value;
            private float endValue;
            private float duration;
            private float elapsedTime;

            public bool IsDone { get; private set; }
            public event EventHandler OnEndTweening;

            public TweenFloatTimer(Action<float> valueSetter, float value, float endValue, float duration)
            {
                this.valueSetter = valueSetter;
                this.value = value;
                this.endValue = endValue;
                this.duration = duration;
                elapsedTime = 0f;

                IsDone = false;
            }

            public TweenFloatTimer(Action<float> valueSetter, float value, float endValue, float duration, Action callback)
            {
                this.valueSetter = valueSetter;
                this.value = value;
                this.endValue = endValue;
                this.duration = duration;
                elapsedTime = 0f;
                OnEndTweening += (sender, args) => callback.Invoke();

                IsDone = false;
            }

            public IEnumerator LerpValue(Func<float, float, float, float> lerpType)
            {

                while (elapsedTime < duration)
                {
                    var v = lerpType(value, endValue, elapsedTime / duration);
                    valueSetter(v);
                    elapsedTime += Time.deltaTime;
                    yield return null;
                }
                //Debug.Log("elapsedTime: " + elapsedTime);
                elapsedTime = 0f;
                valueSetter(endValue);

                IsDone = true;
                OnEndTweening?.Invoke(this, EventArgs.Empty);
            }
            public TweenFloatTimer(Action<float> valueSetter, float value, float endValue)
            {
                this.valueSetter = valueSetter;
                this.value = value;
                this.endValue = endValue;
                elapsedTime = 0f;

                IsDone = false;
            }
            public TweenFloatTimer(Action<float> valueSetter, float value, float endValue, Action callback)
            {
                this.valueSetter = valueSetter;
                this.value = value;
                this.endValue = endValue;
                elapsedTime = 0f;
                OnEndTweening += (sender, args) => callback.Invoke();

                IsDone = false;
            }

            public IEnumerator FrameValue()
            {
                if (value > endValue)
                {
                    while (value > endValue)
                    {
                        value -= Time.deltaTime;
                        yield return null;
                    }
                    value = endValue;

                    OnEndTweening?.Invoke(this, EventArgs.Empty);
                    yield break;
                }
                else if (value < endValue)
                {
                    while (value < endValue)
                    {
                        value += Time.deltaTime;
                        yield return null;
                    }

                    value = endValue;

                    OnEndTweening?.Invoke(this, EventArgs.Empty);
                    yield break;
                }
                yield break;
            }
        }

        public static class TweenBoolen
        {
            public static void Tween(Action<bool> valueSetter, float interval, float duration)
            {
                var c = new TweenBoolenTimer(valueSetter, interval, duration);
                LXF_TimerManage.Instance.StartCrossBoolenCoroutine(c);
            }
            public static void Tween(Action<bool> valueSetter, float interval, float duration, Action callback)
            {
                var c = new TweenBoolenTimer(valueSetter, interval, duration, callback);
                LXF_TimerManage.Instance.StartCrossBoolenCoroutine(c);
            }
        }
        public class TweenBoolenTimer
        {
            private Action<bool> valueSetter;
            float interval;
            private float duration;
            private float elapsedTime;

            public bool IsDone { get; private set; }
            public event EventHandler OnEndTweening;

            public TweenBoolenTimer(Action<bool> valueSetter, float interval, float duration)
            {
                this.valueSetter = valueSetter;
                this.interval = interval;
                this.duration = duration;
                elapsedTime = 0f;

                IsDone = false;
            }

            public TweenBoolenTimer(Action<bool> valueSetter, float interval, float duration, Action callback)
            {
                this.valueSetter = valueSetter;
                this.duration = duration;
                elapsedTime = 0f;
                OnEndTweening += (sender, args) => callback.Invoke();

                IsDone = false;
            }

            public IEnumerator CrossBoolen()
            {
                var boolen = false;
                while (elapsedTime < duration)
                {
                    yield return new WaitForSeconds(interval);
                    boolen = !boolen;
                    valueSetter(boolen);
                }
                elapsedTime = 0f;

                IsDone = true;
                OnEndTweening?.Invoke(this, EventArgs.Empty);
            }
        }

        public static class TweenVector2
        {
            public static void Tween(Action<Vector2> valueSetter, Vector2 value, Vector2 endValue, float duration)
            {
                var c = new TweenVector2Timer(valueSetter, value, endValue, duration);
                LXF_TimerManage.Instance.StartTweenVector2TimerCoroutine(c, (start, end, t) =>
                    new Vector2(Mathf.Lerp(start.x, end.x, t), Mathf.Lerp(start.y, end.y, t)));
            }
            public static void Tween_Speed(Action<Vector2> valueSetter, Vector2 value, Vector2 endValue, float speed)
            {
                var c = new TweenVector2Timer();
                c.Init(valueSetter, value, endValue, speed);
                LXF_TimerManage.Instance.StartTweenVector2TimerCoroutine_Speed(c, (start, end, t) =>
                    new Vector2(Mathf.SmoothStep(start.x, end.x, t), Mathf.SmoothStep(start.y, end.y, t)));
            }
            public static void Tween(Action<Vector2> valueSetter, Vector2 value, Vector2 endValue, float duration, Action callback)
            {
                var c = new TweenVector2Timer(valueSetter, value, endValue, duration, callback);
                LXF_TimerManage.Instance.StartTweenVector2TimerCoroutine(c, (start, end, t) =>
                    new Vector2(Mathf.Lerp(start.x, end.x, t), Mathf.Lerp(start.y, end.y, t)));
            }
            public static void SmoothTween(Action<Vector2> valueSetter, Vector2 value, Vector2 endValue, float duration)
            {
                var c = new TweenVector2Timer(valueSetter, value, endValue, duration);
                LXF_TimerManage.Instance.StartTweenVector2TimerCoroutine(c, (start, end, t) =>
                    new Vector2(Mathf.Lerp(start.x, end.x, t), Mathf.SmoothStep(start.y, end.y, t)));
            }
            public static void SmoothTween(Action<Vector2> valueSetter, Vector2 value, Vector2 endValue, float duration, Action callback)
            {
                var c = new TweenVector2Timer(valueSetter, value, endValue, duration, callback);
                LXF_TimerManage.Instance.StartTweenVector2TimerCoroutine(c, (start, end, t) =>
                    new Vector2(Mathf.Lerp(start.x, end.x, t), Mathf.SmoothStep(start.y, end.y, t)));
            }
        }
        public class TweenVector2Timer
        {
            private Action<Vector2> valueSetter;
            private Vector2 value;
            private Vector2 endValue;
            private float duration;
            private float speed;
            private float elapsedTime;

            public bool IsDone { get; private set; }
            public event EventHandler OnEndTweening;

            public TweenVector2Timer() { }

            public void Init(Action<Vector2> valueSetter, Vector2 value, Vector2 endValue, float speed)
            {
                this.valueSetter = valueSetter;
                this.value = value;
                this.endValue = endValue;
                this.speed = speed;
                elapsedTime = 0f;

                IsDone = false;
            }

            public IEnumerator LerpValue_Speed(Func<Vector2, Vector2, float, Vector2> lerpType)
            {

                while (value != endValue)
                {
                    var v = lerpType(value, endValue, speed);
                    valueSetter(v);
                    value = v;
                    yield return null;
                }
                Debug.Log("elapsedTime: " + elapsedTime);
                elapsedTime = 0f;
                valueSetter(endValue);

                IsDone = true;
                OnEndTweening?.Invoke(this, EventArgs.Empty);
            }


            public TweenVector2Timer(Action<Vector2> valueSetter, Vector2 value, Vector2 endValue, float duration)
            {
                this.valueSetter = valueSetter;
                this.value = value;
                this.endValue = endValue;
                this.duration = duration;
                elapsedTime = 0f;

                IsDone = false;
            }

            public TweenVector2Timer(Action<Vector2> valueSetter, Vector2 value, Vector2 endValue, float duration, Action callback)
            {
                this.valueSetter = valueSetter;
                this.value = value;
                this.endValue = endValue;
                this.duration = duration;
                elapsedTime = 0f;
                OnEndTweening += (sender, args) => callback.Invoke();

                IsDone = false;
            }

            public IEnumerator LerpValue(Func<Vector2, Vector2, float, Vector2> lerpType)
            {

                while (elapsedTime < duration)
                {
                    var v = lerpType(value, endValue, elapsedTime / duration);
                    valueSetter(v);
                    elapsedTime += Time.deltaTime;
                    yield return null;
                }
                //Debug.Log("elapsedTime: " + elapsedTime);
                elapsedTime = 0f;
                valueSetter(endValue);

                IsDone = true;
                OnEndTweening?.Invoke(this, EventArgs.Empty);
            }

        }

        public static class TweenVector3
        {
            public static void Tween(Action<Vector3> valueSetter, Vector3 value, Vector3 endValue, float duration, TweenMode t = TweenMode.Null)
            {
                var c = new TweenVector3Timer(valueSetter, value, endValue, duration);
                LXF_TimerManage.Instance.StartTweenVector3TimerCoroutine(c, (start, end, t) =>
                    new Vector3(Mathf.Lerp(start.x, end.x, t), Mathf.Lerp(start.y, end.y, t), Mathf.Lerp(start.z, end.z, t)), t);
            }
            public static void Tween(Action<Vector3> valueSetter, Vector3 value, Vector3 endValue, float duration, Action callback, TweenMode t = TweenMode.Null)
            {
                var c = new TweenVector3Timer(valueSetter, value, endValue, duration, callback);
                LXF_TimerManage.Instance.StartTweenVector3TimerCoroutine(c, (start, end, t) =>
                    new Vector3(Mathf.Lerp(start.x, end.x, t), Mathf.Lerp(start.y, end.y, t), Mathf.Lerp(start.z, end.z, t)), t);
            }
            public static void SmoothTween(Action<Vector3> valueSetter, Vector3 value, Vector3 endValue, float duration, TweenMode t = TweenMode.Null)
            {
                var c = new TweenVector3Timer(valueSetter, value, endValue, duration);
                LXF_TimerManage.Instance.StartTweenVector3TimerCoroutine(c, (start, end, t) =>
                    new Vector3(Mathf.Lerp(start.x, end.x, t), Mathf.SmoothStep(start.y, end.y, t), Mathf.SmoothStep(start.z, end.z, t)), t);
            }
            public static void SmoothTween(Action<Vector3> valueSetter, Vector3 value, Vector3 endValue, float duration, Action callback, TweenMode t = TweenMode.Null)
            {
                var c = new TweenVector3Timer(valueSetter, value, endValue, duration, callback);
                LXF_TimerManage.Instance.StartTweenVector3TimerCoroutine(c, (start, end, t) =>
                    new Vector3(Mathf.Lerp(start.x, end.x, t), Mathf.SmoothStep(start.y, end.y, t), Mathf.SmoothStep(start.z, end.z, t)), t);
            }
        }
        public class TweenVector3Timer
        {
            private Action<Vector3> valueSetter;
            private Vector3 value;
            private Vector3 endValue;
            private float duration;
            private float elapsedTime;

            public bool IsDone { get; private set; }
            public event EventHandler OnEndTweening;

            public TweenVector3Timer(Action<Vector3> valueSetter, Vector3 value, Vector3 endValue, float duration)
            {
                this.valueSetter = valueSetter;
                this.value = value;
                this.endValue = endValue;
                this.duration = duration;
                elapsedTime = 0f;

                IsDone = false;
            }

            public TweenVector3Timer(Action<Vector3> valueSetter, Vector3 value, Vector3 endValue, float duration, Action callback)
            {
                this.valueSetter = valueSetter;
                this.value = value;
                this.endValue = endValue;
                this.duration = duration;
                elapsedTime = 0f;
                OnEndTweening += (sender, args) => callback.Invoke();

                IsDone = false;
            }

            public IEnumerator LerpValue(Func<Vector3, Vector3, float, Vector3> lerpType)
            {

                while (elapsedTime < duration)
                {
                    var v = lerpType(value, endValue, elapsedTime / duration);
                    valueSetter(v);
                    elapsedTime += Time.deltaTime;
                    yield return null;
                }
                Debug.Log("elapsedTime: " + elapsedTime);
                elapsedTime = 0f;
                valueSetter(endValue);

                IsDone = true;
                OnEndTweening?.Invoke(this, EventArgs.Empty);
            }

            public IEnumerator LerpValueFixed(Func<Vector3, Vector3, float, Vector3> lerpType)
            {

                while (elapsedTime < duration)
                {
                    var v = lerpType(value, endValue, elapsedTime / duration);
                    valueSetter(v);
                    elapsedTime += Time.deltaTime;
                    yield return new WaitForFixedUpdate();
                }
                Debug.Log("elapsedTime: " + elapsedTime);
                elapsedTime = 0f;
                valueSetter(endValue);

                IsDone = true;
                OnEndTweening?.Invoke(this, EventArgs.Empty);
            }

        }

        public static class TweenInt
        {
            public static void Tween(Action<int> valueSetter, int value, int endValue, float duration)
            {
                var c = new TweenIntTimer(valueSetter, value, endValue, duration);
                LXF_TimerManage.Instance.StartTweenIntTimerCoroutine(c, (start, end, t) => Mathf.Lerp(start, end, t));
            }
            public static void Tween(Action<int> valueSetter, int value, int endValue, float duration, Action callback)
            {
                var c = new TweenIntTimer(valueSetter, value, endValue, duration, callback);
                LXF_TimerManage.Instance.StartTweenIntTimerCoroutine(c, (start, end, t) => Mathf.Lerp(start, end, t));
            }
            public static void SmoothTween(Action<int> valueSetter, int value, int endValue, float duration)
            {
                var c = new TweenIntTimer(valueSetter, value, endValue, duration);
                LXF_TimerManage.Instance.StartTweenIntTimerCoroutine(c, (start, end, t) => Mathf.SmoothStep(start, end, t));
            }
            public static void SmoothTween(Action<int> valueSetter, int value, int endValue, float duration, Action callback)
            {
                var c = new TweenIntTimer(valueSetter, value, endValue, duration, callback);
                LXF_TimerManage.Instance.StartTweenIntTimerCoroutine(c, (start, end, t) => Mathf.SmoothStep(start, end, t));
            }
        }
        public class TweenIntTimer
        {
            private Action<int> valueSetter;
            private int value;
            private int endValue;
            private float duration;
            private float elapsedTime;

            public bool IsDone { get; private set; }
            public event EventHandler OnEndTweening;

            public TweenIntTimer(Action<int> valueSetter, int value, int endValue, float duration)
            {
                this.valueSetter = valueSetter;
                this.value = value;
                this.endValue = endValue;
                this.duration = duration;
                elapsedTime = 0;

                IsDone = false;
            }

            public TweenIntTimer(Action<int> valueSetter, int value, int endValue, float duration, Action callback)
            {
                this.valueSetter = valueSetter;
                this.value = value;
                this.endValue = endValue;
                this.duration = duration;
                elapsedTime = 0f;
                OnEndTweening += (sender, args) => callback.Invoke();

                IsDone = false;
            }

            public IEnumerator LerpValue(Func<int, int, float, float> lerpType)
            {

                while (elapsedTime < duration)
                {
                    var v = Mathf.RoundToInt(lerpType(value, endValue, elapsedTime / duration));
                    valueSetter(v);
                    elapsedTime += Time.deltaTime;
                    yield return null;
                }
                Debug.Log("elapsedTime: " + elapsedTime);
                elapsedTime = 0f;
                valueSetter(endValue);

                IsDone = true;
                OnEndTweening?.Invoke(this, EventArgs.Empty);
            }

        }

        public static class TweenQuaternion
        {
            public static void Tween(Action<Quaternion> valueSetter, Quaternion value, Quaternion endValue, float duration)
            {
                var c = new TweenQuaternionTimer(valueSetter, value, endValue, duration);
                LXF_TimerManage.Instance.StartTweenQuaternionTimerCoroutine(c, (start, end, t) => Quaternion.Lerp(start, end, t));
            }
            public static void Tween(Action<Quaternion> valueSetter, Quaternion value, Quaternion endValue, float duration, Action callback)
            {
                var c = new TweenQuaternionTimer(valueSetter, value, endValue, duration, callback);
                LXF_TimerManage.Instance.StartTweenQuaternionTimerCoroutine(c, (start, end, t) => Quaternion.Lerp(start, end, t));
            }
            public static void SmoothTween(Action<Quaternion> valueSetter, Quaternion value, Quaternion endValue, float duration)
            {
                var c = new TweenQuaternionTimer(valueSetter, value, endValue, duration);
                LXF_TimerManage.Instance.StartTweenQuaternionTimerCoroutine(c, (start, end, t) => Quaternion.Slerp(start, end, t));
            }
            public static void SmoothTween(Action<Quaternion> valueSetter, Quaternion value, Quaternion endValue, float duration, Action callback)
            {
                var c = new TweenQuaternionTimer(valueSetter, value, endValue, duration, callback);
                LXF_TimerManage.Instance.StartTweenQuaternionTimerCoroutine(c, (start, end, t) => Quaternion.Slerp(start, end, t));
            }
        }
        public class TweenQuaternionTimer
        {
            private Action<Quaternion> valueSetter;
            private Quaternion value;
            private Quaternion endValue;
            private float duration;
            private float elapsedTime;

            public bool IsDone { get; private set; }
            public event EventHandler OnEndTweening;

            public TweenQuaternionTimer(Action<Quaternion> valueSetter, Quaternion value, Quaternion endValue, float duration)
            {
                this.valueSetter = valueSetter;
                this.value = value;
                this.endValue = endValue;
                this.duration = duration;
                elapsedTime = 0;

                IsDone = false;
            }

            public TweenQuaternionTimer(Action<Quaternion> valueSetter, Quaternion value, Quaternion endValue, float duration, Action callback)
            {
                this.valueSetter = valueSetter;
                this.value = value;
                this.endValue = endValue;
                this.duration = duration;
                elapsedTime = 0f;
                OnEndTweening += (sender, args) => callback.Invoke();

                IsDone = false;
            }

            public IEnumerator LerpValue(Func<Quaternion, Quaternion, float, Quaternion> lerpType)
            {

                while (elapsedTime < duration)
                {
                    var v = lerpType(value, endValue, elapsedTime / duration);
                    valueSetter(v);
                    elapsedTime += Time.deltaTime;
                    yield return null;
                }
                Debug.Log("elapsedTime: " + elapsedTime);
                elapsedTime = 0f;
                valueSetter(endValue);

                IsDone = true;
                OnEndTweening?.Invoke(this, EventArgs.Empty);

                yield break;
            }

        }
        #endregion

        public static class LXF_Timer
        {
            public static IEnumerator Timer(Action valueSetter, float duration)
            {
                float t = 0;
                while (t < duration)
                {
                    valueSetter?.Invoke();
                    t += Time.deltaTime;
                    yield return null;
                }
            }
            public static IEnumerator Timer(Action valueSetter, float duration, Action callback)
            {
                float t = 0;
                while (t < duration)
                {
                    valueSetter?.Invoke();
                    t += Time.deltaTime;
                    yield return null;
                }
                callback?.Invoke();
            }
            public static IEnumerator Timer(float targetTime, Action callback, Action<bool> isDone)
            {
                yield return new WaitForSeconds(targetTime);
                isDone(true);
                callback?.Invoke();
            }
            public static IEnumerator Timer<T>(float targetTime, Action<T> callback, Action<bool> isDone)
            {
                yield return new WaitForSeconds(targetTime);
                isDone(true);
                callback?.Invoke(default(T));
            }
            public static IEnumerator TimerInRealTime(float targetTime, Action callback, Action<bool> isDone)
            {
                yield return new WaitForSecondsRealtime(targetTime);
                isDone(true);
                callback?.Invoke();
            }
        }
        public static class Invoker
        {
            /// <summary>
            /// 在一段时间内每一帧不断设置value的值
            /// </summary>
            public static void ValueSetterInFrame(Action valueSetter, float duration)
            {
                LXF_TimerManage.Instance.StartCoroutineHelper(LXF_Timer.Timer(valueSetter, duration));
            }
            /// <summary>
            /// 在一段时间内每一帧不断设置value的值
            /// </summary>
            public static void ValueSetterInFrame(Action valueSetter, float duration, Action callback)
            {
                LXF_TimerManage.Instance.StartCoroutineHelper(LXF_Timer.Timer(valueSetter, duration, callback));
            }
            public static void DelayInvoke(float time, Action callback)
            {
                LXF_TimerManage.Instance.StartTimerCoroutine(time, callback, (isDone) => isDone = false);
            }
            public static void DelayInvoke<T>(float time, Action<T> callback)
            {
                LXF_TimerManage.Instance.StartTimerCoroutine(time, callback, (isDone) => isDone = false);
            }
            public static void DelayInvokeRealTime(float time, Action callback)
            {
                LXF_TimerManage.Instance.StartTimerInRealTimeCoroutine(time, callback, (isDone) => isDone = false);
            }
        }
    }

    namespace SaveSystem
    {
        [Serializable]
        public class DataCollection { }
        public interface ISaveLoad
        {
            DataCollection M2D();

            void D2M(DataCollection data);
        }
        [Obsolete]
        public static class Saver
        {
            public static void BinarySave(ISaveLoad save, string dataName)
            {
                string filePath = Application.persistentDataPath + dataName;
                var data = save.M2D();
                BinaryFormatter bf = new BinaryFormatter();
                FileStream fileStream = File.Create(filePath);

                Debug.Log("creat a file to save: " + filePath);

                bf.Serialize(fileStream, data);
                fileStream.Close();
            }
            public static void JsonSave(ISaveLoad save, string dataName)
            {
                string filePath = Application.persistentDataPath + dataName;
                var data = save.M2D();
                string jsonData = JsonUtility.ToJson(data);
                File.WriteAllText(filePath, jsonData);

                Debug.Log("Created a file to save: " + filePath);
            }
        }
        [Obsolete]
        public static class Loader
        {
            public static void BinaryLoad(ISaveLoad load, string dataName)
            {
                string filePath = Application.persistentDataPath + dataName;
                if (File.Exists(filePath))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    FileStream fileStream = File.Open(filePath, FileMode.Open);

                    DataCollection data = bf.Deserialize(fileStream) as DataCollection;
                    load.D2M(data);
                    fileStream.Close();
                }
                else
                {
                    Debug.LogError($"load fail, please check {filePath}");
                }
            }
            public static void BinaryLoad<T>(ISaveLoad load, string dataName) where T : DataCollection
            {
                string filePath = Application.persistentDataPath + dataName;
                if (File.Exists(filePath))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    FileStream fileStream = File.Open(filePath, FileMode.Open);
                    T data = bf.Deserialize(fileStream) as T;
                    load.D2M(data);
                    fileStream.Close();
                }
                else
                {
                    Debug.LogError($"load fail, please check {filePath}");
                }
            }

            public static void JsonLoad<T>(ISaveLoad load, string dataName) where T : DataCollection
            {
                string filePath = Application.persistentDataPath + dataName;
                if (File.Exists(filePath))
                {
                    string jsonData = File.ReadAllText(filePath);
                    T data = JsonUtility.FromJson<T>(jsonData);

                    load.D2M(data);
                }
                else
                {
                    Debug.LogError($"Load failed, please check {filePath}");
                }
            }
        }



        //====================================================================================

        public static class LXF_SaveSystem
        {

            public readonly static string FilePath = Application.persistentDataPath;

            public static void SaveAll(IEnumerable<ISerialization> objs)
            {
                foreach (var obj in objs)
                {
                    Save(obj);
                }
            }

            public static void LoadAll(IEnumerable<ISerialization> objs)
            {
                foreach (var obj in objs)
                {
                    Load(obj);
                }
            }

            public static void Save(ISerialization obj) => obj.Serialize(new LXF_Saver(obj.FileName));

            public static void Load(ISerialization obj) => obj.Deserialize(new LXF_Loader(obj.FileName));           //private static
        }

        public interface ISerialization
        {
            string FileName { get; set; }
            void Serialize(LXF_Saver saver);
            void Deserialize(LXF_Loader loader);
        }

        public class LXF_Saver
        {
            readonly string _filePath;
            public LXF_Saver(string fileName)
            {
                _filePath = LXF_SaveSystem.FilePath + "/" + fileName;

                if (!File.Exists(_filePath))
                {
                    Debug.Log($"{_filePath} not exist, automatically create a new file");
                }
            }
            public Dictionary<string, string> Values { get; private set; } = new();

            public void Add<T>(string valueName, T value) where T : struct
            {
                string json = JsonConvert.SerializeObject(value);
                Values.Add(valueName, json);
            }

            public void Add(string valueName, IntegerMonitor integer)
            {
                string json = JsonConvert.SerializeObject(integer);
                Values.Add(valueName, json);
            }

            public void Add(string valueName, FloatMonitor _float)
            {
                string json = JsonConvert.SerializeObject(_float);
                Values.Add(valueName, json);
            }

            public void Add(string valueName, BoolenMonitor boolen)
            {
                string json = JsonConvert.SerializeObject(boolen);
                Values.Add(valueName, json);
            }

            public void End()
            {
                string jsonString = JsonConvert.SerializeObject(Values, Formatting.Indented);
                File.WriteAllText(_filePath, jsonString);
            }
        }

        public class LXF_Loader
        {

            public LXF_Loader(string fileName)
            {
                string filePath = LXF_SaveSystem.FilePath + "/" + fileName;

                if (File.Exists(filePath))
                {
                    string jsonData = File.ReadAllText(filePath);
                    Values = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonData);
                }
                else
                {
                    Debug.LogError($"Load failed, please check {filePath}");
                }
            }

            private Dictionary<string, string> Values = new();


            private T GetValue<T>(string valueKey) where T : struct
            {
                if (Values.ContainsKey(valueKey))
                {
                    return JsonConvert.DeserializeObject<T>(Values[valueKey]);
                }
                else
                {
                    throw new KeyNotFoundException($"Key '{valueKey}' not found in the dictionary.");
                }
            }

            public void OnLoad<T>(string valueName, ref T value) where T : struct
                => value = GetValue<T>(valueName);
        }

    }

    namespace LoadSceneSystem
    {
        public static class LoadScene
        {
            public static async UniTask LoadTargetScene(int index)
            {
                AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(index);
                asyncOperation.allowSceneActivation = false;

                while (!asyncOperation.isDone)
                {
                    Debug.Log("asyncOperation.progress: " + asyncOperation.progress);
                    if (asyncOperation.progress >= 0.9f)
                    {
                        asyncOperation.allowSceneActivation = true;
                    }
                    await UniTask.Yield();
                }
            }

            public static async UniTask LoadTargetScene(string sceneName)
            {
                AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName);
                asyncOperation.allowSceneActivation = false;

                while (!asyncOperation.isDone)
                {
                    Debug.Log("asyncOperation.progress: " + asyncOperation.progress);
                    if (asyncOperation.progress >= 0.9f)
                    {
                        asyncOperation.allowSceneActivation = true;
                    }
                    await UniTask.Yield();
                }
            }
        }

    }

    namespace DependencyInjection
    {
        public enum InjectionMode
        {
            SingleMono,
            TargetObject,
            NormalClass,
            Self,
            Chilren
        }

        public enum ProvideMode
        {
            GameObject,
            SingleMonoList,
            Method
        }

        [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
        public sealed class LXF_InjectAttribute : Attribute
        {
            public LXF_InjectAttribute(InjectionMode injectionType = InjectionMode.SingleMono)
            {
                InjectionMode = injectionType;
#if UNITY_EDITOR
                switch (injectionType)
                {
                    case InjectionMode.SingleMono:                        
                        break;
                    case InjectionMode.TargetObject:
                        throw new Exception("InjectionMode.TargetObject need a target object name");
                    case InjectionMode.NormalClass:
                        break;
                    case InjectionMode.Self:
                        break;
                    case InjectionMode.Chilren:
                        throw new Exception("InjectionMode.Chilren need a child index with root gameobject");
                }
#endif
            }

            public LXF_InjectAttribute(string targetObjectNameIn_LXF_Provider)
            {
                InjectionMode = InjectionMode.TargetObject;

                ObjectName = targetObjectNameIn_LXF_Provider;
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="childIndexWithRootGameObject">RootGameObject is the compoent attached gameObject 
            /// and called this method, childIndexWithRootGameObject is the index of the child component in the root gameobject,
            /// you can put into many index to get child's child</param>
            public LXF_InjectAttribute(params int[] childIndexWithRootGameObject)
            {
                InjectionMode = InjectionMode.Chilren;

                ChildIndexWithRootGameObject = childIndexWithRootGameObject;
            }

            public readonly InjectionMode InjectionMode;
            public readonly string ObjectName;
            public readonly int[] ChildIndexWithRootGameObject;
            }

        [AttributeUsage(AttributeTargets.Method | AttributeTargets.Field)]
        public sealed class LXF_ProvideAttribute : Attribute
        {
            public LXF_ProvideAttribute(ProvideMode provideMode = ProvideMode.GameObject)
            {
                ProvideMode = provideMode;
            }

            public readonly ProvideMode ProvideMode;       

        }

        public interface IDependencyProvider { }
        public interface IInjectable { }

        [RequireComponent(typeof(LXF_Injector))]
        public abstract class LXF_Provider :LXF_Singleton<LXF_Provider>, IDependencyProvider
        {
            protected override void Awake()
            {
                InitializeSingleton(false);
            }

            [LXF_Provide(ProvideMode.SingleMonoList)]
            public List<MonoBehaviour> SingleMonosInScene = new();//the single mono script in scene
        }
    }

    namespace MonoYield
    {
        public class LXF_MonoYield : MonoBehaviour
        {
            #region DI 
            public bool IsInjected { get; private set; } = false;

            [Obsolete("Now you can directly SetActive without use this method")]
            public void SetActive_DI(GameObject gameObject)
            {               
                var monoYield = gameObject.GetComponentInChildren<LXF_MonoYield>();

                if (monoYield == null ||monoYield.IsInjected)
                {
                    gameObject.SetActive(true);
                    return;
                }

                var monoBehaviours = gameObject.GetComponentsInChildren<LXF_MonoYield>();

                foreach (var mb in monoBehaviours)
                {
                    if (mb.IsInjected) continue;

                    LXF_Injector.Instance.Inject(mb);
                    mb.IsInjected = true;
                }

                gameObject.SetActive(true);
            }

            public T Instantiate_DI<T>(T prefab) where T : Component
            {
                var instance = Instantiate(prefab);

                var monoYield = instance.GetComponentInChildren<LXF_MonoYield>();

                if (monoYield == null || monoYield.IsInjected) return instance;

                var monoBehaviours = instance.GetComponentsInChildren<LXF_MonoYield>();

                foreach (var mb in monoBehaviours)
                {
                    if (mb.IsInjected) continue;

                    LXF_Injector.Instance.Inject(mb);
                    mb.IsInjected = true;
                }

                return instance;
            }

            public T Instantiate_DI<T>(T prefab,Transform parent) where T : Component
            {
                var instance = Instantiate(prefab, parent);

                var monoYield = instance.GetComponentInChildren<LXF_MonoYield>();

                if (monoYield == null || monoYield.IsInjected) return instance;

                var monoBehaviours = instance.GetComponentsInChildren<LXF_MonoYield>();

                foreach (var mb in monoBehaviours)
                {
                    if (mb.IsInjected) continue;

                    LXF_Injector.Instance.Inject(mb);
                    mb.IsInjected = true;
                }

                return instance;
            }
            #endregion
        }
    }

    #region Singleton
    public class LXF_Singleton<T> : LXF_MonoYield where T : Component
    {
        public static T Instance
        {
            get
            {
                if(instance == null)
                {
                    instance = FindAnyObjectByType<T>();
                }

                return instance;
            }
        }

        private static T instance;

        public float InitializationTime { get; private set; }

        //[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        /// <summary>
        /// make sure to call base.Awake() in override if you need awake
        /// </summary>
        protected virtual void Awake()
        {
            
        }

        protected void InitializeSingleton(bool isPrisentInAnyScene)
        {
            if (!Application.isPlaying) return;
            InitializationTime = Time.time;

            if(isPrisentInAnyScene)
                DontDestroyOnLoad(gameObject);

            T[] oldInstances = FindObjectsByType<T>(FindObjectsSortMode.None);
            foreach (T old in oldInstances)
            {
                if (old.GetComponent<LXF_Singleton<T>>().InitializationTime < InitializationTime)
                {
                    Destroy(old.gameObject);
                }
            }

            if (instance == null)
            {
                instance = this as T;
            }
            else
            {
                if (instance != this)
                {
                    Debug.LogWarning("Multiple instances of " + typeof(T) + " found. Destroying additional instance.");
                    Destroy(gameObject);
                }
            }

        }
    }
    public abstract class LXF_Singleton : MonoBehaviour
    {
        public static LXF_Singleton Instance { get; protected set; }

        private void Awake()
        {
            Instance ??= this;
            if (Instance != this)
            {
                Destroy(Instance);
                Instance = this;
            }

            DontDestroyOnLoad(gameObject);
        }
    }



    public class Singleton<T> where T : class, new()
    {
        private static readonly Lazy<T> _instance = new Lazy<T>(() => new T());

        protected Singleton()
        {

        }

        public static T Instance => _instance.Value;
    }

    public class SingletonWithParams<T> where T : class
    {
        private static T _instance;
        private static readonly object _lock = new();

        protected SingletonWithParams() { }

        public static T Instance(params object[] args)
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    _instance ??= (T)Activator.CreateInstance(typeof(T), args);
                }
            }
            return _instance;
        }
    }

    #endregion

    #region Timer
    public class LXF_Timer 
    {
        protected float m_TimerLifeTime;
        private readonly Action OnTimerEnd;
        private readonly Action OnTimerRunning;
        private bool RUN;
        public LXF_Timer()
        {
            m_TimerMode = TimerRunningMode.None;
        }

        public LXF_Timer(float lifeTime,Action onTimerEndcallback)
        {
            m_TimerMode = TimerRunningMode.WatchTimerEnd;

            m_TimerLifeTime = lifeTime;
            OnTimerEnd = onTimerEndcallback;
        }

        public LXF_Timer(float lifeTime, Action onTimerEndcallback, Action onTimerRunningCallback)
        {
            m_TimerMode = TimerRunningMode.WatchTimer;

            m_TimerLifeTime = lifeTime;
            OnTimerEnd = onTimerEndcallback;
            OnTimerRunning = onTimerRunningCallback;        
        }

        protected enum TimerRunningMode
        {
            None,
            WatchTimerEnd,
            WatchTimer
        }
        private enum TimerState
        {
            STOP,
            RUNNING,
            End
        }

        protected TimerRunningMode m_TimerMode;
        private TimerState m_TimerState;

        public float TimerRunningTime { get; private set; } = 0;
        public bool IsEnd => m_TimerState == TimerState.End;


        public void Reset()=> TimerRunningTime = 0;
   

        public void Start()
        {
            Reset();

            Running().Forget();
        }
        public void Start(Action onStartCallback)
        {
            Reset();

            onStartCallback?.Invoke();

            Running().Forget();
        }
        public float Start(float startTime)
        {
            TimerRunningTime = startTime;

            Running().Forget();

            return TimerRunningTime;
        }
        public float Start(float startTime,Action onStartCallback)
        {
            TimerRunningTime = startTime;

            onStartCallback?.Invoke();

            Running().Forget();

            return TimerRunningTime;
        }

        public float Pause()
        {
            RUN = false;
            return TimerRunningTime;
        }
        public async UniTask<float> Pause(Action onPauseCallback)
        {
            RUN = false;
            await UniTask.WaitUntil(() => m_TimerState == TimerState.STOP);
            onPauseCallback?.Invoke();
            return TimerRunningTime;
        }

        public float Resume()
        {
            Running().Forget();

            return TimerRunningTime;
        }
      

        private async UniTask Running()
        {
            m_TimerState = TimerState.RUNNING;
            switch (m_TimerMode)
            {
                case TimerRunningMode.None:
                    RUN = true;
                    while (RUN)
                    {
                        await UniTask.Yield();
                        TimerRunningTime += Time.deltaTime;
                    }
                    m_TimerState = TimerState.End;
                    break;
                case TimerRunningMode.WatchTimerEnd:
                    RUN = true;
                    while (RUN)
                    {
                        await UniTask.Yield();
                        TimerRunningTime += Time.deltaTime;

                        if (TimerRunningTime >= m_TimerLifeTime)
                        {
                            OnTimerEndCallback();
                            m_TimerState = TimerState.End;
                            break;
                        }
                    }                
                    break;
                case TimerRunningMode.WatchTimer:
                    RUN = true;
                    while (RUN)
                    {                  
                        await UniTask.Yield();
                        TimerRunningTime += Time.deltaTime;
                        OnTimerRunningCallback();

                        if (TimerRunningTime >= m_TimerLifeTime)
                        {
                            OnTimerEndCallback();
                            m_TimerState = TimerState.End;
                            break;
                        }
                    }               
                    break;                    
            }

            m_TimerState = TimerState.STOP;
        }

        protected virtual void OnTimerEndCallback() =>OnTimerEnd?.Invoke();
        protected virtual void OnTimerRunningCallback() => OnTimerRunning?.Invoke();
    }  

    public class LXF_Timer<T> :LXF_Timer
    {
        readonly Action<T> OnTimerEnd;
        readonly Action<T> OnTimerRunning;
        readonly T TObject;

        public LXF_Timer(T obj, float lifeTime, Action<T> onTimerEndcallback)
        {
            m_TimerMode = TimerRunningMode.WatchTimerEnd;

            TObject = obj;
            OnTimerEnd = onTimerEndcallback;
            m_TimerLifeTime = lifeTime;
        }

        public LXF_Timer(T obj, float lifeTime, Action<T> onTimerEndcallback, Action<T> onTimerRunningCallback)
        {
            m_TimerMode = TimerRunningMode.WatchTimer;

            TObject = obj;
            OnTimerEnd = onTimerEndcallback;
            OnTimerRunning = onTimerRunningCallback;
            m_TimerLifeTime = lifeTime;
        }

        protected override void OnTimerEndCallback() => OnTimerEnd?.Invoke(TObject);
        protected override void OnTimerRunningCallback() => OnTimerRunning?.Invoke(TObject);
    }
    #endregion

    #region Value Monitor
    public class ValueMonitor<T> where T : struct
    {
        protected T mValue;
        public readonly T MaxValue, MinValue;
        public T differLastValue { get; protected set; }
        public ValueMonitor(T defaultValue)
        {
            mValue = defaultValue;
        }
        public ValueMonitor(T minValue, T maxValue, T defaultValue = default)
        {
            MaxValue = maxValue;
            MinValue = minValue;
            mValue = defaultValue;
        }

        public T Value
        {
            get { return mValue; }
            set
            {
                ValueSetter(value);
            }
        }

        protected virtual void ValueSetter(T value) { }


        public EventRunner CrossedUpperBound = new();
        public EventRunner CrossedLowerBound = new();

        public EventRunner VauleChangeEvent = new();
        public EventRunner<T> VauleChange=new();
    }
    public class IntegerMonitor : ValueMonitor<int>
    {
        public IntegerMonitor(int minValue, int maxValue, int defaultValue = 0) : base(minValue, maxValue, defaultValue)
        {
        }

        protected override void ValueSetter(int value)
        {
            if (value >= MaxValue)
            {
                differLastValue = value - MaxValue;
                CrossedUpperBound.Run();
                value = MaxValue;
            }
            if (value <= MinValue)
            {
                differLastValue = value - MinValue;
                CrossedLowerBound?.Run();
                value = MinValue;
            }
            if (mValue != value)
            {
                differLastValue = value - mValue;
                mValue = value;
                VauleChangeEvent?.Run();
                VauleChange?.Run(mValue);
            }
        }
    }

    public class FloatMonitor : ValueMonitor<float>
    {
        public FloatMonitor(float minValue, float maxValue, float defaultValue = 0) : base(minValue, maxValue, defaultValue)
        {
        }

        protected override void ValueSetter(float value)
        {
            if (value >= MaxValue)
            {
                differLastValue = value - MaxValue;
                CrossedUpperBound.Run();
                value = MaxValue;
            }
            if (value <= MinValue)
            {
                differLastValue = value - MinValue;
                CrossedLowerBound?.Run();
                value = MinValue;
            }
            if (mValue != value)
            {
                differLastValue = value - mValue;
                mValue = value;
                VauleChangeEvent?.Run();
                VauleChange?.Run(mValue);
            }
        }
    }

    public class BoolenMonitor : ValueMonitor<bool>
    {
        public BoolenMonitor(bool defaultValue) : base(defaultValue)
        {
        }

        protected override void ValueSetter(bool value)
        {
            if (mValue != value)
            {
                mValue = value;
                VauleChange?.Run(value);
            }
        }
    }


    #endregion

    #region Event Runner
    public class EventRunner 
    {
        private Action mOnEvent = () => { };
        public int mOnEventCount { get => mOnEvent.GetInvocationList().Length; }
        public void UnRegister(Action onEvent)
        {
            mOnEvent -= onEvent;
        }
        public void Clear()=> mOnEvent = () => { };
        public void Register(Action onEvent)
        {
            mOnEvent += onEvent;
        }

        public int EventNum=>mOnEvent.GetInvocationList().Length;
        /// <summary>
        /// 限制同一事件重复注册
        /// </summary>
        public void SingleRegister(Action onEvent)
        {
            Delegate[] invocationList = mOnEvent.GetInvocationList();
            foreach (Delegate handler in invocationList)
            {
                if (handler.Equals(onEvent))
                {
                    // 相同的方法已经注册了
                    return;
                }
                else
                {
                    mOnEvent += onEvent;
                }
            }
        }
       
        public void Run() => mOnEvent?.Invoke();
    }

    public class EventRunner<T>
    {
        private Action<T> mOnEvent;

        public int EventNum=>mOnEvent.GetInvocationList().Length;

        public void UnRegister(Action<T> onEvent)
        {
            mOnEvent -= onEvent;
        }

        public void Register(Action<T> onEvent)
        {
            mOnEvent += onEvent;
        }

        public void Clear() => mOnEvent = null;

        public void Run(T t) => mOnEvent?.Invoke(t);
        // public void Run(T t, EventRunMode mode){
        //     switch(mode){
        //         case EventRunMode.Single:
        //             mOnEvent?.Invoke(t);
        //             UnRegister();
        //             break;
        //     }
        //}
    }

    // public enum EventRunMode{
    //     Single,
    //     Multiple,
    // }
    #endregion

    #region Object Pool
    public abstract class LXF_BasePool<T> : LXF_Singleton<LXF_BasePool<T>> where T : Component
    {
        protected abstract Transform PoolManager{get;}

        [SerializeField] int defaultSize = 100;
        [SerializeField] int maxSize = 500;

        protected ObjectPool<T> pool;

        public int ActiveCount => pool.CountActive;
        public int InactiveCount => pool.CountInactive;
        public int TotalCount => pool.CountAll;

        protected void Initialize() =>
            pool = new ObjectPool<T>(OnCreatePoolItem, OnGetPoolItem, OnReleasePoolItem, OnDestroyPoolItem, false, defaultSize, maxSize);

        protected abstract T Create();

        protected T OnCreatePoolItem(){
            T obj = Create();
            obj.transform.SetParent(PoolManager);
            return obj;
        }
        protected virtual void OnGetPoolItem(T obj) => obj.gameObject.SetActive(true);
        protected virtual void OnReleasePoolItem(T obj) => obj.gameObject.SetActive(false);
        protected virtual void OnDestroyPoolItem(T obj) => Destroy(obj.gameObject);


        public T Get() => pool.Get();
        public void Release(T obj) => pool.Release(obj);
        public void Clear() => pool.Clear();
    }
    #endregion

    #region State Machine
    public interface IState
    {
        void Enter();
        void Execute();
        void Exit();
    }

    public class StateMachineBase
    {
        private IState currentState;
        private string defaultKey;
        private readonly Dictionary<string, IState> m_states;

        public enum MachineState
        {
            RUNNING,
            PAUSED,
            STANDBY
        }
        protected MachineState m_MachineState;

        public StateMachineBase()
        {
            m_MachineState = MachineState.STANDBY;
            m_states = new Dictionary<string, IState>();
        }

        public IBuilder MachineBuilder(string defaultKey)
        {
            this.defaultKey = defaultKey;

            return new StateMachineBuilder(this);
        }

        public interface IBuilder
        {
            IBuilder AddState(string keyName,IState state);
        }

        private class StateMachineBuilder : IBuilder
        {
            public StateMachineBase _stateMachine;

            public StateMachineBuilder(StateMachineBase stateMachine)
            {
                _stateMachine = stateMachine;
            }

            public IBuilder AddState(string keyName,IState state)
            {
                _stateMachine.AddState(keyName,state);

                return this;
            }
        }

        private void AddState(string key, IState state)
        {
            if (!m_states.ContainsKey(key))
            {
                m_states.Add(key, state);
            }
        }

        public void RemoveState(string key)
        {
            if (m_states.ContainsKey(key))
            {
                if (currentState == m_states[key])
                {
                    currentState.Exit();
                    currentState = null;
                }
                m_states.Remove(key);
            }
        }    

        public IStateController Start()
        {
            currentState = m_states[defaultKey];

            m_MachineState = MachineState.RUNNING;

            _ = RUNCORE();

            return new StateController(this);
        }

        private async UniTask RUNCORE()
        {
            while (true)
            {
                if (m_MachineState == MachineState.PAUSED)
                {
                    await UniTask.Yield();
                    continue;
                }

                if (m_MachineState == MachineState.RUNNING)
                {
                    currentState?.Execute();
                }

                if (m_MachineState == MachineState.STANDBY)
                {
                    break;
                }

                await UniTask.Yield();
            }
        }

        private void Pause() => m_MachineState = MachineState.PAUSED;
        private void Run() => m_MachineState = MachineState.RUNNING;
        private void Stop() => m_MachineState = MachineState.STANDBY;
        private void SetState(string key)
        {
            if (m_states.ContainsKey(key))
            {
                currentState?.Exit();

                currentState = m_states[key];
                currentState.Enter();
            }
        }

        public interface IStateController
        {
            void Pause();
            void Run();
            void Stop();
            void SetState(string stateName);
        }

        private class StateController : IStateController
        {
            private readonly StateMachineBase _stateMachine;

            public StateController(StateMachineBase stateMachine)
            {
                _stateMachine = stateMachine;
            }

            public void Pause() => _stateMachine.Pause();
            public void Run() => _stateMachine.Run();
     
            public void Stop() => _stateMachine.Stop();

            public void SetState(string stateName) => _stateMachine.SetState(stateName);
        }
    }

    public abstract class LXF_FSM 
    {
        StateMachineBase m_StateMachine;

        public LXF_FSM()
        {
          
        }
    }


    #endregion

    #region AudioKit
    [DefaultExecutionOrder(-1)]
    public abstract class LXF_AudioKit : LXF_Singleton<LXF_AudioKit>
    {
        private Dictionary<string,AudioClip> _audioClips = new();
    
        private AudioSource[] _audioSources=>GetComponents<AudioSource>();
    
        protected new virtual void Awake()
        {
            base.Awake();
            Init();
        }
    
        protected abstract void Init();
    
        public void Play(string audioClipName,bool isLoop=false)=> Play(audioClipName,GetFreeAudioSource,isLoop);
        public void PlaSingle(string audioClipName,bool isLoop=false)=> Play(audioClipName,()=> GetTargetAudioSource(audioClipName),isLoop);
        public void Pause(string audioClipName) => GetTargetAudioSource(audioClipName)?.Pause();
        public void PauseAll()
        {
            foreach (var source in _audioSources)
            {
                source.Pause();
            }            
        }
        public void Resume(string audioClipName) => GetTargetAudioSource(audioClipName)?.UnPause();

        public void ResumeAll()
        {
            foreach (var source in _audioSources)
            {
                source.UnPause();
            }
        }
        public void StopAll()
        {
            foreach (var source in _audioSources)
            {
                source.Stop();
            }
        }
        public void Stop(string audioClipName) => GetTargetAudioSource(audioClipName)?.Stop();
    
        private void Play(string audioClipName,Func<AudioSource> sourceGetter ,bool isLoop=false){
            if(!_audioClips.ContainsKey(audioClipName)){
                Debug.LogError($"AudioClip {audioClipName} not exist！");
                return;
            }
            AudioClip audioClip=_audioClips[audioClipName];
            var audioSource=sourceGetter();
    
            if(audioSource)
            {
                audioSource.clip=audioClip;
                audioSource.loop=isLoop;
                audioSource.Play();
            }           
        }
    
        protected void AddAudioClip(string audioClipName,AudioClip audioClip){
            _audioClips.Add(audioClipName,audioClip);
        }
    
        private AudioSource GetTargetAudioSource(string audioClipName)
         {
            foreach(var source in _audioSources)
            {
                if(source.clip.Equals(_audioClips[audioClipName])&&
                     source.isPlaying) return source;                   
            }

            return GetFreeAudioSource();
         }
        
        private AudioSource GetFreeAudioSource()
        {
            foreach(var source in _audioSources)
            {
               if(!source.isPlaying&& source.time == 0){

                   return source;
               }            
            }
             
             return gameObject.AddComponent<AudioSource>();
        }
    }

    #endregion

    #region Remap
    public static class LXF_Math
    {
        public static float ReMap(float inputValue, float minInput, float maxInput, float minOutput, float maxOutput)
        {
            if (inputValue < minInput || inputValue > maxInput)
            {
                throw new ArgumentOutOfRangeException(nameof(inputValue), "Input value is out of range.");
            }

            return (inputValue - minInput) / (maxInput - minInput) * (maxOutput - minOutput) + minOutput;
        }
    }
    #endregion

    #region Extensions
    public static class TransformExtensions
    {
        public static Transform[] GetAllChild(this Transform transform)
        {
            var childCount = transform.childCount;
            var children = new Transform[childCount];
            for (int i = 0; i < childCount; i++)
            {
                children[i] = transform.GetChild(i);
            }
            return children;
        }
    }


    public static class ColorExtensions
    {
        public static Color GetColorForHexadecimal(string hex)
        {
            if (ColorUtility.TryParseHtmlString(hex, out Color newColor))
            {
                return newColor;  // 设置Image组件的颜色
            }

            throw new ArgumentException("Invalid color hex string!");
        }
    }


    public static class MathExtensions
    {
        /// <summary>
        /// this section is [min,max)
        /// </summary>    
        public static bool IsInRange(this int value,Vector2Int range) 
        {
            if(value<range.x||value>=range.y) return false;
            return true;
        }

        public static bool IsInRange(this float value, Vector2 range)
        {
            if (value < range.x || value >= range.y) return false;
            return true;
        }
    }
    #endregion
}



