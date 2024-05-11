// project armada

#pragma warning disable 0414

// project armada
using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
//using System;
using System.Linq;
using Unity.Collections;


namespace Harris.Player.Combat
{
	using Harris.UIInterface;
	using Harris.NPC;
	using Harris.Perception;
	using Harris.Util;
	using Harris.Player.Combat.Weapons;
	using System;

	[AddComponentMenu("Combat/ChooseTarget")]
	internal class TargetChooser : MonoBehaviour
	{
		private IDictionary<PriorityCondition, int > priorities = new Dictionary<PriorityCondition, int>();

		public IDictionary<PriorityCondition, int> Priorities => priorities;

		public delegate bool PriorityCondition(Enemy target);

		private int defaultTargetPriority = 3;

		private Enemy chosenTarget = null;

		public Enemy ChosenTarget {get => chosenTarget; set => chosenTarget = value;}

		[SerializeField]
		private int strongestTargetPriority;

		public int StrongestTargetPriority { get => strongestTargetPriority; set => strongestTargetPriority = value;}

		[SerializeField]
		private int nearestTargetPriority;

		public int NearestTargetPriority { get => nearestTargetPriority; set => nearestTargetPriority = value;}

		[SerializeField]
		private float timer = 0f;
		private float interval = .1f;

		private bool enableTargetChoosing;

		private bool cancelTargeting;

		public static TargetChooser Instance;

		public static event Action<Enemy, Enemy> _onSoftLockTargetChanged;
		public static event Action _onSoftLockTargetLost;
		private Enemy oldTarget = null, oldTarget2;
		
		public Enemy OldTarget{get => oldTarget; set => oldTarget = value;}
		public Enemy OldTarget2 => oldTarget2;

		private SoftLockMode softLockMode = SoftLockMode.LONGRANGE;
		public SoftLockMode SoftLockMode => softLockMode;

		public bool IsStrongestTarget(Enemy target)
		{

			int strengthCheck = 0;

			foreach(SensorTarget t in PlayerControllerInstance.Instance.GetSensor<Sight>().TargetsSensed)
			{

				var enemy = t.transform.parent.GetComponent<Enemy>();

				if(enemy == target)
					continue;

				if(enemy.Strength > strengthCheck)
					strengthCheck = enemy.Strength;
			}

			return target.Strength > strengthCheck;

		}

		public bool IsNearestTarget(Enemy target)
		{
			//return GetComponent<Sight>().FindNearestTarget() == target;

			SensorTarget t = target.transform.gameObject.GetComponentInChildren<SensorTarget>();
			return PlayerControllerInstance.Instance.GetSensor<Sight>().FindNearestTarget() == t;
 		}

		private int getDefaultTargetPriority()
		{
			return defaultTargetPriority;
		}

		private void calculateTargetPriority(Enemy target, bool longRange)
		{

			target.setPriority(getDefaultTargetPriority());

			//Do a Linq-Query to sort the priorities in ascending order
			var sortedPriorities = from entry in priorities orderby entry.Value ascending select entry;

			float looseTargetDistance = longRange ? 6 : 0;
			float senseTargetDistance = longRange ? 10 : 3;

			foreach (KeyValuePair<PriorityCondition,int> p in sortedPriorities)
			{
				//What should happen if more than 1 condition is true for the target?
				//lets assume the strongest target is also the nearest target,
				//then its final priority would be 2 and not 1, which is wrong behaviour
				PriorityCondition pCondition = p.Key;

				var v1 = PlayerControllerInstance.Instance.BodyTransform.position;
				v1.y = 0;

				var v2 = target.transform.position;
				v2.y = 0;

				float distanceToTargetXZ  = Vector3.Distance(v1,v2);

				if(pCondition(target) && getHeadAngleToBody() < 45)
				{
					//if(!PlayerControllerInstance.Instance.ResettingHeadRotation || target != oldTarget)

					if(distanceToTargetXZ > looseTargetDistance && distanceToTargetXZ < senseTargetDistance)
					{
						if(!PlayerControllerInstance.Instance.ResettingHeadRotation || target != oldTarget2)
						{
							Debug.Log("oldtarget = " + oldTarget2 + ", target = " + target);
							target.setPriority(priorities[pCondition]);
							return;
						}
					}
				}
			}
		}

		private float getHeadAngleToTarget(Enemy target)
		{
			var directionToTargetXZ = target.transform.position - PlayerControllerInstance.Instance.HeadTransform.position;
				directionToTargetXZ.y = 0;

			Debug.Log("head angle to target = " +  Vector3.Angle(PlayerControllerInstance.Instance.HeadTransform.forward, directionToTargetXZ));
			return Vector3.Angle(PlayerControllerInstance.Instance.HeadTransform.forward, directionToTargetXZ);
		}

		private float getHeadAngleToBody()
		{
			return Vector3.Angle(PlayerControllerInstance.Instance.HeadTransform.forward, PlayerControllerInstance.Instance.BodyTransform.forward);
		}

