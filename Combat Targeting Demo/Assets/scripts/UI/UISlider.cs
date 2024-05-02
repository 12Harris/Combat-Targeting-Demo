// project armada

#pragma warning disable 0414

namespace Harris.UI
{
	using UnityEngine;
	using UnityEngine.UI;

	internal abstract class UISlider : MonoBehaviour
	{

		[SerializeField] private Slider slider;
		public Slider Slider => slider;

        private int minValue;
        public int MinValue {get=> minValue; set => minValue = value;}
        private int maxValue;
        public int MaxValue {get=> maxValue; set => maxValue = value;}
		private int currentValue;
		public int CurrentValue {get => currentValue; set => currentValue = value;}


		public static UISlider Instance;

		private void Awake()
		{
			// init references
			Instance = this;

		}

		private void Start()
		{
			// init logic
			slider.minValue = MinValue;
			slider.maxValue = MaxValue;
		}

		public virtual void Update()
		{
			currentValue = (int)slider.value;
			// do something with turquoise
			//PlayerMovement.instance.JumpForce = slider.value;
		}
	}
}