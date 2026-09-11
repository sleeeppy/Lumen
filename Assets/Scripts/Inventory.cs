using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using DG.Tweening;
using Sirenix.OdinInspector;
using System.IO;
using Newtonsoft.Json;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using System.Collections;

public class Inventory : MonoBehaviour
{
    public static Inventory instance;
    public GameObject inventoryUI; // 인벤토리 UI 패널
    public GameObject DescriptionUI; // 아이템 설명창 UI
    public TextMeshProUGUI itemDescriptionText; // 설명창 텍스트
    public TextMeshProUGUI itemNameText; // 아이템 이름 텍스트
    //[SerializeField] private GameObject canvas;
    
    [FoldoutGroup("Buttons")] public Button[] itemButtons; // 아이템 버튼 배열
    [FoldoutGroup("Buttons")] public Sprite[] equippedSprites; // 장착된 상태의 스프라이트
    [FoldoutGroup("Buttons")] public Sprite[] unequippedSprites; // 비장착 상태의 스프라이트
    [FoldoutGroup("Buttons")] public GameObject[] itemGameObjects; // 게임 오브젝트 배열
    [FoldoutGroup("Buttons")] public Vector2[] equippedPositions; // 장착된 위치 배열
    
    private Button lastHoveredButton = null;
    private Inven inven;

    private Vector2 originPos;
    private Vector2[] originalPositions; // 원래 위치 배열
    [SerializeField] private GameObject warningText;

    // GetComponent 반복 호출을 피하기 위한 캐시 (Start 에서 1회 초기화)
    private RectTransform[] buttonRects;
    private Image[] itemImages;
    private RectTransform descriptionRect;

    private void Awake()
    {
        if (!instance)
            instance = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        warningText.SetActive(false);
    }

    void OnEnable()
    {
    	  // 씬 매니저의 sceneLoaded에 체인을 건다.
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // 체인을 걸어서 이 함수는 매 씬마다 호출된다.
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("OnSceneLoaded: " + scene.name);
        if (scene.name == "Game"|| scene.name.Contains("Boss"))
        {
            inven.LoadEquippedItemsFromJson(); // 장착된 아이템을 JSON에서 로드
        }
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        inven = new Inven();
        inven.Items = new List<ItemId>();

        // 인벤토리 UI 초기화
        InitializeInventoryUI();

        descriptionRect = DescriptionUI.GetComponent<RectTransform>();
        originPos = descriptionRect.anchoredPosition;

        // 버튼 RectTransform 캐싱 + 원래 위치 저장
        buttonRects = new RectTransform[itemButtons.Length];
        originalPositions = new Vector2[itemButtons.Length];
        for (int i = 0; i < itemButtons.Length; i++)
        {
            buttonRects[i] = itemButtons[i].GetComponent<RectTransform>();
            originalPositions[i] = buttonRects[i].anchoredPosition;
        }

        // 장착된 위치 설정 (예시로 설정, 필요에 따라 조정)
        equippedPositions = new Vector2[itemButtons.Length];
        equippedPositions[0] = new Vector2(-185, 234);
        equippedPositions[1] = new Vector2(-190, 146);
        equippedPositions[5] = new Vector2(-256, -164);
        equippedPositions[6] = new Vector2(-265, -153);
        equippedPositions[8] = new Vector2(-446, 275);
        equippedPositions[9] = new Vector2(-439, 265);
        
        // ... 나머지 버튼의 위치 설정 ...

        // 게임 오브젝트 Image 캐싱 + 초기화
        itemImages = new Image[itemGameObjects.Length];
        for (int i = 0; i < itemGameObjects.Length; i++)
        {
            itemImages[i] = itemGameObjects[i].GetComponent<Image>();
            itemImages[i].sprite = unequippedSprites[i];
        }

        // 기본 아이템의 UI 업데이트
        OnItemButtonClick((int)ItemId.Bracelet1);
        OnItemButtonClick((int)ItemId.Nail1);
    }

