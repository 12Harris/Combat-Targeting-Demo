// project armada

#pragma warning disable 0414

namespace Harris.Player
{
	using UnityEngine;
	using System.Collections;
	using System.Collections.Generic;
	using Harris.Util;
	using Harris.Perception;
	using Harris.Player.PlayerLocomotion;
	using Harris.Player.PlayerLocomotion.Rotation;
	using Harris.Player.Combat;
	using Harris.Player.Combat.Weapons;
	using Harris.NPC;

	public class PlayerController : MonoBehaviour
	{
		[SerializeField] private Animator _animator;
		private Sensor[] _sensors = null;

		private static PlayerController instance;
		public static PlayerController Instance => instance;

		[SerializeField]
		private Transform headTransform;

		public Transform HeadTransform => headTransform;

		[SerializeField] 
		private Transform bodyTransform;
		public Transform BodyTransform => bodyTransform;

		[SerializeField]
		private Weapon currentWeapon;

		public Weapon CurrentWeapon => currentWeapon;


		[SerializeField]
		private PlayerRotationController playerRotationController;
		public PlayerRotationController PlayerRotationController => playerRotationController;

		private void Awake()
		{
			instance = this;
			//SoftLock._onSoftLockTargetChanged += handleSoftLockTargetChanged;
			TargetChooser._onSoftLockTargetChanged += handleSoftLockTargetChanged;
			TargetChooser._onSoftLockTargetLost += handleSoftLockTargetLost;
		}

		private void Start()
		{

		}


		public T GetSensor<T>() where T : Sensor
		{
			//return GetFirstExact<T, Sensor>(ref _sensors);
			return GOComponents.GetFirstExact<T, Sensor>(gameObject, ref _sensors);
		}

		private void handleSoftLockTargetChanged(EnemyController oldTarget, EnemyController newTarget)
		{

		}


		private void handleSoftLockTargetLost()
		{

		}

		private void Update()
		{

		}
	}
}