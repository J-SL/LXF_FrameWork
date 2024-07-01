using System.Runtime.Serialization.Formatters.Binary;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;
using UnityEngine.Pool;
using Newtonsoft.Json;
using Unity.VisualScripting;

namespace LXF_Framework
{
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
    //=============================================================

        public static class LXF_SaveSystem{

            public readonly static string FilePath = Application.persistentDataPath;
   
            public static void SaveAll(IEnumerable<ISerialization> objs){
                foreach (var obj in objs){
                    Save(obj);
                }
            }

            public static void LoadAll(IEnumerable<ISerialization> objs){
                foreach (var obj in objs){
                    Load(obj);
                }
            }

            public static void Save(ISerialization obj)=> obj.Serialize(new LXF_Saver(obj.FileName));

            public static void Load(ISerialization obj)=> obj.Deserialize(new LXF_Loader(obj.FileName));           //private static
        }

        public interface ISerialization{
            string FileName { get; set;}
            void Serialize(LXF_Saver saver);
            void Deserialize(LXF_Loader loader);
        }

       public class LXF_Saver {
            readonly string _filePath;
            public LXF_Saver(string fileName){
                _filePath=LXF_SaveSystem.FilePath+"/"+fileName;

                if (!File.Exists(_filePath))
                {            
                    Debug.Log($"{_filePath} not exist, automatically create a new file");
                }
            }
            public Dictionary<string, string> Values{get;private set;} = new();

            public void Add<T>(string valueName,T value) where T : struct {
                string json = JsonConvert.SerializeObject(value);
                Values.Add(valueName,json);
            }

            public void Add(string valueName,IntegerMonitor integer) {
                string json = JsonConvert.SerializeObject(integer);
                Values.Add(valueName,json);
            }

            public void Add(string valueName,FloatMonitor _float) {
                string json = JsonConvert.SerializeObject(_float);
                Values.Add(valueName,json);
            }

            public void Add(string valueName,BoolenMonitor boolen){
                string json = JsonConvert.SerializeObject(boolen);
                Values.Add(valueName,json);           
            }

            public void End(){
                string jsonString = JsonConvert.SerializeObject(Values, Formatting.Indented);
                File.WriteAllText(_filePath, jsonString);
            }        
        }

        public class LXF_Loader{

            public LXF_Loader(string fileName){
                string filePath=LXF_SaveSystem.FilePath+"/"+fileName;

                if (File.Exists(filePath))
                {
                    string jsonData = File.ReadAllText(filePath);  
                    Values = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonData);            
                }else{
                    Debug.LogError($"Load failed, please check {filePath}");
                }
            }

            private Dictionary<string, string> Values = new ();


            private T GetValue<T>(string valueKey) where T : struct
            {
                if (Values.ContainsKey(valueKey)) {
                    return JsonConvert.DeserializeObject<T>(Values[valueKey]);
                } 
                else {
                    throw new KeyNotFoundException($"Key '{valueKey}' not found in the dictionary.");
                }
            }

