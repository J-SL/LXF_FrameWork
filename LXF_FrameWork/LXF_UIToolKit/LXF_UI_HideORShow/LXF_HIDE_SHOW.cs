using Cysharp.Threading.Tasks;
using DG.Tweening;
using LXF_Framework;
using LXF_Framework.MonoYield;
using System;
using UnityEngine;

namespace LXF_UI_HIDE_SHOW
{
    [RequireComponent(typeof(CanvasGroup))]
    public class LXF_HIDE_SHOW : LXF_MonoYield
    {
        CanvasGroup _canvasGroup;

        public EventRunner OnHide = new();
        public EventRunner OnShow = new();

        [SerializeField]
        private float _initialAlpha = 1;

        [SerializeField]
        private bool _autoHide = false;
        [SerializeField]
        private float _autoHideDelay = 0.5f;

        [Space(20)]

        [SerializeField]
        [Range(0,1)]
        private float _alphaWhenHide = 0;
        [SerializeField]
        [Range(0,1)]
        private float _alphaWhenShow = 1;

        [SerializeField]
        private bool _tweenHide = false;
        [SerializeField]
        private float _onHideDuration = 0.7f;
        [SerializeField]
        private bool _tweenShow = false;
        [SerializeField]
        private float _onShowDuration = 0.7f;



        private Tween _tween;

        private void Awake()
        {
            if (GetComponentsInChildren<RectTransform>().Length==0) throw new System.Exception("LXF_HIDE_SHOW: " +
                "This script should be attached to a UI element or the child of a UI element.");

            _canvasGroup = GetComponent<CanvasGroup>();
            _canvasGroup.alpha = _initialAlpha;
        }

        public async UniTask Hide()
        {
            if (_tween!= null && _tween.IsPlaying()) _tween.Kill();

            if (_tweenHide)
            {
                _tween = DOTween.To(() => _canvasGroup.alpha,
                    x => _canvasGroup.alpha = x, _alphaWhenHide, _onHideDuration);

                await _tween.AsyncWaitForCompletion();
            }else
                _canvasGroup.alpha = _alphaWhenHide;

            OnHide?.Run();
        }

        //public void Hide(float hideTime)
        //{
        //    if (_tween != null && _tween.IsPlaying()) _tween.Kill();

        //    _tween = DOTween.To(() => _canvasGroup.alpha,
        //           x => _canvasGroup.alpha = x, _alphaWhenShow, hideTime).OnComplete(() => gameObject
        //           .SetActive(false));
        //}

        public async UniTask Show() 
        {
            if (_tween!= null && _tween.IsPlaying()) _tween.Kill();

            OnShow?.Run();

            if (_tweenShow)
            {
                _tween = DOTween.To(() => _canvasGroup.alpha,
                    x => _canvasGroup.alpha = x, _alphaWhenShow, _onShowDuration);

                await _tween.AsyncWaitForCompletion();
            }else
                _canvasGroup.alpha = _alphaWhenShow;

            if (_autoHide)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(_autoHideDelay));
                await Hide();
            }
        }
    }
}
