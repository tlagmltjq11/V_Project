using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_RUN : FSMSingleton<Enemy_RUN>, IFSMState<Enemy_StateManager>
{
    //  상태 진입..
    public void Enter(Enemy_StateManager e)
    {
        e.m_anim.SetBool("ISRUN", true);
        e.m_navAgent.speed = 2.5f;
        e.m_navAgent.stoppingDistance = e.m_attackSight;
    }

    //  상태 진행..
    public void Execute(Enemy_StateManager e)
    {
        e.m_navAgent.SetDestination(e.m_player.transform.position);

        if(e.canAttack())
        {
            e.ChangeState(Enemy_IDLE.Instance);
        }
    }

    //  상태 종료..
    public void Exit(Enemy_StateManager e)
    {
        e.m_anim.SetBool("ISRUN", false);
        e.m_navAgent.ResetPath();
    }
}
