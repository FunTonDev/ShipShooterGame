using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class Common {
    public static IEnumerator lerpAlpha(SpriteRenderer renderer, float lerpTime, bool isPositiveRate = true) {
        Color start = renderer.color;
        Color end = renderer.color;
        end.a = isPositiveRate ? 1.0f : 0.0f;
        
        for (float timer = 0.0f; timer < lerpTime; timer += Time.deltaTime) {
            try {
                renderer.color = Color.Lerp(start, end, timer / lerpTime);            
            }
            catch (Exception e) {
                Debug.LogWarning(string.Format("lerpAlpha: {0}", e.Message));
                break;
            }
            yield return null;
        }
    }

    public static IEnumerator lerpAlpha(Graphic renderer, float lerpTime, bool isPositiveRate = true) {
        Color start = renderer.color;
        Color end = renderer.color;
        end.a = isPositiveRate ? 1.0f : 0.0f;

        for (float timer = 0.0f; timer < lerpTime; timer += Time.deltaTime) {
            try {
                renderer.color = Color.Lerp(start, end, timer / lerpTime);
            }
            catch (Exception e) {
                Debug.LogWarning(string.Format("lerpAlpha: {0}", e.Message));
                break;
            }
            yield return null;
        }
    }
}
