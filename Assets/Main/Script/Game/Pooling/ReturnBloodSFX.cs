﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnBloodSFX : MonoBehaviour
{
    public bool m_ifFirstEnable = true;

    #region Unity Methods
    private void OnEnable()
    {
        if (!m_ifFirstEnable)
        {
            Invoke("ReturnToPool", 0.2f);
        }

        m_ifFirstEnable = false;
    }

    void Start()
    {
        gameObject.SetActive(false);
    }
    #endregion

    #region Private Methods
    private void ReturnToPool()
    {
        GunEffectObjPool.Instance.m_bloodPool.Set(this);
        gameObject.SetActive(false);
    }
    #endregion
}
