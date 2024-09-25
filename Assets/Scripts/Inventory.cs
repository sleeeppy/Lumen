using System;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    class Inven
    {
        private delegate void Itemdelegate();
        private Itemdelegate _itemDelegate;

        public enum Item
        {
            None,
            Ring1,
            Ring2
        }

        private List<Item> _items;
        private ItemLogic _itemLogic; // ItemLogic 클래스 인스턴스 추가

        public Inven()
        {
            _itemLogic = new ItemLogic();
            _items = new List<Item>();
        }

        // 아이템 목록을 가져오거나 설정하는 프로퍼티
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
                if (item == Item.Ring1)
                    _itemDelegate += _itemLogic.Ring1;
                else if (item == Item.Ring2)
                    _itemDelegate += _itemLogic.Ring2;
            }

            // if (_items.Count == 0)
            //     _itemDelegate += _itemLogic.None;
        }
        
        public void GetItem(Item item)
        {
            _items.Add(item);
            UpdateItemDelegate();
        }
        
        public void Equip()
        {
            // Delegate가 null이 아닐 경우 호출
            _itemDelegate?.Invoke(); 
            Debug.Log($"current item count: {_items.Count}");
        }
    }
    
    private void Start()
    {
        Inven inven = new Inven();
        inven.Items = new List<Inven.Item> {};

        //inven.Items.Add(Inven.Item.Ring1);의 경우에는 프로퍼티의 Set이 호출되지 않음
        inven.GetItem(Inven.Item.Ring1);
        inven.Equip();
    }
}