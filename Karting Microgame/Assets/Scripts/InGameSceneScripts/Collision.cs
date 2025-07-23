using UnityEngine;

public class KartCollisionBounce : MonoBehaviour
{   
    public float bounceForce = 500f;

    void OnCollisionEnter(Collision collision)
    {
        Rigidbody selfRb = GetComponent<Rigidbody>();
        Rigidbody otherRb = collision.collider.GetComponentInParent<Rigidbody>();

        if (selfRb == null || otherRb == null || otherRb == selfRb) return;

        // 자기 방향 (다른 카트로부터 멀어지는 방향)
        Vector3 dirToSelf = (transform.position - collision.transform.position).normalized;
        Vector3 dirToOther = -dirToSelf;

        // 자기에게 힘
        selfRb.AddForce(dirToSelf * bounceForce, ForceMode.Impulse);

        // 상대에게도 힘 (이거 중요!)
        otherRb.AddForce(dirToOther * bounceForce, ForceMode.Impulse);
        Debug.Log("충돌 감지! " + collision.gameObject.name);

    }
}


