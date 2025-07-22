using UnityEngine;

public class RemotePlayerController : MonoBehaviour
{
    public Vector3 targetPosition;
    public Quaternion targetRotation;

    void Start()
    {
        targetPosition = transform.position;
        targetRotation = transform.rotation;
    }

    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, targetPosition, 10f * Time.deltaTime);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10f * Time.deltaTime);
    }

    public void SetTarget(Vector3 pos, Quaternion rot)
    {
        targetPosition = pos;
        targetRotation = rot;
    }
}
