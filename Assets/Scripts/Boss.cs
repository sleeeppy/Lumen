using System;
using System.Collections;
using System.Collections.Generic;
using BulletPro;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Cinemachine;

// Be sure to set maxHP and HP to the same value in the Inspector.
// If not, phase starts decremented by 1.

public class Boss : MonoBehaviour
{
    [SerializeField] public float maxHP = 100f;
    [SerializeField] public float HP;
    [SerializeField] public int phase = 5;

    [SerializeField] public TextMeshProUGUI phaseText;

    [Header("HP Slider")]
    [SerializeField] public Slider slider;
    [SerializeField] private Slider delaySlider;

    [SerializeField] public GameObject bulletSpawnPos;
    [SerializeField] public BulletEmitter bulletEmitter;
    [SerializeField] public EmitterProfile[] bossPattern;

    [SerializeField] private Sprite[] bossProfile;
    [SerializeField] private Image curProfile;

    [SerializeField] private Collider2D leftBorderCollider;
    [SerializeField] private Collider2D rightBorderCollider;
    [SerializeField] private Collider2D bottomBorderCollider;
    [SerializeField] private Collider2D topBorderCollider;

    [SerializeField] private AnimationCurve easeCurve;

    protected float leftBorder, rightBorder, bottomBorder, topBorder;

    private bool isHit = false;
    private float hitTimer = 0f;
    private SpriteRenderer spriteRenderer;

    private Coroutine colorChangeCoroutine; // 색상 변경 코루틴을 저장할 변수

    protected virtual void Awake()
    {
        if (leftBorderCollider != null)
            leftBorder = leftBorderCollider.bounds.max.x;

        if (rightBorderCollider != null)
            rightBorder = rightBorderCollider.bounds.min.x;

        if (bottomBorderCollider != null)
            bottomBorder = bottomBorderCollider.bounds.max.y;

        if (topBorderCollider != null)
            topBorder = topBorderCollider.bounds.min.y;
            
        Init();

        //Debug.Log($"{leftBorder}, {rightBorder}, {bottomBorder}, {topBorder}");

        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Init()
    {
        slider.maxValue = maxHP;
        delaySlider.maxValue = maxHP;
        PhaseChange(phase);
    }

    protected virtual void Update()
    {
        slider.value = HP;
        phaseText.text = "x" + phase;

        if (HP <= 0 && phase != 0)
        {
            phase--;

            if (phase == 0)
            {
                Die();
                return;
            }

            int curPhase = Mathf.Abs(phase - 5);

            curProfile.sprite = bossProfile[curPhase];
            PhaseChange(phase);
            HP = maxHP;
        }

        bulletSpawnPos.transform.position = transform.position;
    }

    // RetroProjectileScript에서 호출되는 함수 (Line 62)
    public void HandleCollision(GameObject bulletObject)
    {
        // 카메라 뷰포트 체크
        if (!IsInCameraView())
            return; // 카메라 뷰포트를 벗어나면 데미지를 받지 않음

        // 색상 변경 코루틴이 실행 중이지 않을 때만 호출
        if (colorChangeCoroutine == null)
            colorChangeCoroutine = StartCoroutine(ChangeColorTemporarily()); // 색상 변경 코루틴 호출


        Bullet bulletScript = bulletObject.GetComponent<Bullet>();
        Player playerScript = FindObjectOfType<Player>();
        if (bulletScript != null)
        {
            HP -= bulletScript.Damage;
            
            playerScript.UpdateSkillGauge();
            playerScript.skillGauge = Mathf.Min(playerScript.skillGauge 
            + (float)(bulletScript.Damage * playerScript.skillGaugeIncreaseRate), playerScript.maxSkillGauge);

            // 슬라이더 애니메이션
            float targetHP = HP;
            DOTween.To(() => slider.value, x => slider.value = x, targetHP, 0.1f) 
            .SetEase(easeCurve);

            // 피격 상태 설정 및 타이머 리셋
            isHit = true;
            hitTimer = 0f;
            StartCoroutine(DelaySliderValue());

            Attack attack = FindObjectOfType<Attack>();
            if (attack != null)
            {
                if (attack.whatAttack != "Baracelet2")
                    Destroy(bulletObject);
            }
        }
    }

    private bool IsInCameraView()
    {
        Vector3 screenPoint = Camera.main.WorldToViewportPoint(transform.position);
        return screenPoint.x >= 0f && screenPoint.x <= 1f && screenPoint.y >= 0f && screenPoint.y <= 1f && screenPoint.z > 0;
    }

    protected virtual void PhaseChange(int bossPhase)
    {
        int curPhase = Mathf.Abs(bossPhase - 5);

        if (bulletEmitter.emitterProfile == null)
        {
            bulletEmitter.emitterProfile = bossPattern[0];
            bulletEmitter.Play();
        }
        else if (bulletEmitter.emitterProfile != bossPattern[curPhase])
        {
            StartCoroutine(NextPattern(2f, curPhase));
        }
    }

    protected IEnumerator NextPattern(float time, int profileNum)
    {
        bulletEmitter.Pause();
        yield return new WaitForSeconds(time);
        bulletEmitter.emitterProfile = bossPattern[profileNum];
        bulletEmitter.Play();
    }

    protected virtual void Die()
    {
        phaseText.text = "x" + phase;
        bulletEmitter.Kill();
        gameObject.SetActive(false);
    }

    protected IEnumerator DelaySliderValue()
    {
        yield return new WaitForSeconds(0.3f);
        DOTween.To(() => delaySlider.value, x => delaySlider.value = x, slider.value, 0.15f);
    }

    private IEnumerator ChangeColorTemporarily()
    {
        spriteRenderer.color = new Color(255f / 255f, 158f / 255f, 158f / 255f); // 색상 변경        spriteRenderer.color = new Color(FF9E9E)
        //spriteRenderer.color = Color.red;ㄴ
        yield return new WaitForSeconds(0.1f); // 0.1초 대기
        spriteRenderer.color = Color.white; // 원래 색상으로 복원
        colorChangeCoroutine = null;
    }
}



