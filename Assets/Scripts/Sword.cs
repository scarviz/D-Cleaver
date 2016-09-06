using UnityEngine;
using System.Collections;
using System;

public class Sword : MonoBehaviour
{
	private const float GRAVITY = 9.8f;

	public float Weight = 3f;
	[SerializeField]
	private AudioClip swingClip;
	[SerializeField]
	private AudioClip hitClip;
	[SerializeField]
	private String enemyTagName;

	private GameObject mSword;
	private AudioSource mAudioSrc;
	private GameObject mSword2hands;

	// Use this for initialization
	void Start()
	{
		mSword = this.gameObject;
		mAudioSrc = mSword.GetComponent<AudioSource>();
		mAudioSrc.clip = swingClip;

		mSword2hands = mSword.transform.FindChild("sword_2hands").gameObject;
	}

	// Update is called once per frame
	void Update()
	{
		if (GameMng.VisibleTitle) return;

		if (GvrController.State != GvrConnectionState.Connected)
		{
			mSword2hands.SetActive(false);
			return;
		}

		mSword2hands.SetActive(true);
		mSword.transform.localRotation = GvrController.Orientation;

		if (mAudioSrc.isPlaying) return;
		float grav = GRAVITY * Weight;
		if (grav < Math.Abs(GvrController.Accel.x)
			|| grav < Math.Abs(GvrController.Accel.y)
			|| grav < Math.Abs(GvrController.Accel.z))
		{
			mAudioSrc.clip = swingClip;
			mAudioSrc.Play();
		}
	}

	void OnTriggerEnter(Collider collider)
	{
		if (!collider.gameObject.CompareTag(enemyTagName)) return;
		if (mAudioSrc.isPlaying) return;

		mAudioSrc.clip = hitClip;
		mAudioSrc.Play();
	}
}
