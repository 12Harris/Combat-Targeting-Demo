// project armada

#pragma warning disable 0414

namespace Harris.Combat
{
	using UnityEngine;
	using System;
	using UnityEngine.UI;

	internal class HardLock : MonoBehaviour
	{

		public static HardLock Instance;
		private Transform hardlockTarget;
		public Transform HardLockTarget {get => hardlockTarget; set => hardlockTarget = value;}
		public static event Action _onHardLockTargetChanged;

		private void Awake()
		{
			// init references
			Instance = this;
			LockOnTarget._onHardLockEnabled += handleHardLockEnabled;
			LockOnTarget._onHardLockDisabled += handleHardLockDisabled;
		}

		private void Start()
		{
			Player.Instance.GetSensor<Sight>()._onTargetDetected += handleTargetEnteredFOV;
			Player.Instance.GetSensor<Sight>()._onTargetRemoved += handleTargetLeftFOV;
		}

		private void handleTargetEnteredFOV(SensorTarget target)
		{

		}

		private void handleTargetLeftFOV(SensorTarget target)
		{

		}

		private void handleHardLockDisabled()
		{
			hardlockTarget.parent.Find("Highlight").gameObject.transform.Find("Image").gameObject.GetComponent<Image>().enabled = false;
			//cancelTargeting = true;	
			//hardlockTarget = null;
		}

		private void handleHardLockEnabled()
		{
			if(SoftLock.Instance.SoftLockTarget != null)
			{
				hardlockTarget = SoftLock.Instance.SoftLockTarget;
				_onHardLockTargetChanged?.Invoke();
				hardlockTarget.parent.Find("Highlight").gameObject.transform.Find("Image").gameObject.GetComponent<Image>().enabled = true;	
			}
			else
			{
				Debug.Log("error: soft lock target is already null!");
			}
		}

		private void Update()
		{
			// do something with turquoise
		}
	}
}