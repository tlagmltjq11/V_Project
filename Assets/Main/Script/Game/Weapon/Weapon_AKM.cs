using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_AKM : Weapon
{
	public enum eAudioClip
	{
		FIRE,
		RELOAD,
		Max
	}

    void OnEnable()
    {
		//m_stateManager.SetCurrentWeapon(this);
    }

    void OnDisable()
    {
        
    }

    // Use this for initialization
    void Start()
	{
		m_currentBullets = m_bulletsPerMag;
		m_anim = GetComponent<Animator>();

		m_stateManager = m_player.GetComponent<Player_StateManager>();
		m_cameraRotate = m_camera.GetComponent<CameraRotate>();
		Init();

		/*
		m_hitSparkObjPool = new GameObjectPool<GameObject>(20, () =>
		{
			var obj = Instantiate(m_hitSparkPrefab) as GameObject;
			obj.SetActive(false);
			return obj;
		});

		m_casingObjPool = new GameObjectPool<GameObject>(40, () =>
		{
			var obj = Instantiate(m_casing) as GameObject;
			obj.SetActive(false);
			return obj;
		});
		*/
	}

    // Update is called once per frame
    void Update()
    {
        m_info = m_anim.GetCurrentAnimatorStateInfo(0);
        m_isReloading = m_info.IsName("RELOAD");

        if (m_isAiming)
        {
			transform.localPosition = Vector3.Lerp(transform.localPosition, m_aimPosition, Time.deltaTime * 8f);
            m_camera.fieldOfView = Mathf.Lerp(m_camera.fieldOfView, 40f, Time.deltaTime * 8f);
        }
        else
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, m_originalPosition, Time.deltaTime * 5f);
            m_camera.fieldOfView = Mathf.Lerp(m_camera.fieldOfView, 60f, Time.deltaTime * 8f);
        }

		RecoilBack();
	}

	public void Init()
	{
		// Weapon Specification
		m_weaponName = "AKM";
		m_bulletsPerMag = 30;
		m_bulletsRemain = 150;
		m_currentBullets = 30;
		m_range = 100f;
		m_fireRate = 0.1f;

		m_recoilAmount = 1f;
		m_recoilAim = 0.1f;
		m_recoilVert = 1.2f;
		m_recoiltHoriz = 0.4f;
		m_recoilKickBack = new Vector3(0.1f, 0.25f, -0.5f);

		m_isFiring = false;

		m_originalPosition = transform.localPosition;
	}

	public override void Fire()
	{
		if (m_fireTimer < m_fireRate)
		{
			return;
		}

		if(m_isFiring)
        {
			m_recoilVert += 0.15f;
			m_recoilVert = Mathf.Clamp(m_recoilVert, 1.2f, 3f);

			m_recoiltHoriz += 0.05f;
			m_recoiltHoriz = Mathf.Clamp(m_recoiltHoriz, 0.4f, 0.8f);
        }
		
		RaycastHit hit;
		if (Physics.Raycast(m_shootPoint.position, m_shootPoint.transform.forward, out hit, m_range, (-1) -  (1 << LayerMask.NameToLayer("Player"))))
		{
			//Debug.DrawRay(m_shootPoint.position, m_shootPoint.transform.forward, Color.green, 1f);

			var hitHole = GunEffectObjPool.Instance.m_hitHoleObjPool.Get();
			hitHole.gameObject.transform.position = hit.point;
			hitHole.gameObject.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
			hitHole.gameObject.SetActive(true);

			var hitSpark = GunEffectObjPool.Instance.m_hitSparkPool.Get();
			hitSpark.gameObject.transform.position = hit.point;
			hitSpark.gameObject.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
			hitSpark.gameObject.SetActive(true);

			/*
			var hitHole = m_hitHoleObjPool.Get();
			hitHole.transform.position = hit.point;
			hitHole.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
			hitHole.gameObject.SetActive(true);
			StartCoroutine(RemoveObjPool_Obj(m_hitHoleObjPool, hitHole, 5f));*/
		}
		m_currentBullets--;
		m_fireTimer = 0.0f;
		m_audioSource.PlayOneShot(m_audioClip[(int)eAudioClip.FIRE]);
		m_anim.CrossFadeInFixedTime("FIRE", 0.01f);

		muzzleFlash.Play();
		Recoil();
		CasingEffect();
	}

    public override void AimIn()
	{
		m_anim.SetBool("ISAIM", true);
		m_isAiming = true;
	}

	public override void AimOut()
	{
		m_isAiming = false;
		m_anim.SetBool("ISAIM", false);
	}

    public override void Reload()
    {
		if (m_currentBullets == m_bulletsPerMag || m_bulletsRemain == 0)
        {
			return;
        }

		m_audioSource.PlayOneShot(m_audioClip[(int)eAudioClip.RELOAD]);
		m_anim.CrossFadeInFixedTime("RELOAD", 0.01f);
	}

	public void ReloadComplete()
    {
		int temp = 0;

		temp = m_bulletsPerMag - m_currentBullets; //장전되어야 할 총알의 수

		if(temp >= m_bulletsRemain)
        {
			m_currentBullets += m_bulletsRemain;
			m_bulletsRemain = 0;
        }
		else
        {
			m_currentBullets += temp;
			m_bulletsRemain -= temp;
        }

		m_stateManager.m_bulletText.text = m_stateManager.m_currentWeapon.m_currentBullets + " / " + m_stateManager.m_currentWeapon.m_bulletsRemain;
	}

    public override void Recoil()
    {
		if(!m_isAiming)
        {
			Vector3 gunRecoil = new Vector3(Random.Range(-m_recoilKickBack.x, m_recoilKickBack.x), m_recoilKickBack.y, m_recoilKickBack.z);
			transform.localPosition = Vector3.Lerp(transform.localPosition, transform.localPosition + gunRecoil, m_recoilAmount);
		}
		else
        {
			Vector3 gunRecoil = new Vector3(Random.Range(-m_recoilKickBack.x, m_recoilKickBack.x) / 2f, 0, m_recoilKickBack.z);
			transform.localPosition = Vector3.Lerp(transform.localPosition, transform.localPosition + gunRecoil, m_recoilAmount);
		}

		Vector3 HorizonCamRecoil = new Vector3(0f, Random.Range(-m_recoiltHoriz, m_recoiltHoriz), 0f);
		Vector3 VerticalCamRecoil = new Vector3(-m_recoilVert, 0f, 0f);

		//m_verticalCamRecoil.transform.localRotation = Quaternion.Slerp(m_verticalCamRecoil.transform.localRotation, Quaternion.Euler(m_verticalCamRecoil.transform.localEulerAngles + VerticalCamRecoil), m_recoilAmount);
		//m_verticalCamRecoil.transform.localRotation = Quaternion.Slerp(m_verticalCamRecoil.transform.localRotation, Quaternion.Euler(m_verticalCamRecoil.transform.localEulerAngles - VerticalCamRecoil), m_recoilAmount / 2);

		if (!m_isAiming)
		{
			m_horizonCamRecoil.transform.localRotation = Quaternion.Slerp(m_horizonCamRecoil.transform.localRotation, Quaternion.Euler(m_horizonCamRecoil.transform.localEulerAngles + HorizonCamRecoil), m_recoilAmount);
			m_cameraRotate.VerticalCamRotate(-VerticalCamRecoil.x); //현재 이걸로 수직반동 올리는 중임.
		}
		else
        {
			m_horizonCamRecoil.transform.localRotation = Quaternion.Slerp(m_horizonCamRecoil.transform.localRotation, Quaternion.Euler(m_horizonCamRecoil.transform.localEulerAngles + HorizonCamRecoil / 1.5f), m_recoilAmount);
			m_cameraRotate.VerticalCamRotate(-VerticalCamRecoil.x / 2f); //현재 이걸로 수직반동 올리는 중임.
		}
	}

	public override void RecoilBack()
    {
		m_horizonCamRecoil.transform.localRotation = Quaternion.Slerp(m_horizonCamRecoil.transform.localRotation, Quaternion.Euler(0f, 0f, 0f), Time.deltaTime * 3f);
	}

	public override void StopFiring()
    {
		m_recoilVert = 1.2f;
		m_recoiltHoriz = 0.65f;
	}

    public override void CasingEffect()
    {
		Quaternion randomQuaternion = new Quaternion(Random.Range(0f, 360f), Random.Range(0f, 360f), Random.Range(0f, 360f), 1);
		var casing = GunEffectObjPool.Instance.m_casingPool.Get();
		//casing.gameObject.transform.localRotation = randomQuaternion;
		casing.gameObject.GetComponent<Rigidbody>().isKinematic = false;
		casing.gameObject.SetActive(true);
		casing.gameObject.GetComponent<Rigidbody>().AddRelativeForce(new Vector3(Random.Range(50f, 100f), Random.Range(50f, 100f), Random.Range(-10f, 20f)));
		casing.gameObject.GetComponent<Rigidbody>().MoveRotation(randomQuaternion.normalized);
		/*
		Quaternion randomQuaternion = new Quaternion(Random.Range(0, 360f), Random.Range(0, 360f), Random.Range(0, 360f), 1);
		var casing = m_casingObjPool.Get();
		casing.transform.parent = m_casingPoint;
		casing.transform.localRotation = randomQuaternion;
		casing.gameObject.SetActive(true);
		casing.GetComponent<Rigidbody>().AddRelativeForce(new Vector3(Random.Range(50f, 80f), Random.Range(50f, 80f), Random.Range(-20f, 20f)));
		StartCoroutine(RemoveObjPool_Obj(m_casingObjPool, casing, 1f));
		*/
	}

	IEnumerator RemoveObjPool_Obj(GameObjectPool<GameObject> objPool, GameObject obj, float time)
    {
		yield return new WaitForSecondsRealtime(time);

		obj.gameObject.SetActive(false);
		objPool.Set(obj);
	}
}