using System;
using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using TMPro;
using DG.Tweening;
using Cinemachine;
using System.IO;

public class NPC : MonoBehaviour
{
    [SerializeField] private GameObject buttonE;
    [SerializeField] private GameObject textBox;
    [SerializeField] private GameObject dialoguePointer;

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
    private string currentFullText; // 현재 대화의 전체 텍스트

    [SerializeField] private CinemachineVirtualCamera vCam;

    private float originSpeed;
    private int originMaxJumpCount;
    private Player playerScript;
    private Animator anim;

    void Start()
    {
        // DOTween 초기화 확인
        DOTween.Init();

        StartCoroutine(ObtainSheetData());
        playerScript = GameObject.FindWithTag("Player").GetComponent<Player>();
        anim = playerScript.GetComponent<Animator>();


        originSpeed = playerScript.moveSpeed;
        originMaxJumpCount = playerScript.maxJumpCount;

        //vCam = vCam.GetComponent<CinemachineVirtualCamera>();

        // vCam이 null인지 확인
        if (vCam == null)
        {
            Debug.LogError("vCam이 null입니다. 확인하세요.");
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && playerInside && !isTyping 
            && !Inventory.instance.inventoryUI.activeSelf && !textBox.activeSelf) 
        {
            anim.enabled = false;
            playerScript.moveSpeed = 0;
            playerScript.maxJumpCount = 0;
        
            textBox.SetActive(true);
            buttonE.SetActive(false);
            
            StartDialogue();
            justOnce = true;

            StartCoroutine(LerpFieldOfView(vCam.m_Lens.FieldOfView, 30f, 0.35f));
        }
        
        // 텍스트 출력 중에 anyKeyDown을 눌렀을 때 즉시 출력 완료
        if (textBox.activeSelf && Input.anyKeyDown)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                EndDialogue();                
                return;
            }

            if (isTyping) 
            {
                // 현재 텍스트가 출력 중이면 즉시 출력 완료
                CompleteTextImmediately();
            }
            else
            {
                // 텍스트 출력이 완료되면 다음 대화로 넘어감
                ShowNextDialogue();
            }
        }

        if(Inventory.instance.inventoryUI.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            Inventory.instance.HideInventory();
            EndDialogue();
        }
    }

    void EndDialogue()
    {
        StartCoroutine(LerpFieldOfView(vCam.m_Lens.FieldOfView, 35f, 0.35f));

        playerScript.moveSpeed = originSpeed;
        playerScript.maxJumpCount = originMaxJumpCount;
        anim.enabled = true;
        justOnce = false;
        textBox.SetActive(false);
    }

    IEnumerator ObtainSheetData()
    {
        UnityWebRequest uwr = UnityWebRequest.Get("https://sheets.googleapis.com/v4/spreadsheets/1v_3dQlqmcSTiuKhtogBPrJjRIwciuZ0abSshqSMP6JA/values/sheet1?key=AIzaSyC09LuyZVWm5f8RrC7_HtIhzooF9J8Bcsg");

        yield return uwr.SendWebRequest();

        if ((uwr.result == UnityWebRequest.Result.ConnectionError) || uwr.timeout > 2000
             || (uwr.result == UnityWebRequest.Result.ProtocolError) )
        {
            Debug.LogError("Error: " + uwr.error);
            Debug.LogError("Offline");
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
            chatText.text = "";
            ShowNextDialogue();
            //Debug.Log($"ddd {currentDialogue[dialogueIndex-1]}, {dialogueIndex}");
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
            StartCoroutine(AnimateText(dialogue));
            dialogueIndex++;
        }
        else
        {
            if (Id == "3")
            {
                textBox.SetActive(false);
                Inventory.instance.ShowInventory();
                StartCoroutine(LerpFieldOfView(vCam.m_Lens.FieldOfView, 23.5f, 0.4f));
                Debug.Log("상점 NPC와의 대화가 모두 끝났습니다!");
            }
            else
                LoadNextScene();
        }
    }


    IEnumerator AnimateText(string text)
    {
        isTyping = true;
        chatText.text = "";
        nameText.text = Name;
        string myString = "";
        currentFullText = text; // 현재 텍스트 전체 저장

        // DOTween을 사용하여 텍스트를 한 글자씩 타이핑 효과로 출력
        dialoguePointer.SetActive(false); // 화살표 비활성화
        DOTween.To(() => myString, x => myString = x, text, text.Length / textSpeed)
            .SetEase(Ease.OutCubic)
            .OnUpdate(() => chatText.text = myString)
            .OnComplete(() => 
            {
                isTyping = false; // 타이핑 완료
                dialoguePointer.SetActive(true); // 화살표 활성화
            });

        yield return new WaitUntil(() => !isTyping);
    }

    // 텍스트 즉시 출력 완료
    void CompleteTextImmediately()
    {
        DOTween.KillAll(); // DOTween 애니메이션을 중지하고
        chatText.text = currentFullText; // 전체 텍스트를 즉시 출력
        isTyping = false; // 타이핑 완료로 전환
        dialoguePointer.SetActive(true); // 화살표 활성화
    }

    void LoadNextScene()
    {
        SceneLoader.instance.LoadGameScene();
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

    private IEnumerator LerpFieldOfView(float startValue, float endValue, float duration)
    {
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            vCam.m_Lens.FieldOfView = Mathf.Lerp(startValue, endValue, (elapsedTime / duration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        vCam.m_Lens.FieldOfView = endValue; // 최종 값 설정
    }
}
