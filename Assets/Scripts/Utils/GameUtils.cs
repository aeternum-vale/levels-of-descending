using System;
using System.Collections;
using UnityEngine;

namespace Utils
{
    public static class GameUtils
    {
        public static string GetNameByPath(string path)
        {
            int lastIndexOfSlash = path.LastIndexOf('/');
            return lastIndexOfSlash == -1 ? path : path.Substring(lastIndexOfSlash + 1);
        }
        
        public static IEnumerator AnimateValue(Func<float> get, Action<float> set, float target, float rate = 0.1f) 
        {
            float startDiff = Math.Abs(get() - target);
            float changeSpeed = rate * startDiff;

            while (Math.Abs(get() - target) > changeSpeed)
            {
                float v = get();
                v += changeSpeed * (v - target > 0 ? -1 : 1);
                set(v);

                yield return new WaitForFixedUpdate();
            }
            
            set(target);
        }

        public static Color SetColorAlpha(Color color, float alpha)
        {
            Color c = color;
            return new Color(c.r, c.g, c.b, alpha);
        }
    }
}