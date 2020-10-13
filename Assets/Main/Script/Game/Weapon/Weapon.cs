using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Weapon : MonoBehaviour
{
	[SerializeField]
	public GameObject m_player;
	public Player_StateManager m_stateManager;

	// Weapon Specification
	public string m_weaponName;
	public int m_bulletsPerMag;
	public int m_bulletsRemain;
	public int m_currentBullets;
	public float m_range;
	public float m_fireRate;

	// Parameters
	public float m_fireTimer;

	// References
	[SerializeField]
	public Transform m_shootPoint;
	public Animator m_anim;
	public ParticleSystem muzzleFlash;

	[SerializeField]
	public AudioClip[] m_audioClip;
	public AudioSource m_audioSource;

	public bool m_isReloading;
	public bool m_isAiming;
	public bool m_isAimOutOver;
	public AnimatorStateInfo m_info;

	//aim 만들때 사용
	public Vector3 m_aimPosition;
	public Vector3 m_dotSightPosition;
	public Vector3 m_originalPosition;
	public Camera m_camera;

	//반동 만들때 사용
	public Vector3 m_recoilKickBack;
	public float m_recoilAmount;
	public float m_recoilAim;
	public float m_recoilVert;
	public float m_recoiltHoriz;
	public CameraRotate m_cameraRotate;
	public GameObject m_horizonCamRecoil;
	public GameObject m_verticalCamRecoil;
	public bool m_isFiring;

	//탄피
	public Transform m_casingPoint;

	public float m_originAccuracy;
	public float m_accuracy;
	public float m_power;
	public float m_beforeAccuracy;

	//레드도트 및 사이트
	public bool m_isSightAttached;
	public GameObject m_attachedSight;

	public abstract void Fire();
	public abstract void Reload();
	public abstract void AimIn();
	public abstract void AimOut();
	public abstract void Recoil();
	public abstract void RecoilBack();
	public abstract void StopFiring();
	public abstract void CasingEffect();
	public abstract void JumpAccuracy(bool j);
	public abstract void CrouchAccuracy(bool c);

}