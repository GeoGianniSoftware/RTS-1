using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class Inventory
{
    public List<ItemSlot> items;
    public int maxItemCount;
    public int currentItemCount;
    public List<ItemSlot> droppedItems = new List<ItemSlot>();
    bool oneItemOnly = true;
    bool oneItemFilled = false;

    public Inventory() {
        items = new List<ItemSlot>();
        maxItemCount = 0;
    }
    public Inventory(int size) {
        items = new List<ItemSlot>();
        Debug.Log(size);
        maxItemCount = size;
    }
    public Inventory(List<ItemSlot> itemSlots, int size) {
        items = new List<ItemSlot>();
        maxItemCount = size;
    }
    public Inventory(int size, bool isOneItemOnly) {
        items = new List<ItemSlot>();
        maxItemCount = size;
        oneItemOnly = isOneItemOnly;
    }
    public Inventory(List<ItemSlot> itemSlots, int size, bool isOneItemOnly) {
        items = new List<ItemSlot>();
        maxItemCount = size;
        oneItemOnly = isOneItemOnly;
    }

    public void clearInventory() {
        foreach(ItemSlot i in items) {
            i.Item = new Item();
            i.Amount = 0;
        }
        currentItemCount = 0;
        if (oneItemFilled)
            oneItemFilled = false;
    }
    public void clearInventoryOfResources() {
        foreach (ItemSlot i in items) {
            if(i.Item.ItemType == ItemType.Resource) {
                i.Item = new Item();
                currentItemCount -= i.Amount;
                i.Amount = 0;
                if (oneItemFilled)
                    oneItemFilled = false;
            }
        }
    }

    public void addItem(Item itemToAdd) {
        bool added = false;
        if((oneItemOnly && !oneItemFilled) || !oneItemOnly) {
            if (containsItem(itemToAdd) && hasSpace()) {
                items[getItemSlotIdFromItem(itemToAdd)].Amount++;
                added = true;
            }
            else if (hasSpace()) {
                if(items.Count == 0) {
                    items.Add(new ItemSlot());
                }
                foreach (ItemSlot i in items) {
                    if (i.Item.ID == -1) {
                        i.Item = itemToAdd;
                        i.Amount = 1;
                        currentItemCount++;
                        added = true;
                    }
                }
            }
            if(oneItemOnly)
            oneItemFilled = true;
        }
        if(!added) {
            droppedItems.Add(new ItemSlot(itemToAdd));
        }
        
    }
    public void addItem(Item itemToAdd, int amt) {
        bool added = false;
            if (containsItem(itemToAdd) && hasSpace(amt)) {
                items[getItemSlotIdFromItem(itemToAdd)].Amount+= amt;
                added = true;
                currentItemCount += amt;
            }
            if (hasSpace(amt) && !containsItem(itemToAdd) && ((oneItemOnly && !oneItemFilled) || !oneItemOnly)) {
                if (items.Count == 0) {
                    items.Add(new ItemSlot());
                }
                foreach (ItemSlot i in items) {
                    if (i.Item.ID == -1) {
                        i.Item = itemToAdd;
                        i.Amount = amt;
                        currentItemCount+= amt;
                        added = true;
                    }
                }
            }
            if(containsItem(itemToAdd) && !hasSpace(amt) && hasSpace()) {
                int tempAmt = amt;
                items[getItemSlotIdFromItem(itemToAdd)].Amount += maxItemCount - currentItemCount;
                tempAmt = maxItemCount - currentItemCount;
                currentItemCount += tempAmt;
                droppedItems.Add(new ItemSlot(itemToAdd, tempAmt));
                added = true;
            }
            if (!hasSpace(amt) && hasSpace() && !containsItem(itemToAdd) && ((oneItemOnly && !oneItemFilled) || !oneItemOnly)) {
                if (items.Count == 0) {
                    items.Add(new ItemSlot());
                }
                foreach (ItemSlot i in items) {
                    if (i.Item.ID == -1) {
                        int tempAmt = amt;
                        int difference = maxItemCount - currentItemCount;
                        i.Item = itemToAdd;
                        i.Amount = difference;
                        currentItemCount += difference;
                        tempAmt -= difference;
                        droppedItems.Add(new ItemSlot(itemToAdd, tempAmt));
                        added = true;
                    }
                }

            }
            if(oneItemOnly)
            oneItemFilled = true;
        
        if(!added) {
            droppedItems.Add(new ItemSlot(itemToAdd, amt));
        }
    }

    public void removeItem(Item itemToRemove) {
            if (containsItem(itemToRemove)) {
                currentItemCount -= items[getItemSlotIdFromItem(itemToRemove)].Amount;
                items[getItemSlotIdFromItem(itemToRemove)] = new ItemSlot();

            }
        if (oneItemOnly) {
            oneItemFilled = false;
        }
                
       
    }

    public void reduceItemAmount(Item itemToReduce, int amount) {
        if (containsItem(itemToReduce)) {
            
            items[getItemSlotIdFromItem(itemToReduce)].Amount -= amount;
            if (
            items[getItemSlotIdFromItem(itemToReduce)].Amount <= 0) {
                removeItem(itemToReduce);
                currentItemCount -= items[getItemSlotIdFromItem(itemToReduce)].Amount;
            }
            else {
                currentItemCount -= amount;
            }
        }
    }

    public void setItemInSlot(int slotId, Item itemToSet, int amt) {
        if(items[slotId] != null) {
            items[slotId] = new ItemSlot(itemToSet, amt);
        }
    }

    public int getItemSlotIdFromItem(Item itemToCheckFor) {
        if (containsItem(itemToCheckFor)) {
            for (int i = 0; i < items.Count; i++) {
                if(items[i].Item == itemToCheckFor)
                return i;
            }
        }
        return -1;
    }

    public Item getItemInSlot(int slotId) {
        if(items[slotId] != null) {
            return items[slotId].Item;
        }
        return null;
    }

    public bool containsItem(Item itemToCheckFor) {
        foreach(ItemSlot i in items) {
            if(i.Item == itemToCheckFor) {
                return true;
            }
        }
        return false;
    }
    public bool hasSpace() {
        return (currentItemCount < maxItemCount);
    }
    public bool hasSpace(int spaceNeeded) {
        return (currentItemCount+spaceNeeded < maxItemCount);
    }

    
}
[System.Serializable]

public class ItemSlot
{
    public Item Item;
    public int Amount;

    public ItemSlot(Item item, int amt) {
        Item = item;
        Amount = amt;
    }

    public ItemSlot(Item item) {
        Item = item;
        Amount = 1;
    }

    public ItemSlot() {
        Item = new Item();
        Amount = 0;
    }

    public void AddItem() {
        Amount++;
    }

    public void AddItem(int amt) {
        Amount = amt;
    }

    public void SetAmount(int amt) {
        Amount = amt;
    }

    public void ClearSlot() {
        Item = new Item();
        Amount = 0;
    }
    
}
