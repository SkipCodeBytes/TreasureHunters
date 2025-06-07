using UnityEngine;

public class BannerScript : MonoBehaviour
{
    private Animator animator;
    private GameManager _gm;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        _gm = GameManager.Instance;
        animator.Play("BannerAwake");
    }

    public void PlayEnd()
    {
        animator.SetTrigger("Destroy");
    }

    public void EndAnimation()
    {
        gameObject.SetActive(false);
    }
}
