using UnityEngine;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

public class Dist2MinQuery : BehaviorTree
{
    public override Result Run()
    {

        Vector3 group1 = GameObject.Find("wp2").transform.position;
        Vector3 group2 = GameObject.Find("wp1").transform.position;
        Vector3 group3 = GameObject.Find("wp3").transform.position;

        if ((agent.transform.position - group2).magnitude < (agent.transform.position - group1).magnitude && (agent.transform.position - group2).magnitude < (agent.transform.position - group3).magnitude) {
            return Result.SUCCESS;
        } else {
            return Result.FAILURE;
        }
    }

    public Dist2MinQuery() {
    }

    public override BehaviorTree Copy()
    {
        return new Dist2MinQuery();
    }
}