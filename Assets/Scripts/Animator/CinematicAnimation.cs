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
    
    static public IEnumerator RotateTo(Transform affectedTransform, Vector3 direction, float time, Action callback = null) =>
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

    static public IEnumerator Rotate(Transform affectedTransform, Vector3 direction, float speed, Action callback = null)
    {
        float duration = 0;
        float angleDifference = Quaternion.Angle(affectedTransform.rotation, Quaternion.LookRotation((direction - affectedTransform.position).normalized));
        duration = angleDifference / speed;
        return InternalRotate(affectedTransform, Quaternion.LookRotation(direction), duration, callback);
    }
        

    static private IEnumerator InternalRotate(Transform affectedTransform, Quaternion rotation, float time, Action callback){
        Quaternion initRotation = affectedTransform.rotation;
        float t = 0f;
        while (t < time)
        {
            affectedTransform.rotation = Quaternion.Slerp(initRotation, rotation, t / time);
            t += Time.deltaTime;
            yield return null;
        }
        affectedTransform.rotation = rotation;
        if (callback != null) { callback?.Invoke(); }
    }



    static public IEnumerator MoveTo(GameObject affectedObject, Vector3 target, float time, Action callback = null) =>
        InternalMove(affectedObject.transform, target, time, callback);

    static public IEnumerator MoveTo(Transform affectedTransform, Vector3 target, float time, Action callback = null) =>
        InternalMove(affectedTransform, target, time, callback);
    static public IEnumerator Move(Transform affectedTransform, Vector3 target, float speed, Action callback = null)
    {
        float duration = 0;
        duration = (affectedTransform.position - target).magnitude / speed;
        return InternalMove(affectedTransform, target, duration, callback);
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
        float t = 0;

        while (t < time)
        {
            t += Time.deltaTime;
            yield return null;
        }
        if (callback != null) { callback?.Invoke(); }
    }
}
