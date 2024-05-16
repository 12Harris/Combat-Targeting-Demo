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

        private bool canMove;

        public MoveState(PlayerMovement playerMovement) :base(playerMovement)
        {
            AddExitGuard("Idle", () => {return idle;});
            AddExitGuard("EncircleTarget", () => {return encircleTarget;});
            TargetChooser._onSoftLockTargetChanged += handleSoftLockTargetChanged;
            PlayerRotationController._onLeavingIdleState += handleRotatorLeavingIdleState;
            PlayerRotationController._onLookingAtTarget += handlePlayerLookingAtTarget;
        }

        private void handleRotatorLeavingIdleState()
        {

        }

        private void handlePlayerLookingAtTarget()
        {

            if(TargetChooser.Instance.SoftLockMode == SoftLockMode.SHORTRANGE)
            {
                encircleTarget = true;
                Debug.Log("encircle target is true");
            }
        }

        private void handleSoftLockTargetChanged(EnemyController oldTarget, EnemyController newTarget)
        {
            canMove = false;
        }

        public override void Enter()
        {
            base.Enter();
            idle = false;
            encircleTarget = false;
            canMove = true;
            Debug.Log("Entering move state");
        }

        public override void Tick(in float deltaTime)
        {
            base.Tick(in deltaTime);

            Debug.Log("PLAYER MFSM: ticking move state");

            Debug.Log("idle is: " + idle);

            if(Move2d == Vector2.zero) 
            {
                decelerateOnGround();
                idle = RB.velocity.magnitude < 0.05f;

                if(idle)
                {
                    Debug.Log("WTF IDLE???");
                }
                return;
            }

            var move3d = new Vector3(Move2d.x,0,Move2d.y);

            if(canMove)
            {
                Debug.Log("Moving = True!");
                //transform the 3d movement vector so the player moves in the direction he is facing
                move3d = Quaternion.AngleAxis(PlayerControllerInstance.Instance.transform.eulerAngles.y, Vector3.up) * move3d;
                RB.velocity = PlayerControllerInstance.Instance.BodyTransform.forward * groundSpeed;
            }
            else
            {
                Debug.Log("Moving = False!");//Eigentlich besser: wechsel zu idle state?
                RB.velocity = Vector3.zero;
            }
        }

        public override void LateTick(in float deltaTime)
        {
            
        }
    }
}