using UnityEngine;

public class PortalScript : MonoBehaviour
{
    private Animator _animator;


    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }


    private void OnEnable()
    {
        _animator.Play("SpawnPortal");
    }

    public void DestroyPortal()
    {
        _animator.SetTrigger("Destroy");
    }

    public void HidePortal()
    {
        gameObject.SetActive(false);
    }
}
