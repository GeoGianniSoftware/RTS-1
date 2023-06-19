using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]

public class Item
{
    public string Name;
    public int ID;
    public string Slug;
    public string Desc;
    public ItemType ItemType;
    public List<int> ItemEffectIds;

    public Item() {
        ID = -1;
        ItemType = ItemType.Basic;
    }

    public Item(string name, int id, string slug, string desc, ItemType itemType) {
        Name = name;
        ID = id;
        Slug = slug;
        Desc = desc;
        ItemType = itemType;
        ItemEffectIds = null;
    }

    public Item(string name, int id, string slug, string desc, ItemType itemType, List<int> effectIds) {
        Name = name;
        ID = id;
        Slug = slug;
        Desc = desc;
        ItemType = itemType;
        ItemEffectIds = effectIds;
    }



}

public enum ItemType
{
    Basic,
    Resource
}
