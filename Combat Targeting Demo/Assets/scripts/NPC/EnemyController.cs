// project armada

#pragma warning disable 0414

namespace Harris.NPC
{
	using UnityEngine;
	using System.Collections;
	using System.Collections.Generic;
	using Harris.Util;
	using Harris.Perception;
	using UnityEngine.UI;

	public class EnemyController : MonoBehaviour
	{
		[SerializeField] private Animator _animator;
		private Sensor[] _sensors = null;

		private RotateObject rotator;

		public RotateObject Rotator => rotator;

		[SerializeField]
        private int strength;
        public int Strength {get => strength; set => strength = value;}

        private int targetPriority;
        public int TargetPriority => targetPriority;

        [SerializeField]
        private Image highLight;

		[SerializeField]
		private Transform highLightTransform;

        public Image HighLight => highLight;

		private void Awake()
		{
			rotator = transform.gameObject.GetComponent<RotateObject>();
			if(rotator == null)
				Debug.Log("enemy rotator = null ?");
		}

		private void Start()
		{
			HighLight.enabled = false;
		}


		public T GetSensor<T>() where T : Sensor
		{
			//return GetFirstExact<T, Sensor>(ref _sensors);
			return GOComponents.GetFirstExact<T, Sensor>(gameObject, ref _sensors);
		}
		
		private void Update()
		{
			highLightTransform.LookAt(transform.position - Vector3.forward);
		}

		public void setPriority(int priority)
        {
            targetPriority = priority;
        }
	}
}