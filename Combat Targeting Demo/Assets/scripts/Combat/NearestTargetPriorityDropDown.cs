// project armada

#pragma warning disable 0414

namespace Harris.Combat
{
	using UnityEngine;
	using UnityEngine.UI;
	using System;

	internal class NearestTargetPriorityDropDown : UIDropDown
	{
        public static event Action<int> _onPriorityChanged;
		private int currentOptionValue;

        [SerializeField]
        private StrongestTargetPriorityDropDown strongestTargetPriorityDropDown;

		public override void Update()
		{
			base.Update();

            var oldPriority = ChooseTarget.Instance.NearestTargetPriority;
            int.TryParse(Dropdown.options[CurrentValue].text, out currentOptionValue);
            //ChooseTarget.Instance.StrongestTargetPriority = CurrentValue;
            Debug.Log("current nearest target priority value = " + currentOptionValue);

			if(oldPriority != currentOptionValue)
			{
                 //update the other drop down
                if(CurrentValue == 0)
                    strongestTargetPriorityDropDown.Dropdown.value = 1;        
                else
                    strongestTargetPriorityDropDown.Dropdown.value = 0;

                Debug.Log("nearest target priority value = " + currentOptionValue);
				_onPriorityChanged.Invoke(currentOptionValue);
				oldPriority = currentOptionValue;
			}
		}
	}
}