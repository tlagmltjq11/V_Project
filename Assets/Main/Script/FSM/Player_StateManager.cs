using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_StateManager : FSM<Player_StateManager>
{
    //상태들의 변수를 매니저에 전부 때려박는 이유는 상태들이 싱글턴이기 때문에 전역으로 관리되는데, 거기서 변수를 관리해버리면
    //해당 상태를 사용하는 모든 유닛들의 변수값이 같이 바뀌게 되기 때문에, 각자 유닛들의 매니저에서 변수를 관리해야하는것임.
    //상태클래스는 오직 하는 일만!★★★
    #region Public Field
    public CharacterController m_charCon;
    public Vector3 m_moveDir = Vector3.zero;
    public Animator m_animator;
    public float m_runSpeed = 3f;
    public float m_walkSpeed = 1.5f;
    public float m_crouchSpeed = 1.5f;
    public float m_horiz = 0f;
    public float m_vert = 0f;
    public float m_diagonalSpeedModifier;

    [SerializeField]
    public Transform m_groundCheck;
    public Vector3 m_velocity;
    public float m_groundDistance = 0.2f;
    public float m_gravity = -13f;
    public float m_jumpHeight = 1.2f;
    public LayerMask m_groundMask;

    public bool m_isGrounded;
    public bool m_isCrouching;
    public bool m_isWalking;
    #endregion

    void Start() 
    {
        Init();
        InitState(this, Player_MOVE.Instance); 
    }

    void Update()
    { 
        FSMUpdate();
    }

    void Init()
    {
        m_charCon = GetComponent<CharacterController>();
        m_animator = GetComponent<Animator>();
        m_isGrounded = false;
        m_isCrouching = false;
        m_isWalking = false;
    }
}