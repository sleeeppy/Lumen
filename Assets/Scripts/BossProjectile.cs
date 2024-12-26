using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ProjectileType
{
    Straight,
    Homing,
    QuadraticHoming,
    CubicHoming
}
public class BossProjectile : BossBullet
{
    public ProjectileType type;
    [SerializeField] private float speed;
    private ProjectileBase projectile;
    private Transform target;


    public void Initialize()
    {
        target = base.player.transform;
        if(projectile != null)
        {
            Destroy(projectile);
        }

        switch (type)
        {
            case ProjectileType.Straight:
                projectile = gameObject.AddComponent<ProjectileStraight>();
                break;
            case ProjectileType.Homing:
                projectile = gameObject.AddComponent<ProjectileHoming>();
                break;
            case ProjectileType.QuadraticHoming:
                break;
            case ProjectileType.CubicHoming:
                break;
        }

        projectile.speed = speed;

        projectile.Setup(target);
    }
    private void Update()
    {
        if(projectile != null)
        {
            projectile.Process();
        }
    }

}
