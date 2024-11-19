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
        ps = GetComponent<ParticleSystem>();
        player = GameObject.FindWithTag("Player");
        // 트리거 모듈 활성화
        triggerModule = ps.trigger;
        triggerModule.enabled = true;
        triggerModule.SetCollider(0, GameObject.FindWithTag("Player3DCollider").GetComponent<Collider>());
    }
    void Start()
    {
        startPosition = transform.position;
    }
    private void OnParticleTrigger()
    {
        player.GetComponent<Player>().OnHitByBullet();
    }
}
