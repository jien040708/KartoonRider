using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public Transform target;      // ���� ��� (��)
    public Vector3 offset = new Vector3(0, 3, -3);  // ī�޶� ��ġ ������

    void LateUpdate()
    {
        if (target != null)
        {
            transform.position = target.position + offset;
            transform.LookAt(target);  // �׻� ���� �ٶ󺸰�
        }
    }
}