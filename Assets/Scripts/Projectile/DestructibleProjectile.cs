using RetroArsenal;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;

public class DestructibleProjectile : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PlayerBullet"))
        {
            RetroProjectileScript projectileScript = GetComponent<RetroProjectileScript>();
            GameObject impactP = Instantiate(projectileScript.impactParticle, transform.position, Quaternion.FromToRotation(Vector3.up, other.transform.position.normalized)) as GameObject;
            Destroy(impactP, 5.0f);
            Destroy(projectileScript.projectileParticle, 3f);
            projectileScript.DestroyMissile();
            Destroy(other.gameObject);
        }
    }
}
