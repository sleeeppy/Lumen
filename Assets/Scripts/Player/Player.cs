using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine.SceneManagement;
using Sirenix.OdinInspector;

public partial class Player : MonoBehaviour
{
    private const int skillSlotCount = 2;

    #region Collision borders
    private bool isTouchTop;
    private bool isTouchBottom;
    private bool isTouchLeft;
    private bool isTouchRight;
    #endregion

    [SerializeField] public float moveSpeed = 8;

    [TabGroup("Tab", "Jump", SdfIconType.Shift, TextColor = "green")]
    [TabGroup("Tab", "Jump")] [SerializeField] private float jumpForce = 7;
    [TabGroup("Tab", "Jump")] [SerializeField] private float lowGravity = 2f;
    [TabGroup("Tab", "Jump")] [SerializeField] private float highGravity = 4f;
    [TabGroup("Tab", "Jump")] [SerializeField] public int maxJumpCount = 1;
    [TabGroup("Tab", "Jump")] [SerializeField] private LayerMask groundLayer;
    [TabGroup("Tab", "Jump")] [SerializeField] private KeyCode jumpKey = KeyCode.Space;
    private int currentJumpCount;

    [TabGroup("Tab", "Gauge", SdfIconType.Toggle2On, TextColor = "Red")]
    [TabGroup("Tab", "Gauge")] [SerializeField] public Slider gauge;
    [TabGroup("Tab", "Gauge")] [SerializeField] private Slider delaySlider;
    [TabGroup("Tab", "Gauge")] [SerializeField] private float recoverySpeed = 0.5f;
    private float targetGauge;

    [TabGroup("Tab", "Dash", SdfIconType.Wind, TextColor = "Blue")]
    [TabGroup("Tab", "Dash")] [SerializeField] private float dashGauge = 0.37f;
    [TabGroup("Tab", "Dash")] [SerializeField] private float dashSpeed = 7f;
    [TabGroup("Tab", "Dash")] [SerializeField] private float dashTime = 0.4f;
    [TabGroup("Tab", "Dash")] [SerializeField] private GameObject dashParticle;
    [TabGroup("Tab", "Dash")] [SerializeField] private float longDashThreshold = 0.2f;
    [TabGroup("Tab", "Life")] [SerializeField] private float dashCooldown;

    [TabGroup("Tab", "Fly", SdfIconType.Send, TextColor = "White")]
    [TabGroup("Tab", "Fly")] [SerializeField] private float flyGauge = 0.5f;
    [TabGroup("Tab", "Fly")] [SerializeField] private float flyCoolTime = 0.3f;
    [TabGroup("Tab", "Fly")] [SerializeField] private float slowFall = 0.15f;
    [TabGroup("Tab", "Fly")] [SerializeField] private GameObject flyTrails;

    [TabGroup("Tab", "Life", SdfIconType.SuitHeart, TextColor = "Magenta")]
    [TabGroup("Tab", "Life")] [SerializeField] public int life;
    [TabGroup("Tab", "Life")] [SerializeField] public int maxLife;
    [TabGroup("Tab", "Life")] [SerializeField] private float hitInvincibilityTime = 0.8f;
    [TabGroup("Tab", "Life")] [SerializeField] private Image[] lifeImage;

    [HideInInspector] public bool isInvincibility;
    [HideInInspector] public bool isDashInvincibility;
    [HideInInspector] public bool isHit;
    [HideInInspector] public bool isFlying;

    [SerializeField] private Collider2D leftBorderCollider;
    [SerializeField] private Collider2D rightBorderCollider;
    private float leftBorder, rightBorder;

    [SerializeField] private CanvasGroup cg;
    private bool flash = true;

    private bool isGrounded;
    private Vector2 footPosition;
    private Vector2 footArea;

    private Rigidbody2D rigid2D;
    private new Collider2D collider2D;
    private SpriteRenderer spriteRenderer;
    private Animator anim;
    private bool isLongJump;

    private float curTime;
    private float airBorneTime;

    [TabGroup("Tab", "Skill", SdfIconType.Lightning, TextColor = "Black")]
    [TabGroup("Tab", "Skill")] [SerializeField] public float skillGauge;
    [TabGroup("Tab", "Skill")] [SerializeField] public float maxSkillGauge = 20f;
    [TabGroup("Tab", "Skill")] [SerializeField] public float skillGaugeIncreaseRate = 0.35f;
    [TabGroup("Tab", "Skill")] [SerializeField] private TextMeshProUGUI skillGaugeText;
    [TabGroup("Tab", "Skill")] [SerializeField] private Image skillGaugeImage;
    [TabGroup("Tab", "Skill")] [SerializeField] private Image skillImage;
    [TabGroup("Tab", "Skill")] [SerializeField] private Image skillCostImage;
    [TabGroup("Tab", "Skill")] [SerializeField] private Image skillFades;
    [TabGroup("Tab", "Skill")] [SerializeField] private GameObject glowObject;
    [TabGroup("Tab", "Skill")] [SerializeField] private float[] skillCooldowns = { 16f, 64f };
    [TabGroup("Tab", "Skill")] [SerializeField] private float[] skillCosts = { 5f, 20f };
    [TabGroup("Tab", "Skill")] [SerializeField] private Sprite[] skillSprites;
    [TabGroup("Tab", "Skill")] [SerializeField] private Sprite[] skillCostSprites;

    [HideInInspector] public bool[] isEquippedSkill = { false, false };
    [HideInInspector] public bool[] canUse = { false, false };
    private readonly float[] skillLastUsedTimes = new float[skillSlotCount];

