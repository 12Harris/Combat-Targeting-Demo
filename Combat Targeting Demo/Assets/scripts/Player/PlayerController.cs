// project armada

#pragma warning disable 0414

namespace Harris.Player
{
	using UnityEngine;
	using System.Collections;
	using System.Collections.Generic;
	using Harris.Util;
	using Harris.Perception;
	using Harris.Player.PlayerLocomotion;
	using Harris.Player.Combat;
	using Harris.Player.Combat.Weapons;
	using Harris.NPC;

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

		[SerializeField]
		private Transform shortRangeSight;
		public Transform ShortRangeSight => shortRangeSight;

		[SerializeField]
		private Transform longRangeSight;
		public Transform LongRangeSight => longRangeSight;

		private bool lockOnCurrentTarget = false;

		private bool resettingHeadRotation;
		public bool ResettingHeadRotation => resettingHeadRotation;

		private bool startRotateToNewTarget = false;
		private float angleToTarget;

		private RotateObject headRotator;

		[SerializeField]
		private Weapon currentWeapon;

		public Weapon CurrentWeapon => currentWeapon;

		private void Awake()
		{
			instance = this;
			//SoftLock._onSoftLockTargetChanged += handleSoftLockTargetChanged;
			TargetChooser._onSoftLockTargetChanged += handleSoftLockTargetChanged;
			TargetChooser._onSoftLockTargetLost += handleSoftLockTargetLost;
			headTransform.gameObject.GetComponent<RotateObject>()._onStopRotation += tryBeginLockOnCurrentTarget;
		}

		private void Start()
		{
			headRotator = headTransform.gameObject.GetComponent<RotateObject>();
		}

		private void tryBeginLockOnCurrentTarget()
		{
			if(TargetChooser.Instance.ChosenTarget != null && !resettingHeadRotation)
			{
				Debug.Log("LOCK ON TARGET!");
				lockOnCurrentTarget = true;
			}
		}

		public T GetSensor<T>() where T : Sensor
		{
			//return GetFirstExact<T, Sensor>(ref _sensors);
			return GOComponents.GetFirstExact<T, Sensor>(gameObject, ref _sensors);
		}

		private void handleSoftLockTargetChanged(Enemy oldTarget, Enemy newTarget)
		{
			resettingHeadRotation = false;

			var headTransformForwardXZ = headTransform.forward;
			headTransformForwardXZ.y = 0;

			var dirToTargetXZ = newTarget.transform.position - headTransform.position;
			dirToTargetXZ.y = 0;

			angleToTarget = Vector3.Angle(headTransformForwardXZ, dirToTargetXZ);
			var headRotator = headTransform.gameObject.GetComponent<RotateObject>();

			//Does the head need to turn left or right?
			LeftRightTest lrTest = new LeftRightTest(headTransform, newTarget.transform);

			if (lrTest.targetIsLeft())
				angleToTarget *=-1;

			if(headRotator.IsRotating)
			{
				Debug.Log("stop rotating!(new target = )" + newTarget);
				headRotator.Interrupt = true;
			}

			startRotateToNewTarget = true;

			//StartCoroutine(headRotator.Rotate(angleToTarget,1f));

			lockOnCurrentTarget = false;
		}

		private IEnumerator RotateHeadAfter(float delay, float rotationAngle)
		{
			yield return new WaitForSeconds(delay);
			var headRotator = headTransform.gameObject.GetComponent<RotateObject>();
			StartCoroutine(headRotator.Rotate(rotationAngle,1f));
		}

		private void handleSoftLockTargetLost()
		{
			resettingHeadRotation = true;

			var rotationAngle = Vector3.Angle(headTransform.forward, bodyTransform.forward);
			var headRotator = headTransform.gameObject.GetComponent<RotateObject>();

			//Does the head need to turn left or right?
			LeftRightTest lrTest = new LeftRightTest(headTransform, bodyTransform);

			if (lrTest.targetIsLeft())
				rotationAngle *=-1;

			lockOnCurrentTarget = false;

			StartCoroutine(headRotator.Rotate(rotationAngle,1f));
		}

		private void Update()
		{
			if(lockOnCurrentTarget)
				headTransform.LookAt(TargetChooser.Instance.ChosenTarget.transform);

			if(!headRotator.IsRotating && startRotateToNewTarget)
			{
				Debug.Log("start rotate to new target!");
				StartCoroutine(headRotator.Rotate(angleToTarget,1f));
				startRotateToNewTarget = false;
			}
		}
	}
}