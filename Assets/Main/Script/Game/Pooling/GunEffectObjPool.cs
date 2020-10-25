using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunEffectObjPool : SingletonMonoBehaviour<GunEffectObjPool>
{
    #region Field
    #region References
    //References
    public GameObject m_hitHolePrefab;
    public GameObject m_hitSparkPrefab;
    public GameObject m_casingPrefab;
    #endregion
    #region About Pooling
    public GameObjectPool<ReturnHitHole> m_hitHoleObjPool;
    public GameObjectPool<ReturnHitSpark> m_hitSparkPool;
    public GameObjectPool<ReturnCasing> m_casingPool;
    #endregion
    #endregion

    #region Unity Methods
    protected override void OnStart()
    {
        //풀링 생성.
        m_hitSparkPool = new GameObjectPool<ReturnHitSpark>(30, () =>
        {
            var obj = Instantiate(m_hitSparkPrefab) as GameObject;
            obj.transform.SetParent(gameObject.transform);
            var script = obj.GetComponent<ReturnHitSpark>();

            return script;
        });

        m_hitHoleObjPool = new GameObjectPool<ReturnHitHole>(40, () =>
		{
			var obj = Instantiate(m_hitHolePrefab) as GameObject;
            obj.transform.SetParent(gameObject.transform);
            var script =  obj.GetComponent<ReturnHitHole>();

			return script;
		});

        m_casingPool = new GameObjectPool<ReturnCasing>(30, () =>
        {
            var obj = Instantiate(m_casingPrefab) as GameObject;
            obj.transform.parent = gameObject.transform;
            var script = obj.GetComponent<ReturnCasing>();

            return script;
        });
    }
    #endregion
}
