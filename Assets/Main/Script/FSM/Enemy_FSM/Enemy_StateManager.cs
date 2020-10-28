using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy_StateManager : FSM<Enemy_StateManager>
{
    #region Field
    public enum eAudioClip
    {
        STEP1,
        STEP2,
        FIRE,
        RELOAD,
        DEATH1,
        DEATH2,
        DEATH3,
        Max
    }

    public Animator m_anim;
    public GameObject m_player;
    public NavMeshAgent m_navAgent;
    public GameObject m_upperBody;
    public GameObject m_shootPoint;
    public AudioSource m_audioSource;
    public AudioClip[] m_audioClip;
    public LineRenderer m_lineRenderer;

    public float m_idleTime;
    public float m_dieTime;
    public float m_detectSight = 30f;
    public float m_attackSight = 15f;
    public int m_check;
    public float m_footstepTimer;
    public float m_footstepCycle;
    public float m_hp;
    #endregion

    #region Unity Methods
    void OnEnable()
    {
        Init();
        RagdollOnOff(true);
        InitState(this, Enemy_IDLE.Instance);
    }

    void OnDisable()
    {
        RagdollOnOff(true);
    }

    void Start()
    {

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

        m_lineRenderer = GetComponent<LineRenderer>();
        m_lineRenderer.startWidth = 0.01f;
        m_lineRenderer.endWidth = 0.01f;
        m_lineRenderer.startColor = Color.white;
        m_lineRenderer.endColor = Color.white;
        m_lineRenderer.enabled = false;
        
        m_idleTime = 0f;
        m_dieTime = 0f;
        m_check = 0;
        m_hp = 100f;

        m_player = GameObject.Find("Player");
        m_anim.Rebind();
    }
    #endregion

    #region Public Methods
    public bool SearchTarget()
    {
        var dir = (m_player.transform.position + Vector3.up * 0.4f) - (gameObject.transform.position + Vector3.up * 1.3f);

        m_check = 0;

        //Debug.DrawRay(gameObject.transform.position + Vector3.up * 1.3f, dir.normalized * m_detectSight, Color.red, 1000f);

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
            return true;
        }
        else
        {
            return false;
        }
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

    public void Fire()
    {
        RaycastHit hit;

        int layerMask = ((1 << LayerMask.NameToLayer("Enemy")) | (1 << LayerMask.NameToLayer("Sfx")));
        layerMask = ~layerMask;

        if (Physics.Raycast(m_shootPoint.transform.position, m_shootPoint.transform.forward + Random.onUnitSphere * 0.05f, out hit, 100f, layerMask))
        {
            m_lineRenderer.SetPosition(0, m_shootPoint.transform.position);
            m_lineRenderer.SetPosition(1, hit.point);
            StartCoroutine(ShowBulletLine());

            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                if(hit.collider.gameObject.name.Equals("Player"))
                {
                    Player_StateManager player = hit.collider.gameObject.GetComponent<Player_StateManager>();

                    if (player != null)
                    {
                        player.Damaged(5f);
                    }
                }
                else if(hit.collider.gameObject.name.Equals("UpperBodyLean"))
                {
                    Player_StateManager player = hit.collider.gameObject.GetComponentInParent<Player_StateManager>();

                    if (player != null)
                    {
                        player.Damaged(10f);
                    }
                }
            }
            else
            {
                var hitSpark = GunEffectObjPool.Instance.m_hitSparkPool.Get();
                hitSpark.gameObject.transform.position = hit.point;
                hitSpark.gameObject.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
                hitSpark.gameObject.SetActive(true);
            }
        }

        //muzzleFlash.Play();
        m_audioSource.PlayOneShot(m_audioClip[(int)eAudioClip.FIRE], 0.4f);
    }

    public void Damaged(float dmg)
    {
        if (m_hp <= 0)
        {
            return;
        }

        if (m_hp - dmg <= 0f)
        {
            m_hp = 0f;
            ChangeState(Enemy_DIE.Instance);
            m_audioSource.PlayOneShot(m_audioClip[Random.Range((int)eAudioClip.DEATH1, (int)eAudioClip.Max)], 2f);
        }
        else
        {
            m_hp = m_hp - dmg;
        }
    }

    public void RagdollOnOff(bool OnOff)
    {
        m_anim.enabled = OnOff;
        Rigidbody[] rigs = gameObject.GetComponentsInChildren<Rigidbody>();

        for (int i = 0; i < rigs.Length; i++)
        {
            rigs[i].isKinematic = OnOff;
        }
    }
    #endregion

    #region Coroutine
    IEnumerator ShowBulletLine()
    {
        m_lineRenderer.enabled = true;
        yield return new WaitForSeconds(0.1f);
        m_lineRenderer.enabled = false;
    }
    #endregion
}
