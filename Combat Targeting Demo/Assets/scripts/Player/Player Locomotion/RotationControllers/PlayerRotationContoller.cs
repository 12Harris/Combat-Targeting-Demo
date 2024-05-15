using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Harris.Player.PlayerLocomotion.Rotation
{
    using Harris.Util;
    using Harris.Player.Combat;
    using Harris.NPC;

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

        private void handleSoftLockTargetChanged(EnemyController e1, EnemyController e2)
        {
            resetRotation = e1 != null && e2 == null;
        }

        private void Awake()
        {
            TargetChooser._onSoftLockTargetChanged += handleSoftLockTargetChanged;

            playerHeadRotationFSM = new PlayerHeadRotationFSM(headTransform,lowerBodyTransform,headRotator);
            playerUpperBodyRotationFSM = new PlayerUpperBodyRotationFSM(headTransform,lowerBodyTransform,headRotator);
            playerLowerBodyRotationFSM = new PlayerLowerBodyRotationFSM(headTransform,lowerBodyTransform,headRotator);

            playerHeadRotationFSM.Init();
            playerUpperBodyRotationFSM.Init();
            playerLowerBodyRotationFSM.Init();
        }

        private void Update()
        {
            playerHeadRotationFSM.Tick(Time.deltaTime);
            playerUpperBodyRotationFSM.Tick(Time.deltaTime);
            playerLowerBodyRotationFSM.Tick(Time.deltaTime);

            playerCanMove = playerLowerBodyRotationFSM.CurrentState is IdleState;
        }


    }
}