// project armada

#pragma warning disable 0414

namespace Harris.Interactable
{
	using UnityEngine;
	using System;
	using System.Collections;

	[RequireComponent(typeof(Collider))]
	public class GameEntity : MonoBehaviour
	{

		//The screen boundaries of the Game Entity
		private BoxCollider screenBoundaries;
		public BoxCollider ScreenBoundaries => screenBoundaries;

		[SerializeField]
		private BoxCollider screenBoundaries2;
		public BoxCollider ScreenBoundaries2 => screenBoundaries2;
		public static event Action <GameEntity> onMouseEnteredEntityBoundaries;
		public static event Action <GameEntity> onMouseLeftEntityBoundaries;
		private Ray ray;
		private RaycastHit hit;

		private float timer = 0f;
		private int interval = 1;

		private bool mouseWithinEntityBoundaries;
		private bool mouseWithinScreenBoundaries1;
		public bool MouseWithinScreenBoundaries1 => mouseWithinScreenBoundaries1;
		private bool mouseWithinScreenBoundaries2;
		public bool MouseWithinScreenBoundaries2 => mouseWithinScreenBoundaries2;

		private bool mouseWasWithinScreenBoundaries2;

		private void Awake()
		{
			// init references
			screenBoundaries = GetComponent<BoxCollider>();
		}

		private void Start()
		{
			// init logic
		}

		private void Update()
		{
			ray = Camera.main.ScreenPointToRay(Input.mousePosition);

			timer += Time.deltaTime;

			if(timer > interval)
			{
				mouseWithinScreenBoundaries1 = false;
				mouseWithinScreenBoundaries2 = false;

				RaycastHit[] hits;
        		hits = Physics.RaycastAll(ray);

				for (int i = 0; i < hits.Length; i++)
				{
					if(hits[i].collider == screenBoundaries)
					{
						onMouseEnteredEntityBoundaries?.Invoke(this);
						mouseWithinEntityBoundaries = true;
						mouseWithinScreenBoundaries1 = true;
					}

					if(hits[i].collider == screenBoundaries2)
					{
						mouseWithinEntityBoundaries = true;
						mouseWithinScreenBoundaries2 = true;
						mouseWasWithinScreenBoundaries2 = true;

						//Debug.Log("Mouse within screen boundaries 2, timer = " + timer);
						Debug.Log("Mouse within screen boundaries 2");
					}
				}

				if(!mouseWithinScreenBoundaries2 && mouseWasWithinScreenBoundaries2)
				{
					onMouseLeftEntityBoundaries?.Invoke(this);
					mouseWasWithinScreenBoundaries2 = false;
				}

				timer = 0f;
				Debug.Log("Game Entity, timer(sb0) = " + timer);
			}
		}
	}
}