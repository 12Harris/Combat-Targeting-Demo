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
        public  EncircleTargetState(PlayerMovement playerMovement) :base(playerMovement)
        {
            AddExitGuard("Moving", () => {return moving;});
            TargetChooser._onSoftLockTargetLost += handleSoftLockTargetLost;

        }

        private void handleSoftLockTargetLost()
        {
            //moving = true;
        }

        public override void Enter()
        {   
            Debug.Log("entered encircle target state");
            moving = false;
            PlayerControllerInstance.Instance.BodyTransform.position += PlayerControllerInstance.Instance.BodyTransform.forward * 0.1f;
            PlayerControllerInstance.Instance.HeadTransform.position += PlayerControllerInstance.Instance.HeadTransform.forward * 0.1f;
        }   



        public override void Tick(in float deltaTime)
        {

            base.Tick(in deltaTime);

            if(TargetChooser.Instance.ChosenTarget != null)
            {
                var v = TargetChooser.Instance.ChosenTarget.transform.position;
                v.y = PlayerControllerInstance.Instance.HeadTransform.position.y;
                PlayerControllerInstance.Instance.HeadTransform.LookAt(v);

                v.y = PlayerControllerInstance.Instance.BodyTransform.position.y;
                PlayerControllerInstance.Instance.BodyTransform.LookAt(v);
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

            else if(PlayerControllerInstance.Instance.PlayerRotationController.PlayerCanMove)
            {
                moving = true;
            }
        }
    }
}