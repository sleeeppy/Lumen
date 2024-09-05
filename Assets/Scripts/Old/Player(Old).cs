// using System.Collections;
// using UnityEngine;  
//
// public class Player : MonoBehaviour
// {
//     public bool isTouchTop;
//     public bool isTouchBottom;
//     public bool isTouchLeft;
//     public bool isTouchRight;
//
//     public int life;
//     public int score;
//     public float speed;
//     public int boom;
//     public int maxBoom;
//     public int power;
//     public int maxPower;
//     public float curShotDelay;
//     public float maxShotDelay;
//
//     public GameObject bulletA;
//     public GameObject bulletB;
//     public GameObject boomEffect;
//
//     public GameManager gameManager;
//     public ObjectManager objectManager;
//
//     public GameObject[] followers;
//     public bool isRespawnTime;
//
//     public bool isHit;
//     public bool isBoomTime;
//     public bool[] joyControl;
//     public bool isControl;
//     public bool isButtonA;
//     public bool isButtonB;
//
//     Animator animator;
//     SpriteRenderer spriteRenderer;
//     Material material;
//
//
//     void Awake()
//     {
//         animator = GetComponent<Animator>();
//         spriteRenderer = GetComponent<SpriteRenderer>();
//         material = spriteRenderer.material;
//     }
//
//     void OnEnable()
//     {
//         Unbeatable();
//         Invoke("Unbeatable", 3);
//     }
//
//     void Unbeatable()
//     {
//         // Apply unbeatable when spawning
//         isRespawnTime = !isRespawnTime;
//         if (isRespawnTime)
//         {
//             spriteRenderer.color = new Color(1, 1, 1, 0.5f);
//
//             for(int i = 0; i < followers.Length; i++)
//                 followers[i].GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.5f);
//         }
//         else
//         {
//             spriteRenderer.color = new Color(1, 1, 1, 1);
//
//             for (int i = 0; i < followers.Length; i++)
//                 followers[i].GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
//         }
//     }
//     void Update()
//     {
//         Move();
//         Fire();
//         Boom();
//         Reload();
//     }
//
//     void Move()
//     {
//         // Control with joy panel
//         float h = Input.GetAxisRaw("Horizontal");
//         float v = Input.GetAxisRaw("Vertical");
//
//         // Set so that it does not go out of range
//         if ((isTouchRight && h == 1) || (isTouchLeft && h == -1))
//             h = 0;
//
//         if ((isTouchTop && v == 1) || (isTouchBottom && v == -1))
//             v = 0;
//
//         Vector3 curPos = transform.position;
//         Vector3 nextPos = new Vector3(h, v, 0) * speed * Time.deltaTime;
//
//         transform.position = curPos + nextPos;
//     }
//
//     void Fire()
//     {
//         if (!Input.GetMouseButton(0))
//             return;
//
//         if (curShotDelay < maxShotDelay)
//             return;
//
//         switch (power)
//         {
//             // Strengthens bullets fired when power increases (up to 3 only)
//             case 1:
//                 GameObject bullet = objectManager.MakeObj("BulletPlayerA");
//                 bullet.transform.position = transform.position;
//
//                 Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
//                 rigid.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
//                 break;
//
//             case 2:
//                 GameObject bulletR = objectManager.MakeObj("BulletPlayerA");
//                 bulletR.transform.position = transform.position + Vector3.right * 0.1f;
//
//                 GameObject bulletL = objectManager.MakeObj("BulletPlayerA");
//                 bulletL.transform.position = transform.position + Vector3.left * 0.1f;
//
//                 Rigidbody2D rigidR = bulletR.GetComponent<Rigidbody2D>();
//                 rigidR.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
//
//                 Rigidbody2D rigidL = bulletL.GetComponent<Rigidbody2D>();
//                 rigidL.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
//
//                 break;
//
//             default:
//                 GameObject bulletRR = objectManager.MakeObj("BulletPlayerA");
//                 bulletRR.transform.position = transform.position + Vector3.right * 0.35f;
//
//                 GameObject bulletCC = objectManager.MakeObj("BulletPlayerB");
//                 bulletCC.transform.position = transform.position;
//
//                 GameObject bulletLL = objectManager.MakeObj("BulletPlayerA");
//                 bulletLL.transform.position = transform.position + Vector3.left * 0.35f;
//
//                 Rigidbody2D rigidRR = bulletRR.GetComponent<Rigidbody2D>();
//                 Rigidbody2D rigidCC = bulletCC.GetComponent<Rigidbody2D>();
//                 Rigidbody2D rigidLL = bulletLL.GetComponent<Rigidbody2D>();
//
//                 rigidRR.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
//                 rigidCC.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
//                 rigidLL.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
//
//                 break;
//         }
//
//         curShotDelay = 0;
//     }
//
//     void Boom()
//     {
//         if (!Input.GetMouseButton(1))
//             return;
//
//         if (isBoomTime)
//             return;
//
//         if (boom == 0)
//             return;
//
//         boom--;
//         isBoomTime = true;
//         gameManager.UpdateBoomIcon(boom);
//
//         boomEffect.SetActive(true);
//         Invoke("OffBoomEffect", 4f);
//
//         GameObject[] enemiesL = objectManager.GetPool("EnemyL");
//         GameObject[] enemiesM = objectManager.GetPool("EnemyM");
//         GameObject[] enemiesS = objectManager.GetPool("EnemyS");
//
//         // Kill all enemies ( except boss )
//         for (int i = 0; i < enemiesL.Length; i++)
//         {
//             if (enemiesL[i].activeSelf)
//             {
//                 Enemy enemyLogic = enemiesL[i].GetComponent<Enemy>();
//                 enemyLogic.OnHit(1000);
//             }
//         }
//         for (int i = 0; i < enemiesM.Length; i++)
//         {
//             if (enemiesM[i].activeSelf)
//             {
//                 Enemy enemyLogic = enemiesM[i].GetComponent<Enemy>();
//                 enemyLogic.OnHit(1000);
//             }
//         }
//         for (int i = 0; i < enemiesS.Length; i++)
//         {
//             if (enemiesS[i].activeSelf)
//             {
//                 Enemy enemyLogic = enemiesS[i].GetComponent<Enemy>();
//                 enemyLogic.OnHit(1000);
//             }
//         }
//
//         GameObject[] bulletsA = objectManager.GetPool("BulletEnemyA");
//         GameObject[] bulletsB = objectManager.GetPool("BulletEnemyB");
//
//         // Delete all enemy bullets
//         for (int i = 0; i < bulletsA.Length; i++)
//         {
//             if (bulletsA[i].activeSelf)
//             {
//                 bulletsA[i].SetActive(false);
//             }
//         }
//         for (int i = 0; i < bulletsB.Length; i++)
//         {
//             if (bulletsB[i].activeSelf)
//             {
//                 bulletsB[i].SetActive(false);
//             }
//         }
//     }
//
//     void Reload()
//     {
//         curShotDelay += Time.deltaTime;
//     }
//
//     void OnTriggerEnter2D(Collider2D collision)
//     {
//         if (collision.gameObject.tag == "Border")
//         {
//             switch (collision.gameObject.name)
//             {
//                 case "Top":
//                     isTouchTop = true;
//                     break;
//                 case "Bottom":
//                     isTouchBottom = true;
//                     break;
//                 case "Left":
//                     isTouchLeft = true;
//                     break;
//                 case "Right":
//                     isTouchRight = true;
//                     break;
//             }
//         }
//         else if (collision.gameObject.tag == "Enemy" || collision.gameObject.tag == "EnemyBullet")
//         {
//             if (isRespawnTime)
//                 return;
//
//             if (isHit)
//                 return;
//
//             // Reduces health when hit by enemies or enemy bullets
//             isHit = true;
//             life--;
//             gameManager.UpdateLifeIcon(life);
//             gameManager.CallExplosion(transform.position, "P");
//
//             // If health is 0, game over
//             if (life == 0)
//             {
//                 gameManager.GameOver();
//             }
//             else
//             {
//                 gameManager.RespawnPlayer();
//             }
//
//             gameManager.RespawnPlayer();
//             gameObject.SetActive(false);
//         }
//         else if(collision.gameObject.tag == "Item")
//         {
//             // When you reach an item
//
//             Item item = collision.gameObject.GetComponent<Item>();
//             switch (item.type)
//             {
//                 case "Coin":
//                     // score increase
//                     score += 1000;
//                     break;
//                 case "Power":
//                     // Only score points when power is at maximum
//                     if (power == maxPower)
//                         score += 500;
//                     else
//                     {
//                         power++;
//                         AddFollower();
//                     }
//                     break;
//                 case "Boom":
//                     // The score only increases when you have the maximum number of bombs.
//                     if (boom == maxBoom)
//                         score += 500;
//                     else
//                     {
//                         boom++;
//                         gameManager.UpdateBoomIcon(boom);
//                     }
//                     break;
//                 case "HP":
//                     if (life <= 3)
//                     {
//                         life++;
//                         gameManager.UpdateLifeIcon(life);
//                     }
//                     break;
//             }
//             // Remove Item
//             collision.gameObject.SetActive(false);
//         }
//     }
//     void OnTriggerExit2D(Collider2D collision)
//     {
//         if (collision.gameObject.tag == "Border")
//         {
//             switch (collision.gameObject.name)
//             {
//                 case "Top":
//                     isTouchTop = false;
//                     break;
//                 case "Bottom":
//                     isTouchBottom = false;
//                     break;
//                 case "Left":
//                     isTouchLeft = false;
//                     break;
//                 case "Right":
//                     isTouchRight = false;
//                     break;
//             }
//         }
//     }
//     void AddFollower()
//     {
//         // Add followers when power is 4 or more (can have up to 3)
//         if (power == 4)
//             followers[0].SetActive(true);
//         else if (power == 5)
//             followers[1].SetActive(true);
//         else if (power == 6)
//             followers[2].SetActive(true);
//     }
//     void OffBoomEffect()
//     {
//         boomEffect.SetActive(false);
//         isBoomTime = false;
//     }
// }
