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
    protected IEnumerator NextPattern(float time, int profileNum)
    {
        bulletEmitter.Pause();
        yield return new WaitForSeconds(time);
        bulletEmitter.emitterProfile = bossPattern[profileNum];
        bulletEmitter.Play();
    }
    protected void PhaseChange(int bossPhase)
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
        randomY = Mathf.Clamp(Random.Range(bottomBorder + 4.5f, topBorder - 3), bottomBorder + 4.5f, topBorder - 3); // 범위 제한

        float clampedX = Mathf.Clamp(playerPos.x, leftBorder + 10, rightBorder - 10);
        transform.DOMove(new Vector3(clampedX, randomY, transform.position.z), 3f).SetEase(Ease.InOutBack);
    }
}
