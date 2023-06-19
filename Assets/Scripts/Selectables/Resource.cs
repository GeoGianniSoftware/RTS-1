using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]

public class Resource : SelectableObject
{
    public enum ResourceType
    {
        Stone,
        Wood
    }

    public bool emptied;
    public ResourceType resourceType;
    [HideInInspector]
    public Item resourceItem;
    public Vector2 maxAmountRange;
    int maxAmount;
    public int resourcesRemaining;


    private void Start() {

        maxAmount = Random.Range((int)maxAmountRange.x, (int)maxAmountRange.y);
        resourcesRemaining = maxAmount;
        if(resourcesRemaining > 0) {
            emptied = false;
        }
        setResourceItem();
        if (gameObject.GetComponentInChildren<Camera>()) {
            selectedCamera = gameObject.GetComponentInChildren<Camera>();
            selectedCamera.enabled = false;
        }
    }

   

    ItemSlot harvestReturn(int amt) {
        if(resourceItem != null) {
            ItemSlot temp = new ItemSlot(resourceItem, amt);
            return temp;
        }
        return null;
    }

    void setResourceItem() {
        if (resourceType == ResourceType.Stone) {
            resourceItem = ItemManager.GetItemByID(1);
        }
        else {
            resourceItem = ItemManager.GetItemByID(0);
        }
    }

    int getResourceAmountGiven(int dmgTaken) {
        float percentLost =( (float)dmgTaken / (float)maxHealth);

        int resourceRatio = (int)(resourcesRemaining * percentLost);
        if(resourceRatio > resourcesRemaining) {
            resourceRatio = resourcesRemaining;
        }
        return resourceRatio;
    }

    public override void TakeDamage(Damage damage) {
        Unit lastDamaged = (Unit)damage.getDealer();
        currentHealth -= damage.getAmount();
        if(lastDamaged != null && lastDamaged.hasTag(UnitTags.Harvester) && damage.getDealer().selectableType == SelectableType.Unit){
            
            lastDamaged.
                inventory
                .addItem
                (resourceItem,
                getResourceAmountGiven(damage.Amount));
            resourcesRemaining -= getResourceAmountGiven(damage.Amount);
        }

        if (currentHealth <= 0) {
            Die();
        }   

    }

    public override void Die() {
        resourcesRemaining = 0;
        emptied = true;
        transform.GetChild(1).gameObject.SetActive(false);
    }

    public Resource GetNearestResourceOfSameType() {
        List<Resource> allResources = new List<Resource>();
        allResources.AddRange(GameObject.FindObjectsOfType<Resource>());
        if(allResources.Count > 0) {
            float closest = float.MaxValue;
            Resource closestRef = null;
            foreach (Resource r in allResources) {
                float dist = Vector3.Distance(transform.position, r.transform.position);
                if (r.resourcesRemaining > 0 && r.resourceType == this.resourceType && dist < closest) {
                    closestRef = r;
                    closest = dist;
                }

            }
            return closestRef;
        }
        else {
            return null;
        }
    }
}
