using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float Damage;
    [SerializeField] private float deletetime = 2f;
    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        // 사거리 제한 체크
        // if (Vector3.Distance(startPosition, transform.position) >= distanceLimit)
        // {
        //     Destroy(gameObject); // 사거리를 초과하면 총알 파괴
        // }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Border") && gameObject.name == "LaserMissileRedOBJ")
            Destroy(gameObject);
    }

    // private void OnTriggerStay(Collider other)
    // {
    //     Debug.Log($"{other.gameObject.name}");
    //     if (gameObject.name == "LaserBeamPurpleStatic" 
    //         && other.gameObject.CompareTag("Boss3DCollider"))
    //     {
    //         Boss boss = other.GetComponentInParent<Boss>();
    //         if (boss != null)
    //         {
    //             boss.HandleCollision(gameObject); // 충돌 처리 호출
    //         }
    //     }
    // }

    // public void SetDistanceLimit(float limit)
    // {
    //     distanceLimit = limit;
    // }

}
