﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnCasing : MonoBehaviour
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
            Invoke("ReturnToPool", 1f);
        }

        m_temp = true;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void ReturnToPool()
    {
        gameObject.transform.localPosition = new Vector3(-1f, -3.5f, 0f);
        gameObject.transform.localRotation = Quaternion.identity;
        gameObject.GetComponent<Rigidbody>().isKinematic = true;

        GunEffectObjPool.Instance.m_casingPool.Set(this);
        gameObject.SetActive(false);
    }
}
