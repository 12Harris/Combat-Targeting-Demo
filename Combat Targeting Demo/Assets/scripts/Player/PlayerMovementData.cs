// project armada

#pragma warning disable 0414

namespace Harris.Player
{
	using UnityEngine;
	using UnityEngine.Events;
	using System;

	[CreateAssetMenu]
	internal class PlayerMovementData : ScriptableObject
	{
	
		public float airSpeed = 5;

		[Min(0.1f)]
		public float walkSpeed = 5;

		[Min(0.1f)]
		public float runSpeed = 8;

		[Min(0.1f)]
		public float sprintSpeed = 15;

		public float jumpForce;

		public bool useGravity;
		
		public float groundDecelerationSpeed;
	
		public float airDecelerationSpeed;

		public float maxSlopeAngle = 60f;

	}
}