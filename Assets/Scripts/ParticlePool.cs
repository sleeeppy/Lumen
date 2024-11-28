using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Pool;
using Unity.VisualScripting;

public class ParticlePool : MonoBehaviour
{
    public static ParticlePool Instance { get; private set; }
    [SerializeField,TabGroup("Prefab")] private GameObject[] particlePrefabs;
    [SerializeField,TabGroup("InitialPoolSize")] private int[] initialPoolSizes;
    [SerializeField,TabGroup("MaxPoolSize")] private int[] maxPoolSizes;

    private ObjectPool<GameObject>[] particlePools;
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

        particlePools = new ObjectPool<GameObject>[particlePrefabs.Length];
        Debug.Log($"Total pools: {particlePrefabs.Length}");

        for (int i = 0; i < particlePrefabs.Length; i++)
        {
            int index = i;
            particlePools[index] = new ObjectPool<GameObject>(
                createFunc: () =>
                {
                    GameObject prefab = particlePrefabs[index];
                    if (prefab == null)
                    {
                        Debug.LogError($"Prefab at index {index} is null!");
                        return null;
                    }
                    return Instantiate(prefab);
                },
                actionOnGet: particle => particle.SetActive(true),
                actionOnRelease: particle => particle.SetActive(false),
                actionOnDestroy: Destroy,
                defaultCapacity: initialPoolSizes[index],
                maxSize: maxPoolSizes[index]
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
