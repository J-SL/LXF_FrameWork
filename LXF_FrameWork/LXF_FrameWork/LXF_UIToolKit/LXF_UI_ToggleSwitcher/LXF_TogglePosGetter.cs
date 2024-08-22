

using LXF_Framework;
using UnityEngine;

namespace LXF_UIToolKit
{
    public static class LXF_TogglePosGetter
    {
        /// <summary>
        /// When many toggle layout horizontal,and a special toggle is different width from others,
        /// this Method can put all the position of toggles.
        /// To use this method, you should ensure all toggles anchor is center, or center buttom or center top.
        /// 
        /// attention: default section is 0~X. if you not use this section, you should remap the section.
        /// </summary>
        public static float[] GetTogglePos(int totalToggleCount, int specialToggleIndex, float normalToggleWidth, float specialToggleWidth,
            float totalWidthForallToggles,float toggleInterval= 0 )
        {
            if ((totalToggleCount - 1) * normalToggleWidth + specialToggleWidth != totalWidthForallToggles)
            {
                throw new System.Exception("The total width of all toggles is not equal to the sum of normal toggle width and special toggle width.");
            }

            float[] togglePos = new float[totalToggleCount];

            for(int i = 0; i < totalToggleCount; i++)
            {
                if (i == specialToggleIndex) togglePos[specialToggleIndex] = i * (normalToggleWidth + toggleInterval) +
                        (specialToggleWidth / 2);

                if (i < specialToggleIndex)
                {
                    togglePos[i] = (i * (normalToggleWidth + toggleInterval)) + (normalToggleWidth / 2);
                }

                if(i > specialToggleIndex)
                {
                    togglePos[i] = ((i - 1) * (normalToggleWidth + toggleInterval) + (specialToggleWidth + toggleInterval) +
                        (normalToggleWidth / 2));
                }
            }

            return togglePos;
        }

        /// <summary>
        /// could be cusomiized section by LXF_Math.Remap Method.
        /// section is a Vector2, x is start section, y is end section.
        /// </summary>   
        public static float[] GetTogglePosWithRemap(int totalToggleCount, int specialToggleIndex, float normalToggleWidth, float specialToggleWidth,
            float totalWidthForallToggles, Vector2 section, float toggleInterval = 0)
        {
            if ((totalToggleCount - 1) * normalToggleWidth + specialToggleWidth != totalWidthForallToggles)
            {
                throw new System.Exception("The total width of all toggles is not equal to the sum of normal toggle width and special toggle width.");
            }

            float[] togglePos = new float[totalToggleCount];

            for (int i = 0; i < totalToggleCount; i++)
            {
                if (i == specialToggleIndex) togglePos[specialToggleIndex] = LXF_Math.ReMap(i * (normalToggleWidth + toggleInterval) +
                        (specialToggleWidth / 2), 0, totalWidthForallToggles, section.x, section.y);

                if (i < specialToggleIndex)
                {
                    togglePos[i] = LXF_Math.ReMap((i * (normalToggleWidth + toggleInterval)) + (normalToggleWidth / 2), 0, totalWidthForallToggles,
                        section.x, section.y);
                }

                if (i > specialToggleIndex)
                {
                    togglePos[i] = LXF_Math.ReMap(((i - 1) * (normalToggleWidth + toggleInterval) + (specialToggleWidth + toggleInterval) +
                        (normalToggleWidth / 2)), 0, totalWidthForallToggles, section.x, section.y);
                }
            }

            return togglePos;
        }
        
    }
}

