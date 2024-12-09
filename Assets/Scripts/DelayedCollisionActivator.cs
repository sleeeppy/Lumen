using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayedCollisionActivator : MonoBehaviour
{
    [SerializeField] float enableTime; //켜질시간
    [SerializeField] float disableTime; //꺼질시간
    [SerializeField] Collider collider;

    private ParticleSystem ps;
    private void Start()
    {
        ps = GetComponent<ParticleSystem>();
        if (collider == null) collider = GetComponent<Collider>();
    }
    private void Update()
    {
        if (collider != null)
        {
            if (ps.time >= enableTime && collider.enabled == false)
            {
                collider.enabled = true;
            }
            else
            {
                collider.enabled = false;
            }
        }
    }
}
