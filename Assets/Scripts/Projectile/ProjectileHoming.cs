using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileHoming : ProjectileBase
{
    private Transform target;
    public override void Setup(Transform target)
    {
        base.Setup(target);
        this.target = target;
    }
    public override void Process()
    {
        MoveTo((target.position - transform.position).normalized);
    }

    
}
