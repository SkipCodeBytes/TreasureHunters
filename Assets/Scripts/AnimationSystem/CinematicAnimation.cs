using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.UI.Image;

public class CinematicAnimation : MonoBehaviour
{
    //VALORES DE VARIABLES

    //Movimiento PingPong
    //Transision de color
    //Movimiento por velocidad y duración
    //Movimiento por lista de puntos


    //ROTACIÓN
    static public IEnumerator RotateToDirectionFor(Transform affectedTransform, Vector3 direction, float duration, Action callback = null) =>
        InternalRotate(affectedTransform, Quaternion.LookRotation(direction) , duration, callback);
    static public IEnumerator RotateToDirectionFor(GameObject affectedObject, Vector3 direction, float duration, Action callback = null) =>
        InternalRotate(affectedObject.transform, Quaternion.LookRotation(direction), duration, callback);


    static public IEnumerator RotateTowardTheTargetAt(Transform affectedTransform, Transform target, float speed, Action callback = null) =>
        RotateToWorldPoint(affectedTransform, target.position, speed, callback);
    static public IEnumerator RotateTowardTheTargetAt(GameObject affectedObject, Transform target, float speed, Action callback = null) =>
        RotateToWorldPoint(affectedObject.transform, target.position, speed, callback);


    static public IEnumerator RotateToQuaternion(Transform affectedTransform, Quaternion rotation, float duration, Action callback = null) => 
        InternalRotate(affectedTransform, rotation , duration, callback);
    static public IEnumerator RotateToToQuaternion(GameObject affectedObject, Quaternion rotation, float duration, Action callback = null) =>
        InternalRotate(affectedObject.transform, rotation, duration, callback);


    //Deseo que InternalRotate, pero con las restricciones que tiene
    public static IEnumerator RotateToWorldPoint(Transform affectedTransform, Vector3 worldPoint, float speed, Action callback = null)
    {
        Vector3 direction = (worldPoint - affectedTransform.position).normalized;

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

    private static IEnumerator InternalRotate(Transform affectedTransform, Quaternion rotation, float duration, Action callback)
    {
        Quaternion initRotation = affectedTransform.rotation;
        float t = 0f;

        while (t < duration)
        {
            affectedTransform.rotation = Quaternion.Slerp(initRotation, rotation, t / duration);
            t += Time.deltaTime;
            yield return null;
        }

        affectedTransform.rotation = rotation;
        callback?.Invoke();
    }


    public static IEnumerator EulerRotationFor(Transform affectedTransform, Vector3 eulerValues, float time, Action callback = null)
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



    //MOVIMIENTO 
    static public IEnumerator MoveTowardTheTargetFor(GameObject affectedObject, Vector3 target, float duration, Action callback = null) =>
        InternalMove(affectedObject.transform, target, duration, callback);
    static public IEnumerator MoveTowardTheTargetFor(Transform affectedTransform, Vector3 target, float duration, Action callback = null) =>
        InternalMove(affectedTransform, target, duration, callback);
    
    static public IEnumerator MoveTowardTheTargetAt(Transform affectedTransform, Vector3 target, float speed, Action callback = null)
    {
        float duration = 0;
        duration = (affectedTransform.position - target).magnitude / speed;
        yield return InternalMove(affectedTransform, target, duration, callback);
    }
        
    static private IEnumerator InternalMove(Transform affectedTransform, Vector3 target, float duration, Action callback){
        return LerpUtils.LerpVector3(value => affectedTransform.position = value, affectedTransform.position, target, duration, callback);
    }


    //ESCALA
    static public IEnumerator ScaleTo(Transform affectedTransform, Vector3 target, float duration, Action callback)
    {
        return LerpUtils.LerpVector3(value => affectedTransform.localScale = value, affectedTransform.localScale, target, duration, callback);
    }




    //MOVIMIENTO PARABÓLICO
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
        yield return new WaitForSeconds(time);
        if (callback != null) { callback?.Invoke(); }
    }



    public static IEnumerator UiMoveTo(RectTransform rect, Vector2 target, float time, Action callback = null)
    {
        return LerpUtils.LerpVector2(
            value => rect.anchoredPosition = value,
            rect.anchoredPosition,
            target,
            time,
            callback
        );
    }

    public static IEnumerator UiImageAlphaLerp(Image img, float alphaTarget, float time, Action callback = null)
    {
        Color originalColor = img.color;

        return LerpUtils.LerpFloat(
            value =>
            {
                Color c = img.color;
                c.a = value;
                img.color = c;
            },
            originalColor.a,
            alphaTarget,
            time,
            callback
        );
    }

    static public IEnumerator UiTextTypewriter(Text textUI, string txt, float timeBetween, Action callback = null)
    {
        foreach (char c in txt)
        {
            textUI.text += c;
            yield return new WaitForSeconds(timeBetween);
        }

        callback?.Invoke();
    }

}