    private void Update()
    {
        CheckButtonHover();

        // if ((Input.GetKeyDown(KeyCode.Escape) && inventoryUI.activeSelf)
        //     ||Input.GetKeyDown(KeyCode.Tab) && SceneManager.GetActiveScene().name == "Lobby")
        // {
        //     if (inventoryUI.activeSelf)
        //     {
        //         HideInventory();
        //     }
        //     else
        //     {   
        //         ShowInventory();
        //     }
        // }
    }

    private void CheckButtonHover()
    {
        // 마우스 위치를 기반으로 레이캐스트 설정
        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        List<RaycastResult> raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, raycastResults);

        Button currentHoverButton = null;

        foreach (RaycastResult result in raycastResults)
        {
            Button btn = result.gameObject.GetComponent<Button>();
            if (btn != null && result.gameObject.CompareTag("Item"))
            {
                currentHoverButton = btn;
                break;
            }
        }

        if (currentHoverButton != lastHoveredButton)
        {
            if (lastHoveredButton != null)
            {
                // 이전에 호버 중이던 버튼에 대한 종료 이벤트 호출
                OnItemButtonExit(lastHoveredButton);
            }

            if (currentHoverButton != null)
            {
                // 현재 호버 중인 버튼에 대한 호버 이벤트 호출
                OnItemButtonHover(currentHoverButton);
            }

            lastHoveredButton = currentHoverButton;
        }
    }

    public void CantEquip()
    {
        StopCoroutine(WarningTextCoroutine());
        StartCoroutine(WarningTextCoroutine());
    }

    IEnumerator WarningTextCoroutine()
    {
        CameraShake.instance.ShakeCamera();
        warningText.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        warningText.SetActive(false);
    }

    class Inven
    {
        private List<ItemId> _items;

        public Inven()
        {
            _items = new List<ItemId>();
        }

        public List<ItemId> Items
        {
            get { return _items; }
            set { _items = value; }
        }

        public void Equip(ItemId item)
        {
            if (_items.Contains(item))
            {
                // 장착 해제
                _items.Remove(item);
                SaveEquippedItemsToJson();
                return;
            }

            // 아이템 분류별 장착 제한 체크
            ItemCategory category = ItemDatabase.Get(item).Category;
            int equippedCount = 0;
            foreach (var equipped in _items)
            {
                if (ItemDatabase.Get(equipped).Category == category)
                    equippedCount++;
            }

            if (equippedCount < ItemDatabase.GetEquipLimit(category))
            {
                _items.Add(item);
                SaveEquippedItemsToJson(); // 장착 정보를 JSON 파일에 저장
            }
            else
            {
                instance.CantEquip(); // 인스턴스를 통해 메서드 호출
            }
        }

        // 장착된 모든 아이템의 효과를 실제 게임에 적용한다 (씬 로드 후 1회 호출).
        public void EquipItems()
        {
            foreach (var item in _items)
                ItemDatabase.Get(item).ApplyEffect?.Invoke();
        }

        private void SaveEquippedItemsToJson()
        {
            string json = JsonConvert.SerializeObject(_items);
            File.WriteAllText(Application.persistentDataPath + "/equippedItems.json", json);
            Debug.Log("장착된 아이템을 JSON 파일에 저장했습니다: " + json); // JSON 저장 시 로그 출력
        }

        public void LoadEquippedItemsFromJson()
        {
            string path = Application.persistentDataPath + "/equippedItems.json";
            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                _items = JsonConvert.DeserializeObject<List<ItemId>>(json);
                Debug.Log("장착된 아이템을 JSON 파일에서 불러왔습니다: " + json); // JSON 불러올 때 로그 출력
                EquipItems(); // 아이템 효과 적용
            }
            else
            {
                Debug.LogWarning("JSON 파일이 존재하지 않습니다: " + path); // 파일이 없을 경우 경고 로그 출력
            }
        }
    }

    private void InitializeInventoryUI()
    {
        for (int i = 0; i < itemButtons.Length; i++)
        {
            Button currentBtn = itemButtons[i]; // 로컬 변로 버튼 캡처
            int index = i; // 버튼 인덱스 캡처

            // 기존 OnClick 이벤트 리스너 제거
            currentBtn.onClick.RemoveAllListeners();

            // EventTrigger 컴포넌트 추가 및 이벤트 리스너 설정
            EventTrigger trigger = currentBtn.GetComponent<EventTrigger>();
            if (trigger == null)
            {
                trigger = currentBtn.gameObject.AddComponent<EventTrigger>();
            }

            // Pointer Click 이벤트 추가
            EventTrigger.Entry entryClick = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerClick
            };
            entryClick.callback.AddListener((data) => { OnItemButtonClick(index); });
            trigger.triggers.Add(entryClick);

            // Pointer Enter 이벤트 추가
            EventTrigger.Entry entryEnter = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerEnter
            };
            entryEnter.callback.AddListener((data) => { OnItemButtonHover(currentBtn); });
            trigger.triggers.Add(entryEnter);

            // Pointer Exit 벤트 추가
            EventTrigger.Entry entryExit = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerExit
            };
            entryExit.callback.AddListener((data) => { OnItemButtonExit(currentBtn); });
            trigger.triggers.Add(entryExit);
        }

        inventoryUI.SetActive(false);
        DescriptionUI.SetActive(false);
    }

    public void ShowInventory()
    {
        inventoryUI.SetActive(true);
        //canvas.SetActive(false);
        //Time.timeScale = 0;
    }

    public void HideInventory()
    {
        inventoryUI.SetActive(false);
        //canvas.SetActive(true);
        //Time.timeScale = 1;
    }

    public void OnItemButtonClick(int index)
    {
        ItemId item = (ItemId)index;
        inven.Equip(item);

        Image img = itemImages[index];
        bool isEquipped = inven.Items.Contains(item);

        // 게임 오브젝트의 이미지 변경
        if (isEquipped)
        {
            img.sprite = equippedSprites[index];
            itemGameObjects[index].transform.DOScale(1, 0.7f).From(1.3f).SetEase(Ease.InQuint);
            img.DOFade(1, 0.7f).From(0).SetEase(Ease.InQuint);
        }
        else
        {
            img.sprite = unequippedSprites[index];
            img.DOFade(1, 0.4f).From(0).SetEase(Ease.InQuint);
        }

        // 버튼 위치/회전 이동
        RectTransform btnTransform = buttonRects[index];
        bool isNail = item == ItemId.Nail1 || item == ItemId.Nail2;

        if (isEquipped)
        {
            btnTransform.anchoredPosition = equippedPositions[index]; // 장착된 위치로 이동
            if (isNail)
                btnTransform.rotation = Quaternion.Euler(0, 0, -81);
        }
        else
        {
            btnTransform.anchoredPosition = originalPositions[index]; // 원래 위치로 이동
            if (isNail)
                btnTransform.rotation = Quaternion.Euler(0, 0, 0);
        }

        OnItemButtonExit(itemButtons[index]);
    }

    private void OnItemButtonHover(Button btn)
    {
        // 아이템 정보 업데이트
        int index = Array.IndexOf(itemButtons, btn);
        if (index >= 0 && index < itemButtons.Length)
        {
            ItemDefinition def = ItemDatabase.Get((ItemId)index);
            itemDescriptionText.text = def.Description;
            itemNameText.text = def.DisplayName;
            DescriptionUI.SetActive(true);
        }

        DOTween.To(() => originPos.x,
            x => descriptionRect.anchoredPosition = new Vector2(x, originPos.y), 0f, 1.2f)
            .SetEase(Ease.OutExpo);
    }

    private void OnItemButtonExit(Button btn)
    {
        DOTween.To(() => descriptionRect.anchoredPosition.x,
            x => descriptionRect.anchoredPosition = new Vector2(x, originPos.y), originPos.x, 0.8f)
            .SetEase(Ease.OutExpo);
    }

    private void OnApplicationQuit()
    {
        DeleteEquippedItemsJson(); // JSON 파일 삭제 메서드 호출
    }

    // JSON 파일 삭제 메서드 추가
    private void DeleteEquippedItemsJson()
    {
        string path = Application.persistentDataPath + "/equippedItems.json";
        if (File.Exists(path))
        {
            File.Delete(path);
            Debug.Log("JSON 파일이 삭제되었습니다: " + path); // 삭제 시 로그 출력
        }
    }
}