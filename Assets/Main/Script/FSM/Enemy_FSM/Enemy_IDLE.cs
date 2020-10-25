using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy_IDLE : FSMSingleton<Enemy_IDLE>, IFSMState<Enemy_StateManager>
{
    //  상태 진입..
    public void Enter(Enemy_StateManager e)
    {

    }

    //  상태 진행..
    public void Execute(Enemy_StateManager e)
    {
        if(e.SearchTarget())
        {
            var distance = Vector3.Distance(e.gameObject.transform.position, e.m_player.transform.position);

            if(distance <= e.m_attackSight)
            {
                e.ChangeState(Enemy_ATTACK.Instance);
            }
            else
            {
                e.ChangeState(Enemy_RUN.Instance);
            }
        }
        else
        {
            e.ChangeState(Enemy_PATROL.Instance);
        }
    }

    //  상태 종료..
    public void Exit(Enemy_StateManager e)
    {

    }
}
