// project armada

#pragma warning disable 0414

namespace Harris.UI
{
	using UnityEngine;
	using UnityEngine.UI;
	using TMPro;

	internal class EnemyStrengthText : UIText
	{
        [SerializeField]
        private EnemyStrengthSlider slider;

		private void Awake()
		{
			// init references
            slider._onEnemyStrengthChanged += handleEnemyStrengthChanged;
		}

		private void Start()
		{
			// init logic
		}

		public void handleEnemyStrengthChanged()
		{
			TextMesh.text = "Strength: " + slider.CurrentValue;
		}
        
        private void Update()
        {
            //TextMesh.text = "Strengtho: " + slider.CurrentValue;
        }
	}
}