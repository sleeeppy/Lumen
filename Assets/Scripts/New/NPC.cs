using System;
using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using TMPro;
using DG.Tweening;

public class NPC : MonoBehaviour
{
    [SerializeField] private GameObject buttonE;
    [SerializeField] private GameObject textBox;

    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI chatText;
    [SerializeField, Range(0, 30f)] private float textSpeed = 15f;
    
    
    [Header("Key")]
    [SerializeField] private string Name;
    [SerializeField] private string Id;

    // 현재 캐릭터의 대화 리스트
    private List<string> currentDialogue = new List<string>();
    
    // 데이터를 저장할 Dictionary
    private Dictionary<string, List<string>> dialoguesByNameAndId = new Dictionary<string, List<string>>();
    
    private int dialogueIndex;
    private bool isTyping;
    
    private bool playerInside;
    private bool justOnce;
    
    void Start()
    {
        StartCoroutine(ObtainSheetData());
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.E) && playerInside && !isTyping && !justOnce) 
        {
            textBox.SetActive(true);
            StartDialogue();
            justOnce = true;
        }
        
        if (textBox.activeSelf && Input.anyKeyDown && !isTyping)
        {
            ShowNextDialogue();
        }
    }

    IEnumerator ObtainSheetData()
    {
        UnityWebRequest uwr = UnityWebRequest.Get("https://sheets.googleapis.com/v4/spreadsheets/1v_3dQlqmcSTiuKhtogBPrJjRIwciuZ0abSshqSMP6JA/values/sheet1?key=AIzaSyC09LuyZVWm5f8RrC7_HtIhzooF9J8Bcsg");

        yield return uwr.SendWebRequest();

        if (uwr.isNetworkError || uwr.isHttpError || uwr.timeout > 2)
        {
            Debug.Log("Error: " + uwr.error);
            Debug.Log("Offline");
        }
        else
        {
            string json = uwr.downloadHandler.text;
            var o = JSON.Parse(json);
            var valuesArray = o["values"].AsArray;

            string currentId = "";
            string currentName = "";
            
            // 대화를 저장할 리스트
            List<string> currentDialogues = new List<string>();

            for (int i = 1; i < valuesArray.Count; i++)
            {
                string id = valuesArray[i][0];
                string name = valuesArray[i][1];
                string dialogue = valuesArray[i][2];

                // 새로운 ID가 나왔을 때, 현재까지의 대화 리스트를 저장하고 새로운 ID로 전환
                if (!string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(name))
                {
                    // 이전 ID에 해당하는 대화 저장
                    if (currentDialogues.Count > 0)
                    {
                        // 키: Name + ID, 값: 해당 대화 리스트
                        dialoguesByNameAndId[currentName + currentId] = new List<string>(currentDialogues);
                        // 대화 리스트 초기화
                        currentDialogues.Clear();
                    }

                    currentId = id;
                    currentName = name;
                }

                // 대화 추가
                currentDialogues.Add(dialogue);
            }

            // 마지막 남은 ID에 대한 대화 저장
            if (currentDialogues.Count > 0)
            {
                dialoguesByNameAndId[currentName + currentId] = new List<string>(currentDialogues);
            }
        }
    }


    void StartDialogue()
    {
        string key = Name + Id;

        if (dialoguesByNameAndId.ContainsKey(key))
        {
            currentDialogue = dialoguesByNameAndId[key];
            dialogueIndex = 0;
            ShowNextDialogue();
        }
        else
        {
            Debug.LogError("대화를 찾을 수 없습니다!");
        }
    }

    void ShowNextDialogue()
    {
        if (dialogueIndex < currentDialogue.Count)
        {
            string dialogue = currentDialogue[dialogueIndex];
            dialogueIndex++;
            StartCoroutine(AnimateText(dialogue));
        }
        else
        {
            LoadNextScene();
        }
    }

    IEnumerator AnimateText(string text)
    {
        isTyping = true;
        chatText.text = "";
        nameText.text = Name;
        string myString = "";

        // DOTween을 사용하여 텍스트를 한 글자씩 타이핑 효과로 출력
        
        DOTween.To(() => myString, x => myString = x, text, text.Length / textSpeed)
            .OnUpdate(() => chatText.text = myString)
            .OnComplete(() => isTyping = false);

        yield return new WaitUntil(() => !isTyping);
    }

    void LoadNextScene()
    {
        SceneManager.LoadScene(1);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            buttonE.SetActive(true);
            playerInside = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            buttonE.SetActive(false);
            playerInside = false;
        }
    }
}
