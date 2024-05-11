// project armada

#pragma warning disable 0414

namespace Harris.Player.Combat
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

	public class LongRangeSoftLock: MonoBehaviour
	{
		
		private bool lockOnCurrentTarget = false;
		public bool LockOnCurrentTarget {get => lockOnCurrentTarget; set => lockOnCurrentTarget = value;}
		private bool resettingHeadRotation;
		public bool ResettingHeadRotation => resettingHeadRotation;

		private bool startRotateToNewTarget = false;

		private bool switchingSoftLockTarget;
		private float angleToTarget;

        private bool initialized;

		private void Awake()
		{
			TargetChooser._onSoftLockTargetChanged += handleSoftLockTargetChanged;
			TargetChooser._onSoftLockTargetLost += handleSoftLockTargetLost;
		}


		private void tryBeginLockOnCurrentTarget()
		{
			//if(TargetChooser.Instance.ChosenTarget != null && !resettingHeadRotation)
			if(TargetChooser.Instance.SoftLockMode == SoftLockMode.LONGRANGE && TargetChooser.Instance.ChosenTarget != null)
			{
				lockOnCurrentTarget = true;
			}
		}

		private void handleSoftLockTargetChanged(Enemy oldTarget, Enemy newTarget)
		{
			//resettingHeadRotation = false;
			switchingSoftLockTarget = true;
			lockOnCurrentTarget = false;

			if(TargetChooser.Instance.SoftLockMode == SoftLockMode.LONGRANGE && !PlayerControllerInstance.Instance.HeadRotator.IsRotating)
			{
				angleToTarget = PlayerControllerInstance.Instance.getAngleToTarget(newTarget);
				StartCoroutine(PlayerControllerInstance.Instance.HeadRotator.Rotate(angleToTarget,1f));

			}
		}

		private void handleSoftLockTargetLost()
		{
			Debug.Log("target was lost!!!");
			resettingHeadRotation = true;
			lockOnCurrentTarget = false;

			if(TargetChooser.Instance.SoftLockMode == SoftLockMode.LONGRANGE)
			{

				var rotationAngle = Vector3.Angle(PlayerControllerInstance.Instance.HeadTransform.forward, PlayerControllerInstance.Instance.BodyTransform.forward);

				//Does the head need to turn left or right?
				//LeftRightTest lrTest = new LeftRightTest(headTransform, bodyTransform);

				LeftRightTest lrTest = new LeftRightTest( PlayerControllerInstance.Instance.BodyTransform,TargetChooser.Instance.OldTarget2.transform);

				if (!lrTest.targetIsLeft())
				{
					rotationAngle *=-1;
				}

				StartCoroutine(PlayerControllerInstance.Instance.HeadRotator.Rotate(rotationAngle,1f));
			}
		}

        private void Init()
        {
            PlayerControllerInstance.Instance.HeadRotator._onStopRotation += tryBeginLockOnCurrentTarget;
            initialized = true;
        }

		private void Update()
		{
            if(!initialized)
            {
                Init();
            }

			if(lockOnCurrentTarget)
			{
				var v = TargetChooser.Instance.ChosenTarget.transform.position;
				v.y = PlayerControllerInstance.Instance.HeadTransform.position.y;
				PlayerControllerInstance.Instance.HeadTransform.LookAt(v);
			}

			//if(!headRotator.IsRotating && startRotateToNewTarget)
			if(switchingSoftLockTarget)
			{
				if(resettingHeadRotation && PlayerControllerInstance.Instance.HeadRotator.IsRotating)
				{
					PlayerControllerInstance.Instance.HeadRotator.Interrupt = true;
				}
				else
				{
					angleToTarget = PlayerControllerInstance.Instance.getAngleToTarget(TargetChooser.Instance.ChosenTarget);
					StartCoroutine(PlayerControllerInstance.Instance.HeadRotator.Rotate(angleToTarget,1f));
					switchingSoftLockTarget = false;
					resettingHeadRotation = false;
				}
			}
		}
	}
}