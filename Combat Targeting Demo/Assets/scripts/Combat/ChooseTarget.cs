// project armada

#pragma warning disable 0414

// project armada
using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using Unity.Collections;


namespace Harris.Combat
{
	using UnityEngine;

	internal class ChooseTarget : MonoBehaviour
	{
		private IDictionary<PriorityCondition, int > priorities = new Dictionary<PriorityCondition, int>();

		public IDictionary<PriorityCondition, int> Priorities => priorities;

		public delegate bool PriorityCondition(SensorTarget target);

		private int defaultTargetPriority = 3;

		[SerializeField]
		private List<SensorTarget> sensorTargets;

		private SensorTarget chosenTarget;

		public SensorTarget ChosenTarget => chosenTarget;

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

		[SerializeField]
		private Transform playerSpineBone;

		//public static event Action<SensorTarget> _onTargetDetected;

		//PRIORITY CONDITIONS

		//should be generic: AddTarget<T>(T target) : where T : NPC
		public void AddTarget(SensorTarget target)
		{
			//if(!sensorTargets.Contains(target))
				//sensorTargets.Add(target);
			
			if(sensorTargets.Contains(target))
				return;

			var directionToTargetXZ = target.transform.position - transform.position;
			directionToTargetXZ.y = 0;

			var angle = Vector3.Angle(transform.forward, directionToTargetXZ);

			if(angle < 90f)
			{
				sensorTargets.Add(target);
			}
		}

		public void RemoveTarget(SensorTarget target)
		{
			if(sensorTargets.Contains(target))
				sensorTargets.Remove(target);
			
		}

		public bool IsStrongestTarget(SensorTarget target)
		{

			int strengthCheck = 0;

			foreach(SensorTarget t in sensorTargets)
			{
				if(t == target)
					continue;

				if(t.Strength > strengthCheck)
					strengthCheck = t.Strength;
			}

			return target.Strength > strengthCheck;

		}

		public bool IsNearestTarget(SensorTarget target)
		{
			//return GetComponent<Sight>().FindNearestTarget() == target;
			return GetComponent<Player>().GetSensor<Sight>().FindNearestTarget() == target;
 		}

		private int getDefaultTargetPriority()
		{
			return defaultTargetPriority;
		}

		private void calculateTargetPriority(SensorTarget target)
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
				if(pCondition(target))
				{
					//target.setPriority(sortedPriorities[pCondition]);
					target.setPriority(priorities[pCondition]);
					return;
				}
			}

		}

		private void Awake()
		{
			Instance = this;
			enableTargetChoosing = true;
			sensorTargets = new List<SensorTarget>();
			PlayerThrowingObjectAnimationState._onAnimationStopped += handleThrowAnimationStopped;

			StrongestTargetPriorityDropDown._onPriorityChanged += handleStrongestTargetPriorityChanged;
			NearestTargetPriorityDropDown._onPriorityChanged += handleNearestTargetPriorityChanged;
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
			Player.Instance.GetSensor<Sight>()._onTargetDetected += AddTarget;
			Player.Instance.GetSensor<Sight>()._onTargetRemoved += RemoveTarget;
	
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

		private void calculateTargetPriorities()
		{
			foreach(SensorTarget target in sensorTargets)
			{
				calculateTargetPriority(target);
				Debug.Log("Target " + target.transform.gameObject.name + " has priority: " + target.getPriority());
			}
		}

		private SensorTarget chooseTarget()
		{
			int currentPriority = 0;

			//To store targets with same priority
			var arr = new List<SensorTarget>();

			if(sensorTargets.Count == 0)
				return null;

			while(arr.Count == 0)
			{
				currentPriority++;
				foreach(var target in sensorTargets)
				{
					if(target.getPriority() == currentPriority)
					{
						arr.Add(target);
					}
				}
			}
			
			//only 1 target has the highest priority, so return that
			if(arr.Count == 1)
			{
				return arr[0];
			}

			//more than 1 target has the highest priority, so return a random target with highest priority
			int randIndex = Random.Range(0, arr.Count);
			return arr[randIndex];

		}

		private void getTargets()
		{
			sensorTargets.Clear();
			if(chosenTarget != null)
				sensorTargets.Add(chosenTarget);

			for(int i = 0; i < GetComponent<Player>().GetSensor<Sight>().TargetsSensed.Count; i++)
			{
				SensorTarget target = GetComponent<Player>().GetSensor<Sight>().TargetsSensed[i];

				var directionToTargetXZ = target.transform.position - transform.position;
				directionToTargetXZ.y = 0;

				var angle1 = Vector3.Angle(transform.forward, directionToTargetXZ);
				var angle2 = Vector3.Angle(transform.forward, playerSpineBone.forward);

				//if(!sensorTargets.Contains(target))

				//if angle < 90f addTarget or ...
				if(angle1 < 90f && angle2 < 90f)
				{
					if(target != chosenTarget)
						sensorTargets.Add(target);
				}

				//else
				/*else if(target == chosenTarget && !cancelTargeting)
				{
					sensorTargets.Add(target);
					cancelTargeting = true;
				
				}*/
			}

			/*for(int i =  sensorTargets.Count - 1; i >= 0; i--)
			{
				var directionToTargetXZ = sensorTargets[i].transform.position - transform.position;
				directionToTargetXZ.y = 0;

				var angle = Vector3.Angle(transform.forward, directionToTargetXZ);

				if(angle > 90f)
				{
					Debug.Log("Angle :) > 90");
					//sensorTargets.RemoveAt(i);

					if(sensorTargets[i] == chosenTarget)
					{
						//sensorTargets.RemoveAt(i);
						cancelTargeting = true;
					}
					
					else
						sensorTargets.RemoveAt(i);
				}
			}*/

			Debug.Log("SensorTarget count = " + sensorTargets.Count);

			//sensorTargets.RemoveAll(angleTowardsTargetGreaterThan90);
		}

		private void handleThrowAnimationStopped()
		{	
			if(chosenTarget == null)
				return;

			var directionToChosenTargetXZ = chosenTarget.transform.position - transform.position;
				directionToChosenTargetXZ.y = 0;

			var angle1 = Vector3.Angle(transform.forward, directionToChosenTargetXZ);
			var angle2 = Vector3.Angle(transform.forward, playerSpineBone.forward);

			Debug.Log("angle2 => " + angle2);

			if(angle1 > 90f || angle2 > 90f) chosenTarget = null;
		}

		private bool angleTowardsTargetGreaterThan90(SensorTarget target)
		{
			var directionToTargetXZ = target.transform.position - transform.position;
			directionToTargetXZ.y = 0;

			return Vector3.Angle(transform.forward, directionToTargetXZ) > 90;
		}

		public void Update()
		{
			//Debug.Log("there are " + sensorTargets.Count + " targets!");
			/*if (Input.GetKeyDown(KeyCode.Tab) && sensorTargets.Count > 0)
			{
				enableTargetChoosing = !enableTargetChoosing;
			}*/

			if(sensorTargets.Count == 0)
			{
				//enableTargetChoosing = false;
				chosenTarget = null;
			}
			else
			{
				enableTargetChoosing = true;
			}

			if(enableTargetChoosing)
			{
				Debug.Log("target choosing enabled...");
			
				timer += Time.deltaTime;
				if(timer > interval)
				{
					getTargets();
					calculateTargetPriorities();
					chosenTarget = chooseTarget();

					if(chosenTarget == null)
						Debug.Log("chosen target is null!");		
					timer = 0f;
				}
			}

			else if(enableTargetChoosing == false)
			{
				chosenTarget = null;
				timer = 0f;
			}
		}
	}
}