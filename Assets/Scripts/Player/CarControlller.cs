using UnityEngine;

public class RearWheelDrive : MonoBehaviour
{
    [Header("Wheel Colliders")]
    public WheelCollider frontLeftWheelCollider;
    public WheelCollider frontRightWheelCollider;
    public WheelCollider rearLeftWheelCollider;
    public WheelCollider rearRightWheelCollider;

    [Header("Wheel Mesh Transforms")]
    public Transform frontLeftWheelTransform;
    public Transform frontRightWheelTransform;
    public Transform rearLeftWheelTransform;
    public Transform rearRightWheelTransform;

    [Header("Driving")]
    public float motorTorque = 500f;
    public float maxSteerAngle = 60f;
    public float brakeForce = 1000f;

    [Header("Drifting")]
    public float driftSteerMultiplier = 1.5f;
    public float normalStiffness = 2.0f;
    public float driftStiffness = 1.0f;

    private float inputVertical;
    private float inputHorizontal;
    private bool isDrifting;

    void Start()
    {
        // 초기 마찰력 설정
        ApplyStiffnessToAllWheels(normalStiffness);
        ApplyStiffnessToAllWheels(normalStiffness);
        GetComponent<Rigidbody>().centerOfMass = new Vector3(0, -0.5f, 0); // 중앙, 약간 아래로

    }

    void Update()
    {
        inputVertical = Input.GetAxis("Vertical");
        inputHorizontal = Input.GetAxis("Horizontal");

        // ✅ 방향 무관하게 드리프트 허용 (Shift + 좌우)
        isDrifting = Input.GetKey(KeyCode.LeftShift) && Mathf.Abs(inputHorizontal) > 0.1f;
    }

    void FixedUpdate()
    {
        ApplyMotorTorque();
        ApplySteering();
        ApplyDriftFriction();
        ApplyBrakes();
        UpdateWheelVisuals();
    }

    void ApplyBrakes()
    {
    // 키를 누르지 않을 때(가속 입력 거의 없음) 브레이크 적용
    bool isBraking = Mathf.Abs(inputVertical) < 0.01f;
    float brake = isBraking ? brakeForce : 0f;

    rearLeftWheelCollider.brakeTorque = brake;
    rearRightWheelCollider.brakeTorque = brake;
    frontLeftWheelCollider.brakeTorque = brake;
    frontRightWheelCollider.brakeTorque = brake;
    }

    void ApplyMotorTorque()
    {
        float torque = inputVertical * motorTorque;
        rearLeftWheelCollider.motorTorque = torque;
        rearRightWheelCollider.motorTorque = torque;
    }

    void ApplySteering()
    {
        float steerAngle = inputHorizontal * maxSteerAngle;
        if (isDrifting)
            steerAngle *= driftSteerMultiplier;

        frontLeftWheelCollider.steerAngle = steerAngle;
        frontRightWheelCollider.steerAngle = steerAngle;
    }

    void ApplyDriftFriction()
    {
        float currentStiffness = isDrifting ? driftStiffness : normalStiffness;
        ApplyStiffnessToAllWheels(currentStiffness);
    }

    void ApplyStiffnessToAllWheels(float stiffness)
    {
        SetWheelFriction(frontLeftWheelCollider, stiffness);
        SetWheelFriction(frontRightWheelCollider, stiffness);
        SetWheelFriction(rearLeftWheelCollider, stiffness);
        SetWheelFriction(rearRightWheelCollider, stiffness);
    }

    void SetWheelFriction(WheelCollider collider, float stiffness)
    {
        var forwardFriction = collider.forwardFriction;
        forwardFriction.stiffness = stiffness;
        collider.forwardFriction = forwardFriction;

        var sidewaysFriction = collider.sidewaysFriction;
        sidewaysFriction.stiffness = stiffness;
        collider.sidewaysFriction = sidewaysFriction;
    }

    void UpdateWheelVisuals()
    {
        UpdateSingleWheel(frontLeftWheelCollider, frontLeftWheelTransform);
        UpdateSingleWheel(frontRightWheelCollider, frontRightWheelTransform);
        UpdateSingleWheel(rearLeftWheelCollider, rearLeftWheelTransform);
        UpdateSingleWheel(rearRightWheelCollider, rearRightWheelTransform);
    }

    void UpdateSingleWheel(WheelCollider collider, Transform visual)
    {
        Vector3 pos;
        Quaternion rot;
        collider.GetWorldPose(out pos, out rot);
        visual.position = pos;
        visual.rotation = rot;
    }
}
