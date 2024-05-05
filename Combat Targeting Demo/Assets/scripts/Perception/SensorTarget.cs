using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Harris.Perception
{
    public class SensorTarget : MonoBehaviour
    {
        [SerializeField]
        private List<Transform> visibilityPoints;
        
        public List<Transform> VisibilityPoints => visibilityPoints;
        
    }
}
