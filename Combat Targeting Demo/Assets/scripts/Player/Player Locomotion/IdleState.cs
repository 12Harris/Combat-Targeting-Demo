using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Harris.Player.PlayerLocomotion
{
    using Harris.Util;

    public class IdleState:PlayerMovementState
    {
        
        private bool moving;
        public bool Moving{get => moving; set => moving = value;}
        private bool turning;

        public bool Turning{get => turning; set => turning = value;}

        public IdleState(PlayerMovement playerMovement) :base(playerMovement)
        {
            AddExitGuard("Moving", () => {return moving;});
            AddExitGuard("Turning", () => {return turning;});
        }

        public override void Enter()
        {   

            base.Enter();
            moving = false;
            turning = false;
            RB.velocity = Vector3.zero;
            Debug.Log("Entering idle state!");
        }

        public override void Tick(in float deltaTime)
        {
            RB.velocity = Vector3.zero;

            base.Tick(in deltaTime);

            Debug.Log("Ticking player idle state");
        }
    }
}