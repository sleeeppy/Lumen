using System.Diagnostics.SymbolStore;
using UnityEngine;
using UnityEngine.Animations;

[RequireComponent(typeof(Rigidbody2D))]
public class MovementRigidbody2D : MonoBehaviour
{
    [Header("Move Horizontal")]
    [SerializeField]
    private float moveSpeed = 8;      // 이동 속도

    [Header("Move Vertical (Jump)")]
    [SerializeField]
    private float jumpForce = 10;     // 점프 힘
    [SerializeField]
    private float lowGravity = 2f;    // 점프키를 오래 누르고 있을 때 적용되는 낮은 중력 계수
    [SerializeField]
    private float highGravity = 4f;   // 일반적으로 적용되는 높은 중력 계수
    [SerializeField]
    private int maxJumpCount = 2;   // 최대 점프 횟수
    private int currentJumpCount;   // 현재 남아잇는 점프 횟수

    [Header("Collision")]
    [SerializeField]
    private LayerMask groundLayer;        // 바닥 충돌 체크를 위한 레이어

    private bool isGrounded;           // 바닥 체크 (바닥에 플레이어의 발이 닿아있을 떄 true)
    private bool isJumpEndArea;
    private Vector2 footPosition;       // 바닥 체크를 위한 플레이어 발 위치
    private Vector2 footArea;           // 바닥 체크를 위한 플레이어 발 인식 범위
    private Vector2 airborenCheckArea;

    private Rigidbody2D rigid2D;            // 속력 제어를 위한 Rigidbody2D
    private new Collider2D collider2D;         // 현재 오브젝트의 충돌 범위 정보

    private Animator anim;
    
    public bool IsLongJump { set; get; } = false;

    private void Awake()
    {
        rigid2D = GetComponent<Rigidbody2D>();
        collider2D = GetComponent<Collider2D>();
        anim = GetComponent<Animator>();
    }

    public void FixedUpdate()
    {
        // 플레이어 오브젝트의 Collider2D min, center, max 위치정보
        Bounds bounds = collider2D.bounds;
        // 플레이어의 발 위치 설정
        footPosition = new Vector2(bounds.center.x, bounds.min.y);
        // 플레이어의 발 인식 범위 설정
        footArea = new Vector2((bounds.max.x - bounds.min.x) * 0.5f, 0.1f);
        // 플레이어의 발 위치에 박스를 생성하고, 바닥과 닿아있으면 isGrounded = true
        isGrounded = Physics2D.OverlapBox(footPosition, footArea, 0, groundLayer);
        
        RaycastHit2D hit = Physics2D.Raycast(footPosition, Vector2.down, 1f, groundLayer);
        
        // Jump_Airborne 상태일 때 Ground에 가까워지면 Jump_End 실행
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Jump_Airborne"))
        {
            // Ground와의 거리 체크
            if (hit.collider != null && hit.distance < 0.8f) // 0.5f는 Ground에 가까운 거리
            {
                anim.SetBool("isAirborne", false); // Jump_End 실행
            }
        }
        
        if ( isGrounded == true && rigid2D.velocity.y <= 0 )
        {
            currentJumpCount = maxJumpCount;
        }

        // 낮은 점프, 높은 점프 구현을 위한 중력 계수(gravityScale) 조절 (Jump Up일 때만 적용)
        // 중력 계수기 낮은 if 문은 높은 점프가 되고, 중력 계수가 높은 else 문은 낮은 점프가 된다.
        if ( IsLongJump && rigid2D.velocity.y > 0)
        {
            rigid2D.gravityScale = lowGravity;
        }
        else
        {
            rigid2D.gravityScale = highGravity;
        }
    }
    

    /// <summary>
    /// x 이동 방향 설정 (
    /// </summary>
    /// <param name="x"></param>
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


    /// <summary>
    /// 점프 (외부 클래스에서 호출)
    /// </summary>
    public bool JumpTo()
    {
        if ( currentJumpCount > 0)
        {
            anim.SetTrigger("isJump_Start");
            rigid2D.velocity = new Vector2(rigid2D.velocity.x, jumpForce);
            currentJumpCount--;

            return true;
        }

        return false;
    }
    
    public void OnJumpStartAnimationEnd()
    {
        anim.SetBool("isAirborne", true); // Jump_Airborne 애니메이션 실행
    }

    public void OnJumpEndAnimationEnd()
    {
        anim.SetTrigger("isJump_End"); // Idle 애니메이션 실행
    }
}
