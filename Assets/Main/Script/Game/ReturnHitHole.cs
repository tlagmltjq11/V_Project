using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnHitHole : MonoBehaviour
{
    public bool m_temp = false;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);  
    }

    private void OnEnable()
    {
        if (m_temp)
        {
            Invoke("ReturnToPool", 5f);
        }

        m_temp = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void ReturnToPool()
    {
        GunEffectObjPool.Instance.m_hitHoleObjPool.Set(this);
        gameObject.SetActive(false);
    }
}
