using UnityEngine;
using LXF_Framework.TweenSystem;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using TMPro;

namespace LXF_UISystem
{
    //----------------------------------------base class-----------------------------
    public static class TweenProtection
    {
        public static void Protection(Action toExecute, Func<bool> end)
        {
            if (end()) toExecute();
        }
    }
    public static class LXF_RectTransform
    {
        #region Set
        /// <summary>
        /// An exception may occur if used under Stretch.
        /// </summary>
        public static void SetPos(this RectTransform rect,Vector2 newPos)=>
            rect.anchoredPosition=new Vector2(newPos.x,newPos.y);
        public static void SetPosRelativedCanvas(this RectTransform rect, RectTransform canvas, Vector2 relativedPos) =>
           rect.SetPos(new Vector2(relativedPos.x*canvas.rect.width,relativedPos.y*canvas.rect.height));

        /// <param name="relativedPos">UI与Canvas的相对位置，当UI元素在Canvas的内部时，这个值在（-1，1）之间</param>
        public static void SetPosRelativedCanvas(this RectTransform rect, Vector2 relativedPos)=>
            rect.SetPosRelativedCanvas(rect.GetCanvas(), relativedPos);
         
        public static void SetAnchor(this RectTransform rect, Vector2 anchorMin, Vector2 anchorMax)
        {
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
        }
        /// <summary>
        /// You can choose to enter left/centerY/right/top/centerX/bottom/Full to set anchor pos
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="pos"></param>
        public static void InitStretchAnchor(this RectTransform rect,string pos)
        {
            switch (pos)
            {
                case "left":
                    rect.SetAnchor(new Vector2(0, 0), new Vector2(0, 1));
                    break;
                case "centerY": rect.SetAnchor(new Vector2(0.5f, 0), new Vector2(0.5f, 1));
                    break;
                case "right":
                    rect.SetAnchor(new Vector2(1, 0), new Vector2(1, 1));
                    break;
                case "top":
                    rect.SetAnchor(new Vector2(0, 1), new Vector2(1, 1));
                    break;
                case "centerX":
                    rect.SetAnchor(new Vector2(0, 0.5f), new Vector2(1, 0.5f));
                    break;
                case "bottom":
                    rect.SetAnchor(new Vector2(0, 0), new Vector2(1, 0));
                    break;
                case "Full":
                    rect.SetAnchor(new Vector2(0, 0), new Vector2(1, 1));
                    break;  
                default: throw new System.Exception("Invalid value entered!");
            }
        }
        /// <summary>
        /// The position of the anchor point corresponds to the numerical position of the keypad,
        /// For example, 1 is the lower left corner
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="pos"></param>
        /// <exception cref="System.Exception"></exception>
        public static void InitAnchor(this RectTransform rect, int pos)
        {
            switch (pos)
            {
                case 7:
                    rect.SetAnchor(new Vector2(0, 1), new Vector2(0, 1));
                    break;
                case 8:
                    rect.SetAnchor(new Vector2(0.5f, 1), new Vector2(0.5f, 1));
                    break;
                case 9:
                    rect.SetAnchor(new Vector2(1, 1), new Vector2(1, 1));
                    break;
                case 4:
                    rect.SetAnchor(new Vector2(0, 0.5f), new Vector2(0,0.5f));
                    break;
                case 5:
                    rect.SetAnchor(new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f));
                    break;
                case 6:
                    rect.SetAnchor(new Vector2(1, 0.5f), new Vector2(1, 0.5f));
                    break;
                case 1:
                    rect.SetAnchor(new Vector2(0, 0), new Vector2(0, 0));
                    break;
                case 2:
                    rect.SetAnchor(new Vector2(0.5f, 0), new Vector2(0.5f, 0));
                    break;
                case 3:
                    rect.SetAnchor(new Vector2(1, 0), new Vector2(1, 0));
                    break;
                default: throw new System.Exception("Invalid value entered!");
            }
        }
        public static void SetPivot(this RectTransform rect, Vector2 pos)
        {
            rect.pivot = pos;
        }
        /// <summary>
        /// The position of the pivot point corresponds to the numerical position of the keypad,
        /// For example, 1 is the lower left corner
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="pos"></param>
        /// <exception cref="System.Exception"></exception>
        public static void InitPivot(this RectTransform rect, int pos)
        {
            switch (pos)
            {
                case 7:
                    rect.SetPivot(new Vector2(0, 1));
                    break;
                case 8:
                    rect.SetPivot(new Vector2(0.5f, 1));
                    break;
                case 9:
                    rect.SetPivot(new Vector2(1, 1));
                    break;
                case 4:
                    rect.SetPivot(new Vector2(0, 0.5f));
                    break;
                case 5:
                    rect.SetPivot(new Vector2(0.5f, 0.5f));
                    break;
                case 6:
                    rect.SetPivot(new Vector2(1, 0.5f));
                    break;
                case 1:
                    rect.SetPivot(new Vector2(0, 0));
                    break;
                case 2:
                    rect.SetPivot(new Vector2(0.5f, 0));
                    break;
                case 3:
                    rect.SetPivot(new Vector2(1, 0));
                    break;
                default: throw new System.Exception("Invalid value entered!");
            }
        }

