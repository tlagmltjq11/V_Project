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
    #region Enums
    //enums
    public enum eAudioClip
    {
        STEP1,
        STEP2,
        JUMP,
        LAND,
        Max
    }

    public enum eWeaponType
    {
        AR,
        PISTOL,
        KNIFE,
        MAX
    }
    #endregion
    #region References
    //References
    public Camera m_camera;
    public CharacterController m_charCon;
    public Animator m_animator;
    public Transform m_groundCheck;
    public GameObject[] m_weapons;
    public Weapon m_currentWeapon;
    public Text m_bulletText;
    public Text m_curWeaponText;
    public GameObject m_crossHair;
    public AudioClip[] m_audioClip;
    public AudioSource m_audioSource;
    public GameObject m_upperBodyLean;
    #endregion
    #region Player Info
    //Player Info
    public float m_runSpeed = 5f;
    public float m_walkSpeed = 1.5f;
    public float m_crouchSpeed = 2.5f;
    public float m_normalSpeed = 3.5f;
    public float m_crouchHeight = 0.475f;
    public float m_playerHeight = 1.075f;
    public float m_jumpHeight = 1.5f;
    #endregion
    #region About Player Move
    //Player Move
    public Vector3 m_moveDir = Vector3.zero;
    public float m_horiz = 0f;
    public float m_vert = 0f;
    public float m_diagonalSpeedModifier;
    public Vector3 m_crouchCheckPos;
    public Vector3 m_normalCheckPos;
    public Vector3 m_velocity;
    public float m_groundDistance = 0.2f;
    public float m_gravity = -21f;
    public LayerMask m_groundMask;
    public float m_footstepTimer;
    public float m_footstepCycle;
    public bool m_PreviouslyGrounded;
    public float m_gunMoveSpeed;//움직일때 총 좌우로 흔들리는 속도조절.
    #endregion
    #region State Check Vars
    //State Check Vars
    public bool m_isGrounded;
    public bool m_isCrouching;
    public bool m_isWalking;
    public bool m_isRunning;
    public bool m_crouchAccuracyApply;
    public bool m_jumpAccruacyApply;
    public bool m_isLeanQ;
    public bool m_isLeanE;
    public bool m_isLeanDouble;
    public eWeaponType m_WPType;
    #endregion
    #endregion

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(m_groundCheck.position, m_groundDistance);
    }

    #region Unity Methods
    void Start() 
    {
        Init();
        InitState(this, Player_MOVE.Instance); 
    }

    void Update()
    { 
        FSMUpdate();
        #region Weapon Handle
        if (m_currentWeapon != null)
        {
            // 총기 발사
            if(m_WPType == eWeaponType.AR)
            {
                if (Input.GetButton("Fire1"))
                {
                    if (m_currentWeapon.m_currentBullets > 0 && !m_currentWeapon.m_isReloading && !m_isRunning && !m_currentWeapon.m_isDrawing)
                    {
                        m_currentWeapon.m_isFiring = true;
                        m_currentWeapon.Fire();
                        m_bulletText.text = m_currentWeapon.m_currentBullets + " / " + m_currentWeapon.m_bulletsRemain;
                    }
                }

                if (Input.GetButtonUp("Fire1") && !m_isRunning)
                {
                    m_currentWeapon.m_isFiring = false;
                    m_currentWeapon.StopFiring();
                }
            }
            else if(m_WPType == eWeaponType.PISTOL)
            {
                if (Input.GetButtonDown("Fire1"))
                {
                    if (m_currentWeapon.m_currentBullets > 0 && !m_currentWeapon.m_isReloading && !m_isRunning && !m_currentWeapon.m_isDrawing)
                    {
                        m_currentWeapon.Fire();
                        m_bulletText.text = m_currentWeapon.m_currentBullets + " / " + m_currentWeapon.m_bulletsRemain;
                    }
                }
            }
            

            // 줌인 줌아웃
            if(Input.GetButtonDown("Fire2") && !m_currentWeapon.m_isReloading && !m_currentWeapon.m_isAiming && !m_isRunning && !m_currentWeapon.m_isDrawing)
            {
                m_currentWeapon.AimIn();
            }
            else if(Input.GetButtonDown("Fire2") && !m_currentWeapon.m_isReloading && m_currentWeapon.m_isAiming && !m_isRunning)
            {
                m_currentWeapon.AimOut();
            }

            // 재장전
            if(Input.GetKeyDown(KeyCode.R) && !m_currentWeapon.m_isReloading && !m_isRunning && !m_currentWeapon.m_isDrawing)
            {
                if(m_currentWeapon.m_isAiming && m_currentWeapon.m_currentBullets != m_currentWeapon.m_bulletsPerMag)
                {
                    m_currentWeapon.AimOut();

                    //기울이기를 취소시킴.
                    m_isLeanQ = false;
                    m_isLeanE = false;
                    m_isLeanDouble = true;
                }

                m_currentWeapon.Reload();
            }

            // 총기 스왑
            for (int i = 49; i < 58; i++)
            {
                if (Input.GetKeyDown((KeyCode)i) && m_weapons.Length > i - 49)
                {
                    if(m_WPType != (eWeaponType)(i-49))
                    {
                        SwitchWeapons(i - 49);
                    }
                }
            }

            // 총 발사 간격
            if (m_currentWeapon.m_fireTimer < m_currentWeapon.m_fireRate)
            {
                m_currentWeapon.m_fireTimer += Time.deltaTime;
            }
        }
        #endregion
    }
    #endregion

    #region Private Methods
    void Init()
    {
        m_charCon = GetComponent<CharacterController>();
        m_animator = GetComponent<Animator>();
        m_crouchCheckPos = m_groundCheck.localPosition + new Vector3(0f, 0.2f, 0f); //앉을때 그라운드체크 포지션이 바닥밑으로 깔리는것을 방지하기 위함.
        m_normalCheckPos = m_groundCheck.localPosition;
        m_isGrounded = false;
        m_isCrouching = false;
        m_isWalking = false;
        m_crouchAccuracyApply = false;
        m_jumpAccruacyApply = false;
        m_isLeanQ = false;
        m_isLeanE = false;
        m_isLeanDouble = false;
        // Default Weapon Setting.
        for (int i=0; i<m_weapons.Length; i++)
        {
            m_weapons[i].SetActive(false);
        }
        m_weapons[0].SetActive(true);
        m_WPType = eWeaponType.AR;
        SetCurrentWeapon(m_weapons[0].GetComponent<Weapon>());
        // Default Weapon Setting END
    }

    void SetCurrentWeapon(Weapon weapon)
    {
        m_currentWeapon = weapon;
        m_bulletText.text = weapon.m_currentBullets + " / " + weapon.m_bulletsRemain;
        m_curWeaponText.text = weapon.m_weaponName;
    }

    private void SwitchWeapons(int newWeapon)
    {
        for (int i = 0; i < m_weapons.Length; i++)
        {
            if (m_weapons[i].activeSelf)
            {
                m_weapons[i].SetActive(false);
                break;
            }
        }

        m_weapons[newWeapon].SetActive(true);
        m_WPType = (eWeaponType)newWeapon;
        SetCurrentWeapon(m_weapons[newWeapon].GetComponent<Weapon>());
    }
    #endregion
}