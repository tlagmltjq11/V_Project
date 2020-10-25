using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_ATTACK : FSMSingleton<Enemy_ATTACK>, IFSMState<Enemy_StateManager>
{
    //  상태 진입..
    public void Enter(Enemy_StateManager e)
    {
        e.m_anim.SetBool("ISATTACK", true);
        e.gameObject.transform.LookAt(e.m_player.transform.position);
    }

    //  상태 진행..
    public void Execute(Enemy_StateManager e)
    {
        if(e.SearchTarget())
        {
            if(e.canAttack())
            {
                Debug.Log("ATTACK");
            }
            else
            {
                e.ChangeState(Enemy_IDLE.Instance);
            }
        }
        else
        {
            e.ChangeState(Enemy_IDLE.Instance);
        }
    }

    //  상태 종료..
    public void Exit(Enemy_StateManager e)
    {
        e.m_anim.SetBool("ISATTACK", false);
    }
}
