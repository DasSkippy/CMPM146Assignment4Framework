using UnityEngine;

public class LessThanNEnemies : BehaviorTree
{
    readonly float _n;
    public LessThanNEnemies(float n) {
        _n = n;
    }

    public override Result Run()
    {
        var enemies = GameManager.Instance.GetEnemiesInRange(agent.transform.position, Mathf.Infinity);

        if (enemies.Count < _n)
            return Result.SUCCESS;
        return Result.FAILURE;
    }
}

public class MoreThanNEnemies : BehaviorTree
{
    readonly float _n;
    public MoreThanNEnemies(float n) {
        _n = n;
    }

    public override Result Run()
    {
        var enemies = GameManager.Instance.GetEnemiesInRange(agent.transform.position, Mathf.Infinity);

        if (enemies.Count > _n)
            return Result.SUCCESS;
        return Result.FAILURE;
    }
}

public static class EnemyAlert
{
    public static bool IsAlerted { get; private set; }
    public static void RaiseAlert() => IsAlerted = true;
    public static void ClearAlert() => IsAlerted = false;
}

public class RaiseAlertNode : BehaviorTree
{
    public override Result Run()
    {
        EnemyAlert.RaiseAlert();
        return Result.SUCCESS;
    }
}

public class ClearAlertNode : BehaviorTree
{
    public override Result Run()
    {
        EnemyAlert.ClearAlert();
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
}

public static class SkeletonAlert
{
    public static bool IsAlerted2 { get; private set; }
    public static void RaiseAlert() => IsAlerted2 = true;
    public static void ClearAlert() => IsAlerted2 = false;
}

public class RaiseSkeletonAlertNode : BehaviorTree
{
    public override Result Run()
    {
        SkeletonAlert.RaiseAlert();
        return Result.SUCCESS;
    }
}

public class GlobalSkeletonAlertQuery : BehaviorTree
{
    public override Result Run()
    {
        return SkeletonAlert.IsAlerted2
            ? Result.SUCCESS 
            : Result.FAILURE;
    }
}

public class BehaviorBuilder
{
    private static Transform wp1 = GameObject.Find("safezone 1").transform;
    private static Transform wp2 = GameObject.Find("safezone 2").transform;
    private static Transform wp3 = GameObject.Find("safezone 3").transform;
    private static Transform wp4 = GameObject.Find("wp1").transform;


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

                        new PlayerInRangeQuery(5f), // try and change 15 to something like: GameManager.Instance.player....
                        new MoveToPlayer(agent.GetAction("attack").range),
                        new Attack()

                    }),

                    new Sequence(new BehaviorTree[] {
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
                        new MoreThanNEnemies(10),
                        new NearbyEnemiesQuery(6, 10f),
                        new RaiseAlertNode(),
                        new MoveToPlayer(agent.GetAction("attack").range),
                        new Attack()

                    })
                    
                }),

                // avoid player
                new Selector(new BehaviorTree[] {

                    new Sequence(new BehaviorTree[] {
                        new Dist1MinQuery(),
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
                    new MoveToPlayer(5f),
                    
                    new Sequence(new BehaviorTree[] {
                            new PlayerInRangeQuery(7f),
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

                        new MoreThanNEnemies(10),
                        new NearbyEnemiesQuery(6, 10f),
                        new RaiseAlertNode(),
                        new MoveToPlayer(agent.GetAction("attack").range),
                        new Attack()

                    })
                    
                }),

                // avoid player
                new Selector(new BehaviorTree[] {

                    new Sequence(new BehaviorTree[] {
                        new Dist1MinQuery(),
                        new GoTo(wp1, 4f)
                    }),

                    new Selector(new BehaviorTree[] {

                        new Sequence(new BehaviorTree[] {
                            new Dist2MinQuery(),
                            new GoTo(wp2, 4f)
                        }),

                        new Selector(new BehaviorTree[] {

                            new Sequence(new BehaviorTree[] {
                                new Dist3MinQuery(),
                                new GoTo(wp3, 4f)
                            }),

                            new MoveToPlayer(agent.GetAction("attack").range)

                        })

                    })

                })

            });
            
            var alertAttack = new Sequence(new BehaviorTree[] {
                new GlobalAlertQuery(),
                new Loop(new BehaviorTree[] {
                    new Sequence(new BehaviorTree[] {
                        new LessThanNEnemies(3),
                        new ClearAlertNode()
                    }),


                    new MoveToPlayer(agent.GetAction("attack").range),
                    new RaiseSkeletonAlertNode(),
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

                        new MoreThanNEnemies(10),
                        new NearbyEnemiesQuery(6, 10f),
                        new RaiseAlertNode(),
                        new MoveToPlayer(agent.GetAction("attack").range),
                        new Attack()

                    })
                    
                }),

                // avoid player
                new Selector(new BehaviorTree[] {

                    new Sequence(new BehaviorTree[] {
                        new Dist1MinQuery(),
                        new GoTo(wp1, 4f)
                    }),

                    new Selector(new BehaviorTree[] {

                        new Sequence(new BehaviorTree[] {
                            new Dist2MinQuery(),
                            new GoTo(wp2, 4f)
                        }),

                        new Selector(new BehaviorTree[] {

                            new Sequence(new BehaviorTree[] {
                                new Dist3MinQuery(),
                                new GoTo(wp3, 4f)
                            }),

                            new MoveToPlayer(agent.GetAction("attack").range)

                        })

                    })

                })

            });
        
            var alertAttack = new Sequence(new BehaviorTree[] {
                new GlobalAlertQuery(),
                new Loop(new BehaviorTree[] {
                    new Selector(new BehaviorTree[] {

                        new Sequence(new BehaviorTree[] {
                            
                            new GlobalSkeletonAlertQuery(),
                            new Loop(new BehaviorTree[] {
                                new MoveToPlayer(agent.GetAction("attack").range),
                                new Attack()
                            })

                        }),

                        new Sequence(new BehaviorTree[] {

                            new GoTo(wp4, 2f),
                            new PlayerInRangeQuery(5f),
                            new Loop(new BehaviorTree[] {
                                new MoveToPlayer(agent.GetAction("attack").range),
                                new Attack()
                            })

                        })


                    })

                })
            });
            // var alertAttack = new Sequence(new BehaviorTree[] {
            //     new GlobalAlertQuery(),
            //     new Loop(new BehaviorTree[] {
            //         new MoveToPlayer(agent.GetAction("attack").range),
            //         new Attack()
            //     })
            // });

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


