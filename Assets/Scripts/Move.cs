using UnityEngine;

public class Move : MonoBehaviour
{
	private const int MAX_X = 12;
	private const int MAX_Y = 12;

	private int[] _mapData = null;
	private int[] MapData
	{
		get
		{
			if (_mapData != null) return _mapData;
			
			_mapData = new int[] {
				1,1,1,1,1,1,1,1,1,1,1,1,
				1,0,0,0,0,0,0,0,0,0,0,1,
				1,0,1,1,1,1,1,1,1,1,1,1,
				1,0,1,0,0,1,0,0,0,0,0,1,
				1,0,1,0,0,1,0,0,0,0,0,1,
				1,0,1,1,1,1,0,0,0,0,0,1,
				1,0,0,0,0,1,0,0,0,0,0,1,
				1,1,1,1,0,1,0,0,0,0,0,1,
				1,0,0,0,0,1,0,0,0,0,0,1,
				1,0,1,1,1,1,1,1,0,1,1,1,
				1,0,0,0,0,0,0,0,0,0,0,1,
				1,1,1,1,1,1,1,1,1,1,1,1,
			};
			
			return _mapData;
		}
	}
	
	private GameObject Player { get; set; }
	private GameObject Camera { get; set; }
	private int PosIdx { get; set; }
	private DragonAnimation DragonAnime { get; set; }

	[SerializeField]
	private GameObject gameEvent;
	[SerializeField]
	private int startPositionIndex = 22;
	[SerializeField]
	private string submitButtonName = "Fire1";

	// Use this for initialization
	void Start()
	{
		Player = this.gameObject;
		Camera = Player.transform.FindChild("Camera").gameObject;
		PosIdx = startPositionIndex;
		DragonAnime = gameEvent.GetComponent<DragonAnimation>();
	}

	// Update is called once per frame
	void Update()
	{
		if(Input.GetButtonDown(submitButtonName))
		{
			Debug.Log("GetButtonDown");
			
			// カメラが向いている方向(前方)の情報を取得する
			var forward = Camera.transform.TransformDirection(Vector3.forward);

			// x方向に向いているか()
			int decX = 0;
			if (0.5 < forward.x) decX = 1;
			else if (forward.x < -0.5) decX = -1;

			int decZ = 0;
			if (0.5 < forward.z) decZ = 1;
			else if (forward.z < -0.5) decZ = -1;

			int nextIdx = PosIdx + decX * -1 + MAX_Y * decZ;
			if (nextIdx < 0 || MapData.Length < nextIdx || MapData[nextIdx] == 1) return;
			PosIdx = nextIdx;

			var playerPos = Player.transform.position;
			var vec = new Vector3(playerPos.x + 1f * decX, playerPos.y, playerPos.z + 1f * decZ);
			Player.transform.position = vec;

			if (PosIdx == 104) { DragonAnime.Play(DragonAnimation.EventId.Approach); }
			else if (PosIdx == 116) { DragonAnime.Play(DragonAnimation.EventId.Init); }

		}
	}
}
