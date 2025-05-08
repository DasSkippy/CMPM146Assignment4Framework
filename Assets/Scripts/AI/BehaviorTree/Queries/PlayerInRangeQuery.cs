public class PlayerInRangeQuery : BehaviorTree
{
    public override Result Run()
    {
        float distance;

        Vector3 direction = GameManager.Instance.player.transform.position - agent.transform.position;
        if (direction.magnitude < distance) {
            return Result.SUCCESS;
        } else {
            return Result.FAILURE;
        }
    }

    public PlayerInRangeQuery(float distance) {
        this.distance = distance;
    }

    public override BehaviorTree Copy()
    {
        return new PlayerInRangeQuery(distance);
    }
}