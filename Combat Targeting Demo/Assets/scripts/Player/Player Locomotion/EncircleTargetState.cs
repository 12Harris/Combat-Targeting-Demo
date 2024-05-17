using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Harris.Player.PlayerLocomotion
{
    using Harris.Util;
    using Harris.Player.Combat;
    using Harris.Player.PlayerLocomotion.Rotation;
    public class EncircleTargetState : PlayerMovementState
    {   

        private bool moving;
        private bool canMove;
        public  EncircleTargetState(PlayerMovement playerMovement) :base(playerMovement)
        {
            AddExitGuard("Moving", () => {return moving;});
            PlayerRotationController._onLeavingTurnState += handleRotatorLeavingTurnState;

        }

        private void handleRotatorLeavingTurnState()
        {
            canMove = true;
        }


        public override void Enter()
        {   
            canMove = false;
            RB.velocity = Vector3.zero;
            Debug.Log("entered encircle target state");
            moving = false;
            //PlayerControllerInstance.Instance.BodyTransform.position += PlayerControllerInstance.Instance.BodyTransform.forward * 0.1f;
            //PlayerControllerInstance.Instance.HeadTransform.position += PlayerControllerInstance.Instance.HeadTransform.forward * 0.1f;
            PlayerControllerInstance.Instance.transform.position += PlayerControllerInstance.Instance.BodyTransform.forward * 0.1f;
           
        }   



        public override void Tick(in float deltaTime)
        {

            base.Tick(in deltaTime);

            Debug.Log("PLAYER MFSM: ticking encircle target state");

            if(TargetChooser.Instance.ChosenTarget != null)
            {
                /*var v = TargetChooser.Instance.ChosenTarget.transform.position;
                v.y = PlayerControllerInstance.Instance.HeadTransform.position.y;
                PlayerControllerInstance.Instance.HeadTransform.LookAt(v);

                v.y = PlayerControllerInstance.Instance.BodyTransform.position.y;
                PlayerControllerInstance.Instance.BodyTransform.LookAt(v);*/
                //PlayerControllerInstance.Instance.HeadTransform.LookAt(v);

                if(Move2d.x < 0)
                {
                    RB.velocity = -PlayerControllerInstance.Instance.BodyTransform.right * strafeSpeed;
                }  
                else if(Move2d.x > 0)
                {
                    RB.velocity = PlayerControllerInstance.Instance.BodyTransform.right * strafeSpeed;
                }  

                if(Move2d.y > 0)
                {
                    RB.velocity = PlayerControllerInstance.Instance.BodyTransform.forward * strafeSpeed;
                }  
                else if(Move2d.y < 0)
                {
                    RB.velocity = -PlayerControllerInstance.Instance.BodyTransform.forward * strafeSpeed;
                }
            }

            else if(canMove)
            {
                moving = true;
            }
        }
    }
}