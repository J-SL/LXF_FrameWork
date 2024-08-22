using Cysharp.Threading.Tasks;
using DG.Tweening;
using LXF_Framework;
using LXF_Framework.MonoYield;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LXF_UI_HIDE_SHOW
{
    [RequireComponent(typeof(CanvasGroup))]
    public class LXF_HIDE_SHOW : LXF_MonoYield
    {
        CanvasGroup _canvasGroup;

        public EventRunner OnHide = new();
        public EventRunner WhenHide = new();
        public EventRunner OnShow = new();
        public EventRunner WhenShow = new();

        [Header("Customization Alpha Options")]
        // If true, only the alpha of the image will be affected, not the canvas group.
        [SerializeField]
        private bool _isOnlyAffectSelf = false;
        //----------- it only works when _isOnlyAffectSelf is true---------------
        [SerializeField]
        private List<Image> _customImages = new List<Image>();
        [SerializeField]
        private List<Vector3> _custoImagesShowHideValue = new();

        [SerializeField] 
        private List<TMP_Text> _customTexts = new List<TMP_Text>();
        [SerializeField]
        private List<Vector3> _customTextsShowHideValue = new();
        //----------- it only works when _isOnlyAffectSelf is true---------------

        [Space(20)]

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
        private Image _image;

        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            _image = GetComponent<Image>();
            _canvasGroup.alpha = _initialAlpha;

            if(_isOnlyAffectSelf)
            {
                _canvasGroup.enabled = false;

                InitCustomUI();
            }
        }

        private void InitCustomUI()
        {
            for (int i = 0; i < _customImages.Count; i++)
            {
                _customImages[i].color = new Color(_customImages[i].color.r, _customImages[i].color.g, _customImages[i].color.b, _custoImagesShowHideValue[i].z);
            }

            for (int i = 0; i < _customTexts.Count; i++)
            {
                _customTexts[i].color = new Color(_customTexts[i].color.r, _customTexts[i].color.g, _customTexts[i].color.b, _customTextsShowHideValue[i].z);
            }
        }

        public async UniTask Hide()
        {
            if (_tween!= null && _tween.IsPlaying()) _tween.Kill();

            OnHide?.Run();

            if (_isOnlyAffectSelf)
            {
                if (_tweenHide)
                {
                    Sequence seq = DOTween.Sequence();

                    seq.Join(_image.DOFade(_alphaWhenHide, _onShowDuration));

                    for (int i = 0; i < _customImages.Count; i++)
                    {
                        seq.Join(_customImages[i].DOFade(_custoImagesShowHideValue[i].y, _onShowDuration));
                    }

                    for (int i = 0; i < _customTexts.Count; i++)
                    {
                        seq.Join(_customTexts[i].DOFade(_customTextsShowHideValue[i].y, _onShowDuration));
                    }

                    _tween = seq;
                    await _tween.AsyncWaitForCompletion();
                }
                else
                    _image.color = new Color(_image.color.r, _image.color.g, _image.color.b, _alphaWhenHide);

                WhenHide?.Run();

                return;
            }


            if (_tweenHide)
            {
                _tween =_canvasGroup.DOFade(_alphaWhenHide, _onHideDuration);

                await _tween.AsyncWaitForCompletion();
            }else
                _canvasGroup.alpha = _alphaWhenHide;

            WhenHide?.Run();
        }


        public async UniTask Show() 
        {         
            if (_tween!= null && _tween.IsPlaying()) _tween.Kill();

            OnShow?.Run();

            if (_isOnlyAffectSelf)
            {
                if (_tweenShow)
                {
                    Sequence seq = DOTween.Sequence();

                    seq.Join(_image.DOFade(_alphaWhenShow, _onShowDuration));

                    for(int i = 0; i < _customImages.Count; i++)
                    {
                        seq.Join(_customImages[i].DOFade(_custoImagesShowHideValue[i].x, _onShowDuration));
                    }

                    for (int i = 0; i < _customTexts.Count; i++)
                    {
                        seq.Join(_customTexts[i].DOFade(_customTextsShowHideValue[i].x, _onShowDuration));
                    }

                    _tween = seq;
                    await _tween.AsyncWaitForCompletion();
                }
                else
                    _image.color = new Color(_image.color.r, _image.color.g, _image.color.b, _alphaWhenShow);

                WhenShow?.Run();

                if (_autoHide)
                {
                    await UniTask.Delay(TimeSpan.FromSeconds(_autoHideDelay));
                    await Hide();
                }

                return;
            }
         
            if (_tweenShow)
            {
                _tween = _canvasGroup.DOFade(_alphaWhenShow, _onShowDuration);

                await _tween.AsyncWaitForCompletion();
            }else
                _canvasGroup.alpha = _alphaWhenShow;

            WhenShow?.Run();

            if (_autoHide)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(_autoHideDelay));
                await Hide();
            }
        }
    }
}
