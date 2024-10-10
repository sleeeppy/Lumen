using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Bird : MonoBehaviour
{
    [SerializeField]
    private GameObject player;
    [SerializeField]
    private GameObject Birds_Pos;
    [SerializeField]
    private Vector3 Birds_tf;
    private Vector3 playerPos;
    [SerializeField]
    private GameObject[] Snowball;
    void OnEnable()
    {
        StartCoroutine(Start_Attack());
    }
    void Update()
    {
        Birds_tf = Birds_Pos.transform.position;
        transform.DOMove(Birds_tf, 0.2f);
    }
    IEnumerator Start_Attack()
    {
        for (int i = 0; i < 5; i++)
        {
            Snowball[i].SetActive(true);
            yield return new WaitForSeconds(0.5f);
        }

    }
}
