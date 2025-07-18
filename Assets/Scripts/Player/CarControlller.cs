using UnityEngine;

public class CarController : MonoBehaviour
{
    public float acceleration = 35f;      // 가속력 (상향)
    public float deceleration = 15f;      // 감속력(브레이크, 상향)
    public float maxSpeed = 55f;          // 최대 속도 (상향)
    public float turnSpeed = 180f;        // 회전 속도 (상향)
    public float driftTurnMultiplier = 1.7f; // 드리프트 시 회전 가중치 (약간 상향)
    public float driftFriction = 0.5f;    // 드리프트 시 마찰력

    public float boostMultiplier = 2.2f;  // 부스트 시 속도 배수 (상향)
    public float boostDuration = 0.7f;    // 부스트 지속 시간(초)
    public float boostInputWindow = 0.3f; // 드리프트 끝나고 이 시간 내에 ↑ 누르면 부스트

    public Transform frontLeftWheel;  // 앞바퀴(FL)
    public Transform frontRightWheel; // 앞바퀴(FR)
    public Transform rearLeftWheel;   // 뒷바퀴(RL)
    public Transform rearRightWheel;  // 뒷바퀴(RR)
    public float wheelTurnAngle = 30f; // 최대 핸들 각도
    public float wheelCircumference = 2.0f; // 바퀴 둘레(임의값, 필요시 조정)
    public float turnPower = 200f; // 차체 회전력(조절 가능)

    private Rigidbody rb;
    private float currentSpeed = 0f;
    private bool isDrifting = false;
    private bool wasDrifting = false;
    private float driftEndTime = 0f;
    private bool isBoosting = false;
    private float boostTimer = 0f;
    private float wheelRotation = 0f;

    void Start()
    {
        Debug.Log("🚗 CarController 시작됨!");
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        float moveInput = Input.GetAxis("Vertical");
        float turnInput = Input.GetAxis("Horizontal");
        bool driftInput = Input.GetKey(KeyCode.LeftShift);

        // 가속/감속
        if (moveInput > 0)
            currentSpeed += moveInput * acceleration * Time.fixedDeltaTime;
        else if (moveInput < 0)
            currentSpeed += moveInput * deceleration * Time.fixedDeltaTime;
        else
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0, deceleration * Time.fixedDeltaTime);

        // 회전/드리프트 시 감속
        float handlingFriction = 1f;
        if (Mathf.Abs(turnInput) > 0.1f)
            handlingFriction += Mathf.Abs(turnInput) * 2f; // 핸들 꺾을수록 감속 증가

        if (driftInput && Mathf.Abs(turnInput) > 0.2f && Mathf.Abs(currentSpeed) > maxSpeed * 0.5f)
        {
            isDrifting = true;
            handlingFriction += 2f; // 드리프트 시 추가 감속
        }
        else if (!driftInput)
        {
            isDrifting = false;
        }

        currentSpeed /= (1f + handlingFriction * Time.fixedDeltaTime);

        // 드리프트 상태 추적 및 종료 시점 기록
        if (isDrifting && !wasDrifting)
        {
            wasDrifting = true;
        }
        else if (!isDrifting && wasDrifting)
        {
            wasDrifting = false;
            driftEndTime = Time.time;
        }

        // 드리프트 종료 후 부스트 입력 체크
        if (!isDrifting && (Time.time - driftEndTime) < boostInputWindow && moveInput > 0 && !isBoosting)
        {
            isBoosting = true;
            boostTimer = boostDuration;
        }

        // 부스트 효과 적용
        float speedMultiplier = 1f;
        if (isBoosting)
        {
            speedMultiplier = boostMultiplier;
            boostTimer -= Time.fixedDeltaTime;
            if (boostTimer <= 0f)
            {
                isBoosting = false;
            }
        }

        // 최고속도 제한
        currentSpeed = Mathf.Clamp(currentSpeed, -maxSpeed * speedMultiplier, maxSpeed * speedMultiplier);

        // 앞바퀴 조향
        float steerAngle = turnInput * wheelTurnAngle;
        // 바퀴 굴러가는 애니메이션
        float distance = currentSpeed * Time.fixedDeltaTime;
        float rotationAmount = (distance / wheelCircumference) * 360f;
        wheelRotation += rotationAmount;

        // 앞바퀴: Y축(조향), X축(굴러감)
        if (frontLeftWheel != null)
            frontLeftWheel.localRotation = Quaternion.Euler(wheelRotation, steerAngle, 0);
        if (frontRightWheel != null)
            frontRightWheel.localRotation = Quaternion.Euler(wheelRotation, steerAngle, 0);

        // 뒷바퀴: X축(굴러감), Y축(항상 0)
        if (rearLeftWheel != null)
            rearLeftWheel.localRotation = Quaternion.Euler(wheelRotation, 0, 0);
        if (rearRightWheel != null)
            rearRightWheel.localRotation = Quaternion.Euler(wheelRotation, 0, 0);

        // 앞바퀴 평균 방향 계산
        Vector3 steerDir = transform.forward;
        if (frontLeftWheel != null && frontRightWheel != null)
            steerDir = (frontLeftWheel.forward + frontRightWheel.forward).normalized;

        // 이동: steerDir 방향으로 힘을 준다
        rb.AddForce(steerDir * currentSpeed, ForceMode.Acceleration);

        // 차체 회전: 앞바퀴 조향 각도와 차체 forward의 각도 차이에 따라 토크 적용
        float angleDiff = Vector3.SignedAngle(transform.forward, steerDir, Vector3.up);
        //rb.AddTorque(Vector3.up * angleDiff * turnPower * Time.fixedDeltaTime, ForceMode.Force);
        rb.AddForce(steerDir * currentSpeed, ForceMode.VelocityChange);
        
    }
}