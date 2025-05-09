using UnityEngine;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

public class Dist3MinQuery : BehaviorTree
{
    public override Result Run()
    {

        Vector3 group2 = GameObject.Find("safezone 2").transform.position;
        Vector3 group1 = GameObject.Find("safezone 1").transform.position;
        Vector3 group3 = GameObject.Find("safezone 3").transform.position;

        if ((agent.transform.position - group3).magnitude < (agent.transform.position - group2).magnitude && (agent.transform.position - group3).magnitude 
            < (agent.transform.position - group1).magnitude) {
            return Result.SUCCESS;
        } else {
            return Result.FAILURE;
        }
    }

    public Dist3MinQuery() {
    }

    public override BehaviorTree Copy()
    {
        return new Dist3MinQuery();
    }
}