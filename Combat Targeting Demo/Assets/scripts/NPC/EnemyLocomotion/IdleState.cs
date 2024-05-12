using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Harris.NPC.EnemyLocomotion
{
    using Harris.Util;
    using Harris.NPC;
    using Harris.Perception;

    public class IdleState:EnemyMovementState
    {
        
        private bool turning;
        public bool Turning{get => turning; set => turning = value;}

        private float idleTimer = 0;
        private float timeToWait;
        private bool targetDetected;

        public IdleState(EnemyMovement enemyMovement) :base(enemyMovement)
        {
            AddExitGuard("Turning", () => {return turning;});
            EnemyControllerInstance.Instance.GetSensor<Sight>()._onTargetDetected += handleTargetDetected;
            EnemyControllerInstance.Instance.GetSensor<Sight>()._onTargetLost += handleTargetLost;

        }

        private void handleTargetDetected(SensorTarget target)
        {
            turning = true;
            targetDetected = true;
        }

        private void handleTargetLost(SensorTarget target)
        {
            turning = true;
            targetDetected = false;
        }

        public override void Enter()
        {   

            base.Enter();
            turning = false;
            RB.velocity = Vector3.zero;
            timeToWait = UnityEngine.Random.Range(5,8);
            Debug.Log("Entering enemy idle state!");

        }

        public override void Exit()
        {
            float turnAngle = 0;
            if(targetDetected)
            {

                var targetXZPos = EnemyControllerInstance.Instance.GetSensor<Sight>().TargetsSensed[0].transform.position;
                targetXZPos.y = EnemyControllerInstance.Instance.transform.position.y;

                turnAngle = Vector3.Angle(EnemyControllerInstance.Instance.transform.forward, targetXZPos - EnemyControllerInstance.Instance.transform.position);

                LeftRightTest lrTest = new LeftRightTest(EnemyControllerInstance.Instance.transform.forward, targetXZPos-EnemyControllerInstance.Instance.transform.position, Vector3.up);

                if (lrTest.targetIsLeft())
                    turnAngle *=-1;
            }

            else
            {
                var v = EnemyMovement.NextWaypoint.transform.position;
                v.y = EnemyControllerInstance.Instance.transform.position.y;

                turnAngle = Vector3.Angle(EnemyControllerInstance.Instance.transform.forward, v-EnemyControllerInstance.Instance.transform.position);

                //Does the head need to turn left or right?
                LeftRightTest lrTest = new LeftRightTest(EnemyControllerInstance.Instance.transform.forward, v-EnemyControllerInstance.Instance.transform.position, Vector3.up);

                if (lrTest.targetIsLeft())
                    turnAngle *=-1;
            
            }
            EnemyMovement.TurnState.TurnAngle = turnAngle;
        }

        public override void Tick(in float deltaTime)
        {
            base.Tick(in deltaTime);

            Debug.Log("Ticking player idle state");

            idleTimer += deltaTime;

            if(idleTimer > timeToWait || targetDetected)
            {
                turning = true;
                idleTimer = 0;
            }
        }
    }
}