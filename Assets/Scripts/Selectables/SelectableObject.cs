using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

public class SelectableObject : MonoBehaviour
{
    public enum SelectableType
    {
        Unit,
        Building,
        Resource
    }
    public enum SelectableState
    {
        Alive,
        Dead
    }

    [SerializeField]
    public string Name;
    public int OwnerID;
    public Player Owner;
    [HideInInspector]
    public bool isSelected;
    public int currentHealth;
    public int maxHealth;
    public Camera selectedCamera;
    public SelectableType selectableType;
    public SelectableState currentSelectableState;
    public AttackStats attackStats;
    public Inventory inventory;
    public int InventorySize;
    public bool inventoryCanOnlyHoldOneItem;
    public float idle;
    public SelectableObject prevDamageDealer;
    public bool canAttack;
    public bool isInvincible;

    private void Start() {
        SetOwner(FindObjectOfType<PlayerManager>().GetPlayerByID(OwnerID));
        inventory = new Inventory(InventorySize, inventoryCanOnlyHoldOneItem);
        if (gameObject.GetComponentInChildren<Camera>()) {
            selectedCamera = gameObject.GetComponentInChildren<Camera>();
            selectedCamera.enabled = false;
        }
        if(currentHealth > 0) {
            currentSelectableState = SelectableState.Alive;
        }
    }

    private void Awake() {
        
    }

    private void Update() {
        idle -= Time.deltaTime;
        if (inventory == null) {
            inventory = new Inventory(InventorySize, inventoryCanOnlyHoldOneItem);
        }
        if (!isSelected) {
            if (selectedCamera != null)
                selectedCamera.enabled = false;
        }
        else {
            if (selectedCamera != null)
                selectedCamera.enabled = true;
        }
    }

    public virtual bool Interact(SelectableObject s) {
        return false;
    }

    public void SetOwner(Player playerToSetOwner) {
        Owner = playerToSetOwner;
    }

    public Player GetOwner() {
        return Owner;
    }

    public void SelectObject() {
        isSelected = true;
        print("Select");
    }

    public void DeselectObject() {
        isSelected = false;
        print("Deselect");

    }

    public void Heal(int healByAmt) {
        currentHealth += healByAmt;
        if (currentHealth > maxHealth) {
            currentHealth = maxHealth;
        }
    }

    public virtual void TakeDamage(Damage damage) {
        prevDamageDealer = damage.Dealer;
        currentHealth -= damage.getAmount();
        if (currentHealth <= 0) {
            Die();
        }

    }

    public virtual void Die() {
        print(Name + " died to: " + prevDamageDealer.Name);
        currentSelectableState = SelectableState.Dead;
        Destroy(gameObject);
    }

    public void AddDelay(float amt) {
        if (idle < 0)
            idle = 0;
        idle += amt;
    }

    public void SetDelay(float delay) {
        idle = delay;
    }

    public bool hasDelay() {
        return (idle > 0);
    }


}


[System.Serializable]
public class AttackStats
{
    public int minAttackDamage = 0;
    public int maxAttackDamage = 0;
    public float attackRange = 0;
    public float attackRate = 0;
}
