using UnityEngine;

public class SlowRotation : MonoBehaviour
{
    [SerializeField] private Vector3 direction = Vector3.left;
    [SerializeField] private float speed = 0.1f;

    void Update()
    {
        transform.Rotate(direction * speed * Time.deltaTime);
    }
}
