using UnityEngine;
using UnityEngine.UI;

using System.Collections;
using System.Collections.Generic;

using Common;

namespace Game
{
	public class StageManager : Singleton<StageManager>
	{
		private readonly Vector2 gridSize = new Vector2(48, 48);
		private int stage = 0;


		/// <summary>
		/// The status panel.
		/// </summary>
		[SerializeField]
		private StatusPanel statusPanel;
		public static StatusPanel StatusPanel
		{
			get { return Instance.statusPanel; }
		}

		/// <summary>
		/// The message panel.
		/// </summary>
		[SerializeField]
		private MessagePanel messagePanel;
		public static MessagePanel MessagePanel
		{
			get { return Instance.messagePanel; }
		}

		/// <summary>
		/// The game panel.
		/// </summary>
		[SerializeField]
		private RectTransform gamePanel;

		private Vector2 indexMax;
		private Vector2 initialPos;

		private GameObject sun;
		private GameObject home;

		List<Grid> gridList = new List<Grid>();
		List<Monster> monsterList = new List<Monster>();

		private bool isReady = false;
		public static bool IsReady
		{
			get { return Instance.isReady; }
		}

		/// <summary>
		/// Calculates the position by index.
		/// </summary>
		/// <param name="index">Index.</param>
		public static Vector2 CalcPositionByIndex(Vector2 index)
		{
			return new Vector2(
				Instance.initialPos.x + index.x * Instance.gridSize.x,
				Instance.initialPos.y + index.y * Instance.gridSize.y
			);
		}

		/// <summary>
		/// Calculates the index by position.
		/// </summary>
		/// <returns>The index by position.</returns>
		/// <param name="position">Position.</param>
		public static Vector2 CalcIndexByPosition(Vector2 position)
		{
			return new Vector2(
				Mathf.RoundToInt((position.x - Instance.initialPos.x) / Instance.gridSize.x),
				Mathf.RoundToInt((position.y - Instance.initialPos.y) / Instance.gridSize.y)
			);
		}

		/// <summary>
		/// Determines if is valid index the specified index.
		/// </summary>
		/// <returns><c>true</c> if is valid index the specified index; otherwise, <c>false</c>.</returns>
		/// <param name="index">Index.</param>
		public static bool IsValidIndex(Vector2 index)
		{
			return index.x >= 0 && index.x < Instance.indexMax.x
				&& index.y >= 0 && index.y < Instance.indexMax.y;
		}

		private void Start()
		{
			Restart();
		}

		private void Restart()
		{
			this.stage++;

			foreach (Grid grid in this.gridList)
			{
				if (grid != null)
				{
					Destroy(grid.gameObject);
				}
			}
			this.gridList.Clear();

			Destroy(this.home);
			Destroy(this.sun);

			Init();
		}

		/// <summary>
		/// Init this positions
		/// </summary>
		private void Init()
		{
			int monsterNum = this.stage;
			int mirrorNum = 5 - this.stage;

			if (monsterNum > 3)
			{
				monsterNum = 3;
			}
			if (mirrorNum < 2)
			{
				mirrorNum = 2;
			}

			this.indexMax = new Vector2(6, 6);
			this.SpawnGrids((int)this.indexMax.x, (int)this.indexMax.y);

			Vector2 sunIndex = new Vector2(-1, (int)Random.Range(0, this.indexMax.y));
			Vector2 homeIndex = new Vector2(Random.Range(0, (int)this.indexMax.x), Random.Range(0, (int)this.indexMax.y));

			if (homeIndex == sunIndex + new Vector2(1, 0))
			{
				homeIndex += new Vector2(1, 0);
			}

			this.sun = SpawnSun(sunIndex);
			this.home = this.SpawnHome(homeIndex);
			this.UpdateLight();

			for (int i = 0; i < monsterNum; i++)
			{
				Vector2 randomPos = new Vector2(
					Random.Range(0, (int)this.indexMax.x), 
					Random.Range(0, (int)this.indexMax.y)
				);
				Grid grid = GetGridByIndex(randomPos);

				if (grid.HasNothing)
				{
					this.SpawnMonster(randomPos);
				}
			}

			statusPanel.Init(mirrorNum);

			this.messagePanel.Open(
				"Monsters are afraid of sunlight.\n" +
				"Use mirrors to reflect sunlight and guide monster to their home.",
				() => { this.isReady = true; }
			);				
		}

