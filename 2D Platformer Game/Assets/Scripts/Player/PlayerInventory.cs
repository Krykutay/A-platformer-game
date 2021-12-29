using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public List<int> inventory { get; private set; }
    public int EquippedOutfit { get; private set; }
    public int EquippedSwords { get; private set; }

    void Awake()
    {
        inventory = new List<int>();
        Load_Inventory();
        Load_EquippedItems();
    }

    private void Update()
    {
        //Debug.Log(inventory.Count);
    }

    void Load_Inventory()
    {
        if (PlayerPrefs.HasKey("inventoryCount"))
        {
            int inventoryCount = PlayerPrefs.GetInt("inventoryCount");
            for (int i = 0; i < inventoryCount; i++)
            {
                inventory.Add(PlayerPrefs.GetInt("inventory" + i.ToString()));
            }
        }
        else
        {
            AddItem((int)Items.ItemType.DefaultOutfit);
            AddItem((int)Items.ItemType.DefaultSword);
        }
    }

    void Load_EquippedItems()
    {
        if (PlayerPrefs.HasKey("equippedOutfit"))
        {
            EquippedOutfit = PlayerPrefs.GetInt("equippedOutfit");
        }
        else
        {
            PlayerPrefs.SetInt("equippedOutfit", (int)Items.ItemType.DefaultOutfit);
            EquippedOutfit = (int)Items.ItemType.DefaultOutfit;
        }

        if (PlayerPrefs.HasKey("equippedSword"))
        {
            EquippedSwords = PlayerPrefs.GetInt("equippedSword");
        }
        else
        {
            PlayerPrefs.SetInt("equippedSword", (int)Items.ItemType.DefaultSword);
            EquippedSwords = (int)Items.ItemType.DefaultSword;
        } 
    }

    public void AddItem(int item)
    {
        if (!inventory.Contains(item))
        {
            inventory.Add(item);
            PlayerPrefs.SetInt("inventory" + (inventory.Count - 1).ToString(), item);
            PlayerPrefs.SetInt("inventoryCount", inventory.Count);
        }
    }

    public void UpdateEquippedOutfit(int item)
    {
        EquippedOutfit = item;
        PlayerPrefs.SetInt("equippedOutfit", item);
    }

    public void UpdateEquippedSword(int item)
    {
        EquippedSwords = item;
        PlayerPrefs.SetInt("equippedSword", item);
    }

}
