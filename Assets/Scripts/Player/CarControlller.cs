using UnityEngine;

public class CarController : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float turnSpeed = 100f;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        float move = Input.GetAxis("Vertical") * moveSpeed;
        float turn = Input.GetAxis("Horizontal") * turnSpeed;

        Vector3 movement = transform.forward * move * Time.fixedDeltaTime;
        Quaternion rotation = Quaternion.Euler(0, turn * Time.fixedDeltaTime, 0);

        rb.MovePosition(rb.position + movement);
        rb.MoveRotation(rb.rotation * rotation);
    }
}