        /// <summary>
        /// Note that operating on the Stretch UI will stretch other properties.
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="targetWidth"></param>
        public static void SetWidth(this RectTransform rect,float targetWidth)=>
            rect.sizeDelta= new Vector2(targetWidth, rect.rect.height);
        /// <summary>
        /// Note that operating on the Stretch UI will stretch other properties.
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="targetWidth"></param>
        public static void SetHeight(this RectTransform rect, float targetHeight)=>
            rect.sizeDelta = new Vector2(rect.rect.width, targetHeight);
        /// <summary>
        /// Only suitable for use when stretching the UI
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="targetTop"></param>
        public static void SetTop(this RectTransform rect, float targetTop)=>
             rect.offsetMax = new Vector2(rect.offsetMax.x, targetTop);
        /// <summary>
        /// Only suitable for use when stretching the UI
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="targetTop"></param>
        public static void SetBottom(this RectTransform rect,float targetBottom)=>
            rect.offsetMin = new Vector2(rect.offsetMin.x, targetBottom);
        /// <summary>
        /// Only suitable for use when stretching the UI
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="targetTop"></param>
        public static void SetLeft(this RectTransform rect, float targetLeft) =>
            rect.offsetMin = new Vector2(targetLeft, rect.offsetMin.y);
        /// <summary>
        /// Only suitable for use when stretching the UI
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="targetTop"></param>
        public static void SetRight(this RectTransform rect, float targetRight) =>
           rect.offsetMax = new Vector2(targetRight, rect.offsetMax.y);

        public static void SetLocalScale(this RectTransform rect, float multipleValue) =>
            rect.localScale = new Vector3(rect.localScale.x * multipleValue, rect.localScale.y * multipleValue, rect.localScale.z * multipleValue);
        public static void SetLocalScaleXY(this RectTransform rect, float multipleValue) =>
           rect.localScale = new Vector2(rect.localScale.x * multipleValue, rect.localScale.y * multipleValue);
        public static void SetLocalScale(this RectTransform rect, Vector3 newScale) =>
          rect.localScale = newScale;
        public static void SetLocalScaleXY(this RectTransform rect, Vector2 newScale) =>
         rect.localScale = newScale;

        public static void SetRotation(this RectTransform rect, Quaternion q) =>
            rect.rotation = Quaternion.Euler(q.eulerAngles);
        public static void SetRotationZ(this RectTransform rect, float Angle)=>
            rect.rotation=Quaternion.Euler(rect.rotation.x, rect.rotation.y, Angle);
        #endregion

        #region Get
        public static (Vector2,Vector2) GetAnchor(this RectTransform rect)=>( rect.anchorMin, rect.anchorMax );
        public static Vector2 GetPos(this RectTransform rect) => rect.anchoredPosition;
        /// <summary>
        /// 获取到UI元素与Canvas的相对值
        /// </summary>
        public static Vector2 GetPosRelativedCanvas(this RectTransform rect, RectTransform canvas) =>
            new Vector2(rect.GetPos().x / canvas.rect.width, rect.GetPos().y / canvas.rect.height);
        /// <summary>
        /// 获取到UI元素与Canvas的相对值
        /// </summary>
        public static Vector2 GetPosRelativedCanvas(this RectTransform rect)=>
            rect.GetPosRelativedCanvas(rect.GetCanvas());    

