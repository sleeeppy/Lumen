using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBullet : MonoBehaviour
{
    protected GameObject player;
    [HideInInspector] public bool isEntering = false;
    void Awake()
    {
        player = GameObject.FindWithTag("Player");
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player3DCollider"))
        {
            player.GetComponent<Player>().OnHitByBullet();
            Debug.Log("Enter");
            isEntering = true;
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
