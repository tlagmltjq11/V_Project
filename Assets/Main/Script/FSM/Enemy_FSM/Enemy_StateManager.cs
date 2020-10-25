using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy_StateManager : FSM<Enemy_StateManager>
{
    #region Field
    public Animator m_anim;
    public GameObject m_player;
    public NavMeshAgent m_navAgent;

    public float m_detectSight = 30f;
    public float m_attackSight = 15f;
    public int m_check = 0;
    public bool m_isPlayerDetected = false;
    #endregion

    #region Unity Methods
    void Start()
    {
        Init();
        InitState(this, Enemy_IDLE.Instance);
    }

    void Update()
    {
        FSMUpdate();
    }
    #endregion

    #region Private Methods
    void Init()
    {
        m_anim = GetComponent<Animator>();
        m_navAgent = GetComponent<NavMeshAgent>();
        m_player = GameObject.Find("Player");
    }
    #endregion

    #region Public Methods
    public bool SearchTarget()
    {
        var dir = (m_player.transform.position + Vector3.up * 0.4f) - (gameObject.transform.position + Vector3.up * 1.3f);

        m_check = 0;

        Debug.DrawRay(gameObject.transform.position + Vector3.up * 1.3f, dir.normalized * m_detectSight, Color.red, 1000f);

        RaycastHit m_hit;
        if (Physics.Raycast(gameObject.transform.position + Vector3.up * 1.3f, dir.normalized, out m_hit, m_detectSight, -1 & (~(1 << LayerMask.NameToLayer("Enemy")))))
        {
            if (m_hit.collider.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                m_check++;
            }
        }

        if (m_check == 1)
        {
            m_isPlayerDetected = true;
        }
        else
        {
            m_isPlayerDetected = false;
        }

        return m_isPlayerDetected;
    }

    public bool canAttack()
    {
        var distance = Vector3.Distance(gameObject.transform.position, m_player.transform.position);

        if (distance <= m_attackSight)
        {
            return true;
        }

        return false;
    }
    #endregion
}
