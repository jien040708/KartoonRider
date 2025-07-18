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

    private Rigidbody rb;
    private float currentSpeed = 0f;
    private bool isDrifting = false;
    private bool wasDrifting = false;
    private float driftEndTime = 0f;
    private bool isBoosting = false;
    private float boostTimer = 0f;

    void Start()
    {
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

        // 회전
        float turn = turnInput * turnSpeed * Time.fixedDeltaTime;
        if (isDrifting)
        {
            turn *= driftTurnMultiplier;
            rb.linearDamping = driftFriction;
        }
        else
        {
            rb.linearDamping = 1f;
        }

        // 실제 이동 및 회전
        Vector3 movement = transform.forward * currentSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + movement);
        rb.MoveRotation(rb.rotation * Quaternion.Euler(0, turn, 0));
    }
}