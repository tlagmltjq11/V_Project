using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player_StateManager : FSM<Player_StateManager>
{
    //상태들의 변수를 매니저에 전부 때려박는 이유는 상태들이 싱글턴이기 때문에 전역으로 관리되는데, 거기서 변수를 관리해버리면
    //해당 상태를 사용하는 모든 유닛들의 변수값이 같이 바뀌게 되기 때문에, 각자 유닛들의 매니저에서 변수를 관리해야하는것임.
    //상태클래스는 오직 하는 일만!★★★
    #region Public Field
    public CharacterController m_charCon;
    public Vector3 m_moveDir = Vector3.zero;
    public Animator m_animator;
    public float m_runSpeed = 5f;
    public float m_walkSpeed = 1.5f;
    public float m_crouchSpeed = 2.5f;
    public float m_normalSpeed = 3.5f;
    public float m_horiz = 0f;
    public float m_vert = 0f;
    public float m_diagonalSpeedModifier;

    public float m_crouchHeight = 1.3f;
    public float m_playerHeight = 1.75f;

    [SerializeField]
    public Transform m_groundCheck;
    public Vector3 m_crouchCheckPos;
    public Vector3 m_normalCheckPos;
    public Vector3 m_velocity;
    public float m_groundDistance = 0.2f;
    public float m_gravity = -21f;
    public float m_jumpHeight = 1.5f;
    public LayerMask m_groundMask;

    public bool m_isGrounded;
    public bool m_isCrouching;
    public bool m_isWalking;
    public bool m_isRunning;
    public bool m_crouchAccuracyApply;
    public bool m_jumpAccruacyApply;

    public Weapon m_currentWeapon;

    [SerializeField]
    public Text m_bulletText;
    [SerializeField]
    public Text m_curWeaponText;
    [SerializeField]
    public GameObject m_crossHair;

    [SerializeField]
    public AudioClip[] m_audioClip;
    public AudioSource m_audioSource;

    public enum eAudioClip
    {
        STEP1,
        STEP2,
        JUMP,
        LAND,
        Max
    }

    public float m_footstepTimer;
    public float m_footstepCycle;
    public bool m_PreviouslyGrounded;

    //움직일때 총 좌우로 흔들리는것.
    public float m_gunMoveSpeed;

    #endregion
    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(m_groundCheck.position, m_groundDistance);
    }
    public void SetCurrentWeapon(Weapon weapon)
    {
        m_currentWeapon = weapon;
        m_bulletText.text = weapon.m_currentBullets + " / " + weapon.m_bulletsRemain;
        m_curWeaponText.text = weapon.m_weaponName;
    }

    void Start() 
    {
        Init();
        InitState(this, Player_MOVE.Instance); 
    }

    void Update()
    { 
        FSMUpdate();
        
        if (m_currentWeapon != null)
        {
            //Fire Attack에 관련된 부분들.
            if (Input.GetButton("Fire1"))
            {
                if (m_currentWeapon.m_currentBullets > 0 && !m_currentWeapon.m_isReloading && !m_isRunning)
                {
                    m_currentWeapon.m_isFiring = true;
                    m_currentWeapon.Fire();
                    m_bulletText.text = m_currentWeapon.m_currentBullets + " / " + m_currentWeapon.m_bulletsRemain;
                }
            }
           
            if(Input.GetButtonUp("Fire1") && !m_isRunning)
            {
                m_currentWeapon.m_isFiring = false;
                m_currentWeapon.StopFiring();
            }

            if(Input.GetButtonDown("Fire2") && !m_currentWeapon.m_isReloading && !m_currentWeapon.m_isAiming && !m_isRunning)
            {
                m_currentWeapon.AimIn();
                m_crossHair.SetActive(false);
            }
            else if(Input.GetButtonDown("Fire2") && !m_currentWeapon.m_isReloading && m_currentWeapon.m_isAiming && !m_isRunning)
            {
                m_currentWeapon.AimOut();
                m_crossHair.SetActive(true);
            }

            if(Input.GetKeyDown(KeyCode.R) && !m_currentWeapon.m_isReloading && !m_isRunning)
            {
                if(m_currentWeapon.m_isAiming && m_currentWeapon.m_currentBullets != m_currentWeapon.m_bulletsPerMag)
                {
                    m_currentWeapon.AimOut();
                }

                m_currentWeapon.Reload();
            }

            if (m_currentWeapon.m_fireTimer < m_currentWeapon.m_fireRate)
            {
                m_currentWeapon.m_fireTimer += Time.deltaTime;
            }
        }
    }

    void Init()
    {
        m_charCon = GetComponent<CharacterController>();
        m_animator = GetComponent<Animator>();
        m_isGrounded = false;
        m_isCrouching = false;
        m_isWalking = false;
        m_crouchCheckPos = m_groundCheck.localPosition + new Vector3(0f, 0.2f, 0f);
        m_normalCheckPos = m_groundCheck.localPosition;
        m_crouchAccuracyApply = false;
        m_jumpAccruacyApply = false;

        //나중에 없애야할 코드들
        m_currentWeapon = GameObject.FindGameObjectWithTag("Weapon").GetComponent<Weapon>();
        m_curWeaponText.text = m_currentWeapon.m_weaponName;
    }
}