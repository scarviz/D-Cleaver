using System;
using System.Collections;
using UnityEngine;

public class DragonAnimation : GameEvent
{
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

	private Vector3 mDefDragonPos;

	private AudioSource mAudioSrc;

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

		mDefDragonPos = mDragon.transform.position;

		mAudioSrc = mDragon.GetComponent<AudioSource>();
		mAudioSrc.clip = roarsClip;
	}

	void Update() {
		if (!IsExecEvent && !mAnime.isPlaying)
		{
			mAnime.clip = mIdleAnime;
			mAnime.Play();
		}
	}

	void OnTriggerEnter(Collider collider)
	{
		if (!collider.gameObject.CompareTag(weaponTagName)) return;

		if (IsExecEvent) return;

		if (--dragonHp > 0)
		{
			StartCoroutine(DelayPlayAnimation(mAnime, mBreathFireAnime, mAudioSrc));
			StartCoroutine(WaitWhilePlayingAnim(mAnime));
		}
		else
		{
			mAudioSrc.clip = deathClip;
			mAudioSrc.Play();

			IsExecEvent = true;
			mAnime.clip = mDeathAnime;
			mAnime.Play();
			StartCoroutine(DelayAction(mAnime, () =>
			{
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
				dragonPos.z += 1.85f;
				StartCoroutine(Move(mDragon.transform, dragonPos, 0.5f));

				StartCoroutine(DelayPlayAnimation(mAnime, mBreathFireAnime, mAudioSrc));

				StartCoroutine(WaitWhilePlayingAnim(mAnime));
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
