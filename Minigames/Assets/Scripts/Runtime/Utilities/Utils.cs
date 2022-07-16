using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Minigames
{
    public static class Utils 
    {
        public static Rect GenerateCameraRect(int viewIndex, int viewCount)
        {
            if (viewCount == 1)
            {
                return new Rect(Vector2.zero, Vector2.one);
            }
            else
            {
                int columnAmount = (int)Mathf.Ceil(viewCount / 2f);
                int columnAmountTop = viewCount - columnAmount;
                if (viewIndex + 1 > columnAmount)
                {
                    return GenerateTopLineRect(viewIndex - columnAmount, columnAmountTop);
                }
                else
                {
                    return GenerateBottomLineRect(viewIndex, columnAmount);
                }
            }
        }

        private static Rect GenerateBottomLineRect(int viewIndex, int columnAmount)
        {
            float heightSize = 0.5f;

            float widthSize = 1f / columnAmount;
            Vector2 position = new Vector2()
            {
                x = viewIndex * widthSize,
                y = 0,
            };

            Vector2 size = new Vector2()
            {
                x = widthSize,
                y = heightSize
            };
            return new Rect(position, size);
        }

        private static Rect GenerateTopLineRect(int viewIndex, int columnAmount)
        {
            float heightSize = 0.5f;

            float widthSize = 1f / columnAmount;
            Vector2 position = new Vector2()
            {
                x = ((columnAmount - 1) - viewIndex) * widthSize,
                y = 0.5f,
            };

            Vector2 size = new Vector2()
            {
                x = widthSize,
                y = heightSize
            };
            return new Rect(position, size);
        }

    }
}
