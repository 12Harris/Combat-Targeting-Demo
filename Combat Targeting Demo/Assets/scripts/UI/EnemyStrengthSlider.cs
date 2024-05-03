// project armada

#pragma warning disable 0414

namespace Harris.UI
{
	using UnityEngine;
	using UnityEngine.UI;
	using System;
	using Harris.NPC;

	internal class EnemyStrengthSlider : UISlider
	{
        [SerializeField] private Enemy npc;
		//[SerializeField] private EnemyStrengthText enemyStrengthText;

		public event Action _onEnemyStrengthChanged;
		private void Awake()
		{
			// init references
			MinValue = 1;
            MaxValue = 10;

		}

		public override void Update()
		{
			base.Update();
			// do something with turquoise
			var oldStrength = npc.Strength;
            npc.Strength = CurrentValue;

			if(oldStrength != npc.Strength)
			{
				_onEnemyStrengthChanged?.Invoke();
				oldStrength = npc.Strength;
			}

		}
	}
}