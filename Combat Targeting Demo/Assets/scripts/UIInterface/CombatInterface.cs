using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Harris.UIInterface
{
	using UnityEngine;
	using UnityEngine.UI;
	using TMPro;
	using Harris;
	using Harris.NPC;

    public class CombatInterface : MonoBehaviour
    {   

		public static CombatInterface Instance;

		private int strongestTargetPriority;
		public int StrongestTargetPriority {get => strongestTargetPriority; set => strongestTargetPriority = value;}

		private int nearestTargetPriority;
		public int NearestTargetPriority {get => nearestTargetPriority; set => nearestTargetPriority = value;}

		//Messages to the SoftLock System
        public static event Action<int> _onStrongestTargetPriorityChangedUI;//to softlock
        public static event Action<int> _onNearestTargetPriorityChangedUI;//to softlock
		public static event Action<SoftLockMode> _onSoftLockModeChanged;//to UI

		//Messages from the SoftLock System
		public static event Action<Enemy, Enemy> _onSoftLockTargetChanged;//to UI

		//Actions to request values from SoftLock System
		public static event Action _requestStrongestTargetPriority;//subscribe to event in choosetarget module
		public static event Action _requestNearestTargetPriority;//subscribe to event in choosetarget module

		//requesting values from SoftLock System
		public int requestValue(string valueText)
		{
			switch (valueText)
			{
				case "StrongestTargetPriority":
					_requestStrongestTargetPriority?.Invoke();
					return strongestTargetPriority;
				break;

				case "NearestTargetPriority":
					_requestNearestTargetPriority?.Invoke();
					return nearestTargetPriority;
				break;

				default:
					return - 1;
			}
		}

		//Sending messages
		public void send(string msg, int value = -1)
		{
			switch(msg)
			{
				//messages to softlock system
				case "StrongestTargetPriorityChanged":
					_onStrongestTargetPriorityChangedUI?.Invoke(value);//subscribe to event in choosetarget module
				break;

				case "NearestTargetPriorityChanged":
					_onNearestTargetPriorityChangedUI?.Invoke(value);//subscribe to event in choosetarget module
				break;

				//messages from softlock system to UI
				case "SoftLockTargetChanged":
					//_onSoftLockTargetChanged?.Invoke();//subscribe to event in UI module
				break;
			}
		}

		public void send(string msg, SoftLockMode value)
		{
			switch(msg)
			{
				//messages from softlock system to UI
				case "SoftLockModeChanged":
					_onSoftLockModeChanged?.Invoke(value);//subscribe to event in UI module
				break;
			}
		}


        private void Awake()
		{
			Instance = this;
			//StrongestTargetPriorityDropDown._onPriorityChanged += handleStrongestTargetPriorityChanged;
			//NearestTargetPriorityDropDown._onPriorityChanged += handleNearestTargetPriorityChanged;
		}

        /*private void handleStrongestTargetPriorityChanged(int newPriority)
		{
			_onStrongestTargetPriorityChanged?.Invoke(newPriority);
		}

		private void handleNearestTargetPriorityChanged(int newPriority)
		{
			_onNearestTargetPriorityChanged?.Invoke(newPriority);
		}*/

    }
}