        public static RectTransform GetCanvas(this RectTransform rect)
        {
            if(rect.parent is null)
            {
                return rect;
            }
            else
            {
                return GetCanvas(rect.parent.GetComponent<RectTransform>());
            }
        }
            
        public static float GetWidth(this RectTransform rect) => rect.sizeDelta.x;
        public static float GetHeight(this RectTransform rect) => rect.sizeDelta.y;
        public static float GetLeft(this RectTransform rect) => rect.offsetMin.x;
        public static float GetRight(this RectTransform rect) => rect.offsetMax.x;
        public static float GetTop(this RectTransform rect) => rect.offsetMax.y;
        public static float GetBottom(this RectTransform rect) => rect.offsetMin.y;

        public static Vector3 GetLocalScale(this RectTransform rect)=> rect.localScale;
        public static Vector2 GetLocalScaleXY(this RectTransform rect) => rect.localScale;

        public static Quaternion GetRotation(this RectTransform rect)=>rect.rotation;
        public static float GetRotationZ(this RectTransform rect) => rect.rotation.z;
        #endregion
    }
    namespace OperateRect
    {
        public static class OperateRectShape
        {
            public static void WidthTween(this RectTransform rect,float targetWidth,float duration)
            {
                TweenFloat.Tween(value => rect.SetWidth(value), rect.GetWidth(), targetWidth, duration);
            }
            public static void WidthTween(this RectTransform rect, float targetWidth, float duration,Action callback)
            {
                TweenFloat.Tween(value => rect.SetWidth(value), rect.GetWidth(), targetWidth, duration, callback);
            }
            public static void HeightTween(this RectTransform rect, float targetWidth, float duration)
            {
                TweenFloat.Tween(value => rect.SetHeight(value), rect.GetHeight(), targetWidth, duration);
            }
            public static void TopTween(this RectTransform rect, float targetWidth, float duration)
            {
                TweenFloat.Tween(value => rect.SetTop(value), rect.GetTop(), targetWidth, duration);
            }
            public static void BottomTween(this RectTransform rect, float targetWidth, float duration)
            {
                TweenFloat.Tween(value => rect.SetBottom(value), rect.GetBottom(), targetWidth, duration);
            }
            public static void LeftTween(this RectTransform rect, float targetWidth, float duration)
            {
                TweenFloat.Tween(value => rect.SetLeft(value), rect.GetLeft(), targetWidth, duration);
            }
            public static void RightTween(this RectTransform rect, float targetWidth, float duration)
            {
                TweenFloat.Tween(value => rect.SetRight(value), rect.GetRight(), targetWidth, duration);
            }
        }

        public static class OperateRectScale
        {
            public static void ScaleTween(this RectTransform rect, float multipleValue, float duration)
            {
                TweenVector2.Tween(value => rect.SetLocalScaleXY(value), rect.GetLocalScaleXY(), rect.GetLocalScaleXY()*multipleValue, duration);
            }
            public static void ScaleTween(this RectTransform rect, float multipleValue, float duration, Action callback)
            {
                TweenVector2.Tween(value => rect.SetLocalScaleXY(value), rect.GetLocalScaleXY(), rect.GetLocalScaleXY() * multipleValue, duration, callback);
            }
            public static void ScaleTween(this RectTransform rect, Vector2 targetScale, float duration)
            {
                TweenVector2.Tween(value => rect.SetLocalScaleXY(value), rect.GetLocalScaleXY(), targetScale, duration);
            }
            public static void ScaleTween(this RectTransform rect, Vector2 targetScale, float duration,Action callback)
            {
                TweenVector2.Tween(value => rect.SetLocalScaleXY(value), rect.GetLocalScaleXY(), targetScale, duration, callback);
            }
        }

        public static class OperateRectRotation
        {
            /// <summary>
            /// 旋转角度（目标角度）
            /// </summary>
            public static void RotationTween(this RectTransform rect,Quaternion targetRotation,float duration)
            {
                TweenQuaternion.Tween(q=>rect.SetRotation(q), rect.GetRotation(), targetRotation, duration);
            }
            /// <summary>
            /// 仅绕Z轴旋转角度（Z的目标角度）
            /// </summary>
            public static void RotationTween(this RectTransform rect, float angleZ, float duration)
            {
                TweenFloat.Tween(q => rect.SetRotationZ(q), rect.GetRotationZ(), angleZ, duration);
            }
            /// <summary>
            /// 仅绕Z轴旋转角度（目标角度至当前角度的差值）
            /// </summary>
            public static void RotationTweenDifference(this RectTransform rect, float angleZ, float duration)
            {
                TweenFloat.Tween(q => rect.SetRotationZ(q), rect.GetRotationZ(), rect.GetRotationZ()+angleZ, duration);
            }
            public static void RotationTweenDifference(this RectTransform rect, float angleZ, float duration,Action callback)
            {
                TweenFloat.Tween(q => rect.SetRotationZ(q), rect.GetRotationZ(), rect.GetRotationZ()+angleZ, duration,callback);
            }          
            /// <summary>
            /// 旋转角度（目标角度至当前角度的差值）
            /// </summary>
            public static void RotationTweenDifference(this RectTransform rect, Quaternion targetRotation, float duration)
            {
                TweenQuaternion.Tween(q => rect.SetRotation(q), rect.GetRotation(),rect.GetRotation()*targetRotation, duration);
            }
        }

        public static class OperateRectPos
        {
            public static void TweenPos(this RectTransform rect,Vector2 targetPos,float duration)
            {
                TweenVector2.Tween(value => rect.SetPos(value), rect.GetPos(), targetPos, duration);
            }
            public static void TweenPos(this RectTransform rect, Vector2 targetPos, float duration, Action callback)
            {
                TweenVector2.Tween(value => rect.SetPos(value), rect.GetPos(), targetPos, duration, callback);
            }
        }
    }
    //----------------------------------------base class-----------------------------




    //////////////////////////////////////////Settings/////////////////////////////////////////////////////////////////////////////////////

    //=======================================Image=============================================
    public static class LXF_Image
    {
        public static void SetSourceImage(this Image i, Sprite s)=>i.sprite = s;
        public static void SetColor(this Image i, Color c)=>i.color = c;
        public static void SetMaterial(this Image i,Material m)=>i.material = m;

        public static Color GetColor(this Image i)=>i.color;
        public static float GetColor_R(this Image i)=> i.color.r;
        public static float GetColor_G(this Image i) => i.color.g;
        public static float GetColor_B(this Image i) => i.color.b;
        public static float GetColor_A(this Image i) => i.color.a;
    }
    //=======================================Image=============================================

    //========================================Text=============================================================
    public static class LXF_Text
    {
        public static void SetColor(this TMP_Text t, Color c) => t.color = c;

        public static Color GetColor(this TMP_Text t) => t.color;
        public static float GetColor_R(this TMP_Text t) => t.color.r;
        public static float GetColor_G(this TMP_Text t) => t.color.g;
        public static float GetColor_B(this TMP_Text t) => t.color.b;
        public static float GetColor_A(this TMP_Text t) => t.color.a;

        public static void SetColor_R(this TMP_Text t) => t.color = new Color32(255, 0, 0, (byte)t.GetColor_A());
        public static void SetColor_G(this TMP_Text t) => t.color = new Color32(0, 255, 0, (byte)t.GetColor_A());
        public static void SetColor_B(this TMP_Text t) => t.color = new Color32(0, 0, 255, (byte)t.GetColor_A());

        public static void SetFontPreset(this TMP_Text t, Material m) => t.fontMaterial = m;
        public static void SetFontBold(this TMP_Text t) => t.fontStyle = FontStyles.Bold;
        public static void SetFontItalic(this TMP_Text t) => t.fontStyle = FontStyles.Italic;    
        public static void SetCenterAlignment(this TMP_Text t) =>t.alignment=TextAlignmentOptions.Center;
        public static void SetColorGradient(this TMP_Text t, Color32[] c)
        {
            t.OpenColorGradient();
            t.colorGradient = new VertexGradient(c[0], c[1], c[2], c[3]);
        }

        public static void OpenFontAutoSize(this TMP_Text t) => t.enableAutoSizing = true;
        public static void OpenColorGradient(this TMP_Text t) => t.enableVertexGradient = true;
        
    }
    //========================================Text=============================================================

    //////////////////////////////////////////Settings/////////////////////////////////////////////////////////////////////////////////////



    //================================================================================================
    //==================================================================================================
    namespace Operator
    {
        //-----------------------------------Color--------------------------------------------------------------
        public static class OperateColor
        {
            public static void TweenColor(Action<Color> originColorSetter,Color originColor, Color targetColor,float duration)
            {
                var c1 = new List<float>() { originColor.r, originColor.g,originColor.b, originColor.a };
                var c2 = new List<float>() { targetColor.r, targetColor.g, targetColor.b, targetColor.a };
                TweenListFloat.Tween(l =>
                {
                    originColorSetter(new Color(l[0], l[1], l[2], l[3]));
                },c1,c2,duration);
            }
           
            public static void TweenColor(Action<float> originColorSetter, float originColor_r, float targetColor_r, float duration)
            {
                TweenFloat.Tween(c=>originColorSetter(c), originColor_r, targetColor_r, duration);
            }
           
        }
        //-----------------------------------Color--------------------------------------------------------------



        //----------------------------------Image--------------------------------------------------------------
        public static class OperateImage
        {
            public static void TweenColor(this Image i,Color targetColor,float duration)
            {
                OperateColor.TweenColor(c=>i.color=c,i.color, targetColor, duration);
            }
            /// <summary>
            /// 虽然去更改TweenFloat.Tween可以避免多开协程，但懒得写了
            /// </summary>
            public static void TweenColor(Image[] imgs, Color targetColor, float duration)
            {
                foreach(Image img in imgs)
                {
                    img.TweenColor(targetColor,duration);
                }
            }
            public static void TweenColor_R(this Image i, float targetColor, float duration)
            {
                OperateColor.TweenColor(c => i.color= new Color(c, i.color.g, i.color.b), i.GetColor_R(), targetColor, duration);
            }
            public static void TweenColor_G(this Image i, float targetColor, float duration)
            {
                OperateColor.TweenColor(c => i.color = new Color(i.color.r, c, i.color.b), i.GetColor_G(), targetColor, duration);
            }
            public static void TweenColor_B(this Image i, float targetColor, float duration)
            {
                OperateColor.TweenColor(c => i.color = new Color(i.color.r, i.color.g, c), i.GetColor_B(), targetColor, duration);
            }
            public static void TweenColor_A(this Image i, float targetColor, float duration)
            {
                OperateColor.TweenColor(c => i.color = new Color(i.color.r, i.color.g, i.color.b,c), i.GetColor_A(), targetColor, duration);
            }
            public static void TweenColor_A(Image[] imgs, float targetColor, float duration)
            {
                foreach (Image img in imgs)
                {
                    img.TweenColor_A(targetColor, duration);
                }
            }
            public static void TweenColor_Transparent(Image[] imgs, float duration)
            {
                foreach (Image img in imgs)
                {
                    img.TweenColor_A(0, duration);
                }
            }
            public static void TweenColor_Opaque(Image[] imgs, float duration)
            {
                foreach (Image img in imgs)
                {
                    img.TweenColor_A(1, duration);
                }
            }
        }
        //----------------------------------Image--------------------------------------------------------------


        //-----------------------------------------------Text--------------------------------------------------------
        public static class OperateText
        {
            public static void TweenColor(this TMP_Text t, Color targetColor, float duration)
            {
                OperateColor.TweenColor(c => t.color = c, t.color, targetColor, duration);
            }
            public static void TweenColor(TMP_Text[] ts, Color targetColor, float duration)
            {
                foreach (var t in ts)
                {
                    t.TweenColor(targetColor, duration);
                }
            }
            public static void TweenColor_A(this TMP_Text t, float targetColor, float duration)
            {
                OperateColor.TweenColor(c => t.color = new Color(t.color.r, t.color.g, t.color.b, c), t.GetColor_A(), targetColor, duration);
            }
            public static void TweenColor_A(TMP_Text[] ts, float targetColor, float duration)
            {
                foreach (TMP_Text t in ts)
                {
                    t.TweenColor_A(targetColor, duration);
                }
            }
            public static void TweenColor_Transparent(TMP_Text[] ts, float duration)
            {
                foreach (TMP_Text t in ts)
                {
                    t.TweenColor_A(0, duration);
                }
            }
            public static void TweenColor_Opaque(TMP_Text[] ts, float duration)
            {
                foreach (TMP_Text t in ts)
                {
                    t.TweenColor_A(1, duration);
                }
            }         
        }
        //-----------------------------------------------Text--------------------------------------------------------
    }
}
