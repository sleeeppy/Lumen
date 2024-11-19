using UnityEngine;
using System.Collections;

namespace RetroArsenal
{
    public class RetroProjectileScript : MonoBehaviour
    {
        public GameObject impactParticle;
        public GameObject projectileParticle;
        public GameObject muzzleParticle;
        public GameObject[] trailParticles;
        [Header("Adjust if not using Sphere Collider")]
        public float colliderRadius = 1f;
        [Range(0f, 1f)]
        public float collideOffset = 0.15f;

        private Rigidbody rb;
        private Transform myTransform;
        private SphereCollider sphereCollider;

        private float destroyTimer = 0f;
        private bool destroyed = false;

        private Attack attackScript;

        void Start()
        {
            rb = GetComponent<Rigidbody>();
            myTransform = transform;
            sphereCollider = GetComponent<SphereCollider>();

            attackScript = FindObjectOfType<Attack>();

            projectileParticle = Instantiate(projectileParticle, myTransform.position, myTransform.rotation) as GameObject;
            projectileParticle.transform.parent = myTransform;

            if (muzzleParticle)
            {
                muzzleParticle = Instantiate(muzzleParticle, myTransform.position, Quaternion.identity) as GameObject;
                muzzleParticle.transform.rotation = Quaternion.Euler(-attackScript.firePos.transform.rotation.eulerAngles.z, 90, 0);
                Destroy(muzzleParticle, 1.5f); // Lifetime of muzzle effect.
            }
            
        }

        void FixedUpdate()
        {
            if (destroyed)
            {
                return;
            }

            float rad = sphereCollider ? sphereCollider.radius : colliderRadius;

            Vector3 dir = rb.velocity;
            float dist = dir.magnitude * Time.deltaTime;

            if (rb.useGravity)
            {
                // Handle gravity separately to correctly calculate the direction.
                dir += Physics.gravity * Time.deltaTime;
                dist = dir.magnitude * Time.deltaTime;
            }

            RaycastHit hit;

            if (Physics.SphereCast(myTransform.position, rad, dir, out hit, dist)
                && !hit.collider.CompareTag("Border"))
            {
                // 총알이 보스를 피격했을 경우
                if (hit.collider.CompareTag("Boss3DCollider"))
                {
                    Boss boss = hit.collider.gameObject.GetComponentInParent<Boss>();
                    if (boss != null)
                    {
                        // HandleCollision에 Bullet gameObject를 넘김
                        boss.HandleCollision(gameObject);
                    }
                    //테스트용입니다. 테스트후 성공하면 삭제하지말아주세요ㅠㅠ
                    Re_Boss re_boss = hit.collider.gameObject.GetComponentInParent<Re_Boss>();
                    if (re_boss != null)
                    {
                        // HandleCollision에 Bullet gameObject를 넘김
                        re_boss.HandleCollision(gameObject);
                    }
                }

                myTransform.position = hit.point + (hit.normal * collideOffset);

                GameObject impactP = Instantiate(impactParticle, myTransform.position, Quaternion.FromToRotation(Vector3.up, hit.normal)) as GameObject;

                if (hit.transform.tag == "Target") // Projectile will affect objects tagged as Target
                {
                    RetroTarget retroTarget = hit.transform.GetComponent<RetroTarget>();
                    if (retroTarget != null)
                    {
                        retroTarget.OnHit();
                    }
                }

                foreach (GameObject trail in trailParticles)
                {
                    GameObject curTrail = myTransform.Find(projectileParticle.name + "/" + trail.name).gameObject;
                    curTrail.transform.parent = null;
                    Destroy(curTrail, 3f);
                }
                Destroy(projectileParticle, 3f);
                Destroy(impactP, 5.0f);
                DestroyMissile();
            }
            else
            {
                // Increment the destroyTimer if the projectile hasn't hit anything.
                destroyTimer += Time.deltaTime;

                // Destroy the missile if the destroyTimer exceeds 5 seconds.
                if (destroyTimer >= 5f)
                {
                    DestroyMissile();
                }
            }

            // 카메라 뷰포트 체크
            // if (!IsInCameraView())
            // {
            //     DestroyMissile(); // 카메라 뷰포트를 벗어나면 총알 삭제
            // }

            RotateTowardsDirection();
        }

        // private bool IsInCameraView()
        // {
        //     Vector3 screenPoint = Camera.main.WorldToViewportPoint(myTransform.position);
        //     return screenPoint.x >= 0 && screenPoint.x <= 1 && screenPoint.y >= 0 && screenPoint.y <= 1 && screenPoint.z > 0;
        // }

        private void DestroyMissile()
        {
            destroyed = true;

            foreach (GameObject trail in trailParticles)
            {
                GameObject curTrail = myTransform.Find(projectileParticle.name + "/" + trail.name).gameObject;
                curTrail.transform.parent = null;
                Destroy(curTrail, 3f);
            }
            Destroy(projectileParticle, 3f);
            Destroy(gameObject);

            ParticleSystem[] trails = GetComponentsInChildren<ParticleSystem>();
            //Component at [0] is that of the parent i.e. this object (if there is any)
            for (int i = 1; i < trails.Length; i++)
            {
                ParticleSystem trail = trails[i];
                if (trail.gameObject.name.Contains("Trail"))
                {
                    trail.transform.SetParent(null);
                    Destroy(trail.gameObject, 2f);
                }
            }
        }

        private void RotateTowardsDirection()
        {
            if (rb.velocity != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(rb.velocity.normalized, Vector3.up);
                float angle = Vector3.Angle(myTransform.forward, rb.velocity.normalized);
                float lerpFactor = angle * Time.deltaTime; // Use the angle as the interpolation factor
                myTransform.rotation = Quaternion.Slerp(myTransform.rotation, targetRotation, lerpFactor);
            }
        }
    }
}
