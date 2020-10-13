using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthController : MonoBehaviour
{
    float m_hp;
    [SerializeField]
    public Text m_HpText;

    // Start is called before the first frame update
    void Start()
    {
        m_hp = 100f;
        m_HpText.text = m_hp.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.K))
        {
            Damaged(Random.Range(5, 20));
        }
    }

    public void Damaged(float dmg)
    {
        if (m_hp - dmg <= 0f)
        {
            m_hp = 0f;
            Debug.Log("You Died!");
        }
        else
        {
            m_hp = m_hp - dmg;
        }

        m_HpText.text = m_hp.ToString();
    }
}
