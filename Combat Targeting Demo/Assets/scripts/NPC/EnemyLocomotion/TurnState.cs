using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Harris.NPC.EnemyLocomotion
{
    using Harris.Util;
    using Harris.Perception;
    using System.Collections;

    public class TurnState:EnemyMovementState
    {
        
        private bool patroling;
        private bool chasing;
        private float turnAngle;
        private bool targetDetected;
        public float TurnAngle {get => turnAngle; set => turnAngle = value;}

        public TurnState(EnemyMovement enemyMovement) :base(enemyMovement)
        {
            AddExitGuard("Patroling", () => {return patroling;});
            AddExitGuard("Chasing", () => {return chasing;});

            if(EnemyControllerInstance.Instance.Rotator == null)
                Debug.Log("enemy controller rotator is null");
            EnemyControllerInstance.Instance.Rotator._onStopRotation += handleTurnCompleted;
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

        private void handleTurnCompleted()
        {
            if(targetDetected)
                chasing = true;
            else
                patroling = true;       
        }

        public override void Enter()
        {   
            Debug.Log("Entering enemy turn state!");
            base.Enter();

            //Decactivate the sight temporarily so we dont get distracted by nearby enemies
            patroling = false;
            RB.velocity = Vector3.zero;
 
            //rotate the player
            EnemyControllerInstance.Instance.Rotator.StartCoroutine(EnemyControllerInstance.Instance.Rotator.Rotate(TurnAngle,0.5f));
      
        }

        public override void Exit()
        {   

        }

        public override void Tick(in float deltaTime)
        {
            base.Tick(in deltaTime);

            Debug.Log("Ticking enemy turn state");
        }
    }
}