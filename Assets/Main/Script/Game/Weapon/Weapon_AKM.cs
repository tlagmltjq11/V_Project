﻿using System.Collections;
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
	}

    // Update is called once per frame
    void Update()
    {
        m_info = m_anim.GetCurrentAnimatorStateInfo(0);
        m_isReloading = m_info.IsName("RELOAD");

        if (m_isAiming)
        {
			if(m_isSightAttached)
            {
				transform.localPosition = Vector3.Lerp(transform.localPosition, m_dotSightPosition, Time.deltaTime * 8f);
				m_camera.fieldOfView = Mathf.Lerp(m_camera.fieldOfView, 40f, Time.deltaTime * 8f);
			}
			else
            {
				transform.localPosition = Vector3.Lerp(transform.localPosition, m_aimPosition, Time.deltaTime * 8f);
				m_camera.fieldOfView = Mathf.Lerp(m_camera.fieldOfView, 40f, Time.deltaTime * 8f);
			}
        }
        else
        {
			transform.localPosition = Vector3.Lerp(transform.localPosition, m_originalPosition, Time.deltaTime * 6f);
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
		m_originAccuracy = 0.015f;
		m_accuracy = m_originAccuracy;
		m_power = 25f;

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

		int layerMask = ((1 << LayerMask.NameToLayer("Player")) | (1 << LayerMask.NameToLayer("Sfx")));
		layerMask = ~layerMask;

		if (Physics.Raycast(m_shootPoint.position, m_shootPoint.transform.forward + Random.onUnitSphere * m_accuracy, out hit, m_range, layerMask))
		{
			var hitHole = GunEffectObjPool.Instance.m_hitHoleObjPool.Get();
			hitHole.gameObject.transform.position = hit.point;
			hitHole.gameObject.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
			hitHole.transform.SetParent(hit.transform); // 탄흔이 오브젝트를 따라가게끔 유도하기 위해 리턴되기 전까지만 부모로 지정
			hitHole.gameObject.SetActive(true);

			var hitSpark = GunEffectObjPool.Instance.m_hitSparkPool.Get();
			hitSpark.gameObject.transform.position = hit.point;
			hitSpark.gameObject.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
			hitSpark.gameObject.SetActive(true);

			if (hit.transform.gameObject.layer.Equals(LayerMask.NameToLayer("Interactable")))
            {
				Rigidbody rig = hit.transform.GetComponent<Rigidbody>();
				
				if(rig)
                {
					rig.AddForceAtPosition(m_shootPoint.forward * m_power * 5f, m_shootPoint.position);
				}					
            }

			HealthController enemy = hit.transform.GetComponent<HealthController>();

			if(enemy)
            {
				enemy.Damaged(m_power);
			}
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

		if (m_attachedSight.activeSelf)
		{
			m_isSightAttached = true;
		}
		else
		{
			m_isSightAttached = false;
		}

		m_isAiming = true;

		m_accuracy = m_accuracy / 4f;
	}

	public override void AimOut()
	{
		m_isAiming = false;
		m_anim.SetBool("ISAIM", false);

		if(m_stateManager.m_isCrouching)
        {
			m_accuracy = m_originAccuracy / 2f;
        }
		else if(!m_stateManager.m_isGrounded)
        {
			m_accuracy = m_originAccuracy * 5f;
        }
		else
        {
			m_accuracy = m_originAccuracy;
		}
	}

	public override void JumpAccuracy(bool j)
    {
		if(j)
        {
			m_accuracy = m_accuracy * 5f;
        }
		else
        {
			if(m_isAiming)
            {
				m_accuracy = m_originAccuracy / 4f;
            }
			else
            {
				m_accuracy = m_originAccuracy;
            }
        }
    }

	public override void CrouchAccuracy(bool c)
    {
		if (c)
        {
			m_accuracy = m_accuracy / 2f;
        }
		else
        {
			if (m_isAiming)
			{
				m_accuracy = m_originAccuracy / 4f;
			}
			else
			{
				m_accuracy = m_originAccuracy;
			}
		}
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
		Vector3 HorizonCamRecoil = new Vector3(0f, Random.Range(-m_recoiltHoriz, m_recoiltHoriz), 0f);
		Vector3 VerticalCamRecoil = new Vector3(-m_recoilVert, 0f, 0f);

		if (!m_isAiming)
        {
			Vector3 gunRecoil = new Vector3(Random.Range(-m_recoilKickBack.x, m_recoilKickBack.x), m_recoilKickBack.y, m_recoilKickBack.z);
			transform.localPosition = Vector3.Lerp(transform.localPosition, transform.localPosition + gunRecoil, m_recoilAmount);

			m_horizonCamRecoil.transform.localRotation = Quaternion.Slerp(m_horizonCamRecoil.transform.localRotation, Quaternion.Euler(m_horizonCamRecoil.transform.localEulerAngles + HorizonCamRecoil), m_recoilAmount);
			m_cameraRotate.VerticalCamRotate(-VerticalCamRecoil.x); //현재 이걸로 수직반동 올리는 중임.
		}
		else
        {
			Vector3 gunRecoil = new Vector3(Random.Range(-m_recoilKickBack.x, m_recoilKickBack.x) / 2f, 0, m_recoilKickBack.z);
			transform.localPosition = Vector3.Lerp(transform.localPosition, transform.localPosition + gunRecoil, m_recoilAmount);

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
		casing.gameObject.GetComponent<Rigidbody>().isKinematic = false;
		casing.gameObject.SetActive(true);
		casing.gameObject.GetComponent<Rigidbody>().AddRelativeForce(new Vector3(Random.Range(50f, 100f), Random.Range(50f, 100f), Random.Range(-10f, 20f)));
		casing.gameObject.GetComponent<Rigidbody>().MoveRotation(randomQuaternion.normalized);
	}
}