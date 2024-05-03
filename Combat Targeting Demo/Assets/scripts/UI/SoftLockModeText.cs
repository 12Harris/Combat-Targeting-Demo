// project armada

#pragma warning disable 0414

namespace Harris.UI
{
	using UnityEngine;
	using UnityEngine.UI;
	using TMPro;
	using Harris.UIInterface;

	internal class SoftLockModeText : UIText
	{
		public override void Awake()
		{
			CombatInterface._onSoftLockModeChanged += handleSoftLockModeChanged;
		}

		private void handleSoftLockModeChanged(SoftLockMode newSoftLockMode)
		{
            if(newSoftLockMode == SoftLockMode.MOUSE)
			    TextMesh.text = "Mouse Targeting Activated";
            else
                TextMesh.text = "Priority Targeting Activated";
		}

	}
}