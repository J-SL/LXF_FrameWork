using System.Collections.Generic;
using UnityEngine;
using LXF_Framework.LXF_UISystem;
using LXF_UIToolKit.UITools;
using LXF_Framework.LXF_UISystem.OperateRect;
using UnityEngine.UI;
using TMPro;
using LXF_Framework.LXF_UISystem.Operator;
using LXF_Framework.TweenSystem;
using System;

namespace LXF_UIToolKit
{
    public abstract class MainUIManage: LXF_TimerManage
    {

    }
    public static class Protection
    {
        public static void ButtonProtection(Button b,Action toExecute,float executeTime)
        {
            b.enabled = false;
            toExecute?.Invoke();
            Invoker.DelayInvoke(executeTime+0.1f, () => b.enabled = true);
        }
    }
    namespace UIInterface
    {

    }
    namespace UITools
    {
        public static class Tools
        {
            /// <summary>
            /// 传入GetPosRelativedCanvas获得的值
            /// </summary>
            /// <returns>AnchoredPosition</returns>
            public static Vector2 GetDynamicAnchoredPosition(Vector2 posRelativedCanvas, RectTransform canvas) =>
                new Vector2(posRelativedCanvas.x * canvas.rect.width, posRelativedCanvas.y * canvas.rect.height);
        }

        ///////////////////////////////////////////////////////Can be used directly methods/////////////////////////////////////////////////////////

        public static class Switch
        {
            public static void Open(this RectTransform r) => r.gameObject.SetActive(true);
            public static void Close(this RectTransform r) => r.gameObject.SetActive(false);
            public static void MoveToOpen(this RectTransform r, Vector2 pos, float duration)
            {
                var(imgs,texts)=FindAllChildrenImageAndText.GetAllChildrenImageAndText(r);
                Open(r);
                OperateImage.TweenColor_Opaque(imgs, duration);
                OperateText.TweenColor_Opaque(texts, duration);
                r.TweenPos(pos, duration);
            }
            public static void MoveToClose(this RectTransform r, Vector2 pos, float duration)
            {
                var (imgs, texts) = FindAllChildrenImageAndText.GetAllChildrenImageAndText(r);
                OperateImage.TweenColor_Transparent(imgs, duration);
                OperateText.TweenColor_Transparent(texts, duration);
                r.TweenPos(pos, duration, () => r.Close());
            }
            public static void CenterToOpen(this RectTransform r, float duration)
            {
                r.Open();
                r.ScaleTween(new Vector2(1,1), duration);
            }
            public static void CenterToClose(this RectTransform r, float duration)
            {               
                r.ScaleTween(new Vector2(0, 0), duration,()=> r.Close());                 
            }

            /// <summary>
            /// 得到所有已激活的子物体
            /// </summary>
            public static List<GameObject> GetActiveChildren(this RectTransform r)
            {
                List<GameObject> children = new List<GameObject>();
                int childCount = r.childCount;

                for (int i = 0; i < childCount; i++)
                {
                    GameObject child = r.GetChild(i).gameObject;
                    if(child.activeSelf) children.Add(child);
                }

                return children;
            }
        }

        public static class StretchBar
        {
            public static void StretchBarWidth(RectTransform bar, float recrMin, float rectMax, float differenceWidth,float duration)
            {
                bar.WidthTween(bar.GetWidth()+differenceWidth,duration);
            }
            public static void StretchBarWidth(RectTransform bar, float recrMin, float rectMax, float differenceWidth, float duration,Action callback)
            {
                bar.WidthTween(bar.GetWidth() + differenceWidth, duration, callback);
            }
            /// <summary>
            /// 默认pivot是左边,注意使用此方法时锚点不可是拉伸状态
            /// </summary>
            /// <param name="mian">主Bar</param>
            /// <param name="mask">副Bar</param>
            /// <param name="percentage">百分比Width</param>
            public static void DoubleBarWidthBuffer(RectTransform main,RectTransform mask,float differenceWidth, float duration)
            {
                var slowBar = differenceWidth >= 0 ? mask : main;
                var quickBar = differenceWidth >= 0 ? main : mask;
                slowBar.WidthTween(slowBar.GetWidth() + differenceWidth, duration,()=> quickBar.SetWidth(slowBar.GetWidth()));
            }
            /// <summary>
            /// 可限制rect的Width最小值和最大值
            /// </summary>
            public static void DoubleBarWidthBuffer(RectTransform main, RectTransform mask,float recrMin,float rectMax,
                float differenceWidth, float duration)
            {
                var slowBar = differenceWidth >= 0 ? mask : main;
                var quickBar = differenceWidth >= 0 ? main : mask;
                slowBar.WidthTween(Mathf.Clamp(slowBar.GetWidth() + differenceWidth,recrMin,rectMax), duration, () => quickBar.SetWidth(slowBar.GetWidth()));
            }

