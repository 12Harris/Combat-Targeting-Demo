using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Harris.Player.PlayerLocomotion
{
    using Harris.Util;
    using Harris.NPC;
    using Harris.Player.Combat;

    public class MoveState:PlayerMovementState
    {
        private bool idle;
        private bool turning;
        private bool encircleTarget;
        public bool Turning{get => turning; set => turning = value;}
        public MoveState(PlayerMovement playerMovement) :base(playerMovement)
        {
            AddExitGuard("Idle", () => {return idle;});
            AddExitGuard("Turning", () => {return turning;});
            TargetChooser._onSoftLockTargetChanged += handleSoftLockTargetChanged;
        }

        private void handleSoftLockTargetChanged(Enemy oldTarget, Enemy newTarget)
        {
            if(TargetChooser.Instance.SoftLockMode == SoftLockMode.SHORTRANGE)
		    {
                turning = true;
                PlayerMovement.TurnState.TurnAngle = PlayerControllerInstance.Instance.getAngleToTarget(newTarget);
            }
        }

        public override void Enter()
        {
            base.Enter();
            idle = false;
            turning = false;
            encircleTarget = false;
        }

        public override void Tick(in float deltaTime)
        {
            base.Tick(in deltaTime);

            Debug.Log("Ticking player move state");

            //var move2d = PlayerMovementState.GetMovementInput();

            if(Move2d == Vector2.zero) 
            {
                decelerateOnGround();
                idle = RB.velocity.magnitude < 0.05f;
                return;
            }

            //Debug.Log("IDLE = " + idle);

            var move3d = new Vector3(Move2d.x,0,Move2d.y);

            //transform the 3d movement vector so the player moves in the direction he is facing
            move3d = Quaternion.AngleAxis(PlayerControllerInstance.Instance.transform.eulerAngles.y, Vector3.up) * move3d;

            //RB.velocity = move3d * groundSpeed;
            RB.velocity = PlayerControllerInstance.Instance.BodyTransform.forward * groundSpeed;
        }
    }
}