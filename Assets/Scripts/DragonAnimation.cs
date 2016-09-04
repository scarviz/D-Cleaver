using System;
using System.Collections;
using UnityEngine;

public class DragonAnimation : MonoBehaviour
{
	[SerializeField]
	private GameObject dragon;
	[SerializeField]
	private AudioClip roarsClip;

	private Animation mAnime;
	private AnimationClip mRunAnime;
	private AnimationClip mBreathFireAnime;
	
	private Vector3 mDefDragonPos;

	private AudioSource mAudioSrc;

	public enum EventId {
		Init,
		Approach
	}

	void Start()
	{
		mAnime = dragon.GetComponent<Animation>();
		mRunAnime = mAnime.GetClip("run");
		mBreathFireAnime = mAnime.GetClip("breath fire");

		mDefDragonPos = dragon.transform.position;

		mAudioSrc = dragon.GetComponent<AudioSource>();
		mAudioSrc.clip = roarsClip;
	}

	public void Play(EventId id)
	{
		switch (id)
		{
			case EventId.Init:
				dragon.transform.position = mDefDragonPos;
				break;
			case EventId.Approach:
				mAnime.clip = mRunAnime;
				mAnime.Play();
				
				var dragonPos = mDefDragonPos;
				dragonPos.z += 1.85f;
				StartCoroutine(Move(dragon.transform, dragonPos, 0.5f));

				StartCoroutine(DelayPlayAnimation(mAnime, mBreathFireAnime, mAudioSrc));
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
		audioSrc.Play();
	}
}
