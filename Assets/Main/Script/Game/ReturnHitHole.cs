using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnHitHole : MonoBehaviour
{
    public bool m_ifFirstEnable = true;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);  
    }

    private void OnEnable()
    {
        if (!m_ifFirstEnable)
        {
            Invoke("ReturnToPool", 5f);
        }

        m_ifFirstEnable = false;
    }

    public void ReturnToPool()
    {
        gameObject.transform.SetParent(GameObject.Find("GunEffectObjPoolManager").transform);
        GunEffectObjPool.Instance.m_hitHoleObjPool.Set(this);
        gameObject.SetActive(false);
    }
}
