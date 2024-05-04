// project armada

#pragma warning disable 0414

namespace Harris.Player
{
	using UnityEngine;
	using System.Collections;
    using Harris.Perception;

    public class PlayerControllerTest:MonoBehaviour
    {
        public void Start()
        {
            Debug.Log(PlayerControllerInstance.Instance.GetSensor<Sight>());
        }
    }
}