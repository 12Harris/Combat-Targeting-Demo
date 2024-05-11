using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Harris.Player.PlayerLocomotion
{
    using Harris.Util;
    using Harris.Player.Combat;
    public class EncircleTargetState : PlayerMovementState
    {   

        private bool turning;
        private List<Vector3> worldAxis;
        public  EncircleTargetState(PlayerMovement playerMovement) :base(playerMovement)
        {
            AddExitGuard("Turning", () => {return turning;});
            TargetChooser._onSoftLockTargetLost += handleSoftLockTargetLost;//move to encircletargetstate
            worldAxis = new List<Vector3>();
            worldAxis.Add(Vector3.forward);
            worldAxis.Add(-Vector3.forward);
        }

        public override void Enter()
        {   
            Debug.Log("entered encircle target state");

            PlayerControllerInstance.Instance.BodyTransform.position += PlayerControllerInstance.Instance.BodyTransform.forward * 0.1f;
            PlayerControllerInstance.Instance.HeadTransform.position += PlayerControllerInstance.Instance.HeadTransform.forward * 0.1f;

            turning = false;
        }   

        private void handleSoftLockTargetLost()//move to encircletargetstate
        {
            Debug.Log("encircle target STATE => TARGET LOST!");
            if(TargetChooser.Instance.SoftLockMode == SoftLockMode.SHORTRANGE)
            {
                var invForward = -PlayerControllerInstance.Instance.BodyTransform.forward;

                if(worldAxis.Contains(-Vector3.right))
                    worldAxis.Remove(-Vector3.right);

                if(worldAxis.Contains(-Vector3.left))
                    worldAxis.Remove(-Vector3.right);

                if(invForward.x < 0)
                {
                    worldAxis.Add(Vector3.left);
                }
                else
                {
                    worldAxis.Add(Vector3.right);
                }

                var minAngle = 360f;

                Vector3 chosenAxis = Vector3.zero;

                foreach(Vector3 axis in worldAxis)
                {
                    if (Vector3.Angle(invForward, axis) < minAngle)
                    {
                        minAngle = Vector3.Angle(invForward, axis);
                        chosenAxis = axis;
                        Debug.Log("MinAngle to axis: " + axis + " = " + minAngle);
                    }
                }

                /*switch(chosenAxis)
                {
                    case Vector3 v when v.Equals(Vector3.up):
                        PlayerMovementState.PlayerDirection = PlayerDirection.NORTH;
                        break;

                    case Vector3 v when v.Equals(Vector3.down):
                        PlayerMovementState.PlayerDirection = PlayerDirection.SOUTH;
                        break;

                    case Vector3 v when v.Equals(Vector3.left):
                        PlayerMovementState.PlayerDirection = PlayerDirection.WEST;
                        break;
                    
                    case Vector3 v when v.Equals(Vector3.right):
                        PlayerMovementState.PlayerDirection = PlayerDirection.EAST;
                        break;
                }*/

                Debug.Log("chosen axis = " + chosenAxis);

                var turnAngle = 180 - Vector3.Angle(invForward, chosenAxis);

                //Does the player need to turn left or right?
                LeftRightTest lrTest = new LeftRightTest(chosenAxis, invForward, Vector3.up);

                if (lrTest.targetIsLeft())
                    turnAngle *=-1;

                PlayerMovement.TurnState.TurnAngle = turnAngle;

                turning = true;
            }
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
        }
    }
}