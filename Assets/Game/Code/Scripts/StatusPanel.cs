using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

using Common;

namespace Game
{
	public class StatusPanel : MonoBehaviour
	{
		[SerializeField]
		private Text turnLabel;

		[SerializeField]
		private GameObject icon1;

		[SerializeField]
		private GameObject icon2;

		[SerializeField]
		private GameObject icon3;

		[SerializeField]
		private GameObject icon4;

		[SerializeField]
		private GameObject icon5;

		private List<GameObject> iconList = new List<GameObject>();

		private int turn = 1;
		private int mirrorNum = 5;
		public int MirrorNum
		{
			get { return this.mirrorNum; }
		}

		public bool CanRequestMirror
		{
			get { return this.mirrorNum > 0; }
		}

		public void Init(int mirrorNum)
		{
			this.turn = 1;
			this.iconList.Clear();

			this.mirrorNum = mirrorNum;
			if (mirrorNum >= 1)
			{
				this.iconList.Add(this.icon1);
			}
			if (mirrorNum >= 2)
			{
				this.iconList.Add(this.icon2);
			}
			if (mirrorNum >= 3)
			{
				this.iconList.Add(this.icon3);
			}
			if (mirrorNum >= 4)
			{
				this.iconList.Add(this.icon4);
			}
			if (mirrorNum >= 5)
			{
				this.iconList.Add(this.icon5);
			}

			foreach (GameObject obj in this.iconList)
			{
				obj.SetActive(true);
			}

			UpdateTurnLabel();
		}

		public void UpdateTurnLabel()
		{
			this.turnLabel.text = "TURN " + this.turn.ToString();
			this.turn++;
		}



		public void RequestMirror()
		{
			if (this.mirrorNum > 0)
			{
				this.mirrorNum--;
				this.iconList[this.mirrorNum].SetActive(false);
			}
		}

		public void RevokeMirror()
		{
			this.iconList[this.mirrorNum].SetActive(true);
			this.mirrorNum++;
		}
	}
}