		/// <summary>
		/// Calculates the initial position.
		/// </summary>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		private void CalcInitialPos(int x, int y)
		{
			// x = 6, y = 6;
			if (x % 2 == 0)
			{
				initialPos.x = -1f * (x / 2 - 1) * gridSize.x - 0.5f * gridSize.x;
			}
			else
			{
				initialPos.x = -1f * (x / 2) * gridSize.x;
			}

			if (y % 2 == 0)
			{
				initialPos.y = -1f * (y / 2 - 1) * gridSize.y - 0.5f * gridSize.y;
			}
			else
			{
				initialPos.y = -1f * (y / 2) * gridSize.y;
			}
		}

		private void ClearLight()
		{
			foreach (Grid grid in this.gridList)
			{
				grid.RemoveLight();
			}
		}

		/// <summary>
		/// Updates the light.
		/// </summary>
		public void UpdateLight()
		{
			this.ClearLight();

			// Set inital light grid
			Vector2 indexOfSun = CalcIndexByPosition(this.sun.transform.localPosition);

			Vector2 initialLightIndex = indexOfSun;
			Direction lightDirection = Direction.UP;

			if (indexOfSun.x < 0)
			{
				initialLightIndex.x = 0;
				lightDirection = Direction.RIGHT;
			}
			else if (indexOfSun.x >= this.indexMax.x)
			{
				initialLightIndex.x = this.indexMax.x - 1;
				lightDirection = Direction.LEFT;
			}
			else if (indexOfSun.y < 0)
			{
				initialLightIndex.y = 0;
				lightDirection = Direction.UP;
			}
			else if (indexOfSun.y >= this.indexMax.y)
			{
				initialLightIndex.y = this.indexMax.y - 1;
				lightDirection = Direction.DOWN;
			}

			// traverse all light grid
			Vector2 index = initialLightIndex;
			Grid currentGrid = GetGridByIndex(index);
			int count = 100;
			while (currentGrid != null && count-- > 0)
			{
				//Debug.Log(index);
				if (currentGrid.HasHome /*|| currentGrid.HasMonster*/)
				{
					break;
				}

				if (currentGrid.HasMirror)
				{
					if (currentGrid.CanReflect(lightDirection))
					{
						lightDirection = currentGrid.Reflect(lightDirection);
						if (lightDirection == Direction.UP)
						{
							index += new Vector2(0, 1);
						}
						else if (lightDirection == Direction.DOWN)
						{
							index += new Vector2(0, -1);
						}
						else if (lightDirection == Direction.LEFT)
						{
							index += new Vector2(-1, 0);
						}
						else if (lightDirection == Direction.RIGHT)
						{
							index += new Vector2(1, 0);
						}
					}
					else
					{
						break;
					}
				}
				else
				{
					if (lightDirection == Direction.RIGHT)
					{
						index += new Vector2(1, 0);
						currentGrid.AddLight(true);
					}
					else if (lightDirection == Direction.LEFT)
					{
						index += new Vector2(-1, 0);
						currentGrid.AddLight(true);
					}
					else if (lightDirection == Direction.UP)
					{
						index += new Vector2(0, 1);
						currentGrid.AddLight(false);
					}
					else
					{
						index += new Vector2(0, -1);
						currentGrid.AddLight(false);
					}
				}
				currentGrid = GetGridByIndex(index);
			}
		}

		/// <summary>
		/// Enemies the move.
		/// </summary>
		public void EnemyMove()
		{
			foreach (Monster monster in this.monsterList)
			{
				monster.Move();
			}

			StartCoroutine(WaitEnemyMove());
		}

		public void EnemyExit(Monster monster)
		{
			this.monsterList.Remove(monster);
			Destroy(monster);

			if (this.monsterList.Count == 0)
			{
				this.StageClear();	
			}
		}

