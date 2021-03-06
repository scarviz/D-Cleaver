﻿using System;
using System.Collections;
using UnityEngine;

public class DragonAnimation : GameEvent
{
	private const float DRAGON_STEP = 8f;

	[SerializeField]
	private AudioClip roarsClip;
	[SerializeField]
	private AudioClip deathClip;
	[SerializeField]
	private String weaponTagName;
	[SerializeField]
	private int dragonHp = 5;

	private GameObject mDragon;
	private Animation mAnime;
	private AnimationClip mIdleAnime;
	private AnimationClip mRunAnime;
	private AnimationClip mBreathFireAnime;
	private AnimationClip mFlyAttackAnime;
	private AnimationClip mDeathAnime;
	private AnimationClip mAttackAnime;
	private AnimationClip mHitAnime;
	private AnimationClip mStandAnime;

	private Vector3 mDefDragonPos;

	private AudioSource mAudioSrc;

	private System.Random mRand;

	private bool _isStartedBattle = false;
	private bool IsStartedBattle
	{
		get { lock (this) { return _isStartedBattle; } }
		set { lock (this) { _isStartedBattle = value; } }
	}

	private bool _isExecEvent = false;
	private bool IsExecEvent
	{
		get	{ lock (this) { return _isExecEvent; } }
		set { lock (this) { _isExecEvent = value; } }
	}

	public class EventId {
		public const int Init = 1;
		public const int Approach = 2;
	}

	void Start()
	{
		mDragon = this.gameObject;
		mAnime = mDragon.GetComponent<Animation>();
		mIdleAnime = mAnime.GetClip("idle");
		mRunAnime = mAnime.GetClip("run");
		mBreathFireAnime = mAnime.GetClip("breath fire");
		mFlyAttackAnime = mAnime.GetClip("fly attack");
		mDeathAnime = mAnime.GetClip("death");
		mAttackAnime = mAnime.GetClip("attack1");
		mHitAnime = mAnime.GetClip("hit2");
		mStandAnime = mAnime.GetClip("stand");

		mDefDragonPos = mDragon.transform.position;

		mAudioSrc = mDragon.GetComponent<AudioSource>();
		mAudioSrc.clip = roarsClip;

		mRand = new System.Random(DateTime.Now.Millisecond);
	}

	void Update() {
		if (IsStartedBattle && !IsExecEvent && !mAnime.isPlaying)
		{
			switch (mRand.Next(10)) {
				case 0:
				case 3:
					mAnime.clip = mStandAnime;
					break;
				case 1:
				case 4:
				case 5:
				case 8:
					mAnime.clip = mAttackAnime;
					break;
				case 2:
				case 6:
				case 7:
				case 9:
					mAnime.clip = mBreathFireAnime;
					break;
			}
			mAnime.Play();
		}
	}

	void OnTriggerEnter(Collider collider)
	{
		if (!collider.gameObject.CompareTag(weaponTagName)) return;

		if (!IsStartedBattle 
			|| IsExecEvent
			|| mAnime.clip == mStandAnime 
			|| mAnime.clip == mHitAnime) return;

		dragonHp--;
		if (dragonHp > 0)
		{
			StartCoroutine(DelayPlayAnimation(mAnime, mHitAnime, mAudioSrc));
			StartCoroutine(WaitWhilePlayingAnim(mAnime));
		}
		else
		{
			IsStartedBattle = false;
			IsExecEvent = true;

			mAudioSrc.clip = deathClip;
			mAudioSrc.Play();

			mAnime.clip = mDeathAnime;
			mAnime.Play();
			StartCoroutine(DelayAction(mAnime, () =>
			{
				GameMng.VisibleTitle = true;
				Destroy(mDragon);
			}));
		}
	}

	public override void Exec(int eventId)
	{
		switch (eventId)
		{
			case EventId.Init:
				mDragon.transform.position = mDefDragonPos;
				break;
			case EventId.Approach:
				mAnime.clip = mRunAnime;
				mAnime.Play();
				
				var dragonPos = mDefDragonPos;
				dragonPos.z += DRAGON_STEP;
				StartCoroutine(Move(mDragon.transform, dragonPos, 0.5f));

				StartCoroutine(DelayPlayAnimation(mAnime, mBreathFireAnime, mAudioSrc));

				StartCoroutine(WaitWhilePlayingAnim(mAnime));

				IsStartedBattle = true;
				break;
		}
	}

	private IEnumerator Move(Transform transform, Vector3 endPosition, float time)
	{
		var startTime = Time.timeSinceLevelLoad;
		var startPosition = transform.position;

		var arrived = false;
		while (!arrived)
		{
			var diff = Time.timeSinceLevelLoad - startTime;
			if (diff > time)
			{
				transform.position = endPosition;
				arrived = true;
				break;
			}

			var rate = diff / time;
			transform.position = Vector3.Lerp(startPosition, endPosition, rate);

			yield return new WaitForSeconds(0.01f);
		}
	}

	private IEnumerator DelayPlayAnimation(Animation animation, AnimationClip clip, AudioSource audioSrc)
	{
		while (animation.isPlaying) { yield return new WaitForSeconds(0.01f); }
		animation.clip = clip;
		animation.Play();
		if(audioSrc != null) audioSrc.Play();
	}

	private IEnumerator WaitWhilePlayingAnim(Animation animation)
	{
		while (animation.isPlaying) {
			IsExecEvent = true;
			yield return new WaitForSeconds(0.01f);
		}
		IsExecEvent = false;
	}

	private IEnumerator DelayAction(Animation animation, Action action)
	{
		while (animation.isPlaying) { yield return new WaitForSeconds(0.01f); }
		action();
	}

}