            /// <summary>
            /// 使背景图片随Text文本框向下延展
            /// </summary>
            public static void AdaptiveTextBox(RectTransform img,RectTransform text,float gap)=>img.SetHeight(text.GetHeight()+gap);
            public static void AdaptiveTextBox(RectTransform img, float gap) => img.SetHeight(img.GetComponentInChildren<RectTransform>().GetHeight() + gap);

        }

        public static class DropDown
        {
            public static void DisAppearAllElements(Queue<RectTransform> imgs)
            {
               foreach(var img in imgs)
                {
                    var (im, texts) = FindAllChildrenImageAndText.GetAllChildrenImageAndText(img);
                    foreach (var i in im)
                    {
                        i.DisAppear();
                    }
                    foreach (var t in texts)
                    {
                        t.DisAppear();
                    }
                }
            }
            public static void DisAppearAllElements(Queue<RectTransform> imgs,float disappearTime)
            {
                foreach (var img in imgs)
                {
                    var (im, texts) = FindAllChildrenImageAndText.GetAllChildrenImageAndText(img);
                    foreach (var i in im)
                    {
                        i.DisAppear(disappearTime);
                    }
                    foreach (var t in texts)
                    {
                        t.DisAppear(disappearTime);
                    }
                }
            }
            public static void ImgsDropDown(Queue<RectTransform> imgs,Vector2 originPos,float arriveTime,float nextImgDeltaTime,float appearTime, float nextImgDeltaAppearTime, float gap=0)
            {
                if(imgs.Count == 0) return;
                var img=imgs.Dequeue();              
                var (im, texts) = FindAllChildrenImageAndText.GetAllChildrenImageAndText(img);
                if(im.Length>0)
                    foreach (var i in im)
                    {
                        i.Appear(appearTime);
                    }
                if(texts.Length>0)
                    foreach (var t in texts)
                    {
                        t.Appear(appearTime);
                    }

                img.TweenPos(originPos, arriveTime);
                arriveTime += nextImgDeltaTime;
                originPos = new Vector2(originPos.x, originPos.y - img.GetHeight() - gap);
                appearTime += nextImgDeltaAppearTime;
                ImgsDropDown(imgs,originPos, arriveTime, nextImgDeltaTime,appearTime, nextImgDeltaAppearTime,gap);
            }
            public static void ImgsWithdraw(Queue<RectTransform> imgs,  float arriveTime, float nextImgDeltaTime, float disAppearTime, float nextImgDeltaDisAppearTime, Vector2 originPos=default)
            {
                if (imgs.Count == 0) return;
                var img = imgs.Dequeue();

                var (im, texts) = FindAllChildrenImageAndText.GetAllChildrenImageAndText(img);
                if (im.Length>0)
                    foreach (var i in im)
                    {
                        i.DisAppear(disAppearTime);
                    }
                if(texts.Length>0)
                    foreach (var t in texts)
                    {
                        t.DisAppear(disAppearTime);
                    }

                img.TweenPos(originPos, arriveTime, () =>
                {
                    img.gameObject.SetActive(false);
                });
                arriveTime += nextImgDeltaTime;
                disAppearTime += nextImgDeltaDisAppearTime;
                ImgsWithdraw(imgs, arriveTime, nextImgDeltaTime, disAppearTime, nextImgDeltaDisAppearTime,originPos);
            }
            public static void SetActiveDropDown(Queue<RectTransform> imgs,bool b)
            {
                foreach (var img in imgs)
                {
                   img.gameObject.SetActive(b);
                }
            }
        }

