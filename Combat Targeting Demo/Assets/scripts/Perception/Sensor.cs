// project armada

namespace Harris.Perception
{
	using UnityEngine;
	using System.Collections.Generic;
	using System;

	public abstract class Sensor : MonoBehaviour
	{
		public List<SensorTarget> _targetsSensed;
		public List<SensorTarget> TargetsSensed => _targetsSensed;
		public bool HasTarget => _targetsSensed.Count > 0;

		private void Awake()
		{
			_targetsSensed = new List<SensorTarget>();
		}

		public bool containsTarget(SensorTarget target)
		{
			return _targetsSensed.Contains(target);
		}

		public SensorTarget FindNearestTarget()
		{
			float minDistance = Mathf.Infinity;
			SensorTarget nearestTarget = null;

			foreach (var target in _targetsSensed)
			{
				float distance = Vector3.Distance(target.transform.position, transform.position);
				if (distance < minDistance)
				{
					nearestTarget = target;
					minDistance = distance;
				}
			}
			return nearestTarget;
		}
	}
}