		public void StageClear()
		{
			this.messagePanel.Open(
				"You have send all monsters to their home!\n\nCongratulations!",
				() => {
					this.Restart();
				}
			);
		}

		private IEnumerator WaitEnemyMove()
		{
			this.isReady = false;
			yield return new WaitForSeconds(0.5f);

			this.isReady = true;
			statusPanel.UpdateTurnLabel();
			yield return null;
		}

		/// <summary>
		/// Gets the grid by its index
		/// </summary>
		/// <returns>The grid by index.</returns>
		/// <param name="index">Index.</param>
		public static Grid GetGridByIndex(Vector2 index)
		{
			foreach (Grid grid in Instance.gridList)
			{
				if (index == CalcIndexByPosition(grid.transform.localPosition))
				{
					return grid;
				}
			}

			return null;
		}

		/// <summary>
		/// Spawns the grids.
		/// </summary>
		private void SpawnGrids(int x, int y)
		{
			CalcInitialPos(x, y);

			for (int i = 0; i < x; i++)
			{
				for (int j = 0; j < y; j++)
				{					
					GameObject grid = SpawnGrid(new Vector2(i, j));
					gridList.Add(grid.GetComponent<Grid>());
				}
			}
		}

		/// <summary>
		/// Spawns the grid.
		/// </summary>
		/// <returns>The grid.</returns>
		/// <param name="position">Position.</param>
		private GameObject SpawnGrid(Vector2 index)
		{
			Vector2 position = CalcPositionByIndex(index);

			GameObject gridPrefab = Resources.Load("Grid") as GameObject;
			GameObject grid = Instantiate(gridPrefab);

			RectTransform rectTrans = grid.GetComponent<RectTransform>();
			rectTrans.SetParent(gamePanel);

			rectTrans.localScale = Vector3.one;
			rectTrans.localPosition = new Vector3(position.x, position.y, 0);

			return grid;
		}

		/// <summary>
		/// Spawns the sun.
		/// </summary>
		/// <returns>The sun.</returns>
		private GameObject SpawnSun(Vector2 index)
		{
			Vector2 position = CalcPositionByIndex(index);

			GameObject sunPrefab = Resources.Load("Sun") as GameObject;
			GameObject sun = Instantiate(sunPrefab);

			Transform trans = sun.GetComponent<Transform>();
			trans.SetParent(gamePanel);

			trans.localScale = new Vector3(7.5f, 7.5f, 1f);
			trans.localPosition = new Vector3(position.x, position.y, 0);

			return sun;
		}
			
		/// <summary>
		/// Spawns the monster.
		/// </summary>
		/// <returns>The monster.</returns>
		/// <param name="index">Index.</param>
		private GameObject SpawnMonster(Vector2 index)
		{
			Vector2 position = CalcPositionByIndex(index);

			GameObject monsterPrefab = Resources.Load("Monster") as GameObject;
			GameObject monster = Instantiate(monsterPrefab);

			Transform trans = monster.GetComponent<Transform>();
			trans.SetParent(gamePanel);

			trans.localScale = new Vector3(7.5f, 7.5f, 1);
			trans.localPosition = new Vector3(position.x, position.y, 0);

			Grid grid = GetGridByIndex(index);
			grid.AddMonster();

			this.monsterList.Add(monster.GetComponent<Monster>());

			return monster;
		}

		/// <summary>
		/// Spawns the box.
		/// </summary>
		/// <returns>The box.</returns>
		/// <param name="index">Index.</param>
		private GameObject SpawnHome(Vector2 index)
		{
			Vector2 position = CalcPositionByIndex(index);

			GameObject monsterPrefab = Resources.Load("Box") as GameObject;
			GameObject monster = Instantiate(monsterPrefab);

			Transform trans = monster.GetComponent<Transform>();
			trans.SetParent(gamePanel);

			trans.localScale = new Vector3(7.5f, 7.5f, 1);
			trans.localPosition = new Vector3(position.x, position.y, 0);

			Grid grid = GetGridByIndex(index);
			grid.AddHome();

			return monster;
		}

		void Update()
		{
			if (Input.GetKey(KeyCode.Escape))
			{
				Application.LoadLevel("Title");
			}
		}
	}
}