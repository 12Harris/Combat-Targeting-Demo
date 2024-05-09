// project armada

#pragma warning disable 0414

namespace Harris.UI
{
	using UnityEngine;
	using UnityEngine.UI;
    using TMPro;

	internal class UIDropDown : MonoBehaviour
	{

		[SerializeField] private TMP_Dropdown dropDown;
		public TMP_Dropdown Dropdown => dropDown;
		private int currentValue;
		public int CurrentValue {get => currentValue; set => currentValue = value;}

        //private OptionData currentOption;
        //public OptionData CurrentOption {get => currentOption; set => currentOption = value;}

		public static UIDropDown Instance;

		private void Awake()
		{
			// init references
			Instance = this;

		}

		private void Start()
		{

		}

		public virtual void Update()
		{
			currentValue = (int)dropDown.value;

			Debug.Log("UI drop down current value = " + currentValue);
			// do something with turquoise
			//PlayerMovement.instance.JumpForce = slider.value;
		}
	}
}