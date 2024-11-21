using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Pool;

public class ParticlePool : MonoBehaviour
{
    public static ParticlePool Instance { get; private set; }
    [AssetsOnly]
    [SerializeField] private GameObject[] particlePrefabs;
    [SerializeField] private int initialPoolSize = 10;
    [SerializeField] private int maxPoolSize = 50;

    private List<IObjectPool<GameObject>> particlePools;
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
        particlePools = new List<IObjectPool<GameObject>>();

        foreach (var prefab in particlePrefabs)
        {
            var pool = new ObjectPool<GameObject>(
                createFunc: () => Instantiate(prefab),         // 생성 로직
                actionOnGet: particle => particle.SetActive(true),  // Get 시
                actionOnRelease: particle => particle.SetActive(false), // Release 시
                actionOnDestroy: Destroy,                    // 풀 제거 시
                defaultCapacity: initialPoolSize,
                maxSize: maxPoolSize
            );
            particlePools.Add(pool);
        }
    }
    //사용
    public GameObject GetParticle(int typeIndex)
    {
        if (typeIndex < 0 || typeIndex >= particlePools.Count)
        {
            Debug.LogError($"Invalid typeIndex: {typeIndex}");
            return null;
        }

        return particlePools[typeIndex].Get();
    }
    
    //반환
    public void ReturnParticle(int typeIndex, GameObject particle)
    {
        if (typeIndex < 0 || typeIndex >= particlePools.Count)
        {
            Debug.LogError($"Invalid typeIndex: {typeIndex}");
            return;
        }

        particlePools[typeIndex].Release(particle);
    }
}
