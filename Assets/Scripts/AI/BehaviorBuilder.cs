using UnityEngine;

// public class ZombieNearPlayerQuery : BehaviorTree
// {
//     float _range;
//     public ZombieNearPlayerQuery(float range) { _range = range; }

//     // replace Status/Execute() with your API’s exact return‐type & method name
//     public override Result Run()
//     {
//         var playerPos = GameManager.Instance.player.transform.position;
//         foreach (var e in GameManager.Instance.AllEnemies)
//         {
//             if (e.monster == "zombie" &&
//                 Vector3.Distance(e.transform.position, playerPos) <= _range)
//                 return Result.SUCCESS;
//         }
//         return Result.FAILURE;
//     }
// }

// public class MoveBehindNearestZombie : BehaviorTree
// {
//     float _offset, _tolerance;
//     public MoveBehindNearestZombie(float offset, float tolerance)
//     {
//         _offset = offset;
//         _tolerance = tolerance;
//     }

//     public override Result Run()
//     {
//         // find the zombie that’s closest to the player
//         var playerPos = GameManager.Instance.player.transform.position;
//         EnemyController nearest = null;
//         float bestDist = float.MaxValue;

//         foreach (var e in GameManager.Instance.AllEnemies)
//         {
//             if (e.monster != "zombie") continue;
//             float d = Vector3.Distance(e.transform.position, playerPos);
//             if (d < bestDist)
//             {
//                 bestDist = d;
//                 nearest = e;
//             }
//         }
//         if (nearest == null) return Result.FAILURE;

//         // compute “behind” position = zombiePos + (zombiePos–playerPos).normalized * offset
//         var dir = (nearest.transform.position - playerPos).normalized;
//         var target = nearest.transform.position + dir * _offset;

//         // if we’re close enough, succeed
//         if (Vector3.Distance(agent.transform.position, target) <= _tolerance)
//             return Result.SUCCESS;

//         // otherwise move toward it
//         agent.MoveTo(target);
//         return Result.IN_PROGRESS;
//     }
// }

public static class EnemyAlert
{
    public static bool IsAlerted { get; private set; }
    public static void RaiseAlert() => IsAlerted = true;
    public static void ClearAlert() => IsAlerted = false;
}

public class RaiseAlertNode : BehaviorTree
{
    // replace “BehaviorStatus” and “Evaluate” with whatever your
    // base class actually uses (e.g. Execute, Run, etc.)
    public override Result Run()
    {
        EnemyAlert.RaiseAlert();
        return Result.SUCCESS;
    }
}

public class GlobalAlertQuery : BehaviorTree
{
    public override Result Run()
    {
        return EnemyAlert.IsAlerted 
            ? Result.SUCCESS 
            : Result.FAILURE;
    }
    // no SetAgent override needed, the MakeTree loop will assign
}

public class BehaviorBuilder
{
    private static Transform wp1 = GameObject.Find("safezone 1").transform;
    private static Transform wp2 = GameObject.Find("safezone 2").transform;
    private static Transform wp3 = GameObject.Find("safezone 3").transform;
    private static Transform wp1first = GameObject.Find("safezone1first").transform;


