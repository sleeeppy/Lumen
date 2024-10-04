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
    }

    protected override void Update()
    {
        base.Update();

        playerPos = player.transform.position;
    }

    protected override void PhaseChange(int bossPhase)
    {
        base.PhaseChange(bossPhase);
        
        // 탄막 패턴 설정
    }
}
