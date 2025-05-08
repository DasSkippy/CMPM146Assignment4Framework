using UnityEngine;

public class BehaviorBuilder
{
    private static Transform wp1 = GameObject.Find("safezone 1").transform;
    private static Transform wp2 = GameObject.Find("safezone 2").transform;
    private static Transform wp3 = GameObject.Find("safezone 3").transform;


    public static BehaviorTree MakeTree(EnemyController agent)
    {
        BehaviorTree result = null;





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
            // result = new Sequence(new BehaviorTree[] {
            //                            new MoveToPlayer(agent.GetAction("attack").range),
            //                            new Attack()
            //                          });
            result = new Selector(new BehaviorTree[] {

                new Selector(new BehaviorTree[] {

                    new Sequence(new BehaviorTree[] {

                        new PlayerInRangeQuery(15f), // try and change 15 to something like: GameManager.Instance.player....
                        new MoveToPlayer(agent.GetAction("attack").range),
                        new Attack()

                    }),

                    new Sequence(new BehaviorTree[] {

                        new NearbyEnemiesQuery(20, 5f),
                        new Sequence(new BehaviorTree[] {
                            new MoveToPlayer(agent.GetAction("attack").range),
                            new Attack()
                        })

                    })
                    
                }),

                // avoid player
                new Selector(new BehaviorTree[] {

                    new Sequence(new BehaviorTree[] {
                        new Dist1MinQuery(),
                        new GoTo(wp2, 0.5f)
                    }),

                    new Selector(new BehaviorTree[] {

                        new Sequence(new BehaviorTree[] {
                            new Dist2MinQuery(),
                            new GoTo(wp1, 0.5f)
                        }),

                        new Selector(new BehaviorTree[] {

                            new Sequence(new BehaviorTree[] {
                                new Dist3MinQuery(),
                                new GoTo(wp3, 0.5f)
                            }),

                            new MoveToPlayer(agent.GetAction("attack").range)

                        })

                    })

                })

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


