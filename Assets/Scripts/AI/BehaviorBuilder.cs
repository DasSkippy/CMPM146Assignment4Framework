using UnityEngine;

public class NearestEnemyOfTypeQuery : BehaviorTree
{
    readonly string _type;
    public NearestEnemyOfTypeQuery(string type) { _type = type; }

    public override Result Run()
    {
        // ask GameManager for the overall closest enemy
        var go = GameManager.Instance.GetClosestEnemy(agent.transform.position);
        if (go == null) 
            return Result.FAILURE;

        // grab its controller and validate
        var e = go.GetComponent<EnemyController>();
        if (e == null || e.monster != _type) {
            return Result.FAILURE;
        }
            

        // bingo — set target and report success
        agent.target = e.transform;
        return Result.SUCCESS;
    }
}


// 2) pick the ally with lowest current HP
// public class LowestHpEnemyQuery : BehaviorTree
// {
//     public override Result Run()
//     {
//         var gos = GameManager.Instance.GetEnemiesInRange(
//             agent.transform.position,
//             Mathf.Infinity
//         );
//         EnemyController lowest = null;
//         float lowHp = float.MaxValue;

//         foreach (var go in gos)
//         {
//             var e = go.GetComponent<EnemyController>();
//             if (e == null || e.dead) 
//                 continue;

//             float h = e.hp;
//             if (h < lowHp)
//             {
//                 lowHp = h;
//                 lowest = e;
//             }
//         }

//         if (lowest != null)
//         {
//             agent.target = lowest.transform;
//             return Result.SUCCESS;
//         }
//         return Result.FAILURE;
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
    // private static Transform wp1first = GameObject.Find("safezone1first").transform;


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
                        // new NearbyEnemiesQuery(1,5f),
                        new Selector(new BehaviorTree[] {
                            new Sequence(new BehaviorTree[] {
                                new AbilityReadyQuery("permabuff"),
                                // new NearestEnemyOfTypeQuery("skeleton"),
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
                        // new GoTo(wp1first, 5f),
                        new GoTo(wp1, 2f)
                    }),

                    new Selector(new BehaviorTree[] {

                        new Sequence(new BehaviorTree[] {
                            new Dist2MinQuery(),
                            new GoTo(wp2, 2f)
                        }),

                        new Selector(new BehaviorTree[] {

                            new Sequence(new BehaviorTree[] {
                                new Dist3MinQuery(),
                                new GoTo(wp3, 2f)
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
                            // new NearestEnemyOfTypeQuery("skeleton"),
                            new Buff()
                    }),
                    new Sequence(new BehaviorTree[] {
                            new AbilityReadyQuery("permabuff"),
                            // new NearestEnemyOfTypeQuery("skeleton"),
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
                        // new GoTo(wp1first, 5f),
                        new GoTo(wp1, 2f)
                    }),

                    new Selector(new BehaviorTree[] {

                        new Sequence(new BehaviorTree[] {
                            new Dist2MinQuery(),
                            new GoTo(wp2, 2f)
                        }),

                        new Selector(new BehaviorTree[] {

                            new Sequence(new BehaviorTree[] {
                                new Dist3MinQuery(),
                                new GoTo(wp3, 2f)
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
        
        else if (agent.monster == "skeleton")
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
                        // new GoTo(wp1first, 5f),
                        new GoTo(wp1, 2f)
                    }),

                    new Selector(new BehaviorTree[] {

                        new Sequence(new BehaviorTree[] {
                            new Dist2MinQuery(),
                            new GoTo(wp2, 2f)
                        }),

                        new Selector(new BehaviorTree[] {

                            new Sequence(new BehaviorTree[] {
                                new Dist3MinQuery(),
                                new GoTo(wp3, 2f)
                            }),

                            new MoveToPlayer(agent.GetAction("attack").range)

                        })

                    })

                })

            });
        
            // var alertAttack = new Sequence(new BehaviorTree[] {
            //     new GlobalAlertQuery(),
            //     new Loop(new BehaviorTree[] {
            //         new Selector(new BehaviorTree[] {

            //             new Sequence(new BehaviorTree[] {

            //                 new PlayerInRangeQuery(5f),
            //                 new Loop(new BehaviorTree[] {
            //                     new MoveToPlayer(agent.GetAction("attack").range),
            //                     new Attack()
            //                 })

            //             }),

            //             new Sequence(new BehaviorTree[] {
            //                 new NearestEnemyOfTypeQuery("zombie"),
            //                 new GoTo(agent.target, 1f)
            //             })


            //         })

            //     })
            // });
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


