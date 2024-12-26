using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ProjectileBase : MonoBehaviour
{
    protected Rigidbody rb;
    public float speed;

    public virtual void Setup(Transform target)
    {
        rb = GetComponent<Rigidbody>();
    }
    public void MoveTo(Vector3 direction)
    {
        rb.velocity = direction * speed;
    }
    private void Update()
    {
        Process();
    }
    public abstract void Process();
}
