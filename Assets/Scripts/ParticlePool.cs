using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class ParticlePool : MonoBehaviour
{
    public static ParticlePool Instance { get; private set; }
    [AssetsOnly]
    [SerializeField] private GameObject[] particlePrefabs;
    [SerializeField] private int poolSize = 10;
    [SerializeField] private int expandSize = 1; // 풀 크기 확장량

    private List<Queue<GameObject>> particlePools;
    //private const string PrefabPath = "Prefabs/Boss2/";

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        //LoadParticlePrefabs();
        particlePools = new List<Queue<GameObject>>();

        for (int i = 0; i < particlePrefabs?.Length; i++)
        {
            InitializePool(i);
        }
    }
    //private void LoadParticlePrefabs()
    //{
    //    particlePrefabs = Resources.LoadAll<GameObject>(PrefabPath);
    //}
    //초기화
    private void InitializePool(int typeIndex)
    {
        Queue<GameObject> pool = new Queue<GameObject>();

        for (int i = 0; i < poolSize; i++)
        {
            GameObject particle = Instantiate(particlePrefabs[typeIndex]);
            particle.SetActive(false);
            pool.Enqueue(particle);
        }

        particlePools.Add(pool);
    }
    //사용
    public GameObject GetParticle(int typeIndex)
    {
        if (particlePools[typeIndex].Count > 0)
        {
            GameObject particle = particlePools[typeIndex].Dequeue();
            particle.SetActive(true);
            return particle;
        }
        else
        {
            ExpandPool(typeIndex);
            return Instantiate(particlePrefabs[typeIndex]);
        }
    }
    // 풀 확장
    private void ExpandPool(int typeIndex)
    {
        Queue<GameObject> pool = particlePools[typeIndex];

        // 풀을 확장하여 새 파티클 추가
        for (int i = 0; i < expandSize; i++)
        {
            GameObject particle = Instantiate(particlePrefabs[typeIndex]);
            particle.SetActive(false);
            pool.Enqueue(particle);
        }

        Debug.Log($"Expanded pool for particle type {typeIndex} by {expandSize}.");
    }
    //반환
    public void ReturnParticle(int typeIndex, GameObject particle)
    {
        particle.SetActive(false);
        particlePools[typeIndex].Enqueue(particle);
    }
}
