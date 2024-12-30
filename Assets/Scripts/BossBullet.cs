using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBullet : MonoBehaviour
{
    protected Player player;
    [HideInInspector] public bool isEntering = false;
    void Awake()
    {
        player = GameObject.FindWithTag("Player").GetComponent<Player>();
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player3DCollider") && !player.isInvincibility)
        {
            player.OnHitByBullet();
            Debug.Log("Enter");
            isEntering = true;
            ParticlePool.Instance.OnParticleRelease(gameObject);
        }
        if (other.gameObject.CompareTag("Border"))
        {
            ParticlePool.Instance.OnParticleRelease(gameObject);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player3DCollider"))
        {
            isEntering = false;
        }
    }
    public void LifeTimeExit()
    {
        if (isEntering)
        {
            Debug.Log("LifeTimeExit");
            isEntering = false;
        }
    }
}
