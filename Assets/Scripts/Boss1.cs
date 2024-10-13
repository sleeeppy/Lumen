using System;
using System.Collections;
using System.Collections.Generic;
using BulletPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Boss1 : Boss
{
    private GameObject player;
    private Vector3 playerPos;

    protected override void Awake()
    {
        base.Awake();
        player = GameObject.FindWithTag("Player");
        //Debug.Log($"{topBorder}, {bottomBorder}");
    }

    protected override void Update()
    {
        base.Update();

        playerPos = player.transform.position;

        if(HP == 60 || HP == 30)
        {
            float randomX = UnityEngine.Random.Range(leftBorder + 7, rightBorder - 7);
            float randomY = UnityEngine.Random.Range(bottomBorder + 6, topBorder - 5);
            transform.DOMove(new Vector3(randomX, randomY, transform.position.z), 1.5f).SetEase(Ease.InOutBack);
        }
    }

    protected override void PhaseChange(int bossPhase)
    {
        base.PhaseChange(bossPhase);
        
        // 탄막 패턴 설정
        float randomY = UnityEngine.Random.Range(bottomBorder + 7, topBorder - 3);
        transform.DOMove(new Vector3(playerPos.x, randomY, transform.position.z), 1.5f).SetEase(Ease.InOutBack);
    }
}
