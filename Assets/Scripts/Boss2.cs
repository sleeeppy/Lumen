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
        PhaseChange(phase);
        player = GameObject.FindWithTag("Player");
        //Debug.Log($"{bottomBorder + 4.5}, {topBorder - 3}");
    }

    protected override void Update()
    {
        base.Update();
        PhaseChange(phase);

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

        
        randomY = Mathf.Clamp(Random.Range(bottomBorder + 4.5f, topBorder - 3), bottomBorder + 4.5f, topBorder - 3); // 범위 제한

        float clampedX = Mathf.Clamp(playerPos.x, leftBorder + 10, rightBorder - 10);
        transform.DOMove(new Vector3(clampedX, randomY, transform.position.z), 3f).SetEase(Ease.InOutBack);
    }
    public IEnumerator NextPattern(float time, int phaseNum)
    {
        yield return new WaitForSeconds(time);
    }
}
