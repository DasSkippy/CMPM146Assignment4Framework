using UnityEngine;

public class BehaviorBuilder
{
    public static BehaviorTree MakeTree(EnemyController agent)
    {
        BehaviorTree result = null;



        new Selector(new BehaviorTree[] {

            new Selector(new BehaviorTree[] {

                new Sequence(new BehaviorTree[] {

                    new PlayerInRangeQuery(15f), // try and change 15 to something like: GameManager.Instance.player....
                    new MoveToPlayer(agent.GetAction("attack").range),
                    new Attack()

                }),

                new Sequence(new BehaviorTree[] {

                    new EnemiesInRangeQuery(3f),
                    new AttackSequence()

                })
                
            }),

            new avoidplayer()

        });





        if (agent.monster == "warlock")
        {
            result = new Sequence(new BehaviorTree[] {
                                        new MoveToPlayer(agent.GetAction("attack").range),
                                        new Attack(),
                                        new PermaBuff(),
                                        new Heal(),
                                        new Buff()
                                     });
        }
        else if (agent.monster == "zombie")
        {
            result = new Sequence(new BehaviorTree[] {
                                       new MoveToPlayer(agent.GetAction("attack").range),
                                       new Attack()
                                     });
        }
        else
        {
            result = new Sequence(new BehaviorTree[] {
                                       new MoveToPlayer(agent.GetAction("attack").range),
                                       new Attack()
                                     });
        }

        // do not change/remove: each node should be given a reference to the agent
        foreach (var n in result.AllNodes())
        {
            n.SetAgent(agent);
        }
        return result;
    }
}


