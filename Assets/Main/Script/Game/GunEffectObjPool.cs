using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunEffectObjPool : SingletonMonoBehaviour<GunEffectObjPool>
{
    public GameObjectPool<ReturnHitHole> m_hitHoleObjPool;
    public GameObjectPool<ReturnHitSpark> m_hitSparkPool;
    public GameObjectPool<ReturnCasing> m_casingPool;

    public GameObject m_hitHolePrefab;
    public GameObject m_hitSparkPrefab;
    public GameObject m_casingPrefab;

    public GameObject m_casingPoint;

    protected override void OnStart()
    {
        m_hitHoleObjPool = new GameObjectPool<ReturnHitHole>(40, () =>
		{
			var obj = Instantiate(m_hitHolePrefab) as GameObject;
            obj.transform.SetParent(gameObject.transform);
            var script =  obj.GetComponent<ReturnHitHole>();

			return script;
		});

        m_hitSparkPool = new GameObjectPool<ReturnHitSpark>(30, () =>
        {
            var obj = Instantiate(m_hitSparkPrefab) as GameObject;
            obj.transform.SetParent(gameObject.transform);
            var script = obj.GetComponent<ReturnHitSpark>();

            return script;
        });

        m_casingPool = new GameObjectPool<ReturnCasing>(30, () =>
        {
            var obj = Instantiate(m_casingPrefab) as GameObject;
            obj.transform.parent = m_casingPoint.transform;
            obj.transform.localPosition = new Vector3(-1f, -3.5f, 0f);
            obj.transform.localScale = new Vector3(25, 25, 25);
            obj.transform.localRotation = Quaternion.identity;

            var script = obj.GetComponent<ReturnCasing>();

            return script;
        });
    }

    private void Update()
    {
        
    }
}
