using LXF_Framework;
using LXF_Framework.DependencyInjection;
using LXF_Framework.MonoYield;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
/// <summary>
/// A script that can be used to trigger events when the user presses or releases the mouse button over a UI element.
/// It will change properties when press the ui element and will reset the properties when release the ui element.
/// </summary>
public class LXF_POINTER_EVENT_PRESS : LXF_MonoYield, IPointerDownHandler, IPointerUpHandler
{
    public enum TriggerEventType
    {
        Color,
        Size,
        ColorAndSize,
    }
 
    #region Inject Fields
    [LXF_Inject(InjectionMode.Self)]
    readonly Image _image;
    #endregion

    #region public Fields
    public EventRunner OnPointerUpEvent = new EventRunner();
    #endregion

    #region private Fields
    [SerializeField]
    private TriggerEventType TriggerType;
    [SerializeField]
    private Color ChangedColor = Color.white;
    [SerializeField]
    private Vector2 SizeMultiple = Vector2.one;


    Action _onPointerDownAction = new(() => { });
    private Color originalColor;
    private Vector2 originalSize;
    #endregion

    void Start()
    {
        Record();

        switch (TriggerType)
        {
            case TriggerEventType.Color:
                _onPointerDownAction = () => OnChangeColor(ChangedColor);
                break;
            case TriggerEventType.Size:
                _onPointerDownAction = () => OnChangeSize(SizeMultiple);
                break;
            case TriggerEventType.ColorAndSize:
                _onPointerDownAction = () =>
                {
                    OnChangeColor(ChangedColor);
                    OnChangeSize(SizeMultiple);
                };
                break;
            default:
                break;
        }
    }



    #region public Methods
   

    public void OnPointerUp(PointerEventData eventData)
    {
        Reset();
        OnPointerUpEvent?.Run();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnExecute();
    }
    #endregion

    #region private Methods
    private void OnExecute() => _onPointerDownAction?.Invoke();

    private void OnChangeColor(Color color) => _image.color = color;

    private void OnChangeSize(Vector2 size) => transform.localScale *= size;


    private void Record()
    {
        originalColor = _image.color;
        originalSize = transform.localScale;
    }
    
    private void Reset()
    {
        _image.color = originalColor;
        transform.localScale = originalSize;
    }
    #endregion
}
