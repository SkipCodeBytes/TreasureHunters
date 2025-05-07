using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CinematicAnimation : MonoBehaviour
{
    //Movimiento PingPong
    //Transision de color
    //Movimiento por velocidad y duración
    //Movimiento por lista de puntos
    
    static public IEnumerator RotateToDirection(Transform affectedTransform, Vector3 direction, float time, Action callback = null) =>
        InternalRotate(affectedTransform, Quaternion.LookRotation(direction) , time, callback);

    static public IEnumerator RotateTo(Transform affectedTransform, Transform target, float time, Action callback = null) =>
        InternalRotate(affectedTransform, Quaternion.LookRotation(target.position) , time, callback);
    
    static public IEnumerator RotateTo(Transform affectedTransform, Quaternion rotation, float time, Action callback = null) => 
        InternalRotate(affectedTransform, rotation , time, callback);

    static public IEnumerator RotateTo(GameObject affectedObject, Vector3 direction, float time, Action callback = null) =>
        InternalRotate(affectedObject.transform, Quaternion.LookRotation(direction) , time, callback);

    static public IEnumerator RotateTo(GameObject affectedObject, Transform target, float time, Action callback = null) =>
        InternalRotate(affectedObject.transform, Quaternion.LookRotation(target.position) , time, callback);
    
    static public IEnumerator RotateTo(GameObject affectedObject, Quaternion rotation, float time, Action callback = null) => 
        InternalRotate(affectedObject.transform, rotation , time, callback);

    public static IEnumerator RotateToPoint(Transform affectedTransform, Vector3 pointPosition, float speed, Action callback = null)
    {
        Vector3 direction = (pointPosition - affectedTransform.position).normalized;

        if (direction == Vector3.zero)
        {
            callback?.Invoke();
            yield break;
        }

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        float angleDifference = Quaternion.Angle(affectedTransform.rotation, targetRotation);

        if (angleDifference < 0.1f)
        {
            callback?.Invoke();
            yield break;
        }

        float duration = angleDifference / speed;

        yield return InternalRotate(affectedTransform, targetRotation, duration, callback);
    }

    private static IEnumerator InternalRotate(Transform affectedTransform, Quaternion rotation, float time, Action callback)
    {
        Quaternion initRotation = affectedTransform.rotation;
        float t = 0f;

        while (t < time)
        {
            affectedTransform.rotation = Quaternion.Slerp(initRotation, rotation, t / time);
            t += Time.deltaTime;
            yield return null;
        }

        affectedTransform.rotation = rotation;
        callback?.Invoke();
    }

    public static IEnumerator EulerRotation(Transform affectedTransform, Vector3 eulerValues, float time, Action callback = null)
    {
        float t = 0f;
        Quaternion rotInicial = affectedTransform.rotation;
        Quaternion rotFinal = rotInicial * Quaternion.Euler(eulerValues.x, eulerValues.y, eulerValues.z);

        while (t < time)
        {
            t += Time.deltaTime;
            affectedTransform.rotation = Quaternion.Euler(0, Mathf.Lerp(0, 360, t/time), 0);
            yield return null;
        }
        affectedTransform.rotation = rotFinal;
        callback?.Invoke();
    }


    static public IEnumerator MoveTo(GameObject affectedObject, Vector3 target, float time, Action callback = null) =>
        InternalMove(affectedObject.transform, target, time, callback);

    static public IEnumerator MoveTo(Transform affectedTransform, Vector3 target, float time, Action callback = null) =>
        InternalMove(affectedTransform, target, time, callback);
    static public IEnumerator Move(Transform affectedTransform, Vector3 target, float speed, Action callback = null)
    {
        float duration = 0;
        duration = (affectedTransform.position - target).magnitude / speed;
        yield return InternalMove(affectedTransform, target, duration, callback);
    }
        

    static private IEnumerator InternalMove(Transform affectedTransform, Vector3 target, float time, Action callback){
        float t = 0;
        Vector3 origin = affectedTransform.position;
        while (t < time) {
            t += Time.deltaTime;
            affectedTransform.position = Vector3.Lerp(origin, target, t/time);
            yield return null;
        }
        affectedTransform.position = target;
        if (callback != null) { callback?.Invoke(); }
    }
    public static IEnumerator MoveTo(RectTransform rect, Vector2 target, float time, Action callback = null)
    {
        Vector2 start = rect.anchoredPosition;
        float elapsed = 0f;
        while (elapsed < time)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / time);
            rect.anchoredPosition = Vector2.Lerp(start, target, t);
            yield return null;
        }
        rect.anchoredPosition = target;
        callback?.Invoke();
    }


    static public IEnumerator ParabolicMotion(Transform affectedTransform, Vector3 target, float time, float height, Action callback = null) =>
        InternalParabolicMotion(affectedTransform, target, height, time, callback);

    static public IEnumerator ParabolicMotion(Transform affectedTransform, Vector3 target, float time, Action callback = null){
        float height =  Vector3.Distance(affectedTransform.position, target)/ 2;
        return InternalParabolicMotion(affectedTransform, target, height, time, callback);
    }
    
    static private IEnumerator InternalParabolicMotion(Transform affectedTransform, Vector3 target, float height, float time, Action callback){
        float t = 0;
        Vector3 origin = affectedTransform.position;
        while (t < time) {
            t += Time.deltaTime;

            affectedTransform.position = ParabolicInterpolation(origin, target, height, t / time);
            yield return null;
        }
        affectedTransform.position = target;
        if (callback != null) { callback?.Invoke(); }
    }

    private static Vector3 ParabolicInterpolation(Vector3 start, Vector3 end, float height, float t)
    {
        Vector3 horizontalPosition = Vector3.Lerp(start, end, t);
        float parabola = 4 * height * t * (1 - t);
        return new Vector3(horizontalPosition.x, parabola + Mathf.Lerp(start.y, end.y, t), horizontalPosition.z);
    }



    static public IEnumerator ImageAlphaLerp(Image img, float alphaTarget, float time, Action callback = null)
    {
        float t = 0;
        float originAlpha = img.color.a;
        Color color = img.color;
        while (t < time)
        {
            t += Time.deltaTime;
            color.a = Mathf.Abs(originAlpha + ((alphaTarget - originAlpha) * t / time));
            img.color = color;
            yield return null;
        }
        color.a = alphaTarget;
        img.color = color;
        if (callback != null) { callback?.Invoke(); }
    }

    static public IEnumerator TextTypewriter(Text textUI, string txt, float timeBetween, Action callback = null)
    {
        foreach (char c in txt)
        {
            textUI.text += c;
            yield return new WaitForSeconds(timeBetween);
        }

        callback?.Invoke();
    }


    static public IEnumerator FieldViewLerp(Camera cam, float fieldViewTarget, float time, Action callback = null)
    {
        float t = 0;
        float origin = cam.fieldOfView;

        while (t < time)
        {
            t += Time.deltaTime;
            cam.fieldOfView = Mathf.Abs(origin + ((fieldViewTarget - origin) * t / time));
            yield return null;
        }
        cam.fieldOfView = fieldViewTarget;
        if (callback != null) { callback?.Invoke(); }
    }

    static public IEnumerator WaitTime(float time, Action callback)
    {
        /*
        float t = 0;

        while (t < time)
        {
            t += Time.deltaTime;
            yield return null;
        }*/
        yield return new WaitForSeconds(time);
        if (callback != null) { callback?.Invoke(); }
    }
}
