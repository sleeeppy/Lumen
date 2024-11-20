using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBullet : MonoBehaviour
{
    [SerializeField] private float deletetime = 2f;
    private Vector3 startPosition;
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
    void Start()
    {
        startPosition = transform.position;
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
