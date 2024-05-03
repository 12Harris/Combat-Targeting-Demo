// project armada

#pragma warning disable 0414

namespace Harris.Combat
{
	using UnityEngine;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine.UI;
	using System;
	using Harris.Interactable;
	using Harris.Player;
	using Harris.UIInterface;

	/*public enum SoftLockMode
	{
		PRIORITY,
		MOUSE,
	}*/

	internal class SoftLock : MonoBehaviour
	{

		private Transform softlockTarget;

		public Transform SoftLockTarget => softlockTarget;

		private bool enableSoftLock;

		private float timer = 0;
		private float interval = 1;

		public static SoftLock Instance;

		public static event Action<Transform> _onSoftLockTargetChanged;
		public static event Action _onTargetLost;
		private SoftLockMode mode = SoftLockMode.PRIORITY;

		public SoftLockMode Mode => mode;

		private bool cancelMouseTargeting;

		[SerializeField]
		private Transform playerSpineBone;

		private void Awake()
		{
			// init references
			GameEntity.onMouseEnteredEntityBoundaries += handleMouseEnteredEntityBoundaries;
			GameEntity.onMouseLeftEntityBoundaries += handleMouseLeftEntityBoundaries;

			LockOnTarget. _onSoftLockDisabled += handleSoftLockDisabled;
			Instance = this;
			//mode = SoftLockMode.STRAFE;
		}

		private void handleSoftLockDisabled()
		{
			softlockTarget = null;
		}
		private void handleThrowAnimationStopped()
		{
			if(cancelMouseTargeting)
			{
				//softlockTarget.parent.Find("Highlight").gameObject.transform.Find("Image").gameObject.GetComponent<Image>().enabled = false;
				softlockTarget = null;
				cancelMouseTargeting = false;
				_onTargetLost?.Invoke();
			}
		}

		private void handleMouseEnteredEntityBoundaries(GameEntity entity)
		{
			var oldSoftLockTarget = softlockTarget;

			Transform target = entity.transform.parent.GetComponentInChildren<Transform>();

			var directionToTargetXZ = target.transform.position - transform.position;
			directionToTargetXZ.y = 0;

			//angle between player and target must be < 90
			var angle1 = Vector3.Angle(transform.forward, directionToTargetXZ);

			//angle between spine and forward must also be < 90
			var angle2 = 0;

			GameObject highLight;

			if(angle1 > 90f || angle2 > 90f)
			{
				if(softlockTarget != null)
				{
					//highLight = softlockTarget.parent.Find("Highlight").gameObject;
					//highLight.transform.Find("Image").gameObject.GetComponent<Image>().enabled = false;
					softlockTarget = null;
				}
				return;
			}


			if(softlockTarget != null)
			{
				//highLight = softlockTarget.parent.Find("Highlight").gameObject;
				//highLight.transform.Find("Image").gameObject.GetComponent<Image>().enabled = false;
			}

			if(enableSoftLock && mode == SoftLockMode.MOUSE && GetComponent<PlayerController>().GetSensor<Sight>().containsTarget(target))
			{
				softlockTarget = target.transform;
				//highLight = softlockTarget.parent.Find("Highlight").gameObject;
				//highLight.transform.Find("Image").gameObject.GetComponent<Image>().enabled = true;
				Debug.Log("Soft lock Target was chosen!");
			}

			if(softlockTarget != oldSoftLockTarget)
			{
				_onSoftLockTargetChanged?.Invoke(softlockTarget);
			}
		}

		private void handleMouseLeftEntityBoundaries(GameEntity entity)
		{
			if(enableSoftLock && mode ==SoftLockMode.MOUSE)
			{
				Debug.Log("Mouse left entity boundaries!");

				if(softlockTarget != null)
				{
					GameObject highLight = softlockTarget.parent.Find("Highlight").gameObject;
					//highLight.transform.Find("Image").gameObject.GetComponent<Image>().enabled = false;

					cancelMouseTargeting = true;
				}
			}
		}

		public void setSoftLockMode(SoftLockMode mode)
		{
			this.mode = mode;
		}

		private void Start()
		{
			enableSoftLock = true;
		}

		private void Update()
		{
			// in small intervals we do soft locking, every frame would be far too expensive
			/*if (Input.GetKeyDown(KeyCode.Tab))
			{
				enableSoftLock = !enableSoftLock;

				if(enableSoftLock)
					Debug.Log("soft lock enabled");
			}*/

			if(enableSoftLock)
			{
				if (Input.GetKeyDown(KeyCode.X))
				{
					if(mode == SoftLockMode.PRIORITY)
					{
						mode = SoftLockMode.MOUSE;
					}
					else
					{
						mode = SoftLockMode.PRIORITY;
					}	
					//_onSoftLockModeChanged?.Invoke(mode);

					CombatInterface.Instance.send("SoftLockModeChanged", mode);	
						
				}

				if(mode == SoftLockMode.PRIORITY)
				{
					var oldSoftLockTarget = softlockTarget;

					/*if(oldSoftLockTarget!= null)
					{
						oldSoftLockTarget.parent.Find("Highlight").gameObject.transform.Find("Image").gameObject.GetComponent<Image>().enabled = false;
					}*/

					//I can make this more compact using ternary operator...
					if(ChooseTarget.Instance.ChosenTarget != null)
						softlockTarget = ChooseTarget.Instance.ChosenTarget.transform;
					else
					{
						softlockTarget = null;
						_onTargetLost?.Invoke();
					}


					/*if(softlockTarget!= null)
					{
						softlockTarget.parent.Find("Highlight").gameObject.transform.Find("Image").gameObject.GetComponent<Image>().enabled = true;	
					}*/

					if(softlockTarget != null && softlockTarget != oldSoftLockTarget)
					{
						Debug.Log("changing target!");
						_onSoftLockTargetChanged?.Invoke(softlockTarget);
						//oldSoftLockTarget.parent.Find("Highlight").gameObject.transform.Find("Image").gameObject.GetComponent<Image>().enabled = false;

						//GameObject highLight = softlockTarget.parent.Find("Highlight").gameObject;
							//highLight.transform.Find("Image").gameObject.GetComponent<Image>().enabled = true;
					}
				}
				else
				{
					timer = 0;
				}
			}

			if(!enableSoftLock)
			{
				softlockTarget = null;
			}

		}
	}
}//Actually 220 lines of code