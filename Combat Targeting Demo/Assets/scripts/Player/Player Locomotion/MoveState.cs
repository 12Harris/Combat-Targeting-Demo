using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Harris.Player.PlayerLocomotion
{
    using Harris.Util;
    using Harris.NPC;
    using Harris.Player.Combat;
    using Harris.Player.PlayerLocomotion.Rotation;

    public class MoveState:PlayerMovementState
    {
        private bool idle;
        private bool encircleTarget;

        public MoveState(PlayerMovement playerMovement) :base(playerMovement)
        {
            AddExitGuard("Idle", () => {return idle;});
            AddExitGuard("EncircleTarget", () => {return encircleTarget;});
            TargetChooser._onSoftLockTargetChanged += handleSoftLockTargetChanged;
        }


        private void handleSoftLockTargetChanged(EnemyController oldTarget, EnemyController newTarget)
        {
            if(TargetChooser.Instance.SoftLockMode == SoftLockMode.SHORTRANGE)
            {
                encircleTarget = true;
            }
        }

        public override void Enter()
        {
            base.Enter();
            idle = false;
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

            var move3d = new Vector3(Move2d.x,0,Move2d.y);

            if(PlayerControllerInstance.Instance.PlayerRotationController.PlayerCanMove)
            {
                //transform the 3d movement vector so the player moves in the direction he is facing
                move3d = Quaternion.AngleAxis(PlayerControllerInstance.Instance.transform.eulerAngles.y, Vector3.up) * move3d;

                //RB.velocity = move3d * groundSpeed;
                RB.velocity = PlayerControllerInstance.Instance.BodyTransform.forward * groundSpeed;
            }
        }
    }
}