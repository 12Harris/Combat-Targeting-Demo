using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Harris.NPC
{
    public class Enemy : MonoBehaviour
    {
        private int strength;
        public int Strength {get => strength; set => strength = value;}

        private int targetPriority;
        public int TargetPriority => targetPriority;

        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        public void setPriority(int priority)
        {
            targetPriority = priority;
        }
    }
}
