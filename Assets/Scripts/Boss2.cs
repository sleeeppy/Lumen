using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Random = UnityEngine.Random;

public class Boss2 : Boss, IBoss
{
    private GameObject player;
    private Vector3 playerPos;
    private float randomY;
    private bool isPatterning = false;
    private Queue<Action> patternQueue = new Queue<Action>();

    protected override void Awake()
    {
        base.Awake();
        player = GameObject.FindWithTag("Player");
        
        //Debug.Log($"{bottomBorder + 4.5}, {topBorder - 3}");
    }

    protected void Start()
    {
        PhaseChange(phase); 
    }

    protected override void Update()
    {
        slider.value = HP;
        phaseText.text = "phase " + phase;

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
        if (HP > 0 && !isPatterning) // 패턴 다완성하고 주석풀기
        {
            PhaseChange(phase);
        }

        playerPos = player.transform.position;
        LookAtPlayer();

        if(HP == 60f || HP == 30f)
        {
            float randomX = Mathf.Clamp(Random.Range(transform.position.x - 10, transform.position.x + 10), leftBorder + 10, rightBorder - 10);
            if (phase != 4)
                randomY = Mathf.Clamp(Random.Range(bottomBorder + 4.5f, topBorder - 3), bottomBorder + 4.5f, topBorder - 3);
            else
                randomY = Mathf.Clamp(Random.Range(bottomBorder + 4.5f, topBorder - 3), bottomBorder + 4.5f, topBorder - 3);
            transform.DOMove(new Vector3(randomX, randomY, transform.position.z), 3f).SetEase(Ease.InOutBack);
        }
    }
    private void LookAtPlayer()
    {
        if (player != null)
        {
            bool flipSprite = transform.position.x < playerPos.x;

            spriteRenderer.flipX = flipSprite;
        }
    }
    public void Phase1()
    {
        ExecutePattern(new List<Action>
        {
            () => StartCoroutine(OrbitalPattern(9, 0.1f)),
            () => StartCoroutine(SnowBallPattern(6, 0.15f))
        });
    }
    public void Phase2()
    {
        
    }
    public void Phase3()
    {

    }
    public void Phase4()
    {

    }
    public void Phase5()
    {

    }

    public void PhaseChange(int bossPhase)
    {
        int curPhase = Mathf.Abs(bossPhase - 5);
        Debug.Log("Phase: "+(curPhase+1));
        switch (curPhase)
        {
            case 0:
                Phase1();
                break;
            case 1:
                Phase2();
                break;
            case 2:
                Phase3();
                break;
            case 3:
                Phase4();
                break;
            case 4:
                Phase5();
                break;
        }
        randomY = Mathf.Clamp(Random.Range(bottomBorder + 4.5f, topBorder - 3), bottomBorder + 4.5f, topBorder - 3); // 범위 제한

        float clampedX = Mathf.Clamp(playerPos.x, leftBorder + 10, rightBorder - 10);
        transform.DOMove(new Vector3(clampedX, randomY, transform.position.z), 3f).SetEase(Ease.InOutBack);
    }
    private void ExecutePattern(IEnumerable<Action> phasePatterns)
    {
        if (!isPatterning)
        {
            foreach (var pattern in phasePatterns)
            {
                patternQueue.Enqueue(pattern);
            }

            StartCoroutine(ProcessPatterns());
        }
    }
    private IEnumerator ProcessPatterns()
    {
        isPatterning = true;

        while (patternQueue.Count > 0)
        {
            var currentPattern = patternQueue.Dequeue();
            currentPattern.Invoke();
            yield return new WaitForSeconds(3f); // 패턴 대기 시간 (조정 가능)
        }

        isPatterning = false;
    }
    private IEnumerator OrbitalPattern(int count, float interval)
    {
        float gap = 1.5f;
        float indicatorGap = 1.5f;
        for (int i = 0; i < count; i++)
        {
            ShowIndicator(new Vector3(0,bottomBorder + 0.5f,0) + new Vector3(indicatorGap, -0.6f, 0), 1f);
            ShowIndicator(new Vector3(0, bottomBorder+ 0.5f, 0) + new Vector3(-indicatorGap, -0.6f, 0), 1f);

            indicatorGap += 2f;

            yield return new WaitForSeconds(0.1f);
        }
        for (int i = 0; i < count; i++)
        {
            FireOrbital(new Vector3(0, bottomBorder + 0.1f, 0) + new Vector3(gap, 0, 0), 1.5f);
            FireOrbital(new Vector3(0, bottomBorder + 0.1f, 0) + new Vector3(-gap, 0, 0), 1.5f);

            gap += 2f;

            yield return new WaitForSeconds(interval);
        }
    }

    private IEnumerator SnowBallPattern(int count, float interval)
    {
        for (int i = 0;i < count; i++)
        {
            FireSnowBall(playerPos);
            yield return new WaitForSeconds(interval);
        }
    }
    private void FireOrbital(Vector3 targetPosition, float duration)
    {
        GameObject orbital = ParticlePool.Instance.GetParticle("orbital");
        if (orbital != null)
        {
            // 레이저 초기화
            orbital.transform.position = targetPosition;
            // 레이저 지속 시간 후 반환
            StartCoroutine(ReturnToPool(orbital, duration));
        }
    }
    private void FireSnowBall(Vector3 targetPosition)
    {
        GameObject snowBall = ParticlePool.Instance.GetParticle("snowBall");

        if (snowBall != null)
        {
            snowBall.transform.position = transform.position;
            Rigidbody rb = snowBall.GetComponent<Rigidbody>();
            Vector3 dir = (targetPosition - transform.position).normalized;
            rb.AddForce(dir * 10f, ForceMode.Impulse);
        }
    }

    private void ShowIndicator(Vector3 targetPosition, float duration)
    {
        GameObject indicator = ParticlePool.Instance.GetParticle("indicator");
        if(indicator != null)
        {
            indicator.transform.position = targetPosition;
            StartCoroutine(ReturnToPool(indicator, duration));
        }
    }

    private IEnumerator ReturnToPool(GameObject particle, float delay)
    {
        particle.TryGetComponent<BossBullet>(out BossBullet bullet);
        if (bullet != null)
        {
            bullet.LifeTimeExit();
        }
        yield return new WaitForSeconds(delay);
        ParticlePool.Instance.OnParticleRelease(particle);
    }
}
