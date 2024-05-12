using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Harris.NPC
{
    using UnityEngine.UI;

    public class Enemy : MonoBehaviour
    {
        [SerializeField]
        private int strength;
        public int Strength {get => strength; set => strength = value;}

        private int targetPriority;
        public int TargetPriority => targetPriority;

        [SerializeField]
        private Image highLight;

        public Image HighLight => highLight;

        // Start is called before the first frame update
        void Start()
        {
            HighLight.enabled = false;
        }   

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
