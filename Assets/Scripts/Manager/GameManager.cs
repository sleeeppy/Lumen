using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    private NPC[] npc;
    [SerializeField] private GameObject setting;
    private bool allTextBoxesInactive;
    [SerializeField] private TextMeshProUGUI startText;
    [SerializeField] private TextMeshProUGUI exitText;

    void OnEnable()
    {
    	// 씬 매니저의 sceneLoaded에 체인을 건다.
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void Awake()
    {
        if(!instance)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        SceneManager.LoadScene("Main");
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Time.timeScale = 1;
        setting.SetActive(false);

        npc = FindObjectsOfType<NPC>();

        // 씬 이름이 "Main"일 때 버튼에 함수 연결
        if (scene.name == "Main")
        {
            startText = GameObject.Find("Start Button").GetComponentInChildren<TextMeshProUGUI>();
            exitText = GameObject.Find("Exit Button").GetComponentInChildren<TextMeshProUGUI>();

            Button startButton = GameObject.Find("Start Button").GetComponent<Button>();
            Button exitButton = GameObject.Find("Exit Button").GetComponent<Button>();

            startButton.onClick.AddListener(LoadLobbyScene);
            exitButton.onClick.AddListener(ExitGame);

            // 버튼에 이벤트 추가
            startButton.gameObject.AddComponent<EventTrigger>().triggers = new List<EventTrigger.Entry>();
            exitButton.gameObject.AddComponent<EventTrigger>().triggers = new List<EventTrigger.Entry>();

            AddHoverEffect(startButton);
            AddHoverEffect(exitButton);
        }
    }

    void AddHoverEffect(Button button)
    {
        EventTrigger trigger = button.gameObject.GetComponent<EventTrigger>();

        EventTrigger.Entry entryEnter = new EventTrigger.Entry();
        entryEnter.eventID = EventTriggerType.PointerEnter;
        entryEnter.callback.AddListener((data) => { OnHoverEnter(button); });
        trigger.triggers.Add(entryEnter);

        EventTrigger.Entry entryExit = new EventTrigger.Entry();
        entryExit.eventID = EventTriggerType.PointerExit;
        entryExit.callback.AddListener((data) => { OnHoverExit(button); });
        trigger.triggers.Add(entryExit);
    }

    void OnHoverEnter(Button button)
    {
        if(button.name == "Start Button")
            startText.transform.DOScale(new Vector3(0.7f, 0.7f, 0.7f), 0.2f);
        else
            exitText.transform.DOScale(new Vector3(0.7f, 0.7f, 0.7f), 0.2f);
    }

    void OnHoverExit(Button button)
    {
        if(button.name == "Start Button")
            startText.transform.DOScale(new Vector3(0.6f, 0.6f, 0.6f), 0.2f);
        else
            exitText.transform.DOScale(new Vector3(0.6f, 0.6f, 0.6f), 0.2f);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (npc.Length > 0)
            {
                allTextBoxesInactive = true;
                foreach (var npc in npc)
                {
                    if (npc.textBox.activeSelf)
                    {
                        allTextBoxesInactive = false;
                        break;
                    }
                }
                if (allTextBoxesInactive && !Inventory.instance.inventoryUI.activeSelf)
                {
                    Setting();
                }
            }
            else if (SceneManager.GetActiveScene().name == "Main") 
                return;
            else
            {
                Setting();
            }
        }
    }

    void Setting()
    {
        if (!setting.activeSelf)
        {
            Time.timeScale = 0;
            setting.SetActive(true);
        }
        else
        {
            Time.timeScale = 1;
            setting.SetActive(false);
        }
    }

    public void LoadMainScene()
    {
        SceneManager.LoadScene("Main");
    }

    public void LoadLobbyScene()
    {
        SceneManager.LoadScene("Lobby");
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
