using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine.SceneManagement;
using Sirenix.OdinInspector;

public class Player : MonoBehaviour
{
    private bool isTouchTop;
    private bool isTouchBottom;
    private bool isTouchLeft;
    private bool isTouchRight;
    
    [SerializeField] public float moveSpeed = 8;

    [TabGroup("Tab","Jump", SdfIconType.Shift, TextColor = "green")]
    [TabGroup("Tab","Jump")] [SerializeField] private float jumpForce = 7;
    [TabGroup("Tab","Jump")] [SerializeField] private float lowGravity = 2f;
    [TabGroup("Tab","Jump")] [SerializeField] private float highGravity = 4f;
    [TabGroup("Tab","Jump")] [SerializeField] public int maxJumpCount = 1;
    [TabGroup("Tab","Jump")] [SerializeField] private LayerMask groundLayer;
    [TabGroup("Tab","Jump")] [SerializeField] private KeyCode jumpKey = KeyCode.Space;
    private int currentJumpCount;

    [TabGroup("Tab","Gauge", SdfIconType.Toggle2On, TextColor = "Red")]
    [TabGroup("Tab","Gauge")] [SerializeField] public Slider gauge;
    [TabGroup("Tab","Gauge")] [SerializeField] private Slider delaySlider;
    [TabGroup("Tab","Gauge")] [SerializeField] private float recoverySpeed = 0.5f;
    private float targetGauge;

    [TabGroup("Tab","Dash", SdfIconType.Wind, TextColor = "Blue")]
    [TabGroup("Tab","Dash")] [SerializeField] private float dashGauge = 0.37f;
    [TabGroup("Tab","Dash")] [SerializeField] private float dashSpeed = 7f;
    [TabGroup("Tab","Dash")] [SerializeField] private float dashTime = 0.4f;
    [TabGroup("Tab","Dash")] [SerializeField] private GameObject dashParticle;
    [TabGroup("Tab","Dash")] [SerializeField] private float longDashThreshold = 0.2f; // 긴 대시 판단 기준 시간
    [TabGroup("Tab","Life")] [SerializeField] private float dashCooldown;

    private float dashHoldTime = 0f; // 대시 버튼을 누르고 있는 시간
    private bool isDashing;
    private Vector2 dashDirection;

    [TabGroup("Tab","Fly", SdfIconType.Send, TextColor = "White")]
    [TabGroup("Tab","Fly")] [SerializeField] private Sprite flySprite;
    [TabGroup("Tab","Fly")] [SerializeField] private float flyGauge = 0.5f;
    [TabGroup("Tab","Fly")] [SerializeField] private float flyCoolTime = 0.3f;
    [TabGroup("Tab","Fly")] [SerializeField] private float slowFall = 0.15f;
    [TabGroup("Tab","Fly")] [SerializeField] private float rotateSpeed = 1f;
    [TabGroup("Tab","Fly")] [SerializeField] private GameObject flyParticle;
    private GameObject flyingParticle;
    private bool canFlyAgainAfterLanding = true;

    [TabGroup("Tab","Life", SdfIconType.SuitHeart, TextColor = "Magenta")]
    [TabGroup("Tab","Life")] [SerializeField] public int life;
    [TabGroup("Tab","Life")] [SerializeField] public int maxLife;
    // [TabGroup("Tab","Life")] [SerializeField] private TextMeshProUGUI lifeText;
    [TabGroup("Tab","Life")] [SerializeField] private float hitInvincibilityTime = 0.8f;
    [TabGroup("Tab","Life")] [SerializeField] private Image[] lifeImage;

    
    [HideInInspector] public bool isInvincibility;
    [HideInInspector] public bool isDashInvincibility;
    [HideInInspector] public bool isHit;

    [SerializeField] private Collider2D leftBorderCollider;
    [SerializeField] private Collider2D rightBorderCollider;
    private float leftBorder, rightBorder;

    [SerializeField] private CanvasGroup cg;
    private bool flash = true;

    private bool isGrounded;
    private bool isJumpEndArea;
    private Vector2 footPosition;
    private Vector2 footArea;
    private Vector2 airborenCheckArea;

    private Rigidbody2D rigid2D;
    private new Collider2D collider2D;

    private SpriteRenderer spriteRenderer;
    private Animator anim;
    public bool IsLongJump { set; private get; }

