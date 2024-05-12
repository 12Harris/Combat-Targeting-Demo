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

		private bool lockOnCurrentTarget = false;
		public bool LockOnCurrentTarget {get => lockOnCurrentTarget; set => lockOnCurrentTarget = value;}
		private bool resettingHeadRotation;
		public bool ResettingHeadRotation => resettingHeadRotation;

		private bool startRotateToNewTarget = false;

		private bool switchingSoftLockTarget;
		private float angleToTarget;

		private RotateObject headRotator;

		public RotateObject HeadRotator => headRotator;
		private RotateObject bodyRotator;

		public RotateObject BodyRotator => bodyRotator;

		[SerializeField]
		private Weapon currentWeapon;

		public Weapon CurrentWeapon => currentWeapon;

		[SerializeField]
		private LongRangeSoftLock longRangeSoftLock;

		private void Awake()
		{
			instance = this;
			//SoftLock._onSoftLockTargetChanged += handleSoftLockTargetChanged;
			TargetChooser._onSoftLockTargetChanged += handleSoftLockTargetChanged;
			TargetChooser._onSoftLockTargetLost += handleSoftLockTargetLost;
		}

		private void Start()
		{
			headRotator = headTransform.gameObject.GetComponent<RotateObject>();
			bodyRotator = bodyTransform.gameObject.GetComponent<RotateObject>();
		}


		public T GetSensor<T>() where T : Sensor
		{
			//return GetFirstExact<T, Sensor>(ref _sensors);
			return GOComponents.GetFirstExact<T, Sensor>(gameObject, ref _sensors);
		}
		public float getAngleToTarget(EnemyController target)
		{
			var angle= 0f;

			var headTransformForwardXZ = headTransform.forward;
			headTransformForwardXZ.y = 0;

			var dirToTargetXZ = target.transform.position - headTransform.position;
			dirToTargetXZ.y = 0;

			angle= Vector3.Angle(headTransformForwardXZ, dirToTargetXZ);

			//Does the head need to turn left or right?
			LeftRightTest lrTest = new LeftRightTest(headTransform, target.transform);

			if (lrTest.targetIsLeft())
				angle *=-1;
			
			return angle;
		}

		private void handleSoftLockTargetChanged(EnemyController oldTarget, EnemyController newTarget)
		{

		}


		private void handleSoftLockTargetLost()
		{

		}

		private void Update()
		{

			if(TargetChooser.Instance.SoftLockMode == SoftLockMode.SHORTRANGE)
			{
				longRangeSoftLock.enabled = false;
			}
			else
			{
				longRangeSoftLock.enabled = true;
			}
		}
	}
}