        public static class LXF_Font
        {
            public static void GetRandomColor(this TMP_Text t)
            {
                Color32[] cornerColors = new Color32[4];

                for (int i = 0; i < 4; i++)
                {
                    cornerColors[i] = new Color32(
                        (byte)UnityEngine.Random.Range(0, 256),
                        (byte)UnityEngine.Random.Range(0, 256),
                        (byte)UnityEngine.Random.Range(0, 256),
                        255);
                }

                t.SetColorGradient(cornerColors);
            }
            public static void Appear(this TMP_Text t,float duration)=>
                t.TweenColor_A(1, duration);
            public static void Appear(this TMP_Text t) =>
                t.SetColor(new Color(t.GetColor_R(), t.GetColor_G(), t.GetColor_B(), 1));
            public static void DisAppear(this TMP_Text t, float duration) =>
                t.TweenColor_A(0, duration);
            public static void DisAppear(this TMP_Text t) =>
               t.SetColor(new Color(t.GetColor_R(), t.GetColor_G(), t.GetColor_B(), 0));
        }

        public static class LXF_Image
        {
            public static void Appear(this Image i, float duration) =>
               i.TweenColor_A(1, duration);
            public static void Appear(this Image i) => 
                i.SetColor(new Color(i.GetColor_R(), i.GetColor_G(), i.GetColor_B(), 1));
            public static void DisAppear(this Image i, float duration) =>
                i.TweenColor_A(0, duration);
            public static void DisAppear(this Image i) =>
               i.SetColor(new Color(i.GetColor_R(), i.GetColor_G(), i.GetColor_B(), 0));
        }

        ///////////////////////////////////////////////////////Can be used directly methods/////////////////////////////////////////////////////////

        /////////////////////////////////////////////////////////Customize Methods////////////////////////////////////////////////////////////////////////////


        public static class FindAllChildrenImageAndText
        {
            /// <summary>
            /// 返回所有子物体的组件，可选择是否包含自身。
            /// </summary>
            public static (Image[], TMP_Text[]) GetAllChildrenImageAndText(RectTransform r,bool includeSelf=true)
            {
                if(!includeSelf)
                {
                    var imgList1 = r.GetComponentsInChildren<Image>();
                    var texList1 = r.GetComponentsInChildren<TMP_Text>();
                    List<Image> imgs = new();
                    List<TMP_Text> texts = new();
                    foreach ( var img in imgList1)
                    {
                        if(img.GetComponent<RectTransform>() != r)
                            imgs.Add(img);
                    }
                    foreach (var text in texList1)
                    {
                        if (text.GetComponent<RectTransform>() != r)
                            texts.Add(text);
                    }

                    return (imgs.ToArray(), texts.ToArray());   
                }
                var imgList2 = r.GetComponentsInChildren<Image>();
                var texList2 = r.GetComponentsInChildren<TMP_Text>();
                return (imgList2, texList2);
            }

            /// <summary>
            /// 不包含自身，且只返回已激活的子物体组件
            /// </summary>
            public static (Image[], TMP_Text[]) GetAllActiveChildrenImageAndText(RectTransform r)
            {            
                var children = r.GetActiveChildren();

                var imgList = new List<Image>();
                var textList = new List<TMP_Text>();

                foreach (var child in children)
                {
                    var list1 = child.GetComponentsInChildren<Image>();
                    var list2 = child.GetComponentsInChildren<TMP_Text>();
                    if(list1.Length != 0)
                    {
                        foreach( var img in list1)
                        {
                            imgList.Add(img);
                        }
                    }

                    if(list2.Length != 0)
                    {
                        foreach(var text in list2)
                        {
                            textList.Add(text);
                        }
                    }
                }

                return (imgList.ToArray(), textList.ToArray());
            }
        }

        public static class Move
        {
            public static void MoveTo(this RectTransform r, Vector2 pos, float duration) =>
                r.TweenPos(pos, duration);
        }

        public static class Rotate
        {
            public static void Rotation(this RectTransform r, float Z, float duration) => r.RotationTweenDifference(Z, duration);
            public static void RotateZHalfOnce(this RectTransform r, float duration)=> r.RotationTweenDifference(180, duration);
            public static void RotateZHalfOnce(this RectTransform r, float duration, Action callback) 
                => r.RotationTweenDifference(180, duration, callback);
            public static void RotateZOnce(this RectTransform r, float duration) => r.RotationTweenDifference(360, duration);
            public static void RotateZOnce(this RectTransform r, float duration, Action callback)
                => r.RotationTweenDifference(360, duration, callback);
        }

