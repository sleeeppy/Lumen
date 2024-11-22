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
        // Ʈ���� ��� Ȱ��ȭ
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
        player.GetComponent<Player>().OnHitByBullet();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
            player.GetComponent<Player>().OnHitByBullet();
    }
}
