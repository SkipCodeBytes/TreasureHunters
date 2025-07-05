using UnityEngine;

public class Billboard : MonoBehaviour
{
    void LateUpdate()
    {
        if (Camera.main != null)
        {
            Vector3 direction = transform.position - Camera.main.transform.position;
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }
}
