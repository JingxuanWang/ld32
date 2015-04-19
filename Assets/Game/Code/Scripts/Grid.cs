using UnityEngine;
using System.Collections;

namespace Game
{
	public enum Direction
	{
		UP,
		RIGHT,
		DOWN,
		LEFT
	}

	enum GridStatus
	{
		NOTHING,
		MONSTER,
		HOME,
		MIRROR_LEFT_UP,
		MIRROR_LEFT_DOWN,
		MIRROR_RIGHT_UP,
		MIRROR_RIGHT_DOWN
	}
		
	enum LightStatus
	{
		NONE,
		LIGHT
	}

	public class Grid : MonoBehaviour
	{
		private GridStatus status;
		private LightStatus lightStatus;

		private GameObject mirror;
		private GameObject mirrorLight;
		private GameObject straightLight;

		public bool HasNothing { get { return this.status == GridStatus.NOTHING; }}
		public bool HasMonster { get { return this.status == GridStatus.MONSTER; }}
		public bool HasHome { get { return this.status == GridStatus.HOME; }}

		public bool HasLight
		{
			get
			{
				return this.lightStatus != LightStatus.NONE;
			}
		}

		public bool HasMirror
		{
			get
			{
				return this.status == GridStatus.MIRROR_LEFT_DOWN
				|| this.status == GridStatus.MIRROR_LEFT_UP
				|| this.status == GridStatus.MIRROR_RIGHT_DOWN
				|| this.status == GridStatus.MIRROR_RIGHT_UP;
			}
		}

		public bool CanMove { get { return !this.HasMirror && !this.HasMonster && !this.HasLight; }}

		/// <summary>
		/// Adds the monster.
		/// </summary>
		public void AddMonster()
		{
			this.status = GridStatus.MONSTER;
		}

		/// <summary>
		/// Removes the monster.
		/// </summary>
		public void RemoveMonster()
		{
			if (this.status == GridStatus.MONSTER)
			{
				this.status = GridStatus.NOTHING;
			}
		}

		/// <summary>
		/// Adds the home.
		/// </summary>
		public void AddHome()
		{
			this.status = GridStatus.HOME;	
		}

		/// <summary>
		/// Adds the light.
		/// </summary>
		/// <param name="horizontal">If set to <c>true</c> horizontal.</param>
		public void AddLight(bool horizontal)
		{
			this.straightLight = SpawnLight();

			if (horizontal)
			{
				this.straightLight.transform.Rotate(0, 0, 90);
			}

			this.lightStatus = LightStatus.LIGHT;
		}

		/// <summary>
		/// Removes the light.
		/// </summary>
		public void RemoveLight()
		{
			if (this.straightLight != null)
			{				
				Destroy(this.straightLight);
			}
			if (this.mirrorLight != null)
			{
				this.mirrorLight.SetActive(false);
			}

			// Destroy other light objects
			for (int i = 0; i < transform.childCount; i++)
			{
				GameObject obj = transform.GetChild(i).gameObject;
				if (obj.name == "Light(Clone)")
				{
					Destroy(obj);
				}
			}

			this.lightStatus = LightStatus.NONE;
		}

		public bool CanReflect(Direction inLight)
		{
			if (this.HasMirror)
			{
				if (this.status == GridStatus.MIRROR_LEFT_DOWN)
				{
					if (inLight == Direction.RIGHT)
					{
						return true;
					}
					if (inLight == Direction.UP)
					{
						return true;
					}
				}
				else if (this.status == GridStatus.MIRROR_RIGHT_DOWN)
				{
					if (inLight == Direction.LEFT)
					{
						return true;
					}
					if (inLight == Direction.UP)
					{
						return true;
					}
				}
				else if (this.status == GridStatus.MIRROR_LEFT_UP)
				{
					if (inLight == Direction.RIGHT)
					{
						return true;
					}
					if (inLight == Direction.DOWN)
					{
						return true;
					}
				}
				else if (this.status == GridStatus.MIRROR_RIGHT_UP)
				{
					if (inLight == Direction.LEFT)
					{
						return true;
					}
					if (inLight == Direction.DOWN)
					{
						return true;
					}
				}
			}

			return false;
		}

