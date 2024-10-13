using System;
using System.Collections;
using System.Collections.Generic;
using BulletPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Random = UnityEngine.Random;

public class Boss1 : Boss
{
    private GameObject player;
    private Vector3 playerPos;

    protected override void Awake()
    {
        base.Awake();
        player = GameObject.FindWithTag("Player");
    }

    protected override void Update()
    {
        base.Update();

        playerPos = player.transform.position;

        if(HP == 60 || HP == 30)
        {
            float randomX = Random.Range(leftBorder + 8, rightBorder - 8);
            float randomY = Random.Range(bottomBorder + 3, topBorder - 3);
            transform.DOMove(new Vector3(randomX, randomY, transform.position.z), 3f).SetEase(Ease.InOutBack);
        }
    }

    protected override void PhaseChange(int bossPhase)
    {
        base.PhaseChange(bossPhase);
        
        // 탄막 패턴 설정

        // 1 ~ 4
        // -2.14 ~ 6.9
        

        float randomY = Random.Range(bottomBorder + 3, topBorder - 3);
        transform.DOMove(new Vector3(playerPos.x, randomY, transform.position.z), 3f).SetEase(Ease.InOutBack);
    }
}
