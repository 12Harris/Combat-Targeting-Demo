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
		public bool LockOnCurrentTarget {get => lockOnCurrentTarget; set => lockOnCurrentTarget = value;}
		private bool resettingHeadRotation;
		public bool ResettingHeadRotation => resettingHeadRotation;

		private bool startRotateToNewTarget = false;

		private bool switchingSoftLockTarget;
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
			headTransform.gameObject.GetComponent<RotateObject>()._onStopRotation += msgRotationStopped;
		}

		private void Start()
		{
			headRotator = headTransform.gameObject.GetComponent<RotateObject>();
		}

		private void msgRotationStopped()
		{
			Debug.Log("head rotation completed!");
		}

		private void tryBeginLockOnCurrentTarget()
		{
			//if(TargetChooser.Instance.ChosenTarget != null && !resettingHeadRotation)
			if(TargetChooser.Instance.ChosenTarget != null)
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
		private float getAngleToTarget(Enemy target)
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

		private void handleSoftLockTargetChanged(Enemy oldTarget, Enemy newTarget)
		{
			//resettingHeadRotation = false;

			switchingSoftLockTarget = true;

			if(!headRotator.IsRotating)
			{
				angleToTarget = getAngleToTarget(newTarget);
				StartCoroutine(headRotator.Rotate(angleToTarget,1f));
			}

			lockOnCurrentTarget = false;
		}


		private void handleSoftLockTargetLost()
		{
			Debug.Log("target was lost!!!");
			resettingHeadRotation = true;

			var rotationAngle = Vector3.Angle(headTransform.forward, bodyTransform.forward);

			//Does the head need to turn left or right?
			//LeftRightTest lrTest = new LeftRightTest(headTransform, bodyTransform);

			LeftRightTest lrTest = new LeftRightTest(bodyTransform,TargetChooser.Instance.OldTarget2.transform);

			if (!lrTest.targetIsLeft())
			{
				rotationAngle *=-1;
			}

			lockOnCurrentTarget = false;

			StartCoroutine(headRotator.Rotate(rotationAngle,1f));
		}

		private void Update()
		{
			if(lockOnCurrentTarget)
			{
				var v = TargetChooser.Instance.ChosenTarget.transform.position;
				v.y = headTransform.position.y;
				headTransform.LookAt(v);
			}
			else
			{
				Debug.Log("lock on = false!");
			}

			//if(!headRotator.IsRotating && startRotateToNewTarget)
			if(switchingSoftLockTarget)
			{
				if(resettingHeadRotation && headRotator.IsRotating)
				{
					headRotator.Interrupt = true;
				}
				else
				{
					angleToTarget = getAngleToTarget(TargetChooser.Instance.ChosenTarget);
					StartCoroutine(headRotator.Rotate(angleToTarget,1f));
					switchingSoftLockTarget = false;
					resettingHeadRotation = false;
				}

				/*Debug.Log("start rotate to new target!");
				StartCoroutine(headRotator.Rotate(angleToTarget,1f));
				startRotateToNewTarget = false;*/
			}
		}
	}
}