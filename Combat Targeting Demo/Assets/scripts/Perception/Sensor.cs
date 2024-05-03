// project armada

namespace Harris
{
	using UnityEngine;
	using System.Collections.Generic;
	using System;

	public abstract class Sensor : MonoBehaviour
	{
		public List<Transform> _targetsSensed;
		public List<Transform> TargetsSensed => _targetsSensed;
		public bool HasTarget => _targetsSensed.Count > 0;

		private void Awake()
		{
			_targetsSensed = new List<Transform>();
		}

		public bool containsTarget(Transform target)
		{
			return _targetsSensed.Contains(target);
		}

		public Transform FindNearestTarget()
		{
			float minDistance = Mathf.Infinity;
			Transform nearestTarget = null;

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