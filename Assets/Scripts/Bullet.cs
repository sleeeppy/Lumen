using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float Damage;
    [SerializeField] private float deletetime = 2f;
    
    void Update()
    {
        Destroy(gameObject, deletetime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Border")
            Destroy(gameObject);
    }
}
