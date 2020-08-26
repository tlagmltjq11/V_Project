using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_MOVE : FSMSingleton<Player_MOVE>, IFSMState<Player_StateManager>
{
    Player_StateManager _e; // invoke 함수를 위해서 캐싱해둔 변수.

    public void Enter(Player_StateManager e)
    {
        _e = e;
    }

    public void Execute(Player_StateManager e)
    {
        #region Movement
        //앞이나 뒤로가는 키가 눌렸으면 1 또는 -1일것임. 이 값을 현재 플레이어의 forward에 곱해준 값과 right도 마찬가지로 왼쪽 오른쪽 값을 곱해줘서 두 벡터3를 합해주면
        //어느 방향으로 나아가야할지 정해지게 되는 것.
        e.m_horiz = Input.GetAxis("Horizontal");
        e.m_vert = Input.GetAxis("Vertical");
        e.m_isWalking = Input.GetKey(KeyCode.LeftShift);
        e.m_isCrouching = Input.GetKey(KeyCode.LeftControl);

        if (e.m_isCrouching)
        {
            e.m_animator.SetBool("ISCROUCH", true);
            //높이낮춰줘야함.************
        }
        else //서있는 상태
        {
            e.m_animator.SetBool("ISCROUCH", false);
        }

        //대각선방향으로 갈때 루트값으로 인해 더 빠른것을 방지하기 위함. 그냥 m_moveDir을 정규화해도되지만, 밀려나는 현상이 생겨서 이걸로 바꿈. <밀려나는 현상은 GetAxisRaw로도 해결할 수 있음>
        if (e.m_horiz != 0 && e.m_vert != 0)
        {
            e.m_diagonalSpeedModifier = 0.714f;
        }
        else
        {
            e.m_diagonalSpeedModifier = 1f;
        }

        Vector3 moveDirSide = e.transform.right * e.m_horiz * e.m_diagonalSpeedModifier;
        Vector3 moveDirForward = e.transform.forward * e.m_vert * e.m_diagonalSpeedModifier;

        e.m_moveDir = moveDirSide + moveDirForward;

        if (e.m_moveDir != Vector3.zero)
        {
            if (e.m_isWalking)
            {
                //m_animator.SetBool("ISWALK", true);
                e.m_charCon.Move(e.m_moveDir * e.m_walkSpeed * Time.deltaTime);
            }
            else if (e.m_isCrouching)
            {
                //m_animator.SetBool("ISCROUCHMOVE", true);
                e.m_charCon.Move(e.m_moveDir * e.m_crouchSpeed * Time.deltaTime);
            }
            else
            {
                e.m_animator.SetBool("ISRUN", true);
                e.m_charCon.Move(e.m_moveDir * e.m_runSpeed * Time.deltaTime);
            }
        }
        else
        {
            e.m_animator.SetBool("ISRUN", false);
        }
        #endregion

        #region Jump
        //CheckSphere(위치, 반지름, 어떤 레이어와의 충돌을 감지? -> 즉 특정 위치에 반지름을 받아 원을 생성하고 그 원내에 해당 레이어와의 충돌이 존재한다면 True를 반환함
        //고로 발밑에 m_groundCheck를 두고, 0.4f 반지름만큼 원을 생성하고 바닥레이어와 충돌 시 바닥에 닿았다는 것을 처리하게됨.
        e.m_isGrounded = Physics.CheckSphere(e.m_groundCheck.position, e.m_groundDistance, e.m_groundMask);
        
        if (!e.m_isGrounded && e.m_velocity.y < 0f)
        {
            e.m_animator.SetInteger("JUMPSTATE", 2);
        }

        if (e.m_isGrounded && e.m_velocity.y < 0f)
        {
            e.m_velocity.y = -2f;
            e.m_animator.SetInteger("JUMPSTATE", 0);
        }

        if (Input.GetKeyDown(KeyCode.Space) && e.m_isGrounded)
        {
            e.m_velocity.y = Mathf.Sqrt(e.m_jumpHeight * -2f * e.m_gravity);
            e.m_animator.SetTrigger("ISJUMP");
            e.m_animator.SetInteger("JUMPSTATE", 1);
            Invoke("fall", 0.533f);
        }

        e.m_velocity.y += e.m_gravity * Time.deltaTime;
        
        e.m_charCon.Move(e.m_velocity * Time.deltaTime);
        #endregion
    }

    public void Exit(Player_StateManager e)
    {
        
    }

    void fall()
    {
        _e.m_animator.SetTrigger("ISJUMP");
        _e.m_animator.SetInteger("JUMPSTATE", 2);
    }
}
