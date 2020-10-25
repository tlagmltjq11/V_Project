using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnCasing : MonoBehaviour
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
            Invoke("ReturnToPool", 1f);
        }

        m_ifFirstEnable = false;
    }
    #endregion

    #region Private Methods
    private void ReturnToPool()
    {
        gameObject.transform.SetParent(GunEffectObjPool.Instance.gameObject.transform);
        gameObject.GetComponent<Rigidbody>().isKinematic = true;

        GunEffectObjPool.Instance.m_casingPool.Set(this);
        gameObject.SetActive(false);
    }
    #endregion
}
