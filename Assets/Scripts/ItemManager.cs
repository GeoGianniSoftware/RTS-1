using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using LitJson;

public class ItemManager : MonoBehaviour
{
    private JsonData itemData;
    public static List<Item> items;
    // Start is called before the first frame update
    void Start()
    {
        itemData = JsonMapper.ToObject(File.ReadAllText(Application.dataPath + "/StreamingAssets/ItemDatabase.json"));
        ConstructDatabase();
    }

    void ConstructDatabase() {
        items = new List<Item>();
        for (int i = 0; i < itemData.Count; i++) {
            Item tempItem = new Item(
                itemData[i]["itemName"].ToString(),
                (int)itemData[i]["itemID"],
                itemData[i]["itemSlug"].ToString(),
                itemData[i]["itemDesc"].ToString(),
                StringToItemType(itemData[i]["itemType"].ToString())


                );
            items.Add(tempItem);
        }
    }

    public static Item GetItemByID(int id) {
        if(items != null && items[id] != null) {
            return items[id];
        }
        return null;
    }

    ItemType StringToItemType(string s) {
        if (s.Equals("Basic")) {
            return ItemType.Basic;
        }else if (s.Equals("Resource")) {
            return ItemType.Resource;
        }
        return ItemType.Basic;
    }
}
