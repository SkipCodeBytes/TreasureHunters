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

    // Para target fijo (estático)
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
        callback?.Invoke();
    }

    // Para target dinámico (ej: Transform en movimiento)
    public static IEnumerator LerpVector3(Action<Vector3> setter, Vector3 origin, Func<Vector3> dynamicTarget, float duration, Action callback = null)
    {
        float t = 0;
        while (t < duration)
        {
            t += Time.deltaTime;
            Vector3 value = Vector3.Lerp(origin, dynamicTarget(), t / duration);
            setter(value);
            yield return null;
        }
        setter(dynamicTarget());
        callback?.Invoke();
    }

    public static IEnumerator LerpVector3(Action<Vector3> setter, Func<Vector3> originGetter, Func<Vector3> targetGetter, float speed, Action callback = null)
    {
        if (speed <= 0f)
        {
            Debug.LogWarning("Speed must be greater than 0.");
            yield break;
        }

        Vector3 current = originGetter();
        Vector3 target = targetGetter();

        while (Vector3.Distance(current, target) > 0.01f)
        {
            target = targetGetter(); // target dinámico
            current = Vector3.MoveTowards(current, target, speed * Time.deltaTime);
            setter(current);
            yield return null;
        }

        setter(target);
        callback?.Invoke();
    }


}
