using RetroArsenal;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;

public class DestructibleProjectile : MonoBehaviour
{
    [SerializeField] private float destructionTime = 0.2f;
    private float hitTimer = 0f;
    private bool isHit = false;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PlayerBullet"))
        {
            DestroyProjectile(other.gameObject);
        }
    }
    private void DestroyProjectile(GameObject gameObject)
    {
        RetroProjectileScript projectileScript = GetComponent<RetroProjectileScript>();
        GameObject impactP = Instantiate(projectileScript.impactParticle, transform.position, Quaternion.FromToRotation(Vector3.up, gameObject.transform.position.normalized)) as GameObject;
        Destroy(impactP, 5.0f);
        Destroy(projectileScript.projectileParticle, 3f);
        projectileScript.DestroyMissile();
        Destroy(gameObject);
    }
    public void OnHit(float deltaTime)
    {
        isHit = true;
        hitTimer += deltaTime;

        if (hitTimer >= destructionTime) DestroyProjectile(this.gameObject);
    }
}
