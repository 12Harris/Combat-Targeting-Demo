// project armada

namespace Harris
{
	using UnityEngine;
	using System.Collections;
	using System;

	//The only sense next to visibility that is currently implemented is the hearing sense
	[AddComponentMenu("Sensor/Hearing")]
	public class Hearing : Sensor
	{
		[Min(0f)]
		[SerializeField] private float _distance = 5f;
		[SerializeField] private Transform[] _targets = { };

		private void Update()
		{
			FindTargets();
		}

		private void FindTargets()
		{
			//check if the npc can hear a target

			foreach (var target in _targets)
			{
				//If we hear a target then only add it to the list once and not every frame
				

				//If we stopped hearing a target then remove it from the list
				
			}
			
			//return null;
		}
	}
}