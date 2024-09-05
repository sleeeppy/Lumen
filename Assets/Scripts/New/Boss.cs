using System;
using System.Collections;
using System.Collections.Generic;
using BulletPro;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

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

    [SerializeField] public GameObject bulletSpawnPos;
    [SerializeField] public BulletEmitter bulletEmitter;
    [SerializeField] public EmitterProfile[] bossPattern;

    [SerializeField] private Sprite[] bossProfile;
    [SerializeField] private Image curProfile;

    protected virtual void Awake()
    {
        Init();
    }

    public void Init()
    {
        slider.maxValue = maxHP;
        PhaseChange(phase);
    }

    void Update()
    {
            
        slider.value = HP;
        phaseText.text = "x" + phase;

        if (HP <= 0)
        {
            phase--;
            curProfile.sprite = bossProfile[Mathf.Abs(phase - 5)];
            // phase  5->0 4->1 3->2 2-3 1->4 0->5 
            PhaseChange(phase);
            HP = maxHP;
        }
        else if (phase == 0)
        {
            bulletEmitter.Kill();
            gameObject.SetActive(false);
        }

        bulletSpawnPos.transform.position = transform.position;
        
    }

    IEnumerator NextPattern(float time, int profileNum)
    {
        bulletEmitter.Pause();
        yield return new WaitForSeconds(time);
        bulletEmitter.emitterProfile = bossPattern[profileNum];
        bulletEmitter.Play();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("PlayerBullet"))
        {
            Bullet bulletScript = other.gameObject.GetComponent<Bullet>();
            HP -= bulletScript.Damage;
            Destroy(other.gameObject);
        }
    }

    public void PhaseChange(int bossPhase)
    {
        switch (bossPhase)
        {
            case 5:
                if (bulletEmitter.emitterProfile != bossPattern[0])
                {
                    bulletEmitter.emitterProfile = bossPattern[0];
                    bulletEmitter.Play();
                }
                break;
            case 4:
                if (bulletEmitter.emitterProfile != bossPattern[1])
                    StartCoroutine(NextPattern(2f, 1));
                break;
            case 3:
                if (bulletEmitter.emitterProfile != bossPattern[2])
                    StartCoroutine(NextPattern(2f, 2));
                break;
            case 2:
                if (bulletEmitter.emitterProfile != bossPattern[3])
                    StartCoroutine(NextPattern(2f, 3));
                break;
            case 1:
                if (bulletEmitter.emitterProfile != bossPattern[4])
                    StartCoroutine(NextPattern(2f, 4));
                break;
        }
    }
}


