using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager
{
    public Transform Root { get { return GetRoot("@Object_Root", _root); } }
    public Transform PlayerProjectileRoot { get { return GetRoot("PlayerProjectileTransform_Root", _playerProjectileRoot); } }
    public Transform BossProjectileRoot { get { return GetRoot("BossProjectileTransform", _bossProjectileRoot); } }
    
    private Transform _root;
    private Transform _playerProjectileRoot;
    private Transform _bossProjectileRoot;
    
    
    public Projectile CreateProjectile(string name, bool isPlayer)
    {
        Transform root = isPlayer == true ? PlayerProjectileRoot : BossProjectileRoot;
        GameObject prefab = Resources.Load<GameObject>($"Prefab/Projectile/{name}");
        GameObject go = Object.Instantiate(prefab, root);

        Projectile projectile = go.GetComponent<Projectile>();
        
        return projectile;
    }

    public Transform GetRoot(string name, Transform transform)
    {
        if (transform != null)
            return transform;

        GameObject go = new GameObject(name);
        go.transform.parent = Root;
        transform = go.transform;
        return transform;
    }
}
