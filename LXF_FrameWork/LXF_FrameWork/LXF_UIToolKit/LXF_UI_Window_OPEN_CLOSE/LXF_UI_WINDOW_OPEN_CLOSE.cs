using Cysharp.Threading.Tasks;
using DG.Tweening;
using LXF_Framework.DependencyInjection;
using LXF_Framework.MonoYield;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class LXF_UI_WINDOW_OPEN_CLOSE : LXF_MonoYield
{
    public enum OpenMode
    {
        None,
        SetScale,
        SetRotate,
        SetPosition,
        SetAlpha,
    }

    public enum CloseMode
    {
        None,
        SetScale,
        SetRotate,
        SetPosition,
        SetAlpha,
    }

    #region Inject Fields
    [LXF_Inject(InjectionMode.Self)]
    readonly CanvasGroup _canvasGroup;
    [LXF_Inject(InjectionMode.Self)]
    readonly RectTransform _rectTransform;
    #endregion

    #region public Fields
    [Header("Initialization")]
    public bool IsOpenOnStart = false;



    [Header("Open Window Settings")]
    public OpenMode Open = OpenMode.SetAlpha;
    [Range(0, 3)]
    public float OpenDuration = 0.5f;

    public float Angle = 360;
   
    [Space(30)]

    [Header("Close Window Settings")]
    public CloseMode Close = CloseMode.SetAlpha;
    [Range(0, 3)]
    public float CloseDuration = 0.5f;

    public bool IsSetActiveFalseWhenClose = true;
    #endregion

    #region private Fields
    private Tween tween;
    #endregion
    
    #region public Methods
    public void OpenWindow()
    {
        switch (Open)
        {
            case OpenMode.None:
                gameObject.SetActive(true);
                break;
            case OpenMode.SetScale:
                transform.localScale = Vector3.zero;
                gameObject.SetActive(true);
                OnScaleTween(Vector3.one).Forget();
                break;
            case OpenMode.SetRotate:
                gameObject.SetActive(true);
                OnRotateTween().Forget();
                break;
            case OpenMode.SetPosition:
                gameObject.SetActive(true);
                OnPositionTween();
                break;
            case OpenMode.SetAlpha:
                gameObject.SetActive(true);
                OnAlphaTween(1);
                break;     
        }
    }

    public async void CloseWindow()
    {
        switch (Close)
        {
            case CloseMode.None:
                gameObject.SetActive(false);
                break;
            case CloseMode.SetScale:
                await OnScaleTween(Vector3.zero);
                gameObject.SetActive(false);
                break;
            case CloseMode.SetRotate:
                await OnRotateTween();
                gameObject.SetActive(false);
                break;
            case CloseMode.SetPosition:
                OnPositionTween();
                break;
            case CloseMode.SetAlpha:
                OnAlphaTween(0);
                break;
        }
    }
    #endregion

    #region private Methods
    private async UniTask OnScaleTween(Vector3 tragetScale)
    {
        tween?.Kill();
        tween = _rectTransform.DOScale(tragetScale, OpenDuration).SetEase(Ease.OutBack);
        await tween.AsyncWaitForCompletion();
    }

    private async UniTask OnRotateTween()
    {
        tween?.Kill();
        tween = _rectTransform.DORotate(new Vector3(0, 0, Angle), OpenDuration, RotateMode.FastBeyond360).SetEase(Ease.OutBack);
        await tween.AsyncWaitForCompletion();
    }

    private void OnPositionTween()
    {
        tween = _rectTransform.DOAnchorPos(new Vector3(0, 0, 0), OpenDuration).SetEase(Ease.OutBack);
    }

    private void OnAlphaTween(float a)
    {
        tween = _canvasGroup.DOFade(a, OpenDuration);
    }

    #endregion
}
