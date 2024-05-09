// project armada

#pragma warning disable 0414

namespace Harris.UI
{
	using UnityEngine;
	using UnityEngine.UI;
	using System;
	using Harris.UIInterface;

	internal class StrongestTargetPriorityDropDown : UIDropDown
	{
        private int currentOptionValue;

        [SerializeField]
        private NearestTargetPriorityDropDown nearestTargetPriorityDropDown;

		public override void Update()
		{
			base.Update();

			//problem: DIRECT DEPENDENCY
            //var oldPriority = ChooseTarget.Instance.StrongestTargetPriority;
			var oldPriority = CombatInterface.Instance.requestValue("StrongestTargetPriority");

			Debug.Log("OLD PRIORITY(strongest target) = " +  oldPriority);

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

				//to softlock
				CombatInterface.Instance.send("StrongestTargetPriorityChanged", currentOptionValue);

				oldPriority = currentOptionValue;
			}
		}
	}
}