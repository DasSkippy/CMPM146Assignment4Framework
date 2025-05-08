public class Dist1MinQuery : BehaviorTree
{
    public override Result Run()
    {

        Vector3 group1 = <wp2>.transform.position
        Vector3 group2 = <wp1>.transform.position
        Vector3 group3 = <wp3>.transform.position

        if ((agent.transform.position - group1).magnitude < (agent.transform.position - group2).magnitude && (agent.transform.position - group1).magnitude < (agent.transform.position - group3).magnitude) {
            return Result.SUCCESS;
        } else {
            return Result.FAILURE;
        }
    }

    public Dist1MinQuery() {
    }

    public override BehaviorTree Copy()
    {
        return new Dist1MinQuery();
    }
}