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
        base.Update();
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
        Debug.Log("Phase1");
        StartCoroutine(LaserPattern(5, 1f));
        StartCoroutine(NextPattern(6f, 2));
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
    public IEnumerator NextPattern(float time, int phaseNum)
    {
        yield return new WaitForSeconds(time);

        PhaseChange(phaseNum);
    }

    private IEnumerator LaserPattern(int count, float interval)
    {
        float gap = 1.5f;
        for (int i = 0; i < count; i++)
        {
            // 플레이어 위치 기준으로 레이저 발사
            FireLaser(player.transform.position + new Vector3(gap, -0.6f, 0), 1.5f);
            FireLaser(player.transform.position + new Vector3(-gap, -0.6f, 0), 1.5f);

            gap += 1.5f;
            yield return new WaitForSeconds(interval);
        }
    }
    public void FireLaser(Vector3 targetPosition, float duration)
    {
        // 풀에서 레이저 가져오기
        GameObject laser = ParticlePool.Instance.GetParticle(0); // 0: 레이저 풀 타입
        if (laser != null)
        {
            Vector3 firePosition = targetPosition;
            // 레이저 초기화
            laser.transform.position = firePosition;

            // 레이저 지속 시간 후 반환
            StartCoroutine(ReturnLaserToPool(laser, duration));
        }
    }

    private IEnumerator ReturnLaserToPool(GameObject laser, float duration)
    {
        yield return new WaitForSeconds(duration);
        ParticlePool.Instance.ReturnParticle(1, laser);
    }

}
