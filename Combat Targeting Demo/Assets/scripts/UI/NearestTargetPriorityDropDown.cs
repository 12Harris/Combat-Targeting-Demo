// project armada

#pragma warning disable 0414

namespace Harris.UI
{
	using UnityEngine;
	using UnityEngine.UI;
	using System;
	using Harris.UIInterface;

	internal class NearestTargetPriorityDropDown : UIDropDown
	{
		private int currentOptionValue;

        [SerializeField]
        private StrongestTargetPriorityDropDown strongestTargetPriorityDropDown;

		public override void Update()
		{
			base.Update();

			//problem: DIRECT DEPENDENCY
			//var oldPriority = CombatInterface.Instance.StrongestTargetPriority;
			var oldPriority = CombatInterface.Instance.requestValue("NearestTargetPriority");

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

				//to softlock
				CombatInterface.Instance.send("NearestTargetPriorityChanged", currentOptionValue);

				oldPriority = currentOptionValue;
			}
		}
	}
}