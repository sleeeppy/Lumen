using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBullet : MonoBehaviour
{
    private ParticleSystem ps;
    private ParticleSystem.TriggerModule triggerModule;
    private GameObject player;
    public bool isEntering = false;
    void Awake()
    {
        TryGetComponent<ParticleSystem>(out ps);
        player = GameObject.FindWithTag("Player");
        // 트리거 모듈 활성화
        if (ps != null)
        {
            triggerModule = ps.trigger;
            if(triggerModule.enabled == true)
            {
                Collider collider = player.GetComponentInChildren<CapsuleCollider>();
                triggerModule.SetCollider(0, collider);
            }
        }
    }
    
    private void OnParticleTrigger()
    {
        if (ps == null || player == null) return;
        // Enter
        List<ParticleSystem.Particle> enterParticles = new List<ParticleSystem.Particle>();
        ps.GetTriggerParticles(ParticleSystemTriggerEventType.Enter, enterParticles);

        if (enterParticles.Count > 0)
        {
            OnParticleTriggerEnter();
        }

        // Exit
        List<ParticleSystem.Particle> exitParticles = new List<ParticleSystem.Particle>();
        ps.GetTriggerParticles(ParticleSystemTriggerEventType.Exit, exitParticles);

        if (exitParticles.Count > 0)
        {
            OnParticleTriggerExit();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player3DCollider"))
        {
            player.GetComponent<Player>().OnHitByBullet();
            Debug.Log("Enter");
            isEntering = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player3DCollider"))
        {
            OnParticleTriggerExit();
        }
    }

    private void OnParticleTriggerEnter()
    {
        Debug.Log("Trigger Enter");
        isEntering = true;
        player.GetComponent<Player>().OnHitByBullet();
    }
    public void OnParticleTriggerExit()
    {
        Debug.Log("Trigger Exit");
        isEntering = false;
    }
}
