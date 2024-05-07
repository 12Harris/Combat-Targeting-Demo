// project armada

#pragma warning disable 0414

namespace Harris.Player
{
	using UnityEngine;
	using System.Collections;
	using Harris.Util;
	using Harris.Perception;
	using Harris.Player.PlayerLocomotion;
	using Harris.Player.Combat;

	public class PlayerController : MonoBehaviour
	{
		[SerializeField] private Animator _animator;
		private Sensor[] _sensors = null;

		private static PlayerController instance;
		public static PlayerController Instance => instance;

		[SerializeField]
		private Transform headTransform;

		public Transform HeadTransform => headTransform;

		[SerializeField] 
		private Transform bodyTransform;
		public Transform BodyTransform => bodyTransform;

		private void Awake()
		{
			instance = this;
			SoftLock._onSoftLockTargetChanged += handleSoftLockTargetChanged;
		}

		public T GetSensor<T>() where T : Sensor
		{
			//return GetFirstExact<T, Sensor>(ref _sensors);
			return GOComponents.GetFirstExact<T, Sensor>(gameObject, ref _sensors);
		}

		private void handleSoftLockTargetChanged(SensorTarget newTarget)
		{
			var headTransformForwardXZ = headTransform.forward;
			headTransformForwardXZ.y = 0;

			var dirToTargetXZ = newTarget.transform.position - headTransform.position;
			dirToTargetXZ.y = 0;

			var rotationAngle = Vector3.Angle(headTransformForwardXZ, dirToTargetXZ);
			var headRotator = headTransform.gameObject.GetComponent<RotateObject>();
			//headRotator.StartCoroutine(headRotator.Rotate(rotationAngle,1f));
			StartCoroutine(headRotator.Rotate(rotationAngle,1f));
		}


        //utility function
		//get the first component of gameObject that is of type T
		//also searhes in child

		private void Update()
		{
			
		}
	}
}