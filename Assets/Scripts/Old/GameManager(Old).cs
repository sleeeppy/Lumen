// using UnityEngine;
// using UnityEngine.UI;
// using UnityEngine.SceneManagement;
// using TMPro;
// using System.Collections.Generic;
// using System.IO;
//
// public class GameManager : MonoBehaviour
// {
//     public int stage;
//     public Animator stageAnim;
//     public Animator clearAnim;
//     public Animator fadeAnim;
//     public Transform playerPos;
//
//     public string[] enemyObjs;
//     public Transform[] spawnPoints;
//
//     public float nextSpawnDelay;
//     public float curSpawnDelay;
//
//     public GameObject player;
//     public GameObject follower;
//     public TextMeshProUGUI scoreText;
//     public Image[] lifeImage;
//     public Image[] boomImage;
//     public GameObject gameOverSet;
//     public ObjectManager objectManager;
//
//     public List<Spawn> spawnList;
//     public int spawni;
//     public bool spawnEnd;
//
//     void Awake()
//     {
//         spawnList = new List<Spawn>();
//         enemyObjs = new string[] { "EnemyS", "EnemyM", "EnemyL", "EnemyB" };
//         StageStart();
//     }
//
//     public void StageStart()
//     {
//         stageAnim.SetTrigger("On");
//         stageAnim.GetComponent<TextMeshProUGUI>().text = "Stage " + stage + "\nStart";
//         clearAnim.GetComponent<TextMeshProUGUI>().text = "Stage " + stage + "\nClear!";
//
//         ReadSpawnFile();
//         fadeAnim.SetTrigger("In");
//     }
//
//     public void StageEnd()
//     {
//         clearAnim.SetTrigger("On");
//         fadeAnim.SetTrigger("Out");
//
//         player.transform.position = playerPos.position;
//
//         stage++;
//         if (stage < 2)
//             Invoke("GameOver", 6);
//         else
//             Invoke("StageStart", 5);
//     }
//
//     void ReadSpawnFile()
//     {
//         spawnList.Clear();
//         spawni = 0;
//         spawnEnd = false;
//
//         // Example of creating a text file
//         // 1. Please be sure to set the file name to "Stage N".
//         // 2. Please be sure to write in the format of "delay, type, point".
//         // 3. Please do not enter anything other than these.
//
//         // Load text file matching stage variable
//         TextAsset textFile = Resources.Load("Stage " + stage) as TextAsset;
//         StringReader stringreader = new StringReader(textFile.text);
//
//         while(stringreader != null)
//         {
//             string line = stringreader.ReadLine();
//             Debug.Log(line);
//
//             if (line == null)
//                 break;
//             Spawn spawnData = new Spawn();
//             spawnData.delay = float.Parse(line.Split(',')[0]);
//             spawnData.type = line.Split(',')[1];
//             spawnData.point = int.Parse(line.Split(',')[2]);
//             spawnList.Add(spawnData);
//         }
//
//         stringreader.Close();
//         nextSpawnDelay = spawnList[0].delay;
//
//     }
//     void Update()
//     {
//         curSpawnDelay += Time.deltaTime;
//         
//         if(curSpawnDelay > nextSpawnDelay && !spawnEnd)
//         {
//             SpawnEnemy();
//             curSpawnDelay = 0;  
//         }
//
//         Player playerLogic = player.GetComponent<Player>();
//
//         // Formatting
//         scoreText.text = string.Format("{0:n0}", playerLogic.score);
//     }
//
//     void SpawnEnemy()
//     {
//         int enemyi = 0;
//         switch (spawnList[spawni].type)
//         {
//             case "S":
//                 enemyi = 0;
//                 break;
//             case "M":
//                 enemyi = 1;
//                 break;
//             case "L":
//                 enemyi = 2;
//                 break;
//             case "B":
//                 enemyi = 3;
//                 break;
//         }
//
//         // Spawning Enemies
//         int enemyPoint = spawnList[spawni].point;
//         GameObject enemy = objectManager.MakeObj(enemyObjs[enemyi]);
//         enemy.transform.position = spawnPoints[enemyPoint].position;
//
//         Rigidbody2D rigid = enemy.GetComponent<Rigidbody2D>();
//         Enemy enemyLogic = enemy.GetComponent<Enemy>();
//         enemyLogic.player = player;
//         enemyLogic.gameManager = this;
//         enemyLogic.objectManager = objectManager;
//
//         if(enemyPoint == 5 || enemyPoint == 6)
//         {
//             enemy.transform.Rotate(Vector3.back * 90);
//             rigid.velocity = new Vector2(enemyLogic.speed * (-1), -1);
//         }
//         else if(enemyPoint == 7 || enemyPoint == 8)
//         {
//             enemy.transform.Rotate(Vector3.forward * 90);
//             rigid.velocity = new Vector2(enemyLogic.speed, -1);
//         }
//         else
//         {
//             rigid.velocity = new Vector2 (0, enemyLogic.speed * (-1));
//         }
//
//         spawni++;
//         if(spawni == spawnList.Count)
//         {
//             spawnEnd = true;
//             return;
//         }
//
//         nextSpawnDelay = spawnList[spawni].delay;
//     }
//     public void UpdateLifeIcon(int life)
//     {
//         // Life icon set
//         for (int i = 0; i < 3; i++)
//         {
//             lifeImage[i].color = new Color(1, 1, 1, 0);
//         }
//
//         for (int i = 0; i < life; i++)
//         {
//             lifeImage[i].color = new Color(1, 1, 1, 1);
//         }
//     }
//     public void UpdateBoomIcon(int boom)
//     {
//         // Boom icon set
//         for (int i = 0; i < 3; i++)
//         {
//             boomImage[i].color = new Color(1, 1, 1, 0);
//         }
//
//         for (int i = 0; i < boom; i++)
//         {
//             boomImage[i].color = new Color(1, 1, 1, 1);
//         }
//     }
//
//     public void RespawnPlayer()
//     {
//         Invoke("RespawnPlayerExe", 2f);
//         player.transform.position = Vector3.down * 3.5f;
//         player.SetActive(true);
//     }
//
//     void RespawnPlayerExe()
//     {
//         player.transform.position = Vector3.down * 3.5f;
//         player.SetActive(true);
//
//         Player playerLogic = player.GetComponent<Player>();
//         playerLogic.isHit = false;
//     }
//
//     public void CallExplosion(Vector3 pos, string type)
//     {
//         GameObject explosion = objectManager.MakeObj("Explosion");
//         Explosion explosionLogic = explosion.GetComponent<Explosion>();
//
//         explosion.transform.position = pos;
//         explosionLogic.StartExplosion(type);
//     }
//     public void GameOver()
//     {
//         gameOverSet.SetActive(true);
//         Time.timeScale = 0;
//     }
//
//     public void GameRetry()
//     {
//         // Restart the game
//         SceneManager.LoadScene(0);
//         Time.timeScale = 1f;
//     }
//     
// }