    [HideInInspector] public bool isFlying;

    
    private float curTime;
    Material material;

    private float lastDashTime;
    private float airBorneTime;

    private bool dashEndedAndCanFly = false;

    [TabGroup("Tab","Skill", SdfIconType.Lightning, TextColor = "Black")]
    [TabGroup("Tab","Skill")] [SerializeField] public float skillGauge = 0f; // 스킬 게이지
    [TabGroup("Tab","Skill")] [SerializeField] public float maxSkillGauge = 20f; // 최대 스킬 게이지
    [TabGroup("Tab","Skill")] [SerializeField] public float skillGaugeIncreaseRate = 0.35f; // 스킬 게이지 증가 속도
    [TabGroup("Tab","Skill")] [SerializeField] private TextMeshProUGUI skillGaugeText;
    [TabGroup("Tab","Skill")] [SerializeField] private Image skillGaugeImage;
    [TabGroup("Tab","Skill")] [SerializeField] private Image skillImage;
    [TabGroup("Tab","Skill")] [SerializeField] private Image skillCostImage;
    [TabGroup("Tab","Skill")] [SerializeField] private Image skillFades;
    [TabGroup("Tab","Skill")] [SerializeField] private GameObject glowObject; // Glow GameObject 배열
    [TabGroup("Tab","Skill")] [SerializeField] private float[] skillCooldowns = { 16f, 64f }; // Nail 쿨타임 배열
    [TabGroup("Tab","Skill")] [SerializeField] private float[] skillCosts = { 5f, 20f }; // Nail 소모량 배열
    [TabGroup("Tab","Skill")] [SerializeField] private Sprite[] skillSprites; // 스킬 이미지 배열
    [TabGroup("Tab","Skill")][SerializeField] private Sprite[] skillCostSprites;
    [HideInInspector] public bool[] isEquippedSkill = { false, false };
    [HideInInspector] public bool[] canUse = { false, false };
    private float[] skillLastUsedTimes = new float[2]; // Nail 사용 시간 배열

    private void Awake()
    {
        rigid2D = GetComponent<Rigidbody2D>();
        collider2D = GetComponent<Collider2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        material = spriteRenderer.material;

        if (SceneManager.GetActiveScene().name == "Game")
        {
            UpdateLifeIcon(life);
            skillFades.fillAmount = 0;
        }

        if (leftBorderCollider)
            leftBorder = leftBorderCollider.bounds.max.x;

        if (rightBorderCollider)
            rightBorder = rightBorderCollider.bounds.min.x;

        if(skillGaugeImage)
            skillGaugeImage.fillAmount = 0f;
    }

    private void Update()
    {
        RigidBodyController();
        UpdateMove();
        UpdateJump();
        HandleState();
        Flash();

        // 아이템 장착 여부에 따라 skillSprite 교체
        for (int i = 0; i < isEquippedSkill.Length; i++)
        {
            if (isEquippedSkill[i])
            {
                skillImage.sprite = skillSprites[i];
                skillCostImage.sprite = skillCostSprites[i];
                break; // 첫 번째 장착된 스킬만 적용
            }
        }

        // 게이지 회복 로직 
        if (!isFlying && !isDashing && isGrounded)
            gauge.value += recoverySpeed * Time.deltaTime;

        curTime += Time.deltaTime;

        if (!isGrounded && !isFlying)
            airBorneTime += Time.deltaTime;

        if (isGrounded)
            airBorneTime = 0;

        if (!isDashing)
            delaySlider.value = gauge.value;

        // Nail1과 Nail2의 사용 가능 여부 체크
        CheckNailAvailability();

        if(Input.GetKeyDown(KeyCode.Q))
        {
            if (isEquippedSkill[0])
            {
                UseNail1();
            }
            else if (isEquippedSkill[1])
            {
                UseNail2();
            }
        }

        // 스킬 쿨타임에 따른 fillAmount 업데이트
        UpdateSkillFades();
    }

    private void HandleState()
    {
        // 대시 버튼을 눌렀을 때 시간을 초기화하고 첫 번째 대시 실행
        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetMouseButtonDown(1))
        {
            dashHoldTime = 0f;  // 대시 버튼을 누를 때 초기화
            if (isGrounded && !isFlying && !isDashing && Time.time >= lastDashTime + dashCooldown)
            {
                if (!Input.GetKey(KeyCode.W))  // W 키가 눌리지 않은 경우에만 Dash 가능
                {
                    Dash();
                }
            }
        }

