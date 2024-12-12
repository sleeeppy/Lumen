using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ProjectileStraight : ProjectileBase
{
    public override void Setup(Transform target)
    {
        base.Setup(target);

        base.MoveTo((target.position - transform.position).normalized);
    }

    public override void Process()
    {
        
    }
}
