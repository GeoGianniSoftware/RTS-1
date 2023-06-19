using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Critter : iBehavior
{
    public float wanderRange;
    public float idleRange;


    public Critter() {
        BehaviorName = "Critter";

    }


    public Vector3 getRandomWanderPos(Unit unitRef) {
        float xOffset = Random.Range(-wanderRange, wanderRange);
        float zOffset = Random.Range(-wanderRange, wanderRange);

        Vector3 transPos = unitRef.transform.position;
        Vector3 newPos = new Vector3(transPos.x + xOffset, transPos.y, transPos.z + zOffset);

        return newPos;
    }

    public float getRandomIdle () {
        float idle = Random.Range(1, idleRange);
        return idle;
    }
}