            public void OnLoad<T>(string valueName,ref T value)where T : struct
                =>value=GetValue<T>(valueName);
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


        }

    }

    #region SequenceExecutor
    public class SequenceExecutor : MonoBehaviour
    {
        public static SequenceExecutor Instance { get; protected set; }

        private void Awake()
        {
            Instance ??= this;
            if (Instance != this)
            {
                Instance = this;
            }

            DontDestroyOnLoad(gameObject);
        }

        public void StartTailEndCallCoroutine(SequentialExecution.Calls c) => StartCoroutine(c.TailEndCall());

    }

    /// <summary>
    /// ����ction����Mark���monitor�ʹ�action.Mark4���
    /// </summary>
    public class PackMethod
    {
        Action action;
        Func<bool> monitor;
        public PackMethod(Action action, Func<bool> monitor)
        {
            this.action = action;
            this.monitor = monitor;
        }

        public void Method() => action();

        public bool Monitor() => monitor();
    }
    public static class SequentialExecution
    {
        public static void SeriesMethodExecutor(params PackMethod[] methods)
        {
            var c = new Calls(methods);
            SequenceExecutor.Instance.StartTailEndCallCoroutine(c);
        }

        public class Calls
        {
            private PackMethod[] methods;
            public Calls(params PackMethod[] methods)
            {
                this.methods = methods;
            }
            public IEnumerator TailEndCall()
            {
                foreach (var method in methods)
                {
                    method.Method();
                    while (!method.Monitor())
                        yield return null;
                }
            }
        }
    }
    #endregion

    #region Singleton
    public class LXF_Singleton<T> : MonoBehaviour where T : Component
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


        /// <summary>
        /// make sure to call base.Awake() in override if you need awake
        /// </summary>
        protected virtual void Awake()
        {
            InitializeSingleton();
        }

        protected virtual void InitializeSingleton()
        {
            if (!Application.isPlaying) return;
            InitializationTime = Time.time;
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
    #endregion

    #region Delayer

    public static class Delayer
    {

    }

    public class FrameDelayer
    {
        public FrameDelayer()
        {
            timeCounter = 0;
            valueTimeCounter = 0;
            valueTimeCounter1 = 0;
            valueTimeCounter2 = 0;
        }
        private float timeCounter;

        public void DelayFunc(float seconds, Action callback)
        {
            timeCounter += Time.deltaTime;
            if (timeCounter >= seconds)
            {
                timeCounter = 0;
                callback?.Invoke();
            }
        }

        private float valueTimeCounter;
        public void DelayValue(ref float value, float endValue, float time)
        {
            if (valueTimeCounter >= time || MathF.Abs(value - endValue) < 0.5f)
            {
                valueTimeCounter = 0;
                return;
            }
            else
            {
                valueTimeCounter += Time.deltaTime;
                value = Mathf.Lerp(value, endValue, valueTimeCounter / time);
            }
        }
        private float valueTimeCounter1;
        public float DelayValue(float value, float endValue, float time)
        {
            if (valueTimeCounter1 >= time || MathF.Abs(value - endValue) < 0.5f)
            {
                valueTimeCounter1 = 0;
                return endValue;
            }
            else
            {
                valueTimeCounter1 += Time.deltaTime;
                return Mathf.Lerp(value, endValue, valueTimeCounter1 / time);
            }
        }

        private float valueTimeCounter2;
        public Quaternion DelayValue(Quaternion value, Quaternion endValue, float time)
        {
            if (valueTimeCounter2 >= time || Quaternion.Angle(value, endValue) < 0.5f)
            {
                valueTimeCounter2 = 0;
                return endValue;
            }
            else
            {
                valueTimeCounter2 += Time.deltaTime;
                return Quaternion.Lerp(value, endValue, valueTimeCounter2 / time);
            }
        }
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

    #region new MCCV----->System
    // LXF_Model：数据的集合 （继承MonoBehaviour）
    public abstract class LXF_Model : LXF_Singleton<LXF_Model>
    {
        protected abstract void LoadData();
    }

    // LXF_View: 提供视图物体引用，并监听数据的变化 （继承MonoBehaviour）
    public abstract class LXF_View : LXF_Singleton<LXF_View>
    {
        protected abstract void GetModel();
    }

    //LXF_ModelController: 提供变更数据的方法
    public abstract class LXF_ModelController<T> where T:LXF_Model
    {
        protected T m;
        protected abstract void Init(T m);
    }
    //LXF_ViewController: 提供操作视图的方法
    public abstract class LXF_ViewController<T> where T : LXF_View
    {
        protected T v;
        protected abstract void Init(T v);
    }
    //LXF_System: 获得LXF_ModelController和LXF_ViewController，组织调用其所有的方法 （继承MonoBehaviour）
    public abstract class LXF_System<CM, CV> :LXF_Singleton<LXF_System<CM, CV>> 
    {
        protected CM cm;
        protected CV cv;
        protected abstract void Init(CM cm, CV cv);
    }

    #endregion

    #region Main Script Manage
    public abstract class MainScript : MonoBehaviour
        {
            protected virtual void Awake()
            {
                GetReferences();
            }

            protected abstract void GetReferences();

            protected T GetComponentBySelf<T>(T component) where T : Component
            {
                if (component) return component;
                else if (component = GetComponent<T>()) return component;
                else return gameObject.AddComponent<T>();
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
        public void Register(Action[] onEvents)
        {
            foreach(var onEvent in onEvents)
            {
                mOnEvent += onEvent;
            }    
        }

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
       
        public void TriggerToClear()
        {
            Run();
            mOnEvent = () => { };
        }
        public void Run() => mOnEvent?.Invoke();
        public void Run(Action onEvent)
        {
            Run();             
            UnRegister(onEvent);
        }
    }

    public class EventRunner<T>
    {
        private Action<T> mOnEvent;

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
    }
    #endregion

    #region Object Pool
    public class LXF_BasePool<T> : MonoBehaviour where T : Component
    {
        public static LXF_BasePool<T> Instance { get; protected set; }

        private T prefab;
        [SerializeField] int defaultSize = 100;
        [SerializeField] int maxSize = 500;

        protected ObjectPool<T> pool;

        public int ActiveCount => pool.CountActive;
        public int InactiveCount => pool.CountInactive;
        public int TotalCount => pool.CountAll;

        protected void Initialize() =>
            pool = new ObjectPool<T>(OnCreatPoolItem, OnGetPoolItem, OnReleasePoolItem, OnDestroyPoolItem, false, defaultSize, maxSize);

        protected virtual T OnCreatPoolItem() => Instantiate(prefab);
        protected virtual void OnGetPoolItem(T obj) => obj.gameObject.SetActive(true);
        protected virtual void OnReleasePoolItem(T obj) => obj.gameObject.SetActive(false);
        protected virtual void OnDestroyPoolItem(T obj) => Destroy(obj.gameObject);


        public T Get() => pool.Get();
        public void Release(T obj) => pool.Release(obj);
        public void Clear() => pool.Clear();
    }
    #endregion

    #region Tools
    public static class TransformTools
    {
        public static Transform[] GetAllChildTransform(this Transform transform)
        {
            int childCount = transform.childCount;
            var childs= new Transform[childCount];
            for (int i = 0; i < childCount; i++)
            {
                childs[i] = transform.GetChild(i);
            }

            return childs;
        }
    }
    #endregion

    #region AudioKit
    [DefaultExecutionOrder(-1)]
    public abstract class LXF_AudioKit : LXF_Singleton<LXF_AudioKit>{
        private AudioSource _audioSource;
        protected Dictionary<string,AudioClip> audioSources = new();
        protected new virtual void Awake()
        {
            base.Awake();
            Init();
            _audioSource=gameObject.AddComponent<AudioSource>();
        }

        protected abstract void Init();
        public void Play(string audioClipName){
            if(!audioSources.ContainsKey(audioClipName)){
                Debug.LogError($"AudioClip {audioClipName} 不存在！");
                return;
            }
            AudioClip audioClip=audioSources[audioClipName];
            _audioSource.clip=audioClip;
            _audioSource.Play();
        }

        protected void AddAudioClip(string audioClipName,AudioClip audioClip){
            audioSources.Add(audioClipName,audioClip);
        }
    }
    #endregion
    #region Debugger
    public static class Debugger
    {
        /// <summary>
    /// 检查指定的引用类型对象是否为空，并打印结果。
    /// </summary>
    /// <typeparam name="T">引用类型</typeparam>
    /// <param name="obj">要检查的对象</param>
    public static void CheckIfObjectIsNull<T>(T obj) where T : class
    {
        if (obj == null)
        {
            Debug.LogError($"传入的对象为空！");
        }
        else
        {
            Debug.Log($"传入的对象 {GetObjectName(obj)} 存在。");
        }
    }

    /// <summary>
    /// 获取对象的名称。如果对象是GameObject或Component，返回它们的名称；否则返回对象的类型名称。
    /// </summary>
    /// <typeparam name="T">引用类型</typeparam>
    /// <param name="obj">要获取名称的对象</param>
    /// <returns>对象的名称</returns>
    private static string GetObjectName<T>(T obj) where T : class
    {
        if (obj is GameObject gameObject)
        {
            return gameObject.name;
        }
        else if (obj is Component component)
        {
            return component.gameObject.name + " (" + component.GetType().Name + ")";
        }
        else
        {
            return obj.GetType().Name;
        }
    }
    }
    #endregion
}



