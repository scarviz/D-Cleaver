﻿using UnityEngine;
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

	private GameObject mSword;
	private AudioSource mAudioSrc;

	// Use this for initialization
	void Start ()
	{
		mSword = this.gameObject;
		mAudioSrc = mSword.GetComponent<AudioSource>();
		mAudioSrc.clip = swingClip;
	}

	// Update is called once per frame
	void Update()
	{
		if (GvrController.State != GvrConnectionState.Connected)
		{
			mSword.SetActive(false);
			return;
		}

		mSword.SetActive(true);
		mSword.transform.localRotation = GvrController.Orientation;

		if (mAudioSrc.isPlaying) return;
		float grav = GRAVITY * Weight;
		if (grav < Math.Abs(GvrController.Accel.x)
			|| grav < Math.Abs(GvrController.Accel.y)
			|| grav < Math.Abs(GvrController.Accel.z))
		{
			mAudioSrc.Play();
		}
	}
}