    public static BehaviorTree MakeTree(EnemyController agent)
    {
        BehaviorTree result = null;





        if (agent.monster == "warlock")
        {
            // result = new Sequence(new BehaviorTree[] {
            //                             new MoveToPlayer(agent.GetAction("attack").range),
            //                             new Attack(),
            //                             new PermaBuff(),
            //                             new Heal(),
            //                             new Buff()
            //                          });
            BehaviorTree maintree = new Selector(new BehaviorTree[] {

                new Selector(new BehaviorTree[] {

                    new Sequence(new BehaviorTree[] {

                        new PlayerInRangeQuery(15f), // try and change 15 to something like: GameManager.Instance.player....
                        new MoveToPlayer(agent.GetAction("attack").range),
                        new Attack()

                    }),

                    new Sequence(new BehaviorTree[] {
                        new NearbyEnemiesQuery(1,5f),
                        new Selector(new BehaviorTree[] {
                            new Sequence(new BehaviorTree[] {
                                new AbilityReadyQuery("permabuff"),
                                new PermaBuff()
                            }),
                            new Sequence(new BehaviorTree[] {
                                new AbilityReadyQuery("heal"),
                                new Heal()
                            })
                        }),
                        new NearbyEnemiesQuery(5, 10f),
                        new RaiseAlertNode(),
                        new MoveToPlayer(agent.GetAction("attack").range),
                        new Attack()

                    })
                    
                }),

                // avoid player
                new Selector(new BehaviorTree[] {

                    new Sequence(new BehaviorTree[] {
                        new Dist1MinQuery(),
                        new GoTo(wp1first, 5f),
                        new GoTo(wp1, 5f)
                    }),

                    new Selector(new BehaviorTree[] {

                        new Sequence(new BehaviorTree[] {
                            new Dist2MinQuery(),
                            new GoTo(wp2, 5f)
                        }),

                        new Selector(new BehaviorTree[] {

                            new Sequence(new BehaviorTree[] {
                                new Dist3MinQuery(),
                                new GoTo(wp3, 5f)
                            }),

                            new MoveToPlayer(agent.GetAction("attack").range)

                        })

                    })

                })

            });
        
            var alertAttack = new Sequence(new BehaviorTree[] {
                new GlobalAlertQuery(),
                new Loop(new BehaviorTree[] {
                    new MoveToPlayer(agent.GetAction("attack").range),
                    
                    new Sequence(new BehaviorTree[] {
                            new PlayerInRangeQuery(5f),
                            new AbilityReadyQuery("buff"),
                            new Buff()
                    }),
                    new Sequence(new BehaviorTree[] {
                            new AbilityReadyQuery("permabuff"),
                            new PermaBuff()
                    }),
                    new Sequence(new BehaviorTree[] {
                            new AbilityReadyQuery("heal"),
                            new Heal()
                    }),
                    new Attack()
                })
            });

            result = new Selector(new BehaviorTree[] {
                alertAttack,
                maintree
            });

        }
        
        else if (agent.monster == "zombie")
        {
            // result = new Sequence(new BehaviorTree[] {
            //                            new MoveToPlayer(agent.GetAction("attack").range),
            //                            new Attack()
            //                          });
            BehaviorTree maintree = new Selector(new BehaviorTree[] {

                new Selector(new BehaviorTree[] {

                    new Sequence(new BehaviorTree[] {

                        new PlayerInRangeQuery(15f), // try and change 15 to something like: GameManager.Instance.player....
                        new MoveToPlayer(agent.GetAction("attack").range),
                        new Attack()

                    }),

                    new Sequence(new BehaviorTree[] {

                        new NearbyEnemiesQuery(5, 10f),
                        new RaiseAlertNode(),
                        new MoveToPlayer(agent.GetAction("attack").range),
                        new Attack()

                    })
                    
                }),

                // avoid player
                new Selector(new BehaviorTree[] {

                    new Sequence(new BehaviorTree[] {
                        new Dist1MinQuery(),
                        new GoTo(wp1first, 5f),
                        new GoTo(wp1, 5f)
                    }),

                    new Selector(new BehaviorTree[] {

                        new Sequence(new BehaviorTree[] {
                            new Dist2MinQuery(),
                            new GoTo(wp2, 5f)
                        }),

                        new Selector(new BehaviorTree[] {

                            new Sequence(new BehaviorTree[] {
                                new Dist3MinQuery(),
                                new GoTo(wp3, 5f)
                            }),

                            new MoveToPlayer(agent.GetAction("attack").range)

                        })

                    })

                })

            });
            
            var alertAttack = new Sequence(new BehaviorTree[] {
                new GlobalAlertQuery(),
                new Loop(new BehaviorTree[] {
                    new MoveToPlayer(agent.GetAction("attack").range),
                    new Attack()
                })
            });
            
            result = new Selector(new BehaviorTree[] {
                alertAttack,
                maintree
            });
        }
        
        else
        {
            // result = new Sequence(new BehaviorTree[] {
            //                            new MoveToPlayer(agent.GetAction("attack").range),
            //                            new Attack()
            //                          });
            BehaviorTree maintree = new Selector(new BehaviorTree[] {

                new Selector(new BehaviorTree[] {

                    new Sequence(new BehaviorTree[] {

                        new PlayerInRangeQuery(15f), // try and change 15 to something like: GameManager.Instance.player....
                        new MoveToPlayer(agent.GetAction("attack").range),
                        new Attack()

                    }),

                    new Sequence(new BehaviorTree[] {

                        new NearbyEnemiesQuery(5, 10f),
                        new RaiseAlertNode(),
                        new MoveToPlayer(agent.GetAction("attack").range),
                        new Attack()

                    })
                    
                }),

                // avoid player
                new Selector(new BehaviorTree[] {

                    new Sequence(new BehaviorTree[] {
                        new Dist1MinQuery(),
                        new GoTo(wp1first, 5f),
                        new GoTo(wp1, 5f)
                    }),

                    new Selector(new BehaviorTree[] {

                        new Sequence(new BehaviorTree[] {
                            new Dist2MinQuery(),
                            new GoTo(wp2, 5f)
                        }),

                        new Selector(new BehaviorTree[] {

                            new Sequence(new BehaviorTree[] {
                                new Dist3MinQuery(),
                                new GoTo(wp3, 5f)
                            }),

                            new MoveToPlayer(agent.GetAction("attack").range)

                        })

                    })

                })

            });
        
            var alertAttack = new Sequence(new BehaviorTree[] {
                new GlobalAlertQuery(),
                new Loop(new BehaviorTree[] {
                    new MoveToPlayer(agent.GetAction("attack").range),
                    new Attack()
                })
            });

            result = new Selector(new BehaviorTree[] {
                alertAttack,
                maintree
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


