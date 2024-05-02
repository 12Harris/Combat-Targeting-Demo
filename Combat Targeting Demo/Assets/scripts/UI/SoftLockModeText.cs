// project armada

#pragma warning disable 0414

namespace Harris.UI
{
	using UnityEngine;
	using UnityEngine.UI;
	using TMPro;

	internal class SoftLockModeText : UIText
	{
		public override void Awake()
		{
			SoftLock._onSoftLockModeChanged += handleSoftLockModeChanged;
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