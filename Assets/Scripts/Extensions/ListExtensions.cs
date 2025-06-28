using System.Collections.Generic;
using UnityEngine;

public static class ListExtensions
{
    public static void Shuffle<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int randomIndex = Random.Range(i, list.Count);
            (list[i], list[randomIndex]) = (list[randomIndex], list[i]);
        }
    }


    /// <summary>
    /// Devuelve una lista con elementos aleatorios únicos de la lista original.
    /// </summary>
    public static List<T> GetRandomElements<T>(this List<T> source, int cantidad)
    {
        if (source == null)
        {
            Debug.LogWarning("La lista original es nula.");
            return new List<T>();
        }

        List<T> copia = new List<T>(source);
        List<T> resultado = new List<T>();

        int n = Mathf.Min(cantidad, copia.Count);

        for (int i = 0; i < n; i++)
        {
            int indice = Random.Range(0, copia.Count);
            resultado.Add(copia[indice]);
            copia.RemoveAt(indice);
        }

        return resultado;
    }
}
