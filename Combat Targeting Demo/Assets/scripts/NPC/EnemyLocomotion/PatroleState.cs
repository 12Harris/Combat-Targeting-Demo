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
            EnemyMovement.transform.GetComponent<EnemyController>().GetSensor<Sight>()._onTargetDetected += handleTargetDetected;
            EnemyMovement.transform.GetComponent<EnemyController>().GetSensor<Sight>()._onTargetLost += handleTargetLost;
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
            v.y = EnemyMovement.transform.GetComponent<EnemyController>().transform.position.y;
        }

        public override void Exit()
        {
            if(targetDetected)
            {
                var targetXZPos = EnemyMovement.transform.GetComponent<EnemyController>().GetSensor<Sight>().TargetsSensed[0].transform.position;
                targetXZPos.y = EnemyMovement.transform.GetComponent<EnemyController>().transform.position.y;

                var turnAngle = Vector3.Angle(EnemyMovement.transform.forward, targetXZPos - EnemyMovement.transform.position);

                LeftRightTest lrTest = new LeftRightTest(EnemyMovement.transform.forward, targetXZPos-EnemyMovement.transform.position, Vector3.up);

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

            Debug.Log("dist to wp = " + Vector3.Distance(EnemyMovement.transform.position,v));

            if(Vector3.Distance(EnemyMovement.transform.position,v) < 0.1f)
            {
                idle = true;
                RB.velocity = Vector3.zero;
                Debug.Log("enemy idle = true");
                return;

            }
            RB.velocity = EnemyMovement.transform.forward * groundSpeed;
        }
    }
}