        // 대시 버튼을 누르고 있는 동안 시간을 누적
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetMouseButton(1))
        {
            dashHoldTime += Time.deltaTime;  // 대시 버튼을 누르고 있는 시간 누적
        }

        // 대시가 완료되었는지 확인
        if (!isDashing && dashEndedAndCanFly && !isFlying && !isGrounded)
        {
            // 대시가 끝났고, W 키가 눌려 있으면 비행 모드로 전환
            if (Input.GetKey(KeyCode.W))
            {
                Fly(); // Fly 호출
            }
            dashEndedAndCanFly = false; // 비행 모드 전환 후 플래그 초기화
        }

        // 비행 상태 전환
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetMouseButton(1))
        {
            if ((Input.GetKey(KeyCode.W) || !isGrounded || isFlying) && canFlyAgainAfterLanding && !isDashing)
            {
                Fly();
            }
        }

        // 비행 종료
        if ((Input.GetKeyUp(KeyCode.LeftShift) || Input.GetMouseButtonUp(1)) && isFlying)
        {
            EndFly();
        }
    }

    private void UpdateMove()
    {
        float x = Input.GetAxisRaw("Horizontal");

        //spriteRenderer.flipX = (x == 1);

        if(!isDashing)
        {
            if (x == 1)
                spriteRenderer.flipX = true;
            else if (x == -1)
                spriteRenderer.flipX = false;
        }


        if ((isTouchRight && x == 1) || (isTouchLeft && x == -1))
            x = 0;

        if (!isFlying && !isDashing)
            MoveTo(x);
    }

    private void UpdateJump()
    {
        if (Input.GetKeyDown(jumpKey))
            JumpTo();
        else if (Input.GetKey(jumpKey))
            IsLongJump = true;
        else if (Input.GetKeyUp(jumpKey))
            IsLongJump = false;
    }

    void RigidBodyController()
    {
        Bounds bounds = collider2D.bounds;
        footPosition = new Vector2(bounds.center.x, bounds.min.y);
        footArea = new Vector2((bounds.max.x - bounds.min.x) * 0.5f, 0.1f);
        isGrounded = Physics2D.OverlapBox(footPosition, footArea, 0, groundLayer);

        RaycastHit2D hit = Physics2D.Raycast(footPosition, Vector2.down, 1f, groundLayer);

        // Jump_Airborne 상태일 때 Ground에 가까워지면 Jump_End 실행
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Jump_Airborne"))
        {
            if (hit.collider != null && hit.distance < 0.5f)
            {
                anim.SetBool("isAirborne", false); // Jump_End 재생
            }
        }

        if (isGrounded && rigid2D.velocity.y <= 0)
        {
            currentJumpCount = maxJumpCount;
            airBorneTime = 0;  // 땅에 으면 airborneTime 초기화
            canFlyAgainAfterLanding = true;

            // Fly 모드가 활성화 되어 있다면 Fly 종료
            if (isFlying)
            {
                EndFly();
            }
        }

        // Space를 누르고 있고, Y축 속도가 양수일 때 lowGravity 적용
        if ((Input.GetKey(KeyCode.W) || IsLongJump) && rigid2D.velocity.y > 0 && !isFlying)
        {
            rigid2D.gravityScale = lowGravity;
        }
        else
        {
            if (!isFlying)
            {
                rigid2D.gravityScale = highGravity;
            }
        }

        // 공중에 있고 Space 혹은 W를 누르고 있으며, 3초 이하일 때 slowFall 적용
        if (!isGrounded && !isFlying && rigid2D.velocity.y < 0 && airBorneTime <= 3
            && (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.Space)))
        {
            rigid2D.gravityScale = slowFall;
        }
        // 공중에 있는 시간이 3초 이상이면 gravityScale을 lowGravity(2)로 설정
        else if (airBorneTime > 3)
        {
            rigid2D.gravityScale = lowGravity;
        }

        // 땅에 닿지 않고 있으면 airborneTime 증가
        if (!isGrounded && !isFlying)
        {
            airBorneTime += Time.deltaTime;
        }
    }

    private void MoveTo(float x)
    {
        rigid2D.velocity = new Vector2(x * moveSpeed, rigid2D.velocity.y);
        anim.SetBool("isRun", isGrounded && x != 0);
    }

    private void JumpTo()
    {
        if (currentJumpCount > 0)
        {
            anim.SetTrigger("isJump_Start");
            rigid2D.velocity = new Vector2(rigid2D.velocity.x, jumpForce);
            currentJumpCount--;
        }
    }

    private void Dash()
    {
        if (gauge.value >= dashGauge && !isDashing)
        {
            dashEndedAndCanFly = false;  // 초기화
            StartCoroutine(DashCoroutine());
        }
    }

    private IEnumerator DashCoroutine()
    {

        float initialDashFactor = 0.65f;
        float additionalDashFactor = 0.35f;
        float dashPhaseTime = dashTime * initialDashFactor;

        anim.speed = 2f;
        yield return StartCoroutine(PerformDashPhase(initialDashFactor));

        Vector2 lastDashDirection = dashDirection;

        if (Input.GetKey(KeyCode.W) && !isGrounded)
        {
            dashEndedAndCanFly = true;
        }

        if (dashHoldTime >= longDashThreshold)
        {
            anim.speed = 0.7f;
            yield return StartCoroutine(PerformDashPhase(additionalDashFactor, dashDirection));
        }
        else
        {
            if (dashEndedAndCanFly)
            {
                Fly();
            }
        }

        if (lastDashDirection == Vector2.left && Input.GetKey(KeyCode.D))
        {
            yield return StartCoroutine(PerformDashPhase(dashHoldTime >= longDashThreshold ? 0.25f : 0.2f, Vector2.right));
        }
        else if (lastDashDirection == Vector2.right && Input.GetKey(KeyCode.A))
        {
            yield return StartCoroutine(PerformDashPhase(dashHoldTime >= longDashThreshold ? 0.25f : 0.2f, Vector2.left));
        }

    }

    private IEnumerator PerformDashPhase(float dashFactor, Vector2? direction = null)
    {
        if ((gauge.value >= dashGauge * dashFactor) || (dashFactor <= 0.3f))
        {
            if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Dash")) // 애니메이션이 이미 실행 중인지 확인
            {
                anim.SetBool("isDash", true);
            }

            // 게이지 소모 및 이동 거리 계산
            float dashGaugeFactor = dashFactor;

            if (dashFactor > 0.3f)
            {
                targetGauge = gauge.value;
                targetGauge -= dashGauge * dashGaugeFactor;
                DOTween.To(() => gauge.value, x => gauge.value = x, targetGauge, 0.15f)
                    .SetEase(Ease.OutQuint);
            }

            StartCoroutine(DelaySliderValue());

            isDashing = true;
            lastDashTime = Time.time;  // 대시 시작 시간 기록
            StartCoroutine(DashInvincibilityCoroutine());

            // 방향이 주어지지 않으면 기본 방향 사용
            if (direction == null) 
                dashDirection = spriteRenderer.flipX ? Vector2.right : Vector2.left;
            else
                dashDirection = direction.Value;

            // 대시 파티클 인스턴스 생성
            if (dashFactor != 0.35f)
            {
                Vector2 particlePosition = new Vector2(transform.position.x, transform.position.y);
                GameObject particles = Instantiate(dashParticle, particlePosition, Quaternion.identity);

                // 파티클 방향 설정
                Quaternion dashParticleDir;
                if (dashDirection == Vector2.right)
                {
                    dashParticleDir = Quaternion.Euler(0, 90, 0);
                }
                else
                {
                    dashParticleDir = Quaternion.Euler(0, -90, 0);
                }
                particles.transform.rotation = dashParticleDir;

                // 파티클 위치를 캐릭터의 현재 위치로 설정
                particles.transform.position = transform.position;

                Destroy(particles, 2f);
            }

            // 대시 목표 위치 계산
            Vector2 currentPosition = transform.position;
            Vector2 dashTargetPosition = currentPosition + dashDirection * dashSpeed * dashFactor;

            // X 경계 내로 목표 위치 제한
            float clampedX = Mathf.Clamp(dashTargetPosition.x, leftBorder, rightBorder);

            // 목표 위치로 이동
            float dashDuration = dashTime * dashFactor;
            Ease curEase = dashFactor <= 0.3f ? Ease.InQuad : Ease.Linear;

            transform.DOMoveX(clampedX, dashDuration)
                .SetEase(curEase)
                .OnComplete(() => isDashing = false);  // 완료 후 대시 상태 리셋

            // 대시 지속 시간만큼 대기
            yield return new WaitForSeconds(dashDuration);

            anim.SetBool("isDash", false);
            anim.speed = 1f;
        }
    }

    void Fly()
    {
        if ((!Input.GetKey(KeyCode.LeftShift) && !Input.GetMouseButton(1)) || !canFlyAgainAfterLanding)
            return;

        if (flyCoolTime <= curTime)
        {
            if (!isFlying)
            {
                gauge.value -= 0.2f;
                isFlying = true;
            }

            isInvincibility = true;
            gauge.value -= flyGauge * Time.deltaTime;

            flyingParticle = Instantiate(flyParticle, transform.position, Quaternion.Euler(0, 90, -90));
            Vector3 dir = transform.position - flyingParticle.transform.position;
            flyingParticle.transform.LookAt(dir);
            flyingParticle.transform.position = transform.position;
            Destroy(flyingParticle, 0.4f);

            if (gauge.value <= 0 && !isTouchBottom)
            {
                EndFly();
                return;
            }

            rigid2D.gravityScale = 0;
            spriteRenderer.sprite = flySprite;
            anim.enabled = false;

            float flyX = Input.GetAxis("Horizontal");
            float flyY = Input.GetAxis("Vertical");

            if (isTouchTop) flyY = 0;
            if (isTouchLeft || isTouchRight) flyX = 0;

            // 이동 방향 벡터
            Vector2 moveDirection = new Vector2(flyX, flyY).normalized;

            rigid2D.velocity = moveDirection * moveSpeed * 1.5f;

            if (flyX != 0)
            {
                float rotationAmount = rotateSpeed * Time.deltaTime;
                transform.Rotate(0, 0, flyX < 0 ? rotationAmount : -rotationAmount);
            }
        }
    }

    private void EndFly()
    {
        anim.SetBool("isAirborne", true);

        rigid2D.velocity = new Vector2(0, 0);

        isFlying = false;
        if (!isHit)
            isInvincibility = false;

        rigid2D.gravityScale = highGravity;
        anim.enabled = true;
        transform.rotation = Quaternion.Euler(0, 0, 0);
        curTime = 0;
        currentJumpCount = 0;

        canFlyAgainAfterLanding = false;
    }

    private IEnumerator InvincibilityCoroutine()
    {
        isInvincibility = true;
        yield return new WaitForSeconds(hitInvincibilityTime);
        isInvincibility = false;
        isHit = false;
    }

    private IEnumerator DashInvincibilityCoroutine()
    {
        isDashInvincibility = true;
        yield return new WaitForSeconds(dashTime);
        isDashInvincibility = false;
    }

    public void OnJumpStartAnimationEnd()
        => anim.SetBool("isAirborne", true); // Jump_Airborne 재생

    public void OnJumpEndAnimationEnd()
        => anim.SetTrigger("isJump_End"); // Idle 애니메이션 재생

    public void UpdateLifeIcon(int life)
    {
        // 라이프 아이콘 설정
        for (int i = 0; i < lifeImage.Length; i++)
            lifeImage[i].color = new Color(1, 1, 1, 0);
        
        for (int i = 0; i < life; i++)
            lifeImage[i].color = new Color(1, 1, 1, 1);

        // lifeText.text = life.ToString();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Border"))
            BorderCheck(collision, true);
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Border"))
            BorderCheck(collision, false);
    }

    private void BorderCheck(Collider2D borderCollider, bool isTouching)
    {
        switch (borderCollider.gameObject.name)
        {
            case "Top2D":
                isTouchTop = isTouching;
                break;
            case "Bottom2D":
                isTouchBottom = isTouching;
                break;
            case "Left2D":
                isTouchLeft = isTouching;
                break;
            case "Right2D":
                isTouchRight = isTouching;
                break;
        }
    }

    private IEnumerator HitInvincibility()
    {
        float elapsedTime = 0f;
        float waitTime = 0.17f;
        float minWaitTime = 0.05f; // 최소 대기 시간 설정
        while (elapsedTime < hitInvincibilityTime)
        {
            Color color = spriteRenderer.color;
            color.a = color.a == 1f ? 0.25f : 1f;
            spriteRenderer.color = color;
            yield return new WaitForSeconds(waitTime);
            elapsedTime += waitTime;
            waitTime = Mathf.Max(waitTime * 0.9f, minWaitTime); // 점점 더 빨라지게, 최소 대기 시간 이하로는 줄어들지 않게

            if (elapsedTime >= hitInvincibilityTime)
            {
                break;
            }
        }
        Color finalColor = spriteRenderer.color;
        finalColor.a = 1f;
        spriteRenderer.color = finalColor;
    }

    // BulletPro에서 실행됨
    public void OnHitByBullet()
    {
        if (!isInvincibility && !isDashInvincibility)
        {
            life--;

            if (life != 0)
            {
                isHit = true;
                UpdateLifeIcon(life);
                StartCoroutine(InvincibilityCoroutine());
                StartCoroutine(HitInvincibility());

                // 스킬 게이지 감소
                skillGauge = Mathf.Max(skillGauge - 5f, 0f);
                Debug.Log("스킬 게이지가 5 감소했습니다.");
                
                flash = true;
                cg.alpha = 0.3f;

                UpdateSkillGauge();
            }
            else
            {
                Die();
            }
        }
    }

    public void UseNail1()
    {
        if (canUse[0]) // 소모량 배열 사용
        {
            skillLastUsedTimes[0] = Time.time; // 배열에 사용 시간 저장
            skillGauge -= skillCosts[0]; // 소모량 배열 사용
            UpdateSkillGauge();
            Debug.Log("Nail1 사용!");
            // Nail1의 효과를 여기에 추가
        }
        else
            Debug.Log("Nail1을 사용할 수 없습니다.");

    }

    public void UseNail2()
    {
        if (canUse[1]) // 소모량 배열 사용
        {
            skillLastUsedTimes[1] = Time.time; // 배열에 사용 시간 저장
            skillGauge -= skillCosts[1]; // 소모량 배열 사용
            UpdateSkillGauge();
            Debug.Log("Nail2 사용!");
            // Nail2의 효과를 여기에 추가
        }
        else
            Debug.Log("Nail2을 사용할 수 없습니다.");
    }

    public void Flash()
    {
        if (flash)
        {
            cg.alpha = cg.alpha - Time.deltaTime * 1.3f;
            if (cg.alpha <= 0)
            {
                cg.alpha = 0;
                flash = false;
            }
        }
    }

    public void Die()
    {
        SceneLoader.instance.LoadLobbyScene();
    }

    IEnumerator DelaySliderValue()
    {
        yield return new WaitForSeconds(0.15f);
        DOTween.To(() => delaySlider.value, x => delaySlider.value = x, gauge.value, 0.15f);
    }

    private void UpdateSkillFades()
    {
        for (int i = 0; i < isEquippedSkill.Length; i++) // isEquippedSkill의 길이만큼 반복
        {
            if (isEquippedSkill[i]) // 현재 장착된 스킬만 체크
            {
                float remainingCooldown = skillLastUsedTimes[i] + skillCooldowns[i] - Time.time;
                if (remainingCooldown > 0)
                {
                    skillFades.fillAmount = Mathf.Clamp01(remainingCooldown / skillCooldowns[i]);
                }
            }
        }
    }

    public void UpdateSkillGauge()
    {
        skillGaugeText.text = Mathf.Floor(skillGauge).ToString();

        DOTween.To(() => skillGaugeImage.fillAmount, x => skillGaugeImage.fillAmount = x, skillGauge / maxSkillGauge, 0.2f)
            .SetEase(Ease.OutBounce);
    }

    // 추가된 메서드: Nail 사용 가능 여부 체크
    private void CheckNailAvailability()
    {
        for (int i = 0; i < isEquippedSkill.Length; i++)
        {
            if (i < skillLastUsedTimes.Length && i < skillCooldowns.Length && i < skillCosts.Length) // 배열의 길이 체크
            {
                if(isEquippedSkill[i])
                {
                    if (Time.time >= skillLastUsedTimes[i] + skillCooldowns[i] && skillGauge >= skillCosts[i])
                    {
                        glowObject.SetActive(true); // 사용 가능 시 Glow 활성화
                        canUse[i] = true;
                    }
                    else
                    {
                        glowObject.SetActive(false); // 사용 불가능 시 Glow 비활성화
                        canUse[i] = false;
                    }
                }
            }
        }
    }
}

