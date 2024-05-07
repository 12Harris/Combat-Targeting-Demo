using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Harris.Player.PlayerLocomotion
{
    using Harris.Util;

    public class IdleState:PlayerMovementState
    {
        
        private bool moving;
        private bool turning;

        public IdleState()
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
        }

        public override void Tick(in float deltaTime)
        {
            RB.velocity = Vector3.zero;

            base.Tick(in deltaTime);

            Debug.Log("Ticking player idle state");

            var move2d = PlayerMovementState.GetMovement();

            if(move2d.x < 0)
            {
                if (PlayerMovementState.PlayerDirection == PlayerDirection.WEST)
                    moving = true;
                else
                    turning = true;
            }

            if(move2d.y > 0)
            {
                if (PlayerMovementState.PlayerDirection == PlayerDirection.NORTH)
                    moving = true;
                else
                    turning = true;
            }

            if(move2d.x > 0)
            {
                if (PlayerMovementState.PlayerDirection == PlayerDirection.EAST)
                    moving = true;
                else
                    turning = true;
            }

             if(move2d.y < 0)
            {
                if (PlayerMovementState.PlayerDirection == PlayerDirection.SOUTH)
                    moving = true;
                else
                    turning = true;
            }

            //moving = PlayerMovementState.GetMovement() != Vector2.zero;
        }
    }
}