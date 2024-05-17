using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Harris.Player.PlayerLocomotion.Rotation
{
    using Harris.Util;
    using Harris.Player.Combat;
    using Harris.NPC;
    using System;

    public class PlayerRotationController:MonoBehaviour
    {   

        [SerializeField]
        private RotateObject headRotator;

        [SerializeField]
        private RotateObject upperBodyRotator;

        [SerializeField]
        private RotateObject lowerBodyRotator;

        [SerializeField]
        private Transform headTransform;

        [SerializeField]
        private Transform lowerBodyTransform;

        private PlayerHeadRotationFSM playerHeadRotationFSM;
        private PlayerUpperBodyRotationFSM playerUpperBodyRotationFSM;
        private PlayerLowerBodyRotationFSM playerLowerBodyRotationFSM;
        public PlayerLowerBodyRotationFSM PlayerLowerBodyRotationFSM => playerLowerBodyRotationFSM;

        private bool playerCanMove = false;
        public bool PlayerCanMove => playerCanMove;

        private bool resetRotation;
        public bool ResetRotation => resetRotation;

        public static event Action _onLeavingIdleState;
        public static event Action _onLookingAtTarget;
        public static event Action _onEnteringTurnState;
        public static event Action _onLeavingTurnState;

        private bool lowerBodyRotating;
        public bool LowerBodyRotating=>lowerBodyRotating;

        private void handleSoftLockTargetChanged(EnemyController e1, EnemyController e2)
        {
            resetRotation = e1 != null && e2 == null;
        }

        private void Start()
        {
            TargetChooser._onSoftLockTargetChanged += handleSoftLockTargetChanged;

            playerHeadRotationFSM = new PlayerHeadRotationFSM(headTransform,lowerBodyTransform,headRotator);
            playerUpperBodyRotationFSM = new PlayerUpperBodyRotationFSM(headTransform,lowerBodyTransform,upperBodyRotator);
            playerLowerBodyRotationFSM = new PlayerLowerBodyRotationFSM(headTransform,lowerBodyTransform,lowerBodyRotator);

            playerHeadRotationFSM.Init();
            playerUpperBodyRotationFSM.Init();
            playerLowerBodyRotationFSM.Init();

            playerLowerBodyRotationFSM.IdleState._onLeavingIdleState += handleLeavingIdleState;
            playerLowerBodyRotationFSM.LookAtTargetState._onLookingAtTarget += handleLookingAtTarget;
            playerLowerBodyRotationFSM.TurnState._onEnteringTurnState += handleEnteringTurnState;
            playerLowerBodyRotationFSM.TurnState._onLeavingTurnState += handleLeavingTurnState;
        }

        private void handleEnteringTurnState()
        {
            _onEnteringTurnState?.Invoke();
        }

        private void handleLeavingIdleState()
        {
            _onLeavingIdleState?.Invoke();
        }

        private void handleLookingAtTarget()
        {
            _onLookingAtTarget?.Invoke();
        }

        private void handleLeavingTurnState()
        {
            _onLeavingTurnState?.Invoke();
        }

        private void Update()
        {
            playerHeadRotationFSM.Tick(Time.deltaTime);
            playerUpperBodyRotationFSM.Tick(Time.deltaTime);
            playerLowerBodyRotationFSM.Tick(Time.deltaTime);

            //playerCanMove = playerLowerBodyRotationFSM.CurrentState is IdleState;
            //playerCanMove =  playerLowerBodyRotationFSM.IdleState.PlayerCanMove;
            playerCanMove =  !playerLowerBodyRotationFSM.IsRotating;

            Debug.Log("lower body rotation state = " + (playerLowerBodyRotationFSM as PlayerRotationFSM).CurrentState);
            Debug.Log("head rotation state = " +  playerHeadRotationFSM.CurrentState);

            lowerBodyRotating = playerLowerBodyRotationFSM.IsRotating;

            //playerCanMove = playerLowerBodyRotationFSM.
        }


    }
}