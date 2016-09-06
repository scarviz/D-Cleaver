using UnityEngine;
using UnityEngine.UI;

public class GameMng : MonoBehaviour {

	public GameEvent EnemyGameEvent;
	public GameObject Title;
	
	public static bool VisibleTitle { get; set; }

	void Start()
	{
		VisibleTitle = false;
		Title.SetActive(false);
	}

	void Update()
	{
		if (VisibleTitle != Title.activeSelf) Title.SetActive(VisibleTitle);
	}

	public void ExecEnemyEvent(int eventId)
	{
		if (EnemyGameEvent != null)
		{
			EnemyGameEvent.Exec(eventId);
		}
	}


}
