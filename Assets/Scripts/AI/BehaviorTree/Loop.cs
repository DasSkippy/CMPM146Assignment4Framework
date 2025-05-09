using UnityEngine;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

public class Loop : InteriorNode 
{
    public override Result Run()
    {
        if (current_child >= children.Count)
        {
            current_child = 0;
        }
        Result res = children[current_child].Run();
        current_child++;
        return Result.IN_PROGRESS;
    }

    public Loop(IEnumerable<BehaviorTree> children) : base (children)
    {
    }

    public override BehaviorTree Copy()
    {
        return new Loop(CopyChildren());
    }
}