		public Direction Reflect(Direction inLight)
		{
			if (this.HasMirror)
			{
				this.mirrorLight.SetActive(true);
				this.lightStatus = LightStatus.LIGHT;

				if (this.status == GridStatus.MIRROR_LEFT_DOWN)
				{
					if (inLight == Direction.RIGHT)
					{
						return Direction.DOWN;
					}
					if (inLight == Direction.UP)
					{
						return Direction.LEFT;
					}
				}
				else if (this.status == GridStatus.MIRROR_RIGHT_DOWN)
				{
					if (inLight == Direction.LEFT)
					{
						return Direction.DOWN;
					}
					if (inLight == Direction.UP)
					{
						return Direction.RIGHT;
					}
				}
				else if (this.status == GridStatus.MIRROR_LEFT_UP)
				{
					if (inLight == Direction.RIGHT)
					{
						return Direction.UP;
					}
					if (inLight == Direction.DOWN)
					{
						return Direction.LEFT;
					}
				}
				else if (this.status == GridStatus.MIRROR_RIGHT_UP)
				{
					if (inLight == Direction.LEFT)
					{
						return Direction.UP;
					}
					if (inLight == Direction.DOWN)
					{
						return Direction.RIGHT;
					}
				}
			}
			else
			{
				throw new UnityException("Can not reflect, no mirror");
			}

			return Direction.UP;
		}

		/// <summary>
		/// Raises the grid clicked event.
		/// </summary>
		public void OnGridClicked()
		{
			if (!StageManager.IsReady)
			{
				return;
			}

			if (this.HasMirror || this.HasNothing)
			{
				this.ToggleMirror();
			}
			else
			{
				// do nothing
			}

			StageManager.Instance.UpdateLight();
		}

		/// <summary>
		/// Toggles the mirror.
		/// </summary>
		private void ToggleMirror()
		{			
			if (this.status == GridStatus.NOTHING && StageManager.StatusPanel.CanRequestMirror)
			{
				if (this.mirror == null)
				{
					this.mirror = this.SpawnMirror();
				}
				else
				{
					this.mirror.transform.Rotate(new Vector3(0, 0, -90));
					this.mirror.SetActive(true);
				}

				StageManager.StatusPanel.RequestMirror();
				this.status = GridStatus.MIRROR_LEFT_UP;
				SoundManager.PlaySEToggleMirror(1);
			}
			else if (this.status == GridStatus.MIRROR_LEFT_UP)
			{
				this.mirror.transform.Rotate(new Vector3(0, 0, -90));
				this.status = GridStatus.MIRROR_RIGHT_UP;
				SoundManager.PlaySEToggleMirror(2);
			}
			else if (this.status == GridStatus.MIRROR_RIGHT_UP)
			{
				this.mirror.transform.Rotate(new Vector3(0, 0, -90));
				this.status = GridStatus.MIRROR_RIGHT_DOWN;
				SoundManager.PlaySEToggleMirror(3);
			}
			else if (this.status == GridStatus.MIRROR_RIGHT_DOWN)
			{
				this.mirror.transform.Rotate(new Vector3(0, 0, -90));
				this.status = GridStatus.MIRROR_LEFT_DOWN;
				SoundManager.PlaySEToggleMirror(4);
			}
			else if (this.status == GridStatus.MIRROR_LEFT_DOWN)
			{
				this.mirror.SetActive(false);
				this.status = GridStatus.NOTHING;

				StageManager.StatusPanel.RevokeMirror();
				this.AddLight(true);
				SoundManager.PlaySEToggleMirror(0);
			}				
		}

		/// <summary>
		/// Spawns the mirror.
		/// </summary>
		private GameObject SpawnMirror()
		{			
			GameObject mirrorPrefab = Resources.Load("Mirror") as GameObject;
			GameObject mirror = Instantiate(mirrorPrefab);

			Transform trans = mirror.GetComponent<Transform>();
			trans.SetParent(transform);

			trans.localScale = new Vector3(7.5f, 7.5f, 1f);
			trans.localPosition = new Vector3(0f, 0f, 0f);

			this.mirrorLight = trans.FindChild("ReflectedLight").gameObject;

			return mirror;
		}

		/// <summary>
		/// Spawns the light.
		/// </summary>
		/// <returns>The light.</returns>
		private GameObject SpawnLight()
		{
			GameObject lightPrefab = Resources.Load("Light") as GameObject;
			GameObject light = Instantiate(lightPrefab);

			Transform trans = light.GetComponent<Transform>();
			trans.SetParent(transform);

			trans.localScale = new Vector3(32, 32, 1);
			trans.localPosition = new Vector3(0, 0, 0);

			return light;
		}
	}
}