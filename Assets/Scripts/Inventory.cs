using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class Inventory : MonoBehaviour
{
    public GameObject inventoryUI; // 인벤토리 UI 패널
    public GameObject itemDescriptionUI; // 아이템 설명창 UI
    public TextMeshProUGUI itemDescriptionText; // 설명창 텍스트
    public TextMeshProUGUI itemNameText; // 아이템 이름 텍스트
    public Button[] itemButtons; // 아이템 버튼 배열
    public Sprite[] equippedSprites; // 장착된 상태의 스프라이트
    public Sprite[] unequippedSprites; // 비장착 상태의 스프라이트

    private Button lastHoveredButton = null;

    private Inven inven;

    private void Start()
    {
        inven = new Inven();
        inven.Items = new List<Inven.Item>();

        // 인벤토리 UI 초기화
        InitializeInventoryUI();
        foreach (var item in itemButtons)
        {
            UpdateItemSprite(item, false);
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
                _items.Remove(item);
                UpdateItemDelegate();
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
        itemDescriptionUI.SetActive(false);
    }

    public void ShowInventory()
    {
        inventoryUI.SetActive(true);
    }

    public void HideInventory()
    {
        inventoryUI.SetActive(false);
    }

    public void OnItemButtonClick(int index)
    {
        Debug.Log($"버튼 클릭: {itemButtons[index].name}");
        Inven.Item item = (Inven.Item)index;
        inven.Equip(item);

        // 스프라이트 업데이트
        UpdateItemSprite(itemButtons[index], inven.Items.Contains(item));
    }

    private void OnItemButtonHover(Button btn)
    {
        Debug.Log($"버튼 호버 중: {btn.name}");
        // 아이템 정보 업데이트
        int index = Array.IndexOf(itemButtons, btn);
        if (index >= 0 && index < itemButtons.Length)
        {
            Inven.Item item = (Inven.Item)index;
            string name = GetItemName(item);
            string description = GetItemDescription(item);
            itemDescriptionText.text = description;
            itemNameText.text = name;
            itemDescriptionUI.SetActive(true);
        }
        else
        {
            Debug.LogError("아이템 인덱스가 범위를 벗어났습니다.");
        }
    }

    private void OnItemButtonExit(Button btn)
    {
        Debug.Log($"버튼 호버 종료: {btn.name}");
        itemDescriptionUI.SetActive(false);
    }

    private string GetItemName(Inven.Item item)
    {
        switch (item)
        {
            case Inven.Item.Ring1:
                return "Ring1";
            case Inven.Item.Ring2:
                return "Ring2";
            case Inven.Item.Ring3:
                return "Ring3";
            case Inven.Item.Ring4:
                return "Ring4";
            case Inven.Item.Ring5:
                return "Ring5";
            case Inven.Item.Bracelet1:
                return "Bracelet1";
            case Inven.Item.Bracelet2:
                return "Bracelet2";
            case Inven.Item.Bracelet3:
                return "Bracelet3";
            case Inven.Item.Nail1:
                return "Nail1";
            case Inven.Item.Nail2:
                return "Nail2";
            default:
                return "";
        }
    }

    private string GetItemDescription(Inven.Item item)
    {
        switch (item)
        {
            case Inven.Item.Ring1:
                return "wow.";
            case Inven.Item.Ring2:
                return "wowow.";
            case Inven.Item.Ring3:
                return "공격력을 증가시킵니다.";
            case Inven.Item.Ring4:
                return "공격력을 증가시킵니다.";
            case Inven.Item.Ring5:
                return "공격력을 증가시킵니다.";
            case Inven.Item.Bracelet1:
                return "속도 증가 효과가 있습니다.";
            case Inven.Item.Bracelet2:
                return "속도 증가 효과가 있습니다.";
            case Inven.Item.Bracelet3:
                return "속도 증가 효과가 있습니다.";
            case Inven.Item.Nail1:
                return "추가 마나를 제공합니다.";
            case Inven.Item.Nail2:
                return "체력을 회복시킵니다.";
            default:
                return "";
        }
    }

    private void UpdateItemSprite(Button btn, bool isEquipped)
    {
        Image img = btn.GetComponent<Image>();
        int index = Array.IndexOf(itemButtons, btn);

        if (index < 0 || index >= equippedSprites.Length || index >= unequippedSprites.Length)
        {
            Debug.LogError("아이템 버튼의 인덱스가 스프라이트 배열 범위를 벗어났습니다.");
            return;
        }

        if (isEquipped)
        {
            img.sprite = equippedSprites[index];
        }
        else
        {
            img.sprite = unequippedSprites[index];
        }
    }
}