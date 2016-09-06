using UnityEngine;

public class GameMng : MonoBehaviour {

	public GameEvent EnemyGameEvent;

	public void ExecEnemyEvent(int eventId)
	{
		if (EnemyGameEvent != null)
		{
			EnemyGameEvent.Exec(eventId);
		}
	}
}
