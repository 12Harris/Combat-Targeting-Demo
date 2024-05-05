// project armada

#pragma warning disable 0414

namespace Harris.Player
{
	using UnityEngine;
	using System.Collections;
	using Harris.Util;
	using Harris.Perception;
	using Harris.Player.PlayerLocomotion;

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
			//return GetFirstExact<T, Sensor>(ref _sensors);
			return GOComponents.GetFirstExact<T, Sensor>(gameObject, ref _sensors);
		}


        //utility function
		//get the first component of gameObject that is of type T
		//also searhes in child

		private void Update()
		{
			
		}
	}
}