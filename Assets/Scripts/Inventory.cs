using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using DG.Tweening;
using Sirenix.OdinInspector;

public class Inventory : MonoBehaviour
{
    public GameObject inventoryUI; // 인벤토리 UI 패널
    public GameObject DescriptionUI; // 아이템 설명창 UI
    public TextMeshProUGUI itemDescriptionText; // 설명창 텍스트
    public TextMeshProUGUI itemNameText; // 아이템 이름 텍스트
    [SerializeField] private GameObject canvas;
    
    [FoldoutGroup("Buttons")] public Button[] itemButtons; // 아이템 버튼 배열
    [FoldoutGroup("Buttons")] public Sprite[] equippedSprites; // 장착된 상태의 스프라이트
    [FoldoutGroup("Buttons")] public Sprite[] unequippedSprites; // 비장착 상태의 스프라이트
    [FoldoutGroup("Buttons")] public GameObject[] itemGameObjects; // 게임 오브젝트 배열
    [FoldoutGroup("Buttons")] public Vector2[] equippedPositions; // 장착된 위치 배열
    
    private Button lastHoveredButton = null;
    private Inven inven;

    private Vector2 originPos;
    private Vector2[] originalPositions; // 원래 위치 배열

    private void Start()
    {
        inven = new Inven();
        inven.Items = new List<Inven.Item>();

        // 인벤토리 UI 초기화
        InitializeInventoryUI();
        // foreach (var item in itemButtons)
        // {
        //     UpdateItemSprite(item, false);
        // }

        originPos = DescriptionUI.GetComponent<RectTransform>().anchoredPosition;

        // 원래 위치 저장
        originalPositions = new Vector2[itemButtons.Length];
        for (int i = 0; i < itemButtons.Length; i++)
        {
            originalPositions[i] = itemButtons[i].GetComponent<RectTransform>().anchoredPosition;
        }

        // 장착된 위치 설정 (예시로 설정, 필요에 따라 조정)
        equippedPositions = new Vector2[itemButtons.Length];
        equippedPositions[0] = new Vector2(-185, 234);
        equippedPositions[1] = new Vector2(-190, 146);
        equippedPositions[5] = new Vector2(-256, -164);
        equippedPositions[8] = new Vector2(-446, 275);
        
        // ... 나머지 버튼의 위치 설정 ...

        // 게임 오브젝트 초기화
        for (int i = 0; i < itemGameObjects.Length; i++)
        {
            Image img = itemGameObjects[i].GetComponent<Image>();
            img.sprite = unequippedSprites[i];
        }
    }

