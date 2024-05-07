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

            //var move2d = PlayerMovementState.GetMovementInput();

            /*if(move2d.magnitude > 0f)
            {
                #region WEST
                //turn 90° clockwise(north)
                if(PlayerMovementState.PlayerDirection == PlayerDirection.WEST && move2d.y > 0)
                {
                    PlayerMovement.TurnState.TurnAngle = 90;
                    turning = true;
                }

                //turn 180° clockwise(east)
                else if(PlayerMovementState.PlayerDirection == PlayerDirection.WEST && move2d.x > 0)
                {
                    PlayerMovement.TurnState.TurnAngle = 180;
                    turning = true;
                }

                //turn 90° counter clockwise(south)
                else if(PlayerMovementState.PlayerDirection == PlayerDirection.WEST && move2d.y < 0)
                {
                    PlayerMovement.TurnState.TurnAngle = -90;
                    turning = true;
                }
                #endregion


                #region NORTH
                //turn 90° counter clockwise(west)
                else if(PlayerMovementState.PlayerDirection == PlayerDirection.NORTH && move2d.x < 0)
                {
                    PlayerMovement.TurnState.TurnAngle = -90;
                    turning = true;
                } 

                //turn 90° clockwise(east)
                else if(PlayerMovementState.PlayerDirection == PlayerDirection.NORTH && move2d.x > 0)
                {
                    PlayerMovement.TurnState.TurnAngle = 90;
                    turning = true;
                }

                //turn 180° clockwise(south)
                else if(PlayerMovementState.PlayerDirection == PlayerDirection.NORTH && move2d.y < 0)
                {
                    PlayerMovement.TurnState.TurnAngle = 180;
                    turning = true;
                }
                #endregion

                #region EAST

                 //turn 90° counter clockwise(north)
                if(PlayerMovementState.PlayerDirection == PlayerDirection.EAST && move2d.y > 0)
                {
                    PlayerMovement.TurnState.TurnAngle = -90;
                    turning = true;
                }

                //turn 180° clockwise(east)
                else if(PlayerMovementState.PlayerDirection == PlayerDirection.EAST && move2d.x < 0)
                {
                    PlayerMovement.TurnState.TurnAngle = 180;
                    turning = true;
                }

                //turn 90°clockwise(south)
                else if(PlayerMovementState.PlayerDirection == PlayerDirection.EAST && move2d.y < 0)
                {
                    PlayerMovement.TurnState.TurnAngle = 90;
                    turning = true;
                }

                #endregion
                

                #region SOUTH
                //turn 90° clockwise(west)
                else if(PlayerMovementState.PlayerDirection == PlayerDirection.SOUTH && move2d.x < 0)
                {
                    PlayerMovement.TurnState.TurnAngle = 90;
                    turning = true;
                } 

                //turn 90° clockwise(east)
                else if(PlayerMovementState.PlayerDirection == PlayerDirection.SOUTH && move2d.x > 0)
                {
                    PlayerMovement.TurnState.TurnAngle = -90;
                    turning = true;
                }

                //turn 180° clockwise(north)
                else if(PlayerMovementState.PlayerDirection == PlayerDirection.SOUTH && move2d.y < 0)
                {
                    Debug.Log("SHOULD TURN 180!!");
                    PlayerMovement.TurnState.TurnAngle = 180;
                    turning = true;
                }
                #endregion

                else
                    moving = true;
            }

            //moving = PlayerMovementState.GetMovement() != Vector2.zero;*/
        }
    }
}