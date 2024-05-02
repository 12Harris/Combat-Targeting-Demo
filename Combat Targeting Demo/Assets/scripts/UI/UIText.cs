// project armada

#pragma warning disable 0414

namespace Harris.UI
{
	using UnityEngine;
	using UnityEngine.UI;
	using TMPro;

	internal class UIText : MonoBehaviour
	{
		[SerializeField]
		private TextMeshProUGUI textMesh;

		public TextMeshProUGUI TextMesh {get => textMesh; set => textMesh = value;}

		public virtual void Awake()
		{
			// init references
		}

		private void Start()
		{
			// init logic
		}

		private void Update()
		{
			// do something with turquoise
		}
	}
}