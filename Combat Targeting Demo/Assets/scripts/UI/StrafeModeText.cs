// project armada

#pragma warning disable 0414

namespace Harris.UI
{
	using UnityEngine;
	using UnityEngine.UI;
	using TMPro;

	internal class StrafeModeText : UIText
	{
		public override void Awake()
		{
			StrafeState._onEnter += handleStrafeStateEntered;
			StrafeState._onExit += handleStrafeStateLeft;
		}

		private void handleStrafeStateEntered()
		{
			TextMesh.text = "Strafe Mode Activated";
		}

		private void handleStrafeStateLeft()
		{
			TextMesh.text = "";
		}
	}
}