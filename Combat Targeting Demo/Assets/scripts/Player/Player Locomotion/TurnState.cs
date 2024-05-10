using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Harris.Player.PlayerLocomotion
{
    using Harris.Util;
    using Harris.Perception;
    using Harris.Player.Combat;
    using System.Collections;

    public class TurnState:PlayerMovementState
    {
        
        private bool moving;
        private bool idle;

        private RotateObject playerBodyRotator;
        private RotateObject playerHeadRotator;

        private float turnAngle;
        public float TurnAngle {get => turnAngle; set => turnAngle = value;}

        public TurnState(PlayerMovement playerMovement) :base(playerMovement)
        {
            AddExitGuard("Moving", () => {return moving;});
            AddExitGuard("Idle", () => {return idle;});
            playerHeadRotator = PlayerControllerInstance.Instance.HeadTransform.GetComponent<RotateObject>();
            playerBodyRotator = PlayerControllerInstance.Instance.BodyTransform.GetComponent<RotateObject>();
            playerBodyRotator._onStopRotation += handleTurnCompleted;
        }

        private void handleTurnCompleted()
        {
            if(PlayerMovementState.Move2d != Vector2.zero)
            {
                moving = true;
                Debug.Log("MOVING!!");
            }
            else
            {
                idle = true;
                Debug.Log("IDLE!!");
            }
        }

        public override void Enter()
        {   

            base.Enter();

            //Decactivate the sight temporarily so we dont get distracted by nearby enemies
            TargetChooser.Instance.ChosenTarget = null;
            TargetChooser.Instance.OldTarget = null;
            TargetChooser.Instance.enabled = false;
            PlayerControllerInstance.Instance.LockOnCurrentTarget = false;
            //PlayerControllerInstance.Instance.GetSensor<Sight>().enabled = false;
            moving = false;
            idle = false;
            RB.velocity = Vector3.zero;
            //Debug.Log("entering turn state!");
            //rotate the player
            playerBodyRotator.StartCoroutine(playerBodyRotator.Rotate(TurnAngle,0.25f));
            //get the angle between headTransform forward and player forward
            var deltaAngle = Vector3.Angle(PlayerControllerInstance.Instance.HeadTransform.forward, PlayerControllerInstance.Instance.BodyTransform.forward);
            //roate the head of the player
            playerHeadRotator.StartCoroutine(playerHeadRotator.Rotate(TurnAngle+deltaAngle,0.25f));
        
        }

        public override void Exit()
        {   
            //Activate Target Choosing again
           TargetChooser.Instance.enabled = true;
        }

        public override void Tick(in float deltaTime)
        {
            base.Tick(in deltaTime);

            Debug.Log("Ticking player turn state");
        }
    }
}