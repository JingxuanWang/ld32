using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

namespace Game
{
	public class MessagePanel : MonoBehaviour
	{		
		[SerializeField]
		private Text message;

		private Action callback = null;

		public void Open(string message, Action callback)
		{
			this.message.text = message;
			this.callback = callback;

			this.gameObject.SetActive(true);
		}

		public void Close()
		{
			if (this.callback != null)
			{
				this.callback();
			}

			this.gameObject.SetActive(false);
		}
	}
}