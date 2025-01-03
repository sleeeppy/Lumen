using System;
using System.Collections;
using System.Collections.Generic;
using BulletPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Rendering;
using Unity.VisualScripting;

public class Boss_Pattern : Re_Boss
{
    [SerializeField]
    private GameObject player;
    private Vector3 playerPos;
    [Header("Attack")]
    [SerializeField]
    private Volume PostProcessing_Component;


    protected override void Awake()
    {
        base.Awake();
    }



    protected override void Update()
    {
        base.Update();

        playerPos = player.transform.position;

        if (HP == 60 || HP == 30)
        {
            float randomX = UnityEngine.Random.Range(leftBorder + 7, rightBorder - 7);
            float randomY = UnityEngine.Random.Range(bottomBorder + 5, topBorder - 2);
            transform.DOMove(new Vector3(randomX, randomY, transform.position.z), 1.5f).SetEase(Ease.InOutBack);
        }
    }

    protected override void PhaseChange(int bossPhase)
    {
        base.PhaseChange(bossPhase);
        float randomY = UnityEngine.Random.Range(bottomBorder + 7, topBorder - 3);
        transform.DOMove(new Vector3(playerPos.x, randomY, transform.position.z), 1.5f).SetEase(Ease.InOutBack);
    }

}
