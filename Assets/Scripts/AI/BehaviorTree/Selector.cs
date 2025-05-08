using UnityEngine;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

public class Selector : InteriorNode
{
    public override Result Run()
    {
        if (current_child >= children.Count)
        {
            current_child = 0;
            return Result.SUCCESS;
        }

        Result res = children[current_child].Run();

        if (res == Result.SUCCESS)
        {
            current_child = 0;
            return Result.FAILURE;
        }

        if (res == Result.FAILURE) 
        {
            current_child++;
        }

        if ()
        return Result.IN_PROGRESS;
    }

    public Selector(IEnumerable<BehaviorTree> children) : base(children)
    {
    }

    public override BehaviorTree Copy()
    {
        return new Selector(CopyChildren());
    }

}
