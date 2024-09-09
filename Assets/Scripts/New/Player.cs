using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;

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
    [SerializeField] private float slowFall = 1f;
    [SerializeField] private float rotateSpeed = 30f;
    [SerializeField] private GameObject flyParticle;
    private GameObject flyingParticle;
    private bool canFly = true;

    [Header("Life")]
    [SerializeField] private int life;
    [SerializeField] private int maxLife;
    [SerializeField] private Image[] lifeImage;
    [SerializeField] private float hitInvincibilityTime = 1f;
    public bool isInvincibility;
    public bool isDashInvincibility;
    public bool isHit;
    private float curTime;
    Material material;

    [SerializeField] private float dashCooldown; // Dash 쿨다운 시간
    private float lastDashTime;  // 마지막 Dash 실행 시간

    private bool spendAllGauge;
    
    private void Awake()
    {
        rigid2D = GetComponent<Rigidbody2D>();
        collider2D = GetComponent<Collider2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        material = spriteRenderer.material;
    }

    private void Update()
    {
        UpdateMove();
        UpdateJump();

        // Dash 쿨다운 체크: 마지막 Dash 후 설정한 시간만큼 지나야 다시 Dash 실행 가능
        if (Input.GetKey(KeyCode.LeftShift) && Time.time >= lastDashTime + dashCooldown)
        {
            if (isGrounded && !isFlying && !isDashing)
                Dash();

            // 공중에 있거나, 이미 비행 중이면 Fly
            else if ((!isGrounded || isFlying) && !spendAllGauge )
                Fly();
        }

        if (Input.GetKeyUp(KeyCode.LeftShift) && isFlying)
            EndFly();


        if (!isFlying)
            gauge.value += recoverySpeed * Time.deltaTime;

        curTime += Time.deltaTime;

        if (spendAllGauge && gauge.value >= 0.3)
        {
            spendAllGauge = false;
        }
    }

    public void FixedUpdate()
    {
        RigidBodyController();
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
            currentJumpCount = maxJumpCount;


        if (IsLongJump && rigid2D.velocity.y > 0 && !isFlying)
        {
            rigid2D.gravityScale = lowGravity;
        }
        else
        {
            if (!isFlying)
                rigid2D.gravityScale = highGravity;
        }

        if (!isGrounded && Input.GetKey(KeyCode.Space) && !isFlying && rigid2D.velocity.y < 0)
        {
            rigid2D.gravityScale = slowFall;
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
        if (canFly)
        {
            if (flyCoolTime <= curTime)
            {
                isFlying = true;
                isInvincibility = true;
                gauge.value -= flyGauge * Time.deltaTime;

                flyingParticle = Instantiate(flyParticle, transform.position, Quaternion.Euler(0, 90, -90));

                Vector3 dir = transform.position - flyingParticle.transform.position;
                flyingParticle.transform.LookAt(dir);
                flyingParticle.transform.position = transform.position;

                Destroy(flyingParticle, 0.4f);
            }

            if (gauge.value <= 0)
            {
                EndFly();
                spendAllGauge = true;
            }
            else
            {
                if (flyCoolTime <= curTime)
                {
                    rigid2D.gravityScale = 0;
                    spriteRenderer.sprite = flySprite;
                    anim.enabled = false;
                    float x = Input.GetAxisRaw("Horizontal");
                    float y = Input.GetAxisRaw("Vertical");

                    if ((isTouchRight && x == 1) || (isTouchLeft && x == -1))
                        x = 0;

                    if ((isTouchTop && y == 1) || (isTouchBottom && y == -1))
                        y = 0;

                    rigid2D.velocity = new Vector2(x * moveSpeed, y * moveSpeed);

                    if (x < 0)
                        transform.Rotate(0, 0, rotateSpeed * 1000f * Time.deltaTime);

                    if (x > 0)
                        transform.Rotate(0, 0, rotateSpeed * -1000f * Time.deltaTime);
                }
            }
        }
        // else if (Input.GetKeyUp(KeyCode.LeftShift) || isGrounded)
        //     EndFly();

        if (gauge.value >= 0.5f)
            canFly = true;

    }

    private void EndFly()
    {
        isFlying = false;
        if (!isHit)
            isInvincibility = false;

        rigid2D.gravityScale = highGravity;
        anim.enabled = true;
        transform.rotation = Quaternion.Euler(0, 0, 0);
        curTime = 0;
        currentJumpCount = 0;
    }

    private IEnumerator InvincibilityCoroutine()
    {
        isInvincibility = true;
        yield return new WaitForSeconds(hitInvincibilityTime);
        isInvincibility = false;
        isHit = false;
        Debug.Log("맞아서 멈춤");
    }
    private IEnumerator DashInvincibilityCoroutine()
    {
        isDashInvincibility = true;
        yield return new WaitForSeconds(dashTime);
        isDashInvincibility = false;
        Debug.Log("대쉬로 멈춤");
    }

    public void OnJumpStartAnimationEnd()
        => anim.SetBool("isAirborne", true); // Play Jump_Airborne

    public void OnJumpEndAnimationEnd()
        => anim.SetTrigger("isJump_End"); // Play Idle Animation

    public void UpdateLifeIcon(int life)
    {
        // Life icon set
        for (int i = 0; i < maxLife; i++)
            lifeImage[i].color = new Color(1, 1, 1, 0);

        for (int i = 0; i < life; i++)
            lifeImage[i].color = new Color(1, 1, 1, 1);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Border"))
            BorderCheck(collision);

        else if (collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("EnemyBullet"))
        {
            if (!isInvincibility)
            {
                life--;
                UpdateLifeIcon(life);
                StartCoroutine(InvincibilityCoroutine());
                StartCoroutine(HitInvincibility());
            }
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Border"))
            BorderCheck(collision);
    }

    private void BorderCheck(Collider2D borderCollider)
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
        Debug.Log("보내기전");
        if (!isInvincibility && !isDashInvincibility)
        {
            isHit = true;
            life--;
            UpdateLifeIcon(life);
            StartCoroutine(InvincibilityCoroutine());
            StartCoroutine(HitInvincibility());
            Debug.Log("고쳐졌나?");
        }
    }
}