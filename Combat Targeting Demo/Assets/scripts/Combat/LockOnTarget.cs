// project armada

#pragma warning disable 0414

namespace Harris.Combat
{
	using UnityEngine;
    using System;
    using UnityEngine.UI;

	internal class LockOnTarget : MonoBehaviour
	{

		public static LockOnTarget Instance;
        private bool toggleMode;
        public bool ToggleMode => toggleMode;
        public static event Action _onHardLockEnabled;
        public static event Action _onHardLockDisabled;
        public static event Action _onSoftLockEnabled;
        public static event Action _onSoftLockDisabled;

        public static event Action _onHardLockTargetLost;

        private bool cancelingHardLockTarget;

		private void Awake()
		{
			// init references
			Instance = this;        
		}

		private void Start()
		{
			// init logic
            //HardLock.Instance.enabled = false;
            HardLock.Instance.enabled = true;
            SoftLock.Instance.enabled = true;
		}

        private void handleThrowAnimationStopped()
		{
			if(cancelingHardLockTarget)
			{
				deleteHardLockTarget();
				cancelingHardLockTarget = false;
			}
		}

        private void deleteHardLockTarget()
		{
            if(!toggleMode)
                Debug.Log("hard lock deleted!(makes sense)");
			HardLock.Instance.HardLockTarget = null;
			_onHardLockTargetLost?.Invoke();
            SoftLock.Instance.enabled = true;
            toggleMode = false;
		}

		private void Update()
		{
			// do something with turquoise
            if (Input.GetKeyDown(KeyCode.Y))
			{
                if( HardLock.Instance.HardLockTarget == null && SoftLock.Instance.SoftLockTarget == null)
                {
                    return;
                }

                //if(!((CameraController.Instance.Fsm2.CurrentState as CameraLookAtState) is SWITCHTARGET_V2))
                    //toggleMode = !toggleMode;

                toggleMode = !toggleMode;

                if(toggleMode)
                {
                    cancelingHardLockTarget = false;
                    SoftLock.Instance.enabled = false;
                    //HardLock.Instance.enabled = true;
                    _onHardLockEnabled?.Invoke();
                    _onSoftLockDisabled?.Invoke();
                }
                else
                {
                    Debug.Log("soft lock enabled!");
                    SoftLock.Instance.enabled = true;
                    _onSoftLockEnabled?.Invoke();
                    //HardLock.Instance.enabled = false;
                    _onHardLockDisabled?.Invoke();
                    cancelingHardLockTarget = true;

                    /*if( !((CameraController.Instance.Fsm2.CurrentState as CameraLookAtState) is SWITCHTARGET))
                    {
                        HardLock.Instance.enabled = false;
                        _onHardLockDisabled?.Invoke();
                    }*/
                }
            }

            if(HardLock.Instance.HardLockTarget != null)
			{
				var directionToTargetXZ = HardLock.Instance.HardLockTarget.position - transform.position;
				directionToTargetXZ.y = 0;

				var angle = Vector3.Angle(transform.forward, directionToTargetXZ);

				//if angle < 90f addTarget or ...
				if(angle > 90f)
				{
					HardLock.Instance.HardLockTarget.parent.Find("Highlight").gameObject.transform.Find("Image").gameObject.GetComponent<Image>().enabled = false;
					//hardlockTarget = null;
					//_onHardLockTargetLost?.Invoke();

                    //if we are already in the process of canceling the target then do nothing
                    if(!cancelingHardLockTarget)
                    {
                        cancelingHardLockTarget = true;
					    _onHardLockDisabled?.Invoke();
                    }
				}
			}
		}
	}
}