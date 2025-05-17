using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LerpUtils
{
    public static IEnumerator LerpFloat(Action<float> setter, float origin, float target, float duration, Action callback = null)
    {
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float value = Mathf.Lerp(origin, target, t / duration);
            setter(value);
            yield return null;
        }
        setter(target);
        callback?.Invoke();
    }

    public static IEnumerator LerpVector2(Action<Vector2> setter, Vector2 origin, Vector2 target, float duration, Action callback = null)
    {
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            Vector2 value = Vector2.Lerp(origin, target, t / duration);
            setter(value);
            yield return null;
        }
        setter(target);
        callback?.Invoke();
    }

    public static IEnumerator LerpVector3(Action<Vector3> setter, Vector3 origin, Vector3 target, float duration, Action callback = null)
    {
        float t = 0;
        while (t < duration)
        {
            t += Time.deltaTime;
            Vector3 value = Vector3.Lerp(origin, target, t / duration);
            setter(value);
            yield return null;
        }
        setter(target);
        if (callback != null) { callback?.Invoke(); }
    }
}