		private void Awake()
		{
			Instance = this;
			enableTargetChoosing = true;

			CombatInterface._onStrongestTargetPriorityChangedUI += handleStrongestTargetPriorityChanged;
			CombatInterface._onNearestTargetPriorityChangedUI += handleNearestTargetPriorityChanged;

			CombatInterface._requestStrongestTargetPriority += sendStrongestTargetPriority;
			CombatInterface._requestNearestTargetPriority += sendNearestTargetPriority;
		}

		private void Start()
		{
			//Set the priorities for each priority condition
			//they MUST be added in the order from highest priority to lowest priority

			priorities.Add(IsNearestTarget, nearestTargetPriority);
			priorities.Add(IsStrongestTarget, strongestTargetPriority);
	
		}

		private void handleStrongestTargetPriorityChanged(int newPriority)
		{
			Debug.Log("new strongest target priority = " + newPriority);
			strongestTargetPriority = newPriority;
			priorities[IsStrongestTarget] = strongestTargetPriority;
		}

		private void handleNearestTargetPriorityChanged(int newPriority)
		{
			Debug.Log("new nearest target priority = " + newPriority);
			nearestTargetPriority = newPriority;
			priorities[IsNearestTarget] = nearestTargetPriority;
		}

		private void sendStrongestTargetPriority()
		{
			CombatInterface.Instance.StrongestTargetPriority = strongestTargetPriority;
		}

		private void sendNearestTargetPriority()
		{
			CombatInterface.Instance.NearestTargetPriority = nearestTargetPriority;
		}

		private void calculateTargetPriorities(bool longRange)
		{
			//foreach(Enemy target in sensorTargets)
			foreach(var target in PlayerControllerInstance.Instance.GetSensor<Sight>().TargetsSensed)
			{
				var enemy = target.transform.parent.GetComponent<Enemy>();
				calculateTargetPriority(enemy, longRange);
				Debug.Log("Target " + enemy.transform.gameObject.name + " has priority: " + enemy.TargetPriority);
			}
		}

		private Enemy chooseTarget()
		{
			int currentPriority = 0;

			//To store targets with same priority
			var arr = new List<Enemy>();

			if(PlayerControllerInstance.Instance.GetSensor<Sight>().TargetsSensed.Count == 0)
				return null;

			while(arr.Count == 0)
			{
				currentPriority++;
				foreach(var target in PlayerControllerInstance.Instance.GetSensor<Sight>().TargetsSensed)
				{
					var enemy = target.transform.parent.GetComponent<Enemy>();
					if(enemy.TargetPriority == currentPriority)
					{
						arr.Add(enemy);
					}
				}
			}
			
			if(currentPriority != defaultTargetPriority)
			{
				//only 1 target has the highest priority, so return that
				if(arr.Count == 1)
				{
					return arr[0];
				}

				//more than 1 target has the highest priority, so return a random target with highest priority
				int randIndex = UnityEngine.Random.Range(0, arr.Count);
				return arr[randIndex];
			}
			return null;

		}

		
		public void Update()
		{

			if(enableTargetChoosing)
			{
				Debug.Log("target choosing enabled...");

				timer += Time.deltaTime;
				//var oldTarget = chosenTarget;
				/*oldTarget = chosenTarget;

				if(chosenTarget != null)
					oldTarget2 = chosenTarget;*/

				if(timer > interval)
				{

					oldTarget = chosenTarget;

					if(chosenTarget != null)
						oldTarget2 = chosenTarget;

					if (PlayerControllerInstance.Instance.CurrentWeapon is LongRangeWeapon)
					{
						softLockMode = SoftLockMode.LONGRANGE;
						calculateTargetPriorities(true);
					}
					else
					{
						softLockMode = SoftLockMode.SHORTRANGE;
						calculateTargetPriorities(false);						
					}

					chosenTarget = chooseTarget();

					Debug.Log("chosen target is: " + chosenTarget);

					if(chosenTarget != oldTarget)
					{
						if(oldTarget != null)
								oldTarget.HighLight.enabled = false;

						if(chosenTarget != null)
						{
							chosenTarget.HighLight.enabled = true;

							_onSoftLockTargetChanged?.Invoke(oldTarget, chosenTarget);
						}
						else
						{
							Debug.Log("TARGET LOST(TC)");
							_onSoftLockTargetLost?.Invoke();
						}

					}

					timer = 0f;
				}

				/*if(chosenTarget != oldTarget)
				{
					if(chosenTarget != null)
					{

						if(oldTarget != null)
							oldTarget.HighLight.enabled = false;
						
						chosenTarget.HighLight.enabled = true;

						_onSoftLockTargetChanged?.Invoke(oldTarget, chosenTarget);
					}
					else
					{
						Debug.Log("TARGET LOST(TC)");
						_onSoftLockTargetLost?.Invoke();
					}

				}*/
			}
			else
			{
				chosenTarget = null;
				timer = 0f;
			}
		}
	}
}