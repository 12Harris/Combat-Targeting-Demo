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

	[AddComponentMenu("Combat/ChooseTarget")]
	internal class ChooseTarget : MonoBehaviour
	{
		private IDictionary<PriorityCondition, int > priorities = new Dictionary<PriorityCondition, int>();

		public IDictionary<PriorityCondition, int> Priorities => priorities;

		public delegate bool PriorityCondition(Enemy target);

		private int defaultTargetPriority = 3;

		private Enemy chosenTarget;

		public Enemy ChosenTarget => chosenTarget;

		[SerializeField]
		private int strongestTargetPriority;

		public int StrongestTargetPriority { get => strongestTargetPriority; set => strongestTargetPriority = value;}

		[SerializeField]
		private int nearestTargetPriority;

		public int NearestTargetPriority { get => nearestTargetPriority; set => nearestTargetPriority = value;}

		[SerializeField]
		private float timer = 0f;
		private float interval = 1f;

		private bool enableTargetChoosing;

		private bool cancelTargeting;

		public static ChooseTarget Instance;

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

			Transform t = target.gameObject.transform;
			return GetComponent<PlayerController>().GetSensor<Sight>().FindNearestTarget() == t;
 		}

		private int getDefaultTargetPriority()
		{
			return defaultTargetPriority;
		}

		private void calculateTargetPriority(Enemy target)
		{

			target.setPriority(getDefaultTargetPriority());

			//Do a Linq-Query to sort the priorities in ascending order
			var sortedPriorities = from entry in priorities orderby entry.Value ascending select entry;

			foreach (KeyValuePair<PriorityCondition,int> p in sortedPriorities)
			{
				//What should happen if more than 1 condition is true for the target?
				//lets assume the strongest target is also the nearest target,
				//then its final priority would be 2 and not 1, which is wrong behaviour
				PriorityCondition pCondition = p.Key;
				if(pCondition(target) && getAngleToTarget(target) < 45)
				{
					//target.setPriority(sortedPriorities[pCondition]);
					target.setPriority(priorities[pCondition]);
					return;
				}
			}

		}

		private float getAngleToTarget(Enemy target)
		{
			var directionToTargetXZ = target.transform.position - PlayerControllerInstance.Instance.HeadTransform.position;
				directionToTargetXZ.y = 0;

			return Vector3.Angle(PlayerControllerInstance.Instance.HeadTransform.forward, directionToTargetXZ);
		}

		private void Awake()
		{
			Instance = this;
			enableTargetChoosing = true;

			CombatInterface._onStrongestTargetPriorityChangedUI += handleStrongestTargetPriorityChanged;
			CombatInterface._onNearestTargetPriorityChangedUI += handleNearestTargetPriorityChanged;

			CombatInterface._requestStrongestTargetPriority += sendStrongestTargetPriority;
			CombatInterface._requestNearestTargetPriority += sendNearestTargetPriority;

			//UIEventListener._onStrongestTargetPriorityChanged += handleStrongestTargetPriorityChanged;
			//UIEventListener._onNearestTargetPriorityChanged += handleNearestTargetPriorityChanged;
		}

		private void Start()
		{
			//Set the priorities for each priority condition
			//they MUST be added in the order from highest priority to lowest priority
			//priorities.Add(IsStrongestTarget, 1);
			//priorities.Add(IsNearestTarget, 2);

			//Sorted => priority = 1
			//Unsorted => priority = 2(false)
			priorities.Add(IsNearestTarget, nearestTargetPriority);
			priorities.Add(IsStrongestTarget, strongestTargetPriority);
			//PlayerController.Instance.GetSensor<Sight>()._onTargetDetected += AddTarget;
			//PlayerController.Instance.GetSensor<Sight>()._onTargetLost += RemoveTarget;
	
		}

		private void handleStrongestTargetPriorityChanged(int newPriority)
		{
			strongestTargetPriority = newPriority;
			priorities[IsStrongestTarget] = strongestTargetPriority;
			//Debug.Log("strongest target priority = " + newPriority);
		}

		private void handleNearestTargetPriorityChanged(int newPriority)
		{
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

		private void calculateTargetPriorities()
		{
			//foreach(Enemy target in sensorTargets)
			foreach(SensorTarget target in PlayerControllerInstance.Instance.GetSensor<Sight>().TargetsSensed)
			{
				var enemy = target.transform.parent.GetComponent<Enemy>();
				calculateTargetPriority(enemy);
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
				int randIndex = Random.Range(0, arr.Count);
				return arr[randIndex];
			}
			return null;

		}

		
		public void Update()
		{
			//Debug.Log("there are " + sensorTargets.Count + " targets!");
			/*if (Input.GetKeyDown(KeyCode.Tab) && sensorTargets.Count > 0)
			{
				enableTargetChoosing = !enableTargetChoosing;
			}*/

			/*if(sensorTargets.Count == 0)
			{
				//enableTargetChoosing = false;
				chosenTarget = null;
			}
			else
			{
				enableTargetChoosing = true;
			}*/

			if(enableTargetChoosing)
			{
				Debug.Log("target choosing enabled...");
			
				timer += Time.deltaTime;
				if(timer > interval)
				{
					calculateTargetPriorities();
					chosenTarget = chooseTarget();

					if(chosenTarget == null)
						Debug.Log("chosen target is null!");
					else
						Debug.Log("chosen target is: " + chosenTarget);		
					timer = 0f;
				}
			}

			else
			{
				chosenTarget = null;
				timer = 0f;
			}
		}
	}
}