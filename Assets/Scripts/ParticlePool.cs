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
    //�ʱ�ȭ
    private void InitializePools()
    {
        if (particlePrefabs.Length != initialPoolSizes.Length || particlePrefabs.Length != maxPoolSizes.Length)
        {
            Debug.LogError("The lengths of particlePrefabs, initialPoolSizes, and maxPoolSizes must match.");
            return;
        }

        // �� �����պ��� Ǯ �ʱ�ȭ
        particlePools = new ObjectPool<GameObject>[particlePrefabs.Length];

        for (int i = 0; i < particlePrefabs.Length; i++)
        {
            int initialSize = initialPoolSizes[i];
            int maxSize = maxPoolSizes[i];

            particlePools[i] = new ObjectPool<GameObject>(
                createFunc: () => Instantiate(particlePrefabs[i]),  // ��ü ����
                actionOnGet: particle => particle.SetActive(true),  // Get �� Ȱ��ȭ
                actionOnRelease: particle => particle.SetActive(false), // Release �� ��Ȱ��ȭ
                actionOnDestroy: Destroy,                            // Clear �Ǵ� MaxSize �ʰ� �� ����
                defaultCapacity: initialSize,                        // �ʱ� Ǯ ũ��
                maxSize: maxSize                                     // �ִ� Ǯ ũ��
            );
        }
    }
    //���
    public GameObject GetParticle(int typeIndex)
    {
        if (typeIndex < 0 || typeIndex >= particlePools.Length)
        {
            Debug.LogError($"Invalid typeIndex: {typeIndex}");
            return null;
        }

        return particlePools[typeIndex].Get();
    }
    
    //��ȯ
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
