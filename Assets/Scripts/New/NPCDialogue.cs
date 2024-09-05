using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class NPCDialogue : MonoBehaviour
{
    bool tryJustOnce = true;
    public GameObject e;
    public GameObject textBox;

    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI chatText;
    [SerializeField] private string characterName;
    [SerializeField, Range(0, 0.1f)] private float textSpeed = 0.03f;

    private bool playerInside;
    private string writerText = "";
    

    void Start()
    {
        e.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.E) && playerInside && tryJustOnce)
        {
            tryJustOnce = false;
            textBox.SetActive(true);
            StartCoroutine(TextPractice());
        }
    }
    
    public IEnumerator NormalChat(string narrator, string narration)
    {
        int a = 0;
        nameText.text = narrator;
        writerText = "";

        for (a = 0; a < narration.Length; a++)
        {
            writerText += narration[a];
            yield return new WaitForSeconds(textSpeed);
            chatText.text = writerText;
            yield return null;
        }
        while (true)
        {
            if (Input.GetMouseButton(0))
            {
                break;
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                break;
            }
            yield return null;
        }
    }
    
    IEnumerator TextPractice()
    {
        yield return StartCoroutine(NormalChat(characterName, "Demo Dialougue"));
        yield return StartCoroutine(NormalChat(characterName, "Would you like to go to the battle scene?"));
        SceneManager.LoadScene("GameScene");
        tryJustOnce = true;
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            e.SetActive(true);
            playerInside = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            e.SetActive(false);
            playerInside = false;
        }
    }
}
