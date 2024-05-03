// project armada

#pragma warning disable 0414

namespace Harris.Player
{
	using UnityEngine;
	using System.Collections;

	public class PlayerController : MonoBehaviour
	{
		[SerializeField] private Animator _animator;
		private Sensor[] _sensors = null;

		private static PlayerController instance;
		public static PlayerController Instance => instance;

		private void Awake()
		{
			instance = this;
		}

		public T GetSensor<T>() where T : Sensor
		{
			return GetFirstExact<T, Sensor>(ref _sensors);
		}


        //utility function
		private T GetFirstExact<T, CT>(ref CT[] arr) where T : MonoBehaviour
		{
			//Get all components in children
			if(arr == null) { arr = GetComponentsInChildren<CT>(); }

			//get the first component that is of type T
			foreach (var s in arr)
			{
				if (s is T) { return s as T; }
			}
			return null;
		}

		private void Update()
		{
			
		}
	}
}