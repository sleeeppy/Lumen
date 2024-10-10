using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Bird_Bullet : MonoBehaviour
{
    public GameObject Player;
    private Vector3 PlayerPos;
    [SerializeField]
    private ParticleSystem Pung;
    public float Damage;
    [SerializeField]
    private GameObject Bird;


    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            other.gameObject.GetComponent<Player>().OnHitByBullet();
            Debug.Log("닿였다");
            gameObject.SetActive(false);
            Pung.Play();
        }

    }
    void Update()
    {
        transform.DORotate(new Vector3(0, 0, 1080), 2f, RotateMode.FastBeyond360).SetEase(Ease.InQuint);
    }
    void OnEnable()
    {
        transform.position = new Vector3(Bird.transform.position.x - 0.5f, Bird.transform.position.y, Bird.transform.position.z);
        PlayerPos = Player.transform.position;
        transform.DOMove(PlayerPos, 2f).SetEase(Ease.InQuint);
        Invoke("Disable", 2);
        Invoke("PungParticle", 2.1f);
    }
    void PungParticle()
    {
        Pung.Play();
    }
    void Disable()
    {
        Debug.Log("안 닿였다");
        gameObject.SetActive(false);
    }
}