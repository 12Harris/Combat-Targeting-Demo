using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Harris.Player.PlayerLocomotion
{
    using Harris.Util;

    public class MoveState:PlayerMovementState
    {
            private bool idle;
            public MoveState()
            {
                AddExitGuard("Idle", () => {return idle;});
            }

            public override void Enter()
            {
                base.Enter();
                idle = false;
            }

            public override void Tick(in float deltaTime)
            {
                base.Tick(in deltaTime);

                Debug.Log("Ticking player move state");

                var move2d = PlayerMovementState.GetMovement();

                if(move2d == Vector2.zero) 
                {
                    decelerateOnGround();
                    idle = RB.velocity.magnitude < 0.05f;
                    return;
                }

                var move3d = new Vector3(move2d.x,0,move2d.y);

			    //transform the 3d movement vector so the player moves in the direction he is facing
			    move3d = Quaternion.AngleAxis(PlayerControllerInstance.Instance.transform.eulerAngles.y, Vector3.up) * move3d;

                RB.velocity = move3d * groundSpeed;
            }
    }
}