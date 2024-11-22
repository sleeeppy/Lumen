using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateCollider : MonoBehaviour
{
    private BoxCollider boxCollider;
    
    private void Start()
    {
        boxCollider = gameObject.AddComponent<BoxCollider>();
        boxCollider.isTrigger = true;
    }
}
