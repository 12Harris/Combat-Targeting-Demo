// project armada

#pragma warning disable 0414

namespace Harris.Combat
{
	using UnityEngine;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine.UI;
	using System;

	internal enum SoftLockMode
	{
		PRIORITY,
		MOUSE,
	}

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
		public static event Action<SoftLockMode> _onSoftLockModeChanged;

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
			PlayerThrowingObjectAnimationState._onAnimationStopped += handleThrowAnimationStopped;
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

			SensorTarget target = entity.transform.parent.GetComponentInChildren<SensorTarget>();

			var directionToTargetXZ = target.transform.position - transform.position;
			directionToTargetXZ.y = 0;

			//angle between player and target must be < 90
			var angle1 = Vector3.Angle(transform.forward, directionToTargetXZ);

			//angle between spine and forward must also be < 90
			var angle2 = Vector3.Angle(transform.forward, playerSpineBone.forward);

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

			if(enableSoftLock && mode == SoftLockMode.MOUSE && GetComponent<Player>().GetSensor<Sight>().containsTarget(target))
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

		/*private void calculateSoftLockTarget()
		{
			var potentialTargets = getLeftAndRightTarget();

			var oldSoftLockTarget = softlockTarget;

			if(potentialTargets.Count > 0)				
			{
				Debug.Log("There are: " + potentialTargets.Count + " potential targets");
								
				softlockTarget = potentialTargets[0].transform;

				if(potentialTargets.Count > 1)	
				{
					softlockTarget = Vector3.Distance(potentialTargets[0].transform.position, transform.position) < 
						Vector3.Distance(potentialTargets[1].transform.position, transform.position) ? potentialTargets[0].transform : potentialTargets[1].transform;
				}

				//if(oldSoftLockTarget != null && softlockTarget != oldSoftLockTarget)
				if(softlockTarget != oldSoftLockTarget)
				{
					_onSoftLockTargetChanged?.Invoke(softlockTarget);
					oldSoftLockTarget.parent.Find("Highlight").gameObject.transform.Find("Image").gameObject.GetComponent<Image>().enabled = false;
				}

				GameObject highLight = softlockTarget.parent.Find("Highlight").gameObject;
					highLight.transform.Find("Image").gameObject.GetComponent<Image>().enabled = true;
			}
			else
			{
				Debug.Log("NO POTENTIAL TARGETS");
				if(softlockTarget)
					softlockTarget.parent.Find("Highlight").gameObject.transform.Find("Image").gameObject.GetComponent<Image>().enabled = false;
				softlockTarget = null;
			}
			
		}*/

		//private SensorTarget[] getLeftAndRightTarget()
		/*private List<SensorTarget> getLeftAndRightTarget()
		{

			var arr = new List<SensorTarget>();

			var rightTargets = getRightTargets();

			var leftTargets = getLeftTargets();

			SensorTarget leftTarget, rightTarget;

			var dirXZ = transform.forward;
			dirXZ.y = 0;
			
			var minAngle = 360f;

			leftTarget = rightTarget = null;

			//Get left target
			foreach(SensorTarget target in leftTargets)
			{
				Debug.Log("there are: " + leftTargets.Count + " left targets");
				var directionToTargetXZ = target.transform.position - playerSpineBone.position;
				directionToTargetXZ.y = 0;

				if(Vector3.Angle(directionToTargetXZ, dirXZ) < minAngle)
				{
					leftTarget = target;
					minAngle = Vector3.Angle(directionToTargetXZ, dirXZ);
				}
			}	

			//Get right target
			foreach(SensorTarget target in rightTargets)
			{
				Debug.Log("there are: " + rightTargets.Count + " right targets");
				var directionToTargetXZ = target.transform.position - playerSpineBone.position;
				directionToTargetXZ.y = 0;

				if(Vector3.Angle(directionToTargetXZ, dirXZ) < minAngle)
				{
					rightTarget = target;
					minAngle = Vector3.Angle(directionToTargetXZ, dirXZ);
				}
			}	

			GameObject highLight;

			if(leftTarget)
			{
				arr.Add(leftTarget);
			}

			if(rightTarget)
			{
				arr.Add(rightTarget);
				//highLight.transform.Find("Image").gameObject.GetComponent<Image>().enabled = true;
			}

			return arr;
		}*/

		/*private List<SensorTarget> getLeftTargets()
		{
			return getTargets("left");
		}

		private List<SensorTarget> getRightTargets()
		{
			return getTargets("right");
		}*/

		/*private List<SensorTarget> getTargets(string relative)
		{
			var result = new List<SensorTarget>();

			var dirXZ = transform.forward;
			dirXZ.y = 0;

			int directionToCheck = relative == "left" ? 1 : -1;

			foreach(SensorTarget target in GetComponent<Player>().GetSensor<Sight>().TargetsSensed)
			{
				//SensorTarget currentTarget = GetComponent<Player>().GetSensor<Sight>().TargetsSensed[i];
				var directionToTargetXZ = target.transform.position - playerSpineBone.position;

				//Get the target that is closest to the direction the player is facing
				LeftRightTest test = new LeftRightTest(dirXZ,directionToTargetXZ,dirXZ, Vector3.up);

				if(test.DirNum == directionToCheck)
				{
					result.Add(target);
				}
			}

			return result;
		}*/

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
					_onSoftLockModeChanged?.Invoke(mode);			
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