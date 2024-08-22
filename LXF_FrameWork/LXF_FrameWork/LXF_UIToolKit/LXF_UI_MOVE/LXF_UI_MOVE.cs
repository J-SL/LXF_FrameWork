using DG.Tweening;
using LXF_Framework;
using LXF_Framework.DependencyInjection;
using LXF_Framework.MonoYield;
using System;
using UnityEngine;

public class LXF_UI_MOVE : LXF_MonoYield
{
    /// <summary>
    /// VERTICAL: Move the UI object vertically to a target Y position
    /// HORIZONTAL: Move the UI object horizontally to a target X position
    /// BOTH: Move the UI object to a specific position
    /// </summary>
    public enum MoveType
    {
        VERTICAL,
        HORIZONTAL,
        BOTH
    }


    #region Inject Fields
    [LXF_Inject(InjectionMode.Self)]
    readonly RectTransform _rectTransform;
    #endregion

    #region public Fields
    public EventRunner OnMoveEndEvent = new EventRunner();
    #endregion

    #region private Fields
    private Func<Tween> onMoveFunc;
    private Tween moveTween;
    private Vector2 originalPosition;

    [SerializeField] private bool isMoveOnStart = false;

    [Space(10)]

    [Header("Move Type")]
    [SerializeField]
    private MoveType moveType;

    [SerializeField] private float moveXDistance = 0;
    [SerializeField] private float moveYDistance = 0;

    [SerializeField]
    private Vector2 moveTargetPosition;


    [SerializeField] private float moveDuration = 0.5f;
    #endregion

    void Start()
    {
        originalPosition = _rectTransform.anchoredPosition;

        switch (moveType)
        {
            case MoveType.VERTICAL:
                onMoveFunc = () => MoveY();
                break;
            case MoveType.HORIZONTAL:
                onMoveFunc = () => MoveX();
                break;
            case MoveType.BOTH:
                onMoveFunc = () => MoveToTargetPosition(moveTargetPosition);
                break;
            default:
                break;
        }

        if (isMoveOnStart)
        {
            OnExecute();
        }
    }

    #region public Methods
    public Tween UIMove()
    {
        return OnExecute();
    }

    public Tween BackToOriginalPosition()
    {
        return MoveToTargetPosition(originalPosition);
    }

    #endregion

    #region private Methods

    private Tween OnExecute()
    {
        return onMoveFunc?.Invoke();
    }

    private Tween MoveToTargetPosition(Vector2 targetPosition)
    {
        if(moveTween!= null && moveTween.IsPlaying()) moveTween.Kill();

        moveTween = _rectTransform.DOAnchorPos(targetPosition, moveDuration);
        moveTween.OnComplete(() =>
        {
            OnMoveEndEvent.Run();
        });

        return moveTween;
    }

    private Tween MoveX()
    {
        if (moveTween != null && moveTween.IsPlaying()) moveTween.Kill();

        moveTween = _rectTransform.DOAnchorPosX(moveXDistance, moveDuration);
        moveTween.OnComplete(() =>
        {
            OnMoveEndEvent.Run();
        });

        return moveTween;
    }

    private Tween MoveY()
    {
        if (moveTween != null && moveTween.IsPlaying()) moveTween.Kill();

        moveTween = _rectTransform.DOAnchorPosY(moveYDistance, moveDuration);

        moveTween.OnComplete(() =>
        {
            OnMoveEndEvent.Run();
        });

        return moveTween;
    }
    #endregion
}
