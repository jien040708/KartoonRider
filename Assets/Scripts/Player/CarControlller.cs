using UnityEngine;

public class RearWheelDrive : MonoBehaviour
{
    public WheelCollider frontLeftWheelCollider;
    public WheelCollider frontRightWheelCollider;
    public WheelCollider rearLeftWheelCollider;
    public WheelCollider rearRightWheelCollider;

    public Transform frontLeftWheelTransform;
    public Transform frontRightWheelTransform;
    public Transform rearLeftWheelTransform;
    public Transform rearRightWheelTransform;

    public float motorTorque = 1500f;
    public float maxSteerAngle = 30f;
    public float driftSteerMultiplier = 1.7f;

    public float normalStiffness = 1.5f;
    public float driftStiffness = 0.4f;

    private float inputVertical;
    private float inputHorizontal;
    private bool isDrifting;

    void Start()
    {
        // 자동으로 Player 태그 설정 (모든 하위 객체 포함)
        SetPlayerTagRecursively(gameObject);
    }

    void SetPlayerTagRecursively(GameObject obj)
    {
        // 현재 객체에 Player 태그 설정
        if (obj.tag != "Player")
        {
            obj.tag = "Player";
            Debug.Log($"'{obj.name}'에 Player 태그가 설정되었습니다.");
        }

        // 모든 하위 객체에도 Player 태그 설정
        foreach (Transform child in obj.transform)
        {
            SetPlayerTagRecursively(child.gameObject);
        }
    }

    void Update()
    {
        inputVertical = Input.GetAxis("Vertical");
        inputHorizontal = Input.GetAxis("Horizontal");

        // ❗ 왼쪽 드리프트도 가능하게
        isDrifting = Input.GetKey(KeyCode.LeftShift) && Mathf.Abs(inputHorizontal) > 0.2f;
    }

    void FixedUpdate()
    {
        ApplyMotor();
        ApplySteering();
        ApplyDriftFriction();
        UpdateWheelVisuals();
    }

    void ApplyMotor()
    {
        if (rearLeftWheelCollider != null)
            rearLeftWheelCollider.motorTorque = inputVertical * motorTorque;
        if (rearRightWheelCollider != null)
            rearRightWheelCollider.motorTorque = inputVertical * motorTorque;
    }

    void ApplySteering()
    {
        // ❗ 드리프트 시 방향 유지하면서 회전 더 크게
        float steerAngle = inputHorizontal * maxSteerAngle;
        if (isDrifting)
            steerAngle *= driftSteerMultiplier;

        if (frontLeftWheelCollider != null)
            frontLeftWheelCollider.steerAngle = steerAngle;
        if (frontRightWheelCollider != null)
            frontRightWheelCollider.steerAngle = steerAngle;
    }

    void ApplyDriftFriction()
    {
        float stiffness = isDrifting ? driftStiffness : normalStiffness;

        if (frontLeftWheelCollider != null)
            SetFriction(frontLeftWheelCollider, stiffness);
        if (frontRightWheelCollider != null)
            SetFriction(frontRightWheelCollider, stiffness);
        if (rearLeftWheelCollider != null)
            SetFriction(rearLeftWheelCollider, stiffness);
        if (rearRightWheelCollider != null)
            SetFriction(rearRightWheelCollider, stiffness);
    }

    void SetFriction(WheelCollider wc, float stiffness)
    {
        WheelFrictionCurve sideFriction = wc.sidewaysFriction;
        sideFriction.stiffness = stiffness;
        wc.sidewaysFriction = sideFriction;

        WheelFrictionCurve forwardFriction = wc.forwardFriction;
        forwardFriction.stiffness = stiffness;
        wc.forwardFriction = forwardFriction;
    }

    void UpdateWheelVisuals()
    {
        if (frontLeftWheelCollider != null && frontLeftWheelTransform != null)
            UpdateWheelPose(frontLeftWheelCollider, frontLeftWheelTransform);
        if (frontRightWheelCollider != null && frontRightWheelTransform != null)
            UpdateWheelPose(frontRightWheelCollider, frontRightWheelTransform);
        if (rearLeftWheelCollider != null && rearLeftWheelTransform != null)
            UpdateWheelPose(rearLeftWheelCollider, rearLeftWheelTransform);
        if (rearRightWheelCollider != null && rearRightWheelTransform != null)
            UpdateWheelPose(rearRightWheelCollider, rearRightWheelTransform);
    }

    void UpdateWheelPose(WheelCollider collider, Transform wheelTransform)
    {
        Vector3 pos;
        Quaternion rot;
        collider.GetWorldPose(out pos, out rot);
        wheelTransform.position = pos;
        wheelTransform.rotation = rot;
    }
}
   