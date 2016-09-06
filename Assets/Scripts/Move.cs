using UnityEngine;

public class Move : MonoBehaviour
{
	private const int MAX_X = 12;
	private const int MAX_Y = 12;

	[SerializeField]
	private GameMng gameMng;
	[SerializeField]
	private int startPositionIndex = 22;
	[SerializeField]
	private string submitButtonName = "Fire1";
	[SerializeField]
	private AudioClip footstepsClip;

	private int[] mMapData = new int[] {
		1,1,1,1,1,1,1,1,1,1,1,1,
		1,0,0,0,0,0,0,0,0,0,0,1,
		1,0,1,1,1,1,1,1,1,1,1,1,
		1,0,1,0,0,1,0,0,0,0,0,1,
		1,0,1,0,0,1,0,0,0,0,0,1,
		1,0,1,1,1,1,0,0,2,0,0,1,
		1,0,0,0,0,1,0,0,2,0,0,1,
		1,1,1,1,0,1,0,0,2,0,0,1,
		1,0,0,0,0,1,0,0,0,0,0,1,
		1,0,1,1,1,1,1,1,0,1,1,1,
		1,0,0,0,0,0,0,0,0,0,0,1,
		1,1,1,1,1,1,1,1,1,1,1,1,
	};

	private GameObject mPlayer;
	private GameObject mCamera;
	private int mPosIdx;
	private AudioSource mAudioSrc;

	// Use this for initialization
	void Start()
	{
		mPlayer = this.gameObject;
		mCamera = mPlayer.transform.FindChild("Camera").gameObject;
		mPosIdx = startPositionIndex;

		mAudioSrc = mPlayer.GetComponent<AudioSource>();
		mAudioSrc.clip = footstepsClip;
	}

	// Update is called once per frame
	void Update()
	{
		if(Input.GetButtonDown(submitButtonName) || GvrController.TouchUp)
		{
			Debug.Log("GetButtonDown");

			if (MovePosition())
			{
				ExecPositionEvent(mPosIdx);
			}
		}
	}

	private bool MovePosition()
	{
		// カメラが向いている方向(前方)の情報を取得する
		var forward = mCamera.transform.TransformDirection(Vector3.forward);

		// x方向に向いているか()
		int decX = 0;
		if (0.5 < forward.x) decX = 1;
		else if (forward.x < -0.5) decX = -1;

		int decZ = 0;
		if (0.5 < forward.z) decZ = 1;
		else if (forward.z < -0.5) decZ = -1;

		int nextIdx = mPosIdx + decX * -1 + MAX_Y * decZ;
		if (nextIdx < 0 || mMapData.Length < nextIdx || mMapData[nextIdx] != 0) return false;
		mPosIdx = nextIdx;

		var playerPos = mPlayer.transform.position;
		var vec = new Vector3(playerPos.x + 1f * decX, playerPos.y, playerPos.z + 1f * decZ);
		mPlayer.transform.position = vec;
		mAudioSrc.Play();

		return true;
	}

	private void ExecPositionEvent(int potisionIdx)
	{
		switch (potisionIdx)
		{
			case 104:
				gameMng.ExecEnemyEvent(DragonAnimation.EventId.Approach);
				break;
			case 116:
				gameMng.ExecEnemyEvent(DragonAnimation.EventId.Init);
				break;
		}
	}
}
