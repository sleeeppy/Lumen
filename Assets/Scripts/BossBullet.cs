using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBullet : MonoBehaviour
{
    private ParticleSystem ps;
    private ParticleSystem.TriggerModule triggerModule;
    private GameObject player;
    void Awake()
    {
        TryGetComponent<ParticleSystem>(out ps);
        player = GameObject.FindWithTag("Player");
        // 트리거 모듈 활성화
        if (ps != null)
        {
            triggerModule = ps.trigger;
            triggerModule.enabled = true;
            Collider collider = player.GetComponentInChildren<CapsuleCollider>();
            triggerModule.SetCollider(0, collider);
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
            OnParticleTriggerEnter(enterParticles);
        }

        // Exit
        List<ParticleSystem.Particle> exitParticles = new List<ParticleSystem.Particle>();
        ps.GetTriggerParticles(ParticleSystemTriggerEventType.Exit, exitParticles);

        if (exitParticles.Count > 0)
        {
            OnParticleTriggerExit(exitParticles);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
            player.GetComponent<Player>().OnHitByBullet();
    }

    private void OnParticleTriggerEnter(List<ParticleSystem.Particle> enterParticles)
    {
        foreach (var particle in enterParticles) //입자 정보 안쓸거면 if(enterParticles.Count > 0)
        {
            Debug.Log("Trigger Enter");
            player.GetComponent<Player>().OnHitByBullet();
        }
    }
    private void OnParticleTriggerExit(List<ParticleSystem.Particle> exitParticles)
    {
        foreach (var particle in exitParticles)
        {
            Debug.Log("Trigger Exit");
        }
    }
}
