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
        // 사거리 제한 체크 코드 제거
        // if (Vector3.Distance(startPosition, transform.position) >= distanceLimit)
        // {
        //     Destroy(gameObject); // 사거리를 초과하면 총알 파괴
        // }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Border")
            Destroy(gameObject);
    }

    // SetDistanceLimit 메서드 제거
    // public void SetDistanceLimit(float limit)
    // {
    //     distanceLimit = limit;
    // }
}