    private void Update()
    {
        CheckButtonHover();

        if (Input.GetKeyDown(KeyCode.I))
        {
            if (inventoryUI.activeSelf)
            {
                HideInventory();
            }
            else
            {
                ShowInventory();
            }
        }
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
            if (btn != null)
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

    class Inven
    {
        private delegate void ItemDelegate();
        private ItemDelegate _itemDelegate;

        public enum Item
        {
            Ring1,
            Ring2,
            Ring3,
            Ring4,
            Ring5,
            Bracelet1,
            Bracelet2,
            Bracelet3,
            Nail1,
            Nail2,
        }

        private List<Item> _items;
        private ItemLogic _itemLogic;

        public Inven()
        {
            _itemLogic = new ItemLogic();
            _items = new List<Item>();
        }

        public List<Item> Items
        {
            get { return _items; }
            set
            {
                _items = value;
                UpdateItemDelegate();
            }
        }

        private void UpdateItemDelegate()
        {
            _itemDelegate = null;

            foreach (var item in _items)
            {
                switch (item)
                {
                    case Item.Ring1:
                        _itemDelegate += _itemLogic.Ring1;
                        break;
                    case Item.Ring2:
                        _itemDelegate += _itemLogic.Ring2;
                        break;
                    case Item.Ring3:
                        _itemDelegate += _itemLogic.Ring3;
                        break;
                    case Item.Ring4:
                        _itemDelegate += _itemLogic.Ring4;
                        break;
                    case Item.Ring5:
                        _itemDelegate += _itemLogic.Ring5;
                        break;
                    case Item.Bracelet1:
                        _itemDelegate += _itemLogic.Bracelet1;
                        break;
                    case Item.Bracelet2:
                        _itemDelegate += _itemLogic.Bracelet2;
                        break;
                    case Item.Bracelet3:
                        _itemDelegate += _itemLogic.Bracelet3;
                        break;
                    case Item.Nail1:
                        _itemDelegate += _itemLogic.Nail1;
                        break;
                    case Item.Nail2:
                        _itemDelegate += _itemLogic.Nail2;
                        break;
                        // 다른 아이템도 추가
                }
            }
        }

        public void GetItem(Item item)
        {
            if (!_items.Contains(item))
            {
                _items.Add(item);
                UpdateItemDelegate();
            }
        }

        public void Equip(Item item)
        {
            if (_items.Contains(item))
            {
                // 장착 해제 로직
                // 아이템 효과 해제
                UnEquip(item); // 기존 아이템 효과 해제
                _items.Remove(item);
                UpdateItemDelegate(); // 아이템 제거 후 delegate 업데이트
            }
            else
            {
                // 아이템 종류별 장착 제한
                int equippedCount = 0;
                foreach (var i in _items)
                {
                    if (GetItemType(i) == GetItemType(item))
                        equippedCount++;
                }

                if (CanEquip(item, equippedCount))
                {
                    _items.Add(item);
                    UpdateItemDelegate();
                    // 아이템 효과 적용
                    //_itemDelegate?.Invoke(); // 현재 장착된 아이템의 효과 적용
                }
                else
                {
                    Debug.Log("해당 유형의 아이템은 더 이상 장착할 수 없습니다.");
                    return;
                }
            }

            EquipItems();
        }

        private string GetItemType(Item item)
        {
            switch (item)
            {
                case Item.Ring1:
                case Item.Ring2:
                case Item.Ring3:
                case Item.Ring4:
                case Item.Ring5:
                    return "Ring";
                case Item.Bracelet1:
                case Item.Bracelet2:
                case Item.Bracelet3:
                    return "Bracelet";
                case Item.Nail1:
                case Item.Nail2:
                    return "Nail";
                default:
                    return "None";
            }
        }

        private bool CanEquip(Item item, int currentCount)
        {
            string type = GetItemType(item);
            switch (type)
            {
                case "Ring":
                    return currentCount < 2;
                case "Bracelet":
                    return currentCount < 1;
                case "Nail":
                    return currentCount < 1;
                default:
                    return false;
            }
        }

        public void EquipItems()
        {
            _itemDelegate?.Invoke();
            Debug.Log($"현재 장착된 아이템 수: {_items.Count}");
        }

        // 새로운 메서드 추가: 아이템 효과 해제
        private void UnEquip(Item item)
        {
            switch (item)
            {
                case Item.Ring1:
                    _itemLogic.UnEquipRing1();
                    break;
                case Item.Ring2:
                    _itemLogic.UnEquipRing2();
                    break;
                case Item.Ring3:
                    _itemLogic.UnEquipRing3();
                    break;
                
                // Add More

                case Item.Bracelet1:
                    _itemLogic.UnEquipBracelet1();
                    break;
            }
        }
    }

    private void InitializeInventoryUI()
    {
        for (int i = 0; i < itemButtons.Length; i++)
        {
            Button currentBtn = itemButtons[i]; // 로컬 변수로 버튼 캡처
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

            // Pointer Exit 이벤트 추가
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
        canvas.SetActive(false);
        //Time.timeScale = 0;
    }

    public void HideInventory()
    {
        inventoryUI.SetActive(false);
        canvas.SetActive(true);
        //Time.timeScale = 1;
    }

    public void OnItemButtonClick(int index)
    {
        Debug.Log($"버튼 클릭: {itemButtons[index].name}");
        Inven.Item item = (Inven.Item)index;
        inven.Equip(item);

        // 게임 오브젝트의 이미지 변경
        Image img = itemGameObjects[index].GetComponent<Image>(); // itemGameObjects의 이미지 가져오기
        if (inven.Items.Contains(item))
        {
            img.sprite = equippedSprites[index];
            itemGameObjects[index].transform.DOScale(1, 0.7f).From(1.3f).SetEase(Ease.InQuint);
            itemGameObjects[index].GetComponent<Image>().DOFade(1, 0.7f).From(0).SetEase(Ease.InQuint);
        }
        else
        {
            img.sprite = unequippedSprites[index];
            itemGameObjects[index].GetComponent<Image>().DOFade(1, 0.4f).From(0).SetEase(Ease.InQuint);
        }
        // 버튼 위치 이동
        RectTransform btnTransform = itemButtons[index].GetComponent<RectTransform>();
        if (inven.Items.Contains(item))
        {
            btnTransform.anchoredPosition = equippedPositions[index]; // 장착된 위치로 이동
            Debug.Log($"{equippedPositions[8]}");
            if (index == 8)
                itemButtons[index].transform.rotation = Quaternion.Euler(0, 0, -81);
        }
        else
        {
            btnTransform.anchoredPosition = originalPositions[index]; // 원래 위치로 이동
            if(index == 8)
                itemButtons[index].transform.rotation = Quaternion.Euler(0, 0, 0);
        }

        OnItemButtonExit(itemButtons[index]);
    }

    private void OnItemButtonHover(Button btn)
    {
        Debug.Log($"버튼 호버 중: {btn.name}");
        // 아이템 정보 업데이트
        int index = Array.IndexOf(itemButtons, btn);
        if (index >= 0 && index < itemButtons.Length)
        {
            Inven.Item item = (Inven.Item)index;
            (string itemName, string description) = GetItemDescription(item);
            itemDescriptionText.text = description;
            itemNameText.text = itemName;
            DescriptionUI.SetActive(true);
        }
        else
        {
            Debug.LogError("아이템 인덱스가 범위를 벗어났습니다.");
        }

        DOTween.To(() => originPos.x,
        x => DescriptionUI.GetComponent<RectTransform>().anchoredPosition = new Vector2(x, originPos.y), 0f, 1.2f)
        .SetEase(Ease.OutExpo)
        .OnComplete(() =>
        {

        });
    }

    private void OnItemButtonExit(Button btn)
    {
        Debug.Log($"버튼 호버 종료: {btn.name}");
        DOTween.To(() => DescriptionUI.GetComponent<RectTransform>().anchoredPosition.x,
        x => DescriptionUI.GetComponent<RectTransform>().anchoredPosition = new Vector2(x, originPos.y), originPos.x, 0.8f)
        .SetEase(Ease.OutExpo)
        .OnComplete(() =>
        {

        });
    }

    private (string itemName, string description) GetItemDescription(Inven.Item item)
    {
        string description = "";
        string itemName = "";

        switch (item)
        {
            case Inven.Item.Ring1:
                itemName = "하트 로켓";
                description = "최대 체력이 2 증가합니다.";
                break;
            case Inven.Item.Ring2:
                itemName = "별의 반지";
                description = "최대 에너지가 소폭 증가합니다.";
                break;
            case Inven.Item.Ring3:
                itemName = "Ring3";
                description = "공격력을 증가시킵니다.";
                break;
            case Inven.Item.Ring4:
                itemName = "Ring4";
                description = "공격력을 증가시킵니다.";
                break;
            case Inven.Item.Ring5:
                itemName = "Ring5";
                description = "공격력을 증가시킵니다.";
                break;
            case Inven.Item.Bracelet1:
                itemName = "에리스의 팔찌";
                description = "기본 타입의 공격을 가집니다.";
                break;
            case Inven.Item.Bracelet2:
                itemName = "Bracelet2";
                description = "속도 증가 효과가 있습니다.";
                break;
            case Inven.Item.Bracelet3:
                itemName = "Bracelet3";
                description = "속도 증가 효과가 있습니다.";
                break;
            case Inven.Item.Nail1:
                itemName = "별의 메아리";
                description = "플레이어 주변 탄막을 제거합니다.\n (개발중입니다.)";
                break;
            case Inven.Item.Nail2:
                itemName = "Nail2";
                description = "체력을 회복시킵니다.";
                break;
            default:
                return ("", "");
        }

        return (itemName, description);
    }

    // private void UpdateItemSprite(Button btn, bool isEquipped)
    // {
    //     Image img = btn.GetComponent<Image>();
    //     int index = Array.IndexOf(itemButtons, btn);

    //     if (index < 0 || index >= equippedSprites.Length || index >= unequippedSprites.Length)
    //     {
    //         Debug.LogError("아이템 버튼의 인덱스가 스프라이트 배열 범위를 벗어났습니다.");
    //         return;
    //     }

    //     if (isEquipped)
    //     {
    //         img.sprite = equippedSprites[index];
    //     }
    //     else
    //     {
    //         img.sprite = unequippedSprites[index];
    //     }
    // }
}