using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnHitSpark : MonoBehaviour
{
    public bool m_ifFirstEnable = true;

    #region Unity Methods
    void Start()
    {
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        if (!m_ifFirstEnable)
        {
            Invoke("ReturnToPool", 0.5f);
        }

        m_ifFirstEnable = false;
    }
    #endregion

    #region Private Methods
    private void ReturnToPool()
    {
        GunEffectObjPool.Instance.m_hitSparkPool.Set(this);
        gameObject.SetActive(false);
    }
    #endregion
}
