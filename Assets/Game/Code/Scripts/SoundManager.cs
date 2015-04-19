using UnityEngine;
using System.Collections;

using Common;

namespace Game
{
	public class SoundManager : Singleton<SoundManager>
	{
		AudioSource audioSource = null;

		private void Start()
		{
			this.audioSource = GetComponent<AudioSource>();
		}

		public static void PlaySEToggleMirror(int state)
		{
			Instance.audioSource.Stop();

			if (state == 1)
			{
				Instance.audioSource.clip = Resources.Load("se_c1") as AudioClip;
			}
			else if (state == 2)
			{
				Instance.audioSource.clip = Resources.Load("se_c2") as AudioClip;
			}
			else if (state == 3)
			{
				Instance.audioSource.clip = Resources.Load("se_c3") as AudioClip;
			}
			else if (state == 4)
			{
				Instance.audioSource.clip = Resources.Load("se_c4") as AudioClip;
			}
			else if (state == 0)
			{
				Instance.audioSource.clip = Resources.Load("se_c5") as AudioClip;
			}

			Instance.audioSource.Play();
		}

		public static void PlaySEMonsterExit()
		{
			
		}

		public static void PlaySEStageClear()
		{
			
		}
	}
}