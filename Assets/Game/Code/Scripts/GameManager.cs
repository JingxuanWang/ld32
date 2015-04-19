using UnityEngine;
using System.Collections;

namespace Game.Core
{
	/// <summary>
	/// Game manager.
	/// </summary>
	public class GameManager : MonoBehaviour
	{
		public void GameStart()
		{
			Application.LoadLevel("Game");
		}
	}
}