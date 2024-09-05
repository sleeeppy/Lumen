// using JetBrains.Annotations;
// using System.ComponentModel.Design;
// using UnityEngine;
// using UnityEngine.UI;
// using UnityEngine.UIElements;
//
// public class Enemy : MonoBehaviour
// {
//     public string enemyName;
//     public float speed;
//     public int health;
//     public int enemyScore;
//
//     public Sprite[] sprites;
//
//     public float curShotDelay;
//     public float maxShotDelay;
//
//     public GameObject bulletObjA;
//     public GameObject bulletObjB;
//     public GameObject itemCoin;
//     public GameObject itemPower;
//     public GameObject itemBoom;
//     public GameObject player;
//     public ObjectManager objectManager;
//     public GameManager gameManager;
//
//     SpriteRenderer spriteRenderer;
//     Animator anim;
//
//     public int patterni;
//     public int curPatternCount;
//     public int[] maxPatternCount;
//
//     void Awake()
//     {
//         spriteRenderer = GetComponent<SpriteRenderer>();
//         if(enemyName =="B")
//             anim = GetComponent<Animator>();
//     }
//
//     void OnEnable()
//     {
//         // HP settings
//         switch (enemyName)
//         {
//             case "B":
//                 health = 1000;
//                 Invoke("Stop", 2);
//                 break;
//             case "L":
//                 health = 40; 
//                 break;
//             case "M":
//                 health = 10;
//                 break;
//             case "S":
//                 health = 3;
//                 break;
//         }
//     }
//
//     void Stop()
//     {
//         // Boss wait time
//         if (!gameObject.activeSelf)
//             return;
//
//         Rigidbody2D rigid = GetComponent<Rigidbody2D>();
//         rigid.velocity = Vector2.zero;
//
//         Invoke("Think", 2);
//     }
//
//     void Think()
//     {
//         // Boss patterns
//
//         // patterni must be set starting from -1 ( in Inspecter )
//         patterni = patterni == 3 ? 0 : patterni + 1;
//         curPatternCount = 0;
//
//         switch(patterni)
//         {
//             case 0:
//                 FireFoward();
//                 break;
//             case 1:
//                 FireShot();
//                 break;
//             case 2:
//                 FireArc();
//                 break;
//             case 3:   
//                 FireAround();
//                 break;
//         }
//     }
//
//     void FireFoward()
//     {
//         // Pattern 1
//         if (health <= 0) 
//             return;
//
//         GameObject bulletR = objectManager.MakeObj("BulletBossA");
//         bulletR.transform.position = transform.position + Vector3.right * 0.3f;
//         GameObject bulletRR = objectManager.MakeObj("BulletBossA");
//         bulletRR.transform.position = transform.position + Vector3.right * 0.45f;
//         GameObject bulletL = objectManager.MakeObj("BulletBossA");
//         bulletL.transform.position = transform.position + Vector3.left * 0.3f;
//         GameObject bulletLL = objectManager.MakeObj("BulletBossA");
//         bulletLL.transform.position = transform.position + Vector3.left * 0.45f;
//
//         Rigidbody2D rigidR = bulletR.GetComponent<Rigidbody2D>();
//         Rigidbody2D rigidRR = bulletRR.GetComponent<Rigidbody2D>();
//         Rigidbody2D rigidL = bulletL.GetComponent<Rigidbody2D>();
//         Rigidbody2D rigidLL = bulletLL.GetComponent<Rigidbody2D>();
//
//         rigidR.AddForce(Vector2.down * 8, ForceMode2D.Impulse);
//         rigidRR.AddForce(Vector2.down * 8, ForceMode2D.Impulse);
//         rigidL.AddForce(Vector2.down * 8, ForceMode2D.Impulse);
//         rigidLL.AddForce(Vector2.down * 8, ForceMode2D.Impulse);
//
//         curPatternCount++;
//
//         if (curPatternCount < maxPatternCount[patterni])
//             Invoke("FireFoward", 2);
//         else 
//             Invoke("Think", 3);
//     }
//
//     void FireShot()
//     {
//         // Pattern 2
//         if (health <= 0)
//             return;
//
//         for (int i = 0; i < 5; i++)
//         {
//             GameObject bullet = objectManager.MakeObj("BulletEnemyB");
//             bullet.transform.position = transform.position;
//
//             Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
//
//             // Fires with a small random value added to the player's direction
//             Vector2 dirVec = player.transform.position - transform.position;
//             Vector2 ranVec = new Vector2(Random.Range(-1f, 1f), Random.Range(0f, 4f));
//             dirVec += ranVec;
//             rigid.AddForce(dirVec.normalized * 6, ForceMode2D.Impulse);
//         }
//
//         curPatternCount++;
//
//         if (curPatternCount < maxPatternCount[patterni])
//             Invoke("FireShot", 3.5f);
//         else
//             Invoke("Think", 3);
//     }
//
//     void FireArc()
//     {
//         // Pattern 3
//         if (health <= 0)
//             return;
//
//         GameObject bullet = objectManager.MakeObj("BulletEnemyA");
//         bullet.transform.position = transform.position;
//         bullet.transform.rotation = Quaternion.identity;
//
//         Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
//
//         // Fire in a curved line
//         Vector2 dirVec = new Vector2(Mathf.Cos(Mathf.PI * 5 * curPatternCount / maxPatternCount[patterni]), -1);
//         rigid.AddForce(dirVec.normalized * 3, ForceMode2D.Impulse);
//
//         curPatternCount++;
//
//         // Set maxPatternCount to odd number
//         // -> The bullet's trajectory continues to change.
//         if (curPatternCount < maxPatternCount[patterni])
//             Invoke("FireArc", 0.18f);
//         else
//             Invoke("Think", 3);
//     }
//
//     void FireAround()
//     {
//         // Pattern 4
//         if (health <= 0)
//             return;
//
//         int roundNumA = 20;
//         int roundNumB = 19;
//         int roundNum = curPatternCount % 2 == 0 ? roundNumA : roundNumB;
//
//         for(int i = 0; i < roundNum; i++)
//         {
//             GameObject bullet = objectManager.MakeObj("BulletBossB");
//             bullet.transform.position = transform.position;
//             bullet.transform.rotation = Quaternion.identity;
//
//             Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
//
//             // Fire in a circle
//             Vector2 dirVec = new Vector2(Mathf.Cos(Mathf.PI * 2 * i / roundNum)
//                                         ,Mathf.Sin(Mathf.PI * 2 * i / roundNum));
//             rigid.AddForce(dirVec.normalized * 2, ForceMode2D.Impulse);
//
//             Vector3 rotVec = Vector3.forward * 360 * i / roundNum + Vector3.forward * 90;
//             bullet.transform.Rotate(rotVec);
//         }
//
//         curPatternCount++;
//
//         if (curPatternCount < maxPatternCount[patterni])
//             Invoke("FireAround", 0.7f);
//         else
//             Invoke("Think", 3);
//     }
//
//     void Update()
//     {
//         if (enemyName == "B")
//             return;
//
//         Fire();
//         Reload();
//     }
//
//     void Fire()
//     {
//         if (curShotDelay < maxShotDelay)
//             return;
//
//         // Fire bullets according to enemy type
//         if (enemyName == "S")
//         {
//             GameObject bullet = objectManager.MakeObj("BulletEnemyA");
//             bullet.transform.position = transform.position;
//
//             Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
//             Vector3 dirVec = player.transform.position - transform.position;
//             rigid.AddForce(dirVec.normalized * 3, ForceMode2D.Impulse);
//         }
//         else if(enemyName == "L")
//         {
//             GameObject bulletR = objectManager.MakeObj("BulletEnemyB");
//             bulletR.transform.position = transform.position + Vector3.right * 0.3f;
//
//             GameObject bulletL = objectManager.MakeObj("BulletEnemyB");
//             bulletL.transform.position = transform.position + Vector3.left * 0.3f;
//
//             Rigidbody2D rigidR = bulletR.GetComponent<Rigidbody2D>();
//             Rigidbody2D rigidL = bulletL.GetComponent<Rigidbody2D>();
//
//             Vector3 dirVecR = player.transform.position - (transform.position + Vector3.right * 0.3f);
//             Vector3 dirVecL = player.transform.position - (transform.position + Vector3.left * 0.3f);
//
//             rigidR.AddForce(dirVecR.normalized * 4, ForceMode2D.Impulse);
//             rigidL.AddForce(dirVecL.normalized * 4, ForceMode2D.Impulse);
//         }
//
//         curShotDelay = 0;
//     }
//
//     void Reload()
//     {
//         curShotDelay += Time.deltaTime;
//     }
//
//     public void OnHit(int dmg)
//     {
//         if (health <= 0)
//             return;
//
//         health -= dmg;
//
//         // Changes to sprite when hit
//         if (enemyName == "B")
//         {
//             anim.SetTrigger("OnHit");
//         }
//         else
//         {
//             spriteRenderer.sprite = sprites[1];
//             Invoke("ReturnSprite", 0.1f);
//         }
//
//         // Item drop when killing an enemy
//         if (health <= 0)
//         {
//             Player playerLogic = player.GetComponent<Player>();
//             playerLogic.score += enemyScore;
//
//             int ran = enemyName == "B" ? 0 : Random.Range(0, 10);
//
//             if (ran < 4) // 40%
//                 Debug.Log("Not Item");
//             else if (ran < 8) // 40%
//             {
//                 GameObject itemCoin = objectManager.MakeObj("ItemCoin");
//                 itemCoin.transform.position = transform.position;
//             }
//             else if (ran < 9) // 10%
//             {
//                 GameObject itemPower = objectManager.MakeObj("ItemPower");
//                 itemPower.transform.position = transform.position;
//             }
//             else if (ran < 10) // 10%
//             {
//                 GameObject itemBoom = objectManager.MakeObj("ItemBoom");
//                 itemBoom.transform.position = transform.position;
//             }
//             /* else if (ran < 10) // 5%
//             {
//                 GameObject itemHP = objectManager.MakeObj("ItemHP");
//                 itemHP.transform.position = transform.position;
//             }*/
//
//             gameObject.SetActive(false);
//             transform.rotation = Quaternion.identity;
//             gameManager.CallExplosion(transform.position, enemyName);
//
//             // Clear the stage by killing the boss
//             if (enemyName == "B")
//                 gameManager.StageEnd();
//         } 
//     }
//
//     void ReturnSprite()
//     {
//         spriteRenderer.sprite = sprites[0];
//     }
//
//     void OnTriggerEnter2D(Collider2D collision)
//     {
//         // Delete bullet when it touches something
//         if (collision.gameObject.tag == "BorderBullet" && enemyName != "B")
//         {   
//             gameObject.SetActive(false);
//             transform.rotation = Quaternion.identity;
//         }
//
//         else if(collision.gameObject.tag == "PlayerBullet")
//         {
//             Bullet bullet = collision.gameObject.GetComponent<Bullet>();
//
//             collision.gameObject.SetActive(false);
//         }
//     }
// }
