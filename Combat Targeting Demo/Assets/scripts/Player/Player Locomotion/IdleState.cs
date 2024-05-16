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
        private bool encircleTarget;
        private bool moving;
        public bool Moving{get => moving; set => moving = value;}
        private bool canMove;

        public IdleState(PlayerMovement playerMovement) :base(playerMovement)
        {
            AddExitGuard("Moving", () => {return moving;});
            AddExitGuard("EncircleTarget", () => {return encircleTarget;});
            PlayerRotationController._onLeavingIdleState += handleRotatorLeavingIdleState;
            PlayerRotationController._onLookingAtTarget += handlePlayerLookingAtTarget;
        }

        private void handleRotatorLeavingIdleState()
        {
            canMove = false;
        }

        private void handlePlayerLookingAtTarget()
        {
            if(TargetChooser.Instance.SoftLockMode == SoftLockMode.SHORTRANGE)
            {
                encircleTarget = true;
                Debug.Log("encircle target is true");
            }
        }

        public override void Enter()
        {   
            base.Enter();
            canMove = true;
            moving = false;
            encircleTarget = false;
            RB.velocity = Vector3.zero;
            Debug.Log("Entering idle state!");
        }

        public override void Tick(in float deltaTime)
        {
            
            Debug.Log("Ticking player idle state");

            RB.velocity = Vector3.zero;

            base.Tick(in deltaTime);

            if(Move2d.magnitude > 0f && canMove)
            {
                moving = true;
            }
        }

    }
}