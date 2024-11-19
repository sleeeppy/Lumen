using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBullet : MonoBehaviour
{
    [SerializeField] private float deletetime = 2f;
    private Vector3 startPosition;
    void Start()
    {
        startPosition = transform.position;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Border") && other.gameObject.CompareTag("Player"))
            ParticlePool.Instance.ReturnParticle(0, gameObject);
        else if (other.gameObject.CompareTag("Player"))
            other.gameObject.GetComponent<Player>().OnHitByBullet();
    }
}
