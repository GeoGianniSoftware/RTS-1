using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Harvester : iBehavior
{

    public Harvester() {
        BehaviorName = "Harvester";
    }


    public void FindNewResourceTarget(Unit unitRef) {
        SelectableObject newResource;
        if (unitRef.attackTarget != null) {
            newResource = unitRef.attackTarget.GetComponent<Resource>().GetNearestResourceOfSameType();
            if (newResource != null) {
                unitRef.attackTarget = newResource;
            }
            else {
                unitRef.attackTarget = null;
            }
        }
    }

    public void Harvest(Unit unitRef, SelectableObject objectToHarvest) {
        if (!unitRef.inventory.hasSpace())
            ReturnHarvest(unitRef);
        unitRef.attackTarget = objectToHarvest;
    }

    public void ReturnHarvest(Unit unitRef) {
        if (unitRef.nearestHomeBuilding != null) {
            unitRef.NMA.SetDestination(unitRef.nearestHomeBuilding.transform.position);
            if (Vector3.Distance(unitRef.transform.position, unitRef.nearestHomeBuilding.transform.position) <= 7f) {
                PlayerManager PM = GameObject.FindObjectOfType<PlayerManager>();
                foreach (ItemSlot i in unitRef.inventory.items) {
                    if (i.Item.ItemType == ItemType.Resource) {
                        PM.GivePlayerResource(PM.GetPlayerIDByPlayer(unitRef.Owner), i.Item, i.Amount);
                    }
                }
                unitRef.inventory.clearInventoryOfResources();
            }
        }
        else {
            unitRef.attackTarget = null;
        }
    }
}