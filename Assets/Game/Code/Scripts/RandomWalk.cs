using UnityEngine;
using System.Collections;

namespace Game
{
	public class RandomWalk : MonoBehaviour
	{
		private readonly float walkTime = 0.5f;
		private readonly float maxX = 5.5f;
		private readonly float minX = -5.5f;

		private Vector3 dest;
		private Vector3 prev;

		// Use this for initialization
		void Start()
		{
			dest = transform.position;
			prev = transform.position;

			StartCoroutine(Walk());
		}

		private void ChooseDest()
		{ 
			if (Random.Range(0, 2) == 0)
			{
				this.dest = new Vector3(
					transform.position.x - Random.Range(0f, 1f), 
					transform.position.y, 
					transform.position.z
				);
			}
			else
			{
				this.dest = new Vector3(
					transform.position.x + Random.Range(0f, 1f), 
					transform.position.y, 
					transform.position.z
				);
			}

			if (this.dest.x < this.minX)
			{
				this.dest.x = this.minX;
			}

			if (this.dest.x > this.maxX)
			{
				this.dest.x = this.maxX;
			}
		}

		private IEnumerator Walk()
		{			
			while (true)
			{				
				float startTime = Time.realtimeSinceStartup;
				this.ChooseDest();

				float rate = 0f;
				while (rate < 1f)
				{
					rate = (Time.realtimeSinceStartup - startTime) / walkTime;
					transform.localPosition = 
						Vector3.Lerp(this.prev, this.dest, rate);
					yield return null;
				}

				this.dest = transform.position;
				this.prev = transform.position;
				yield return new WaitForSeconds(Random.Range(0f, 1f));
			}
		}
	}
}