using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Pool;
using Unity.VisualScripting;
using System;

public class ParticlePool : MonoBehaviour
{
    [System.Serializable]
    private class ObjectInfo
    {
        public string name;
        public GameObject prefab;
        public int count;
    }
    public static ParticlePool Instance { get; private set; }
    [SerializeField] private ObjectInfo[] objectInfos = null;
    private Dictionary<string, IObjectPool<GameObject>> pools = new Dictionary<string, IObjectPool<GameObject>>();
    private Dictionary<string, GameObject> poolCreate = new Dictionary<string, GameObject>();
    private string objectName;

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
        Init();
    }

    private void Init()
    {
        for (int idx = 0; idx < objectInfos.Length; idx++)
        {
            IObjectPool<GameObject> pool = new ObjectPool<GameObject>(CreatePooledParticle, OnParticleGet, OnParticleRelease,
            OnParticleDestroy, true, objectInfos[idx].count, objectInfos[idx].count);

            if (poolCreate.ContainsKey(objectInfos[idx].name))
            {
                Debug.LogFormat("{0} 이미 등록된 오브젝트입니다.", objectInfos[idx].name);
                return;
            }

            poolCreate.Add(objectInfos[idx].name, objectInfos[idx].prefab);
            pools.Add(objectInfos[idx].name, pool);

            // 미리 오브젝트 생성 해놓기
            for (int i = 0; i < objectInfos[idx].count; i++)
            {
                objectName = objectInfos[idx].name;
                PoolAble poolAble = CreatePooledParticle().GetComponent<PoolAble>();
                poolAble.Pool.Release(poolAble.gameObject);
            }
        }
    }

    // 생성
    private GameObject CreatePooledParticle()
    {
        GameObject pool = Instantiate(poolCreate[objectName]);
        pool.GetComponent<PoolAble>().Pool = pools[objectName];
        return pool;
    }

    // 활성
    private void OnParticleGet(GameObject particle)
    {
        particle.SetActive(true);
    }

    // 비활성
    public void OnParticleRelease(GameObject particle)
    {
        particle.SetActive(false);
    }

    // 삭제
    private void OnParticleDestroy(GameObject particle)
    {
        Destroy(particle);
    }

    //사용
    public GameObject GetParticle(string name)
    {
        objectName = name;

        if (poolCreate.ContainsKey(name) == false)
        {
            Debug.LogFormat("{0} 오브젝트풀에 등록되지 않은 오브젝트입니다.", name);
            return null;
        }

        return pools[name].Get();
    }
}
