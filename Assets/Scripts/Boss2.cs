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

    protected override void Awake()
    {
        base.Awake();
        player = GameObject.FindWithTag("Player");
        
        //Debug.Log($"{bottomBorder + 4.5}, {topBorder - 3}");
    }

    void Start()
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
        //if (HP > 0 && !isPatterning) // 패턴 다완성하고 주석풀기
        //{
        //    PhaseChange(phase);
        //}

        playerPos = player.transform.position;

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
    
    public void Phase1()
    {
        StartCoroutine(LaserPattern(8, 0.5f));
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
                ExecutePattern(Phase1, 5f);
                break;
            case 1:
                ExecutePattern(Phase2, 5f);
                break;
            case 2:
                ExecutePattern(Phase3, 5f);
                break;
            case 3:
                ExecutePattern(Phase4, 5f);
                break;
            case 4:
                ExecutePattern(Phase5, 5f);
                break;
        }
        randomY = Mathf.Clamp(Random.Range(bottomBorder + 4.5f, topBorder - 3), bottomBorder + 4.5f, topBorder - 3); // 범위 제한

        float clampedX = Mathf.Clamp(playerPos.x, leftBorder + 10, rightBorder - 10);
        transform.DOMove(new Vector3(clampedX, randomY, transform.position.z), 3f).SetEase(Ease.InOutBack);
    }
    private void ExecutePattern(Action phasePattern, float waitTime)
    {
        if (!isPatterning)
        {
            StartCoroutine(RunPattern(phasePattern, waitTime));
        }
    }
    private IEnumerator RunPattern(Action phasePattern, float waitTime)
    {
        isPatterning = true;
        phasePattern.Invoke();
        yield return new WaitForSeconds(waitTime); // 지정된 시간만큼 대기
        isPatterning = false; 
    }
    private IEnumerator LaserPattern(int count, float interval)
    {
        float gap = 1.5f;
        float indicatorGap = 1.5f;
        FireHomingMissile(this.gameObject.transform.position, playerPos, 5f, 2);
        for (int i = 0; i < count; i++)
        {
            ShowIndicator(new Vector3(0,bottomBorder + 0.5f,0) + new Vector3(indicatorGap, -0.6f, 0), 0.8f);
            ShowIndicator(new Vector3(0, bottomBorder+ 0.5f, 0) + new Vector3(-indicatorGap, -0.6f, 0), 0.8f);
            yield return new WaitForSeconds(0.3f);

            indicatorGap += 2f;
        }
        for (int i = 0; i < count; i++)
        {
            FireLaser(new Vector3(0, bottomBorder + 0.5f, 0) + new Vector3(gap, -0.6f, 0), 1.5f);
            FireLaser(new Vector3(0, bottomBorder + 0.5f, 0) + new Vector3(-gap, -0.6f, 0), 1.5f);

            gap += 2f;
            yield return new WaitForSeconds(interval);
        }
        
    }
    private void FireLaser(Vector3 targetPosition, float duration)
    {
        GameObject laser = ParticlePool.Instance.GetParticle(0); // 0: 레이저 풀 타입
        
        if (laser != null)
        {
            // 레이저 초기화
            laser.transform.position = targetPosition;
            // 레이저 지속 시간 후 반환
            StartCoroutine(ReturnToPool(0,laser, duration));
        }
    }
    private void FireHomingMissile(Vector3 startPosition, Vector3 endPosition, float duration, int type)
    {
        //type = 발사할 유도탄 풀타입
        // 유도탄 생성
        GameObject missile = ParticlePool.Instance.GetParticle(type);
        if(missile != null)
        {
            missile.transform.position = startPosition;
            // DOTween으로 유도탄 이동
            missile.transform.DOMove(endPosition, duration)
                .SetEase(Ease.Linear)                   // 선형 이동
                .OnComplete(() => StartCoroutine(ReturnToPool(type, missile, 0.5f)));   // 도착 후 반환

        }
        //// 미사일 회전 효과 (옵션)
        //missile.transform.DOLookAt(endPosition, 0.5f)
        //    .SetEase(Ease.OutSine);
    }

    private void ShowIndicator(Vector3 targetPosition, float duration)
    {
        GameObject indicator = ParticlePool.Instance.GetParticle(1); // 1: 인디케이터 풀 타입
        if(indicator != null)
        {
            indicator.transform.position = targetPosition;
            StartCoroutine(ReturnToPool(1, indicator, duration));
        }
    }
    private IEnumerator ReturnToPool(int typeIndex, GameObject particle, float duration)
    {
        particle.TryGetComponent<BossBullet>(out BossBullet bullet);
        if(bullet != null)
        {
            if (bullet.isEntering)
            {
                bullet.OnParticleTriggerExit();
            }
        }

        yield return new WaitForSeconds(duration);

        ParticlePool.Instance.ReturnParticle(typeIndex, particle);
    }
}
