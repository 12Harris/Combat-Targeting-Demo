using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Harris.Player.PlayerLocomotion
{
    using Harris.Util;
    using Harris.NPC;
    using Harris.Player.Combat;
    using Harris.Player.PlayerLocomotion.Rotation;

    public class IdleState:PlayerMovementState
    {
        
        private bool moving;
        public bool Moving{get => moving; set => moving = value;}

        public IdleState(PlayerMovement playerMovement) :base(playerMovement)
        {
            AddExitGuard("Moving", () => {return moving;});
        }

        public override void Enter()
        {   

            base.Enter();
            moving = false;
            RB.velocity = Vector3.zero;
            Debug.Log("Entering idle state!");
        }

        public override void Tick(in float deltaTime)
        {
            
            Debug.Log("Ticking player idle state");

            RB.velocity = Vector3.zero;

            base.Tick(in deltaTime);

            if(Move2d.magnitude > 0f && PlayerControllerInstance.Instance.PlayerRotationController.PlayerCanMove);
                moving = true;
        }
    }
}