using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Harris.NPC.EnemyLocomotion
{
    using Harris.Util;
    using Harris.NPC;
    using Harris.Perception;

    public class PatroleState:EnemyMovementState
    {
        private bool idle;
        private bool targetDetected;

        private Vector3 v;
        public PatroleState(EnemyMovement enemyMovement) :base(enemyMovement)
        {
            AddExitGuard("Idle", () => {return idle;});
            AddExitGuard("Target Detected", () => {return targetDetected;});
            EnemyControllerInstance.Instance.GetSensor<Sight>()._onTargetDetected += handleTargetDetected;
            EnemyControllerInstance.Instance.GetSensor<Sight>()._onTargetLost += handleTargetLost;
        }

        private void handleTargetDetected(SensorTarget target)
        {
            targetDetected = true;
        }

        private void handleTargetLost(SensorTarget target)
        {
            targetDetected = false;
        }

        public override void Enter()
        {
            base.Enter();
            idle = false;
            v = EnemyMovement.NextWaypoint.transform.position;
            v.y = EnemyControllerInstance.Instance.transform.position.y;
        }

        public override void Exit()
        {
            if(targetDetected)
            {
                var targetXZPos = EnemyControllerInstance.Instance.GetSensor<Sight>().TargetsSensed[0].transform.position;
                targetXZPos.y = EnemyControllerInstance.Instance.transform.position.y;

                var turnAngle = Vector3.Angle(EnemyControllerInstance.Instance.transform.forward, targetXZPos - EnemyControllerInstance.Instance.transform.position);

                LeftRightTest lrTest = new LeftRightTest(EnemyControllerInstance.Instance.transform.forward, targetXZPos-EnemyControllerInstance.Instance.transform.position, Vector3.up);

                if (lrTest.targetIsLeft())
                    turnAngle *=-1;
                
                EnemyMovement.TurnState.TurnAngle = turnAngle;
            }

            else
                EnemyMovement.NextWaypoint = EnemyMovement.NextWaypoint.GetNext(EnemyMovement.NextWaypoint);
        }

        public override void Tick(in float deltaTime)
        {
            base.Tick(in deltaTime);

            Debug.Log("Ticking enemy patrole state");

            //RB.velocity = move3d * groundSpeed;

            if(Vector3.Distance(EnemyControllerInstance.Instance.transform.position,v) < 0.05f)
            {
                idle = true;
                RB.velocity = Vector3.zero;
                return;

            }
            RB.velocity = EnemyControllerInstance.Instance.transform.forward * groundSpeed;
        }
    }
}