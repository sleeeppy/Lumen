using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Pool;

public class ParticlePool : MonoBehaviour
{
    public static ParticlePool Instance { get; private set; }
    [SerializeField,TabGroup("Prefab")] private GameObject[] particlePrefabs;
    [SerializeField,TabGroup("InitialPoolSize")] private int[] initialPoolSizes;
    [SerializeField,TabGroup("MaxPoolSize")] private int[] maxPoolSizes;

    private IObjectPool<GameObject>[] particlePools;
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
        InitializePools();
    }
    //private void LoadParticlePrefabs()
    //{
    //    particlePrefabs = Resources.LoadAll<GameObject>(PrefabPath);
    //}
    //초기화
    private void InitializePools()
    {
        if (particlePrefabs.Length != initialPoolSizes.Length || particlePrefabs.Length != maxPoolSizes.Length)
        {
            Debug.LogError("The lengths of particlePrefabs, initialPoolSizes, and maxPoolSizes must match.");
            return;
        }

        // 각 프리팹별로 풀 초기화
        particlePools = new ObjectPool<GameObject>[particlePrefabs.Length];

        for (int i = 0; i < particlePrefabs.Length; i++)
        {
            int initialSize = initialPoolSizes[i];
            int maxSize = maxPoolSizes[i];

            particlePools[i] = new ObjectPool<GameObject>(
                createFunc: () => Instantiate(particlePrefabs[i]),  // 객체 생성
                actionOnGet: particle => particle.SetActive(true),  // Get 시 활성화
                actionOnRelease: particle => particle.SetActive(false), // Release 시 비활성화
                actionOnDestroy: Destroy,                            // Clear 또는 MaxSize 초과 시 삭제
                defaultCapacity: initialSize,                        // 초기 풀 크기
                maxSize: maxSize                                     // 최대 풀 크기
            );
        }
    }
    //사용
    public GameObject GetParticle(int typeIndex)
    {
        if (typeIndex < 0 || typeIndex >= particlePools.Length)
        {
            Debug.LogError($"Invalid typeIndex: {typeIndex}");
            return null;
        }

        return particlePools[typeIndex].Get();
    }
    
    //반환
    public void ReturnParticle(int typeIndex, GameObject particle)
    {
        if (typeIndex < 0 || typeIndex >= particlePools.Length)
        {
            Debug.LogError($"Invalid typeIndex: {typeIndex}");
            return;
        }

        particlePools[typeIndex].Release(particle);
    }
}
