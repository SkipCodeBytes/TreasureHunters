using System.Collections;
using UnityEngine;

public class ParticleAutoDisable : MonoBehaviour
{
    private ParticleSystem ps;

    void OnEnable()
    {
        ps = GetComponent<ParticleSystem>();
        ps.Play();
        StartCoroutine(WaitToDeactivate());
    }

    private IEnumerator WaitToDeactivate()
    {
        // Espera hasta que termine el sistema
        yield return new WaitWhile(() => ps.IsAlive(true));
        gameObject.SetActive(false);
    }
}
