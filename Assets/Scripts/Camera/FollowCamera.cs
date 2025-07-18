using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public Transform target;      // 따라갈 대상 (차)
    public Vector3 offset = new Vector3(0, 3, -3);  // 카메라 위치 오프셋

    void LateUpdate()
    {
        if (target != null)
        {
            transform.position = target.position + offset;
            transform.LookAt(target);  // 항상 차를 바라보게
        }
    }
}