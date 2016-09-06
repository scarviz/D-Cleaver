using UnityEngine;

[System.Serializable]
public abstract class GameEvent : MonoBehaviour
{
	public virtual void Exec(int eventId) { }
}
