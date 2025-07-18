using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public Transform target; // Car
    public Vector3 offset = new Vector3(0, 1, -6); // Car 뒤쪽, 위쪽

    void LateUpdate()
    {
        if (target != null)
        {
            // 1️⃣ target의 회전을 고려한 오프셋 위치
            transform.position = target.position + target.rotation * offset;

            // 2️⃣ 항상 target 바라보게
            transform.LookAt(target);
        }
    }
}
