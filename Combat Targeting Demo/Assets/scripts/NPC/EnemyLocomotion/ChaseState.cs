using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Harris.NPC.EnemyLocomotion
{
    using Harris.Util;
    using Harris.Perception;
    using System.Collections;

    public class ChaseState:EnemyMovementState
    {
        private bool chasing;
        private float turnAngle;
        private bool targetLost;
        public float TurnAngle {get => turnAngle; set => turnAngle = value;}

        public ChaseState(EnemyMovement enemyMovement) :base(enemyMovement)
        {
            AddExitGuard("TargetLost", () => {return targetLost;});
            EnemyControllerInstance.Instance.GetSensor<Sight>()._onTargetLost += handleTargetLost;
        }

        private void handleTargetLost(SensorTarget target)
        {
            targetLost = true;
        }

        public override void Enter()
        {   
            Debug.Log("Entering target chase state!");
            base.Enter();

            targetLost = false;
      
        }

        public override void Exit()
        {   

        }

        public override void Tick(in float deltaTime)
        {
            base.Tick(in deltaTime);

            Debug.Log("Ticking enemy chase state");

            var v = EnemyControllerInstance.Instance.GetSensor<Sight>().TargetsSensed[0].transform.position;
            v.y = EnemyControllerInstance.Instance.transform.position.y;

            EnemyControllerInstance.Instance.transform.LookAt(v);

            RB.velocity = EnemyControllerInstance.Instance.transform.forward * groundSpeed;


        }
    }
}