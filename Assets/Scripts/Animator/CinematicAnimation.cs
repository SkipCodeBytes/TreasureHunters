using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CinematicAnimation : MonoBehaviour
{

    //Escala en base a velocidad
    //Escala en base a duraci�n

    //Rotaci�n en base a velocidad
    //Rotaci�n en base a duraci�n

    //Movimiento en base a velocidad
    //Movimiento en base a una duraci�n
    static public IEnumerator MoveObject(GameObject obj, Vector3 target, float duration, Action callback = null)
    {
        float t = 0;
        Vector3 origin = obj.transform.position;
        while (t < duration) {
            t += Time.deltaTime;
            obj.transform.position = Vector3.Lerp(origin, target, t/duration);
            yield return null;
        }
        if (callback != null) { callback?.Invoke(); }
    }

    //Transici�n de imagen en opacidad en base a una duraci�n
    static public IEnumerator FadeImage(Image img, float alphaTarget, float duration, Action callback = null)
    {
        float t = 0;
        float originAlpha = img.color.a;
        Color color = img.color;
        while (t < duration)
        {
            t += Time.deltaTime;
            color.a = Mathf.Abs(originAlpha + ((alphaTarget - originAlpha) * t / duration));
            img.color = color;
            yield return null;
        }
        color.a = alphaTarget;
        img.color = color;
        if (callback != null) { callback?.Invoke(); }
    }
}
