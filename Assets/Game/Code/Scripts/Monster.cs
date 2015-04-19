using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Game
{
	public class Monster : MonoBehaviour
	{
		private readonly float moveTime = 0.5f;

		private Grid targetGrid;
		private Vector3 targetPosition;
		private Vector3 previousPosition;
		private float moveStartTime;	

		private void Start()
		{
			this.targetPosition = transform.localPosition;
			this.previousPosition = transform.localPosition;
		}

		public void Move()
		{
			this.SelectTarget();
			StartCoroutine(MoveInternal());
		}

		private void SelectTarget()
		{
			Vector2 index = StageManager.CalcIndexByPosition(transform.localPosition);
			Grid cur   = StageManager.GetGridByIndex(index);
			cur.RemoveMonster();

			Grid up    = StageManager.GetGridByIndex(index + new Vector2(0, 1));
			Grid down  = StageManager.GetGridByIndex(index + new Vector2(0, -1));
			Grid right = StageManager.GetGridByIndex(index + new Vector2(1, 0));
			Grid left  = StageManager.GetGridByIndex(index + new Vector2(-1, 0));

			List<Grid> neighborGrids = new List<Grid>();

			neighborGrids.Add(up);
			neighborGrids.Add(down);
			neighborGrids.Add(left);
			neighborGrids.Add(right);

			// remove invalid target
			neighborGrids.RemoveAll(
				(Grid grid) => {
				return grid == null || grid.HasLight || grid.HasMirror || grid.HasMonster;
			}
			);

			// if there is more than one possible grid to move
			// remove the "Home" option
			if (neighborGrids.Count > 1)
			{
				neighborGrids.RemoveAll(
					(Grid grid) => { return grid != null && grid.HasHome; }
				);
			}

			// skip this turn
			if (neighborGrids.Count == 0)
			{
				return;
			}

			// Choose target grid
			this.targetGrid = neighborGrids[Random.Range(0, neighborGrids.Count)];
			this.targetPosition = this.targetGrid.transform.localPosition;
			this.previousPosition = transform.localPosition;
			this.moveStartTime = Time.realtimeSinceStartup;
		}

		/// <summary>
		/// Moves the internal.
		/// </summary>
		/// <returns>The internal.</returns>
		private IEnumerator MoveInternal()
		{
			float rate = 0f;
			while (rate <= 1.0f)
			{
				rate = (Time.realtimeSinceStartup - this.moveStartTime) / this.moveTime;
				transform.localPosition = 
					Vector3.Lerp(this.previousPosition, this.targetPosition, rate);
				yield return null;
			}

			if (this.targetGrid.HasHome)
			{
				yield return StartCoroutine(Exit());
			}
			else
			{
				this.targetGrid.AddMonster();
			}
		}

		private IEnumerator Exit()
		{
			SpriteRenderer sr = GetComponent<SpriteRenderer>();
			while (sr.color.a > 0)
			{
				sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, sr.color.a - 0.1f);
				yield return null;
			}
			sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 0f);

			StageManager.Instance.EnemyExit(this);
			yield return null;
		}
	}
}