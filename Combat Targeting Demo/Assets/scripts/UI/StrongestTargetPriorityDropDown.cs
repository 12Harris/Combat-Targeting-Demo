// project armada

#pragma warning disable 0414

namespace Harris.UI
{
	using UnityEngine;
	using UnityEngine.UI;
	using System;

	internal class StrongestTargetPriorityDropDown : UIDropDown
	{
        public static event Action<int> _onPriorityChanged;
        private int currentOptionValue;

        [SerializeField]
        private NearestTargetPriorityDropDown nearestTargetPriorityDropDown;

		public override void Update()
		{
			base.Update();

            var oldPriority = ChooseTarget.Instance.StrongestTargetPriority;
            int.TryParse(Dropdown.options[CurrentValue].text, out currentOptionValue);
            //ChooseTarget.Instance.StrongestTargetPriority = CurrentValue;
            Debug.Log("current strongest target priority value = " + currentOptionValue);

			if(oldPriority != currentOptionValue)
			{
                //update the other drop down
                if(CurrentValue == 0)
                    nearestTargetPriorityDropDown.Dropdown.value = 1;        
                else
                    nearestTargetPriorityDropDown.Dropdown.value = 0;

                Debug.Log("strongest target priority value = " + currentOptionValue);
				_onPriorityChanged.Invoke(currentOptionValue);
				oldPriority = currentOptionValue;
			}
		}
	}
}