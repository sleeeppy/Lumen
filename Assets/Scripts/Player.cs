using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    private bool isTouchTop;
    private bool isTouchBottom;
    private bool isTouchLeft;
    private bool isTouchRight;

    [Header("Move Horizontal")]
    [SerializeField] public float moveSpeed = 8;

    [Header("Move Vertical (Jump)")]
    [SerializeField] private float jumpForce = 10;
    [SerializeField] private float lowGravity = 2f;
    [SerializeField] private float highGravity = 4f;
    [SerializeField] private int maxJumpCount = 1;
    private int currentJumpCount;

    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;

    private bool isGrounded;
    private bool isJumpEndArea;
    private Vector2 footPosition;
    private Vector2 footArea;
    private Vector2 airborenCheckArea;

    private Rigidbody2D rigid2D;
    private new Collider2D collider2D;

    private SpriteRenderer spriteRenderer;
    private Animator anim;
    public bool IsLongJump { set; get; }

    [HideInInspector] public bool isFlying;

    [Header("Gauge")]
    [SerializeField] public Slider gauge;
    [SerializeField] private float recoverySpeed = 0.25f;

    [Header("Dash")]
    private bool isDashing;
    private Vector2 dashDirection;
    [SerializeField] private float dashGauge = 0.25f;
    [SerializeField] private float dashSpeed = 1f;
    [SerializeField] private float dashTime = 1f;
    [SerializeField] private GameObject dashParticle;

    [Header("Fly")]
    [SerializeField] private Sprite flySprite;
    [SerializeField] private float flyGauge = 0.7f;
    [SerializeField] private float flyCoolTime = 0.5f;
    [SerializeField] private float slowFall = 0.15f;
    [SerializeField] private float rotateSpeed = 30f;
    [SerializeField] private GameObject flyParticle;
    private GameObject flyingParticle;
    private bool canFlyAgainAfterLanding = true;

    [Header("Life")]
    [SerializeField] private int life;
    [SerializeField] private int maxLife;
    //[SerializeField] private Image[] lifeImage;
    [SerializeField] private float hitInvincibilityTime = 1f;
    public bool isInvincibility;
    public bool isDashInvincibility;
    public bool isHit;
    private float curTime;
    Material material;

    [SerializeField] private float dashCooldown; // Dash 쿨다운 시간
    private float lastDashTime;  // 마지막 Dash 실행 시간
    
    private float airBorneTime;

    [SerializeField] private Image HPImage;
    [SerializeField] private TextMeshProUGUI lifeText;
    
    
    private void Awake()
    {
        rigid2D = GetComponent<Rigidbody2D>();
        collider2D = GetComponent<Collider2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        material = spriteRenderer.material;

        if(SceneManager.GetActiveScene().name == "GameScene")
            UpdateLifeIcon(life);
    }

    private void Update()
    {
        RigidBodyController();
        UpdateMove();
        UpdateJump();

        // Dash cooldown check
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetMouseButton(1))
        {
            if (isGrounded && !isFlying && !isDashing && Time.time >= lastDashTime + dashCooldown)
                Dash();

            // Fly logic, ensuring it activates correctly
            if ((!isGrounded || isFlying) && canFlyAgainAfterLanding)
                Fly();
        }
        
        if ((Input.GetKeyUp(KeyCode.LeftShift) || Input.GetMouseButtonUp(1)) && isFlying)
            EndFly();

        if (!isFlying && !isDashing)
            gauge.value += recoverySpeed * Time.deltaTime;

        curTime += Time.deltaTime;

        if (!isGrounded && !isFlying)
            airBorneTime += Time.deltaTime;

        if (isGrounded)
            airBorneTime = 0;
    }

    private void UpdateMove()
    {
        float x = Input.GetAxisRaw("Horizontal");

        if (x == 1)
            spriteRenderer.flipX = true;

        else if (x == -1)
            spriteRenderer.flipX = false;

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
            if (hit.collider != null && hit.distance < 0.8f)
            {
                anim.SetBool("isAirborne", false); // Jump_End 실행
            }
        }

        if (isGrounded && rigid2D.velocity.y <= 0)
        {
            currentJumpCount = maxJumpCount;
            airBorneTime = 0;  // 땅에 닿으면 airborneTime 초기화
            canFlyAgainAfterLanding = true;
        }

        // Space를 누르고 있고, Y축 속도가 양수일 때 lowGravity 적용
        if (IsLongJump && rigid2D.velocity.y > 0 && !isFlying)
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

        // 공중에 있고 Space를 누르고 있으며, 3초 이하일 때 slowFall 적용
        if (!isGrounded && Input.GetKey(KeyCode.Space) && !isFlying && rigid2D.velocity.y < 0 && airBorneTime <= 3)
        {
            rigid2D.gravityScale = slowFall;
        }
        // 공중에 있는 시간이 3초 이상이면 gravityScale을 1로 설정
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


    public void MoveTo(float x)
    {
        rigid2D.velocity = new Vector2(x * moveSpeed, rigid2D.velocity.y);

        if (isGrounded && x != 0)
        {
            anim.SetBool("isRun", true); // Run 애니메이션 시작
        }
        else
        {
            anim.SetBool("isRun", false); // Run 애니메이션 종료
        }
    }

    public bool JumpTo()
    {
        if (currentJumpCount > 0)
        {
            anim.SetTrigger("isJump_Start");
            rigid2D.velocity = new Vector2(rigid2D.velocity.x, jumpForce);
            currentJumpCount--;

            return true;
        }

        return false;
    }

    void Dash()
    {
        if (gauge.value >= dashGauge && !isDashing)
        {
            gauge.value -= dashGauge;

            isDashing = true;

            lastDashTime = Time.time;  // Dash 실행 시간을 기록
            StartCoroutine(DashInvincibilityCoroutine());

            dashDirection = spriteRenderer.flipX ? Vector2.right : Vector2.left;
            Quaternion dashParticleDir = spriteRenderer.flipX ? Quaternion.Euler(0, 90, 0) : Quaternion.Euler(0, -90, 0);

            Vector2 particlePosition = new Vector2(transform.position.x, transform.position.y);
            GameObject particles = Instantiate(dashParticle, particlePosition, Quaternion.identity);

            particles.transform.rotation = dashParticleDir;
            Destroy(particles, 2f);
            StartCoroutine(DashCoroutine());
        }
    }
    
    private IEnumerator DashCoroutine()
    {
        float startTime = Time.time;
        Vector2 startPosition = transform.position;

        while (Time.time < startTime + dashTime && !(isTouchRight || isTouchLeft))
        {
            // Lerp를 사용하여 부드럽게 이동
            transform.position = Vector2.Lerp(startPosition, startPosition + dashDirection * dashSpeed,
                (Time.time - startTime) / dashTime);
            yield return null;
        }
        isDashing = false;
    }

    void Fly()
    {
        // Ensure flying can only start if the left shift key is held down and conditions are met
        if ((!Input.GetKey(KeyCode.LeftShift) && !Input.GetMouseButton(1))|| !canFlyAgainAfterLanding)
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
            
            // 이동 방향 벡터 생성
            Vector2 moveDirection = new Vector2(flyX, flyY).normalized;

            
            rigid2D.velocity = moveDirection * moveSpeed * 1.5f;
            
            
            // 현재 속도를 초기화하여 관성을 제거
            // rigid2D.velocity = Vector2.zero;
            //
            // // 힘을 적용하여 이동
            // rigid2D.AddForce(moveDirection * moveSpeed, ForceMode2D.Impulse);

            //rigid2D.velocity = new Vector2(flyX * moveSpeed, flyY * moveSpeed);
            //transform.Translate(flyX * moveSpeed * Time.deltaTime, flyY * moveSpeed * Time.deltaTime, 0);

            if (flyX != 0)
            {
                float rotationAmount = rotateSpeed * Time.deltaTime;
                transform.Rotate(0, 0, flyX < 0 ? rotationAmount : -rotationAmount);
            }
        }
    }
    
    private void EndFly()
    {
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
        => anim.SetBool("isAirborne", true); // Play Jump_Airborne

    public void OnJumpEndAnimationEnd()
        => anim.SetTrigger("isJump_End"); // Play Idle Animation

    public void UpdateLifeIcon(int life)
    {
        // Life icon set
        //HPImage.fillAmount = (float)life / maxLife;
        lifeText.text = life.ToString();
        DOTween.To(()=>HPImage.fillAmount, x=>HPImage.fillAmount = x, (float)life / maxLife, 0.2f)
            .SetEase(Ease.OutBounce);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Border"))
            BorderCheckT(collision);

        // else if (collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("EnemyBullet"))
        // {
        //     if (!isInvincibility)
        //     {
        //         life--;
        //         UpdateLifeIcon(life);
        //         StartCoroutine(InvincibilityCoroutine());
        //         StartCoroutine(HitInvincibility());
        //     }
        // }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Border"))
            BorderCheckF(collision);
    }

    private void BorderCheckT(Collider2D borderCollider)
    {
        switch (borderCollider.gameObject.name)
        {
            case "Top":
                isTouchTop = true;
                break;
            case "Bottom":
                isTouchBottom = true;
                break;
            case "Left":
                isTouchLeft = true;
                break;
            case "Right":
                isTouchRight = true;
                break;
        }
    }
    
    private void BorderCheckF(Collider2D borderCollider)
    {
        switch (borderCollider.gameObject.name)
        {
            case "Top":
                isTouchTop = false;
                break;
            case "Bottom":
                isTouchBottom = false;
                break;
            case "Left":
                isTouchLeft = false;
                break;
            case "Right":
                isTouchRight = false;
                break;
        }
    }

    private IEnumerator HitInvincibility()
    {
        material.SetFloat("_HologramFade", 0.25f);
        yield return new WaitForSeconds(hitInvincibilityTime);
        material.SetFloat("_HologramFade", 0f);
    }

    public void OnHitByBullet()
    {
        if (!isInvincibility && !isDashInvincibility)
        {
            isHit = true;
            life--;
            UpdateLifeIcon(life);
            StartCoroutine(InvincibilityCoroutine());
            StartCoroutine(HitInvincibility());
        }
    }
}