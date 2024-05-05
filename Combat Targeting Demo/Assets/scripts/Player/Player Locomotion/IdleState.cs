using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Harris.Player.PlayerLocomotion
{
    using Harris.Util;

    public class IdleState:PlayerMovementState
    {
        
        private bool moving;
        public IdleState()
        {
            AddExitGuard("Moving", () => {return moving;});
        }

        public override void Enter()
        {   

            base.Enter();
            moving = false;
            RB.velocity = Vector3.zero;
        }

        public override void Tick(in float deltaTime)
        {
            RB.velocity = Vector3.zero;
            
            base.Tick(in deltaTime);

            Debug.Log("Ticking player idle state");

            moving = PlayerMovementState.GetMovement() != Vector2.zero;
        }
    }
}