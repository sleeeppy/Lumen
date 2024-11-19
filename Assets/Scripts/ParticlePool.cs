using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class ParticlePool : MonoBehaviour
{
    public static ParticlePool Instance { get; private set; }
    [AssetsOnly]
    [SerializeField] private GameObject[] particlePrefabs;
    [SerializeField] private int poolSize = 10;

    private List<Queue<GameObject>> particlePools;

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

        particlePools = new List<Queue<GameObject>>();

        for (int i = 0; i < particlePrefabs.Length; i++)
        {
            InitializePool(i);
        }
    }
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
            return Instantiate(particlePrefabs[typeIndex]);
        }
    }
    //반환
    public void ReturnParticle(int typeIndex, GameObject particle)
    {
        particle.SetActive(false);
        particlePools[typeIndex].Enqueue(particle);
    }
}
