using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Harris.Player.PlayerLocomotion
{
    using Harris.Util;
    using System.Collections;

    public class TurnState:PlayerMovementState
    {
        
        private bool moving;
        private bool idle;

        private RotateObject playerRotator;

        public TurnState()
        {
            AddExitGuard("Moving", () => {return moving;});
            AddExitGuard("Idle", () => {return idle;});
            playerRotator = PlayerControllerInstance.Instance.transform.GetComponent<RotateObject>();
            playerRotator._onStopRotation += handleTurnCompleted;
        }

        private void handleTurnCompleted()
        {
            if(PlayerMovementState.GetMovement() != Vector2.zero)
                moving = true;
            else
                idle = true;
        }

        public override void Enter()
        {   

            base.Enter();
            moving = false;
            idle = false;
            RB.velocity = Vector3.zero;
            playerRotator.StartCoroutine(playerRotator.Rotate(90,0.5f));
        }

        public override void Tick(in float deltaTime)
        {
            base.Tick(in deltaTime);

            Debug.Log("Ticking player turn state");
        }
    }
}