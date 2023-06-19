using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable]

[RequireComponent(typeof(NavMeshAgent))]
public class Unit : SelectableObject
{
  

    public UnitState currentUnitState;
    public int speed;

    [HideInInspector]
    public List<Vector3> path = new List<Vector3>();

    [HideInInspector]
    public NavMeshAgent NMA;

    public SelectableObject currentInteractionObject;
    public SelectableObject attackTarget;
    float stoppingDistance = .35f;
    GameObject selectionCircle;
    public List<UnitTags> unitTags = new List<UnitTags>();

    [HideInInspector]
    public SelectableObject nearestHomeBuilding = null;

    [SerializeField]
    public List<iBehavior> unitBehaviors;

    // Start is called before the first frame update
    void Start()
    {
        NMA = GetComponent<NavMeshAgent>();
        currentHealth = maxHealth;
        NMA.speed = speed;
        selectionCircle = gameObject.GetComponentInChildren<UnitSelectionCircle>().gameObject;
        inventory = new Inventory(InventorySize, inventoryCanOnlyHoldOneItem);
        if (selectionCircle == null) {
            GameObject circ = Instantiate((GameObject)Resources.Load("UI/UnitSelectionCircle") as GameObject, transform.position, Quaternion.identity);
            circ.transform.SetParent(this.transform, false);
            selectionCircle = circ;
        }
        //Set Unit Owner TO Player!
        SetOwner(FindObjectOfType<PlayerManager>().players[0]);
        if (gameObject.GetComponentInChildren<Camera>()) {
            selectedCamera = gameObject.GetComponentInChildren<Camera>();
            selectedCamera.enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        idle -= Time.deltaTime;
        //Create Inventory
        if(inventory == null && InventorySize > 0)
            inventory = new Inventory(InventorySize, inventoryCanOnlyHoldOneItem);
        //Find Nearest Home Building
        if (FindObjectOfType<PlayerManager>().homeBuilding != null) {
            nearestHomeBuilding = FindObjectOfType<PlayerManager>().homeBuilding;
        }
        selectionCircle.GetComponent<MeshRenderer>().enabled = isSelected;
        //Idle
        if (currentUnitState == UnitState.idle) {
            if (hasTag(UnitTags.Critter) && idle <= 0) {
                foreach (iBehavior b in unitBehaviors) {
                    if (b.BehaviorName == "Critter") {
                        Critter c = (Critter)b;
                        MoveToDest(c.getRandomWanderPos(this));
                    }
                }
            }
            if (attackTarget != null)
                currentUnitState = UnitState.moving;
            NMA.isStopped = true;
            //Moving
        }
        else if (currentUnitState == UnitState.moving) {
            if (attackTarget == null) {
                NMA.stoppingDistance = stoppingDistance;
                //Has Movement Queued
                if (path.Count > 0) {
                    NMA.isStopped = false;
                    NMA.SetDestination(path[0]);
                }
                //Set Idle
                else {
                    currentUnitState = UnitState.idle;
                }
                //Finished Moving
                if (pathComplete()) {
                    if (hasTag(UnitTags.Critter)) {
                        foreach (iBehavior b in unitBehaviors) {
                            if (b.BehaviorName == "Critter") {
                                Critter c = (Critter)b;
                                AddDelay(c.getRandomIdle());

                            }
                        }
                    }
                    if (path.Count > 0)
                        path.RemoveAt(0);
                }
            }
            else {
                //Harvester
                if (hasTag(UnitTags.Harvester) && attackTarget.selectableType == SelectableType.Resource && inventory.hasSpace() && attackTarget.GetComponent<Resource>().resourcesRemaining > 0) {
                    NMA.stoppingDistance = attackStats.attackRange - .5f;
                    NMA.isStopped = false;

                    NMA.SetDestination(attackTarget.transform.position);
                    if (Vector3.Distance(transform.position, attackTarget.transform.position) < attackStats.attackRange) {
                        currentUnitState = UnitState.attacking;
                    }
                }
                else if (hasTag(UnitTags.Harvester) && attackTarget.selectableType == SelectableType.Resource && inventory.hasSpace() && attackTarget.GetComponent<Resource>().resourcesRemaining <= 0) {
                    foreach (iBehavior b in unitBehaviors) {
                        if (b.BehaviorName == "Harvester") {
                            Harvester h = (Harvester)b;
                            h.FindNewResourceTarget(this);
                        }
                    }
                }
                //Attacker
                if (canAttack && !attackTarget.isInvincible && Owner != attackTarget.Owner) {
                    NMA.stoppingDistance = attackStats.attackRange - .5f;
                    NMA.isStopped = false;

                    NMA.SetDestination(attackTarget.transform.position);
                    if (Vector3.Distance(transform.position, attackTarget.transform.position) < attackStats.attackRange) {
                        currentUnitState = UnitState.attacking;
                    }
                }



            }
        }
        //Atacking
        else if (currentUnitState == UnitState.attacking) {
            if (attackTarget != null) {
                //Harvester Attacking Resource
                if (hasTag(UnitTags.Harvester) && attackTarget.selectableType == SelectableType.Resource && !inventory.hasSpace()) {
                    foreach (iBehavior b in unitBehaviors) {
                        if (b.BehaviorName == "Harvester") {
                            Harvester h = (Harvester)b;
                            NMA.stoppingDistance = attackStats.attackRange - .5f;
                            NMA.isStopped = false;
                            h.ReturnHarvest(this);
                        }
                    }

                }
                // Close Enough To Attack
                else if (Vector3.Distance(transform.position, attackTarget.transform.position) < attackStats.attackRange) {
                    Attack(attackTarget);
                }
                else {
                    currentUnitState = UnitState.idle;
                }

            }
        }
        //Following Selectable
        else if (currentUnitState == UnitState.following) {
            NMA.stoppingDistance = 5f;
            if (currentInteractionObject != null) {
                NMA.isStopped = false;
                NMA.SetDestination(currentInteractionObject.transform.position);
            }
            else {
                currentUnitState = UnitState.idle;
            }
        }
       



    }


    public bool hasTag(UnitTags tag) {
        if (unitTags.Contains(tag)) {
            return true;
        }
        else {
            return false;
        }
    }
    protected bool pathComplete() {
        if (!NMA.pathPending) {
            if (NMA.remainingDistance <= NMA.stoppingDistance) {
                if (!NMA.hasPath || NMA.velocity.sqrMagnitude == 0f) {
                    return true;
                }
            }
        }
        return false;
    }

    public override bool Interact(SelectableObject s) {

        if (s.GetComponent<Unit>()) {
                Unit u = s.GetComponent<Unit>();
                FollowUnit(u);
            return true;
        }
        if(s.selectableType == SelectableType.Resource && unitTags.Contains(UnitTags.Harvester)) {
            foreach (iBehavior b in unitBehaviors) {
                if (b.BehaviorName == "Harvester") {
                    Harvester h = (Harvester)b;
                    h.Harvest(this, s);
                    return true;
                }
            }
            
        } 
        
        return false;
    }

   

    void FollowUnit(Unit u) {
        currentUnitState = UnitState.following;
        currentInteractionObject = u;
    }



    void Attack(SelectableObject objectToAttack) {
        if (!hasDelay() && objectToAttack != null) {
            objectToAttack.SendMessage("TakeDamage", new Damage(Random.Range(attackStats.minAttackDamage, attackStats.maxAttackDamage), this), SendMessageOptions.DontRequireReceiver);
            
            AddDelay(attackStats.attackRate);
        }
    }
  

    void ClearPath(bool haltMove) {
        if(path.Count > 0) {
            path.Clear();

            if(haltMove)
            currentUnitState = UnitState.idle;

            currentInteractionObject = null;
        }

    }

    //MOVE COMANDS
    public void AddMove(Vector3 position) {
        AddToPath(position, Vector3.zero);


    }
    public void AddMove(Vector3 position, int index, int groupMoveCount) {
        AddToPath(position, getUnitGroupOffset(index, groupMoveCount));
        currentUnitState = UnitState.moving;
    }

    public void MoveToDest(Vector3 position) {
        ClearPath(false);
        NMA.SetDestination(position);
        AddToPath(position, Vector3.zero);
        if(NMA.pathStatus == NavMeshPathStatus.PathInvalid || NMA.pathStatus == NavMeshPathStatus.PathPartial) {
            print("Invalid");
            ClearPath(false);
            return;
        }
        currentUnitState = UnitState.moving;
        attackTarget = null;

    }
    public void MoveToDest(Vector3 position, int index, int groupMoveCount) {
        ClearPath(false);
        AddToPath(position, getUnitGroupOffset(index, groupMoveCount));
        currentUnitState = UnitState.moving;
        attackTarget = null;

    }

    void AddToPath(Vector3 position, Vector3 offset) {
        path.Add(position + offset);

    }
    //END MOVE COMMANDS

    Vector3 getUnitGroupOffset(int index, int groupMoveCount) {
        int rows = groupMoveCount / 5;
        int currentRow = index / 5;
        int indexPos = ((index) - (groupMoveCount / 2) -(index / 5)*5);


        Vector3 offset = new Vector3(currentRow*2, 0, indexPos*2);
        return offset;
    }


}

[System.Serializable]
public enum UnitTags
{
    Harvester,
    Builder,
    Flying,
    Ground,
    Critter
}
[System.Serializable]
public enum UnitState
{
    idle,
    moving,
    following,
    attacking,
    stunned,
    dead

}