        public static class Scale
        {
            public static void ShrinkToDisappear(this RectTransform r,float duration)
            {
                var s = r.GetLocalScaleXY();
                r.ScaleTween(0, duration, () =>
                {
                    r.SetLocalScaleXY(s);
                });
            }
            public static void ShrinkToDisappear(this RectTransform r, float duration,Action callback)
            {
                var s = r.GetLocalScaleXY();
                r.ScaleTween(0, duration, () =>
                {
                    r.SetLocalScaleXY(s);
                    callback();
                });
            }

        }

        /////////////////////////////////////////////////////////Customize Methods////////////////////////////////////////////////////////////////////////////
    }
    namespace UIPrefab
    {
       public static class DropDownPrefab
        {
            /// <summary>
            /// 在指定父物体下成一系列下拉菜单
            /// </summary>
            /// <param name="list">存储UI的列表</param>
            /// <param name="num">需生成的UI数量</param>
            /// <param name="originPos">相对目标父物体生成的位置</param>
            /// <param name="arriveTime"生成UI到达指定位置的时间></param>
            /// <param name="nextImgDeltaTime">生成的相邻的UI到达目的地的时间间隔</param>
            /// <param name="appearTime">UI浮现的时长</param>
            /// <param name="nextImgDeltaAppearTime">与生成的相邻的UI浮现的时间之差</param>
            /// <param name="disAppearTime">UI渐隐的时长</param>
            /// <param name="nextImgDeltaDisAppearTime">与生成的相邻的UI渐隐的时间之差</param>
            /// <param name="gap">相邻UI的间隔</param>
            public static void SimpleDropDownMenu(List<RectTransform> list, int num,
                Vector2 originPos,  float arriveTime, float nextImgDeltaTime, float appearTime, float nextImgDeltaAppearTime,
                float disAppearTime, float nextImgDeltaDisAppearTime, float gap = 0)
            {
                if (list.Count == 0)
                {
                    return;
                }

                if (!list[0].gameObject.activeSelf)
                {
                    var imgs = new Queue<RectTransform>();

                    for (int i = 0; i < num; i++)
                    {
                        imgs.Enqueue(list[i]);                     
                    }
                    DropDown.SetActiveDropDown(imgs, true);
                    DropDown.DisAppearAllElements(imgs);
                    DropDown.ImgsDropDown(imgs, originPos, arriveTime, nextImgDeltaTime, appearTime, nextImgDeltaAppearTime, gap);
                }
                else
                {
                    Queue<RectTransform> imgs = new();
                    for (int i = 0; i < list.Count; i++)
                    {
                        if (list[i].gameObject.activeSelf)
                        {
                            imgs.Enqueue(list[i]);
                        }
                    }
                    DropDown.ImgsWithdraw(imgs, arriveTime, nextImgDeltaTime, disAppearTime, nextImgDeltaDisAppearTime);
                }
            }

            public static void SimpleDropDownMenu(List<RectTransform> list, int num,
               Vector2 originPos, Action onDrop, Action onBack, float gap = 0)
            {
                if (list.Count == 0)
                {
                    return;
                }

                if (!list[0].gameObject.activeSelf)
                {
                    var imgs = new Queue<RectTransform>();

                    for (int i = 0; i < num; i++)
                    {
                        imgs.Enqueue(list[i]);
                    }
                    DropDown.SetActiveDropDown(imgs, true);
                    DropDown.DisAppearAllElements(imgs);
                    DropDown.ImgsDropDown(imgs, originPos, 0.2f, 0.1f, 0.8f, 0.1f,  gap);
                    onDrop?.Invoke();
                }
                else
                {
                    Queue<RectTransform> imgs = new();
                    for (int i = 0; i < list.Count; i++)
                    {
                        if (list[i].gameObject.activeSelf)
                        {
                            imgs.Enqueue(list[i]);
                        }
                    }
                    DropDown.ImgsWithdraw(imgs, 0.2f, 0.1f, 0.3f, 0.1f);
                    onBack?.Invoke();
                }
            }

        }
    }   
}