    private bool isDashHeld => Input.GetKey(KeyCode.LeftShift) || Input.GetMouseButton(1);
    private bool isDashPressed => Input.GetKeyDown(KeyCode.LeftShift) || Input.GetMouseButtonDown(1);
    private bool isDashReleased => Input.GetKeyUp(KeyCode.LeftShift) || Input.GetMouseButtonUp(1);

    private void Awake()
    {
        rigid2D = GetComponent<Rigidbody2D>();
        collider2D = GetComponent<Collider2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (SceneManager.GetActiveScene().name == "Game")
        {
            UpdateLifeIcon(life);
            skillFades.fillAmount = 0;
        }

        if (leftBorderCollider)
            leftBorder = leftBorderCollider.bounds.max.x;

        if (rightBorderCollider)
            rightBorder = rightBorderCollider.bounds.min.x;

        if (skillGaugeImage)
            skillGaugeImage.fillAmount = 0f;
    }

    private void Update()
    {
        UpdateGroundingAndGravity();
        UpdateMove();
        UpdateJump();
        UpdateDashAndFlyInput();
        UpdateFlash();

        UpdateEquippedSkillIcons();
        UpdateGaugeRecovery();
        UpdateDelaySlider();
        UpdateSkillInput();
        CheckNailAvailability();
        UpdateSkillFades();
    }

    private void UpdateEquippedSkillIcons()
    {
        for (int i = 0; i < isEquippedSkill.Length; i++)
        {
            if (!isEquippedSkill[i])
                continue;

            skillImage.sprite = skillSprites[i];
            skillCostImage.sprite = skillCostSprites[i];
            return;
        }
    }

    private void UpdateGaugeRecovery()
    {
        curTime += Time.deltaTime;

        if (!isFlying && !isDashing && isGrounded)
            gauge.value += recoverySpeed * Time.deltaTime;
    }

    private void UpdateDelaySlider()
    {
        if (!isDashing)
            delaySlider.value = gauge.value;
    }

    private void UpdateSkillInput()
    {
        if (!Input.GetKeyDown(KeyCode.Q))
            return;

        for (int i = 0; i < isEquippedSkill.Length; i++)
        {
            if (isEquippedSkill[i])
            {
                UseNail(i);
                return;
            }
        }
    }

    private void UpdateMove()
    {
        float x = Input.GetAxisRaw("Horizontal");

        if (!isDashing)
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
            isLongJump = true;
        else if (Input.GetKeyUp(jumpKey))
            isLongJump = false;
    }

    private void UpdateGroundingAndGravity()
    {
        Bounds bounds = collider2D.bounds;
        footPosition = new Vector2(bounds.center.x, bounds.min.y);
        footArea = new Vector2((bounds.max.x - bounds.min.x) * 0.5f, 0.1f);
        isGrounded = Physics2D.OverlapBox(footPosition, footArea, 0, groundLayer);

        RaycastHit2D hit = Physics2D.Raycast(footPosition, Vector2.down, 1f, groundLayer);

        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Jump_Airborne")
            && hit.collider != null && hit.distance < 0.5f)
        {
            anim.SetBool("isAirborne", false);
        }

        if (isGrounded && rigid2D.velocity.y <= 0)
        {
            currentJumpCount = maxJumpCount;
            airBorneTime = 0;
            canFlyAgainAfterLanding = true;

            if (isFlying)
                EndFly();
        }
        else if (!isFlying)
        {
            airBorneTime += Time.deltaTime;
        }

        ApplyGravityScale();
    }

    private void ApplyGravityScale()
    {
        if (isFlying)
            return;

        if ((Input.GetKey(KeyCode.W) || isLongJump) && rigid2D.velocity.y > 0)
        {
            rigid2D.gravityScale = lowGravity;
            return;
        }

        if (!isGrounded && rigid2D.velocity.y < 0 && airBorneTime <= 3
            && (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.Space)))
        {
            rigid2D.gravityScale = slowFall;
            return;
        }

        rigid2D.gravityScale = airBorneTime > 3 ? lowGravity : highGravity;
    }

    private void MoveTo(float x)
    {
        rigid2D.velocity = new Vector2(x * moveSpeed, rigid2D.velocity.y);
        anim.SetBool("isRun", isGrounded && x != 0);
    }

    private void JumpTo()
    {
        if (currentJumpCount <= 0)
            return;

        anim.SetTrigger("isJump_Start");
        rigid2D.velocity = new Vector2(rigid2D.velocity.x, jumpForce);
        currentJumpCount--;
    }

    public void OnJumpStartAnimationEnd()
        => anim.SetBool("isAirborne", true);

    public void OnJumpEndAnimationEnd()
        => anim.SetTrigger("isJump_End");

    public void UpdateLifeIcon(int currentLife)
    {
        for (int i = 0; i < lifeImage.Length; i++)
            lifeImage[i].color = new Color(1, 1, 1, 0);

        for (int i = 0; i < currentLife; i++)
            lifeImage[i].color = Color.white;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Border"))
            BorderCheck(collision, true);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Border"))
            BorderCheck(collision, false);
    }

    private void BorderCheck(Collider2D borderCollider, bool isTouching)
    {
        switch (borderCollider.gameObject.name)
        {
            case "Top2D": isTouchTop = isTouching; break;
            case "Bottom2D": isTouchBottom = isTouching; break;
            case "Left2D": isTouchLeft = isTouching; break;
            case "Right2D": isTouchRight = isTouching; break;
        }
    }

    public void Flash()
    {
        if (!flash)
            return;

        cg.alpha -= Time.deltaTime * 1.3f;
        if (cg.alpha <= 0)
        {
            cg.alpha = 0;
            flash = false;
        }
    }

    private void UpdateFlash() => Flash();
}
