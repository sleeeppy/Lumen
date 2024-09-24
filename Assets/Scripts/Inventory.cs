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
                    _itemDelegate += Ring1;
                else if (item == Item.Ring2)
                    _itemDelegate += Ring2;
            }

            if (_items.Count == 0)
                _itemDelegate += Ring3;
        }

        public void Attack()
        {
            _itemDelegate?.Invoke(); // Invoke the delegate if not null
            Debug.Log("Attack Logic");
        }
        
        public void GetItem(Item item)
        {
            _items.Add(item);
            UpdateItemDelegate();
        }
        
        void Ring1()
        {
            Debug.Log("Item1 Logic");
        }

        void Ring2()
        {
            Debug.Log("Item2 Logic");
        }

        void Ring3()
        {
            Debug.Log("None Item Logic");
        }

        void Ring4()
        {

        }

        void Ring5()
        {
            
        }

        void Bracelet1()
        {
            
        }

        void Bracelet2()
        {
            
        }

        void Baracelet3()
        {
            
        }

        void Nail1()
        {
            
        }

        void Nail2()
        {
            
        }
    }
    
    private void Start()
    {
        Inven inven = new Inven();
        inven.Items = new List<Inven.Item> {};
        inven.GetItem(Inven.Item.Ring1);
        // player.Items.Add(Player.Item.Item1);
        inven.Attack();
    }
}