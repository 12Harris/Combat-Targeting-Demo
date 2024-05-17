using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Harris.Camera
{
    using Harris.Util;
    using Harris.NPC;
    using Harris.Player;
    using Harris.Player.Combat;
    
    public class CameraController : MonoBehaviour
    {
        private CameraFSM fsm;
        [SerializeField]
        private Transform target;

        void Start()
        {
            fsm = new CameraFSM(target, transform);
            fsm.Init();
        }

        // Update is called once per frame
        void Update()
        {
            fsm.Tick(Time.deltaTime);
        }
    }

    public class IdleState:FSM_State
    {
        private CameraFSM fsm;
        private bool followTarget;
        Vector3 oldTargetPosition;
        private Transform camTransform;
        private Transform target;
        public IdleState(CameraFSM _fsm, Transform _target, Transform _camTransform)
        {
            fsm = _fsm;
            camTransform = _camTransform;
            target = _target;
            AddExitGuard("FollowTarget", () => {return followTarget;});
        }
        public override void Enter()
        {
            followTarget = false;
            oldTargetPosition = target.position;
            camTransform.position = target.position - Vector3.forward*6 + Vector3.up*3;
        }

        public override void Tick(in float dt)
        {
            if(target.position != oldTargetPosition)
                followTarget = true;
        }

        public override void Exit()
        {
           
        }
    }

    public class FollowTargetState:FSM_State
    {   
        private Transform target;
        private bool idle;
        private CameraFSM fsm;
        Vector3 oldTargetPosition;
        private Transform camTransform;

        public FollowTargetState(CameraFSM _fsm,Transform _target, Transform _camTransform)
        {
            AddExitGuard("Idle", () => {return idle;});
            target = _target;
            fsm = _fsm;
            camTransform = _camTransform;
        }

        public override void Enter()
        {
           idle = false;
           oldTargetPosition = target.position;
           camTransform.position = target.position - Vector3.forward*6 + Vector3.up*3;
        }

        public override void Tick(in float dt)
        {
            if(target.position != oldTargetPosition)
                camTransform.position = target.position - Vector3.forward*6 + Vector3.up*3;

            else
                idle = true;
        }

        public override void Exit()
        {
           
        }
    }

    public class LerpToTargetState:FSM_State
    {
        private Transform previousTarget;
        private Transform firstTarget;
        private Transform target;
        private bool lerpFinished;
        private Vector3 lookAtPoint = Vector3.zero;
        private Transform camTransform;
        private CameraFSM fsm;

        public LerpToTargetState(CameraFSM _fsm,Transform _target,Transform _camTransform)
        {
            AddExitGuard("LerpFinished", () => {return lerpFinished;});
            target = _target;
            previousTarget = _target;
            firstTarget = _target;
            fsm = _fsm;
            camTransform = _camTransform;
            TargetChooser._onSoftLockTargetChanged += handleTargetChanged;
            TargetChooser._onSoftLockTargetLost += handleTargetLost;
        }

        private void handleTargetChanged(EnemyController e1, EnemyController e2)
        {
            Debug.Log("cam target changed(enemy)");
            previousTarget = target;
            target = e2.gameObject.transform;
        }

        private void handleTargetLost()
        {
            Debug.Log("cam target changed(player)");
            previousTarget = target;
            target = firstTarget;
        }

        public override void Enter()
        {
           Debug.Log("Lerping to new target!");
           lerpFinished = false;
           camTransform.LookAt(previousTarget.position);
           camTransform.GetComponent<CameraController>().StartCoroutine(lerpToTarget());
        }

        private IEnumerator lerpToTarget()
        {
            lerpFinished = false;
            float elapsedTime = 0f;
            float waitTime = 0.5f;
            while (elapsedTime < waitTime)
            {
                lookAtPoint = Vector3.Lerp(previousTarget.position, target.position, (elapsedTime / waitTime));
                elapsedTime += Time.deltaTime;

                // Yield here
                yield return null;
            } 
            lerpFinished = true; 
        }

        public override void Tick(in float dt)
        {
           camTransform.LookAt(lookAtPoint);
        }
    }

    public class LookAtTargetState:FSM_State
    {
        
        private Transform firstTarget;
        private Transform previousTarget;
        private Transform target;
        private bool targetChanged;
        private Vector3 lookAtPoint = Vector3.zero;
        private Transform camTranform;
        private CameraFSM fsm;

        public LookAtTargetState(CameraFSM _fsm,Transform _target,Transform _camTransform)
        {
            AddExitGuard("TargetChanged", () => {return targetChanged;});
            target = _target;
            firstTarget = target;
            previousTarget = _target;
            fsm = _fsm;
            camTranform = _camTransform;
            TargetChooser._onSoftLockTargetChanged += handleTargetChanged;
            TargetChooser._onSoftLockTargetLost += handleTargetLost;
        }

        private void handleTargetChanged(EnemyController e1, EnemyController e2)
        {
            previousTarget = target;
            target = e2.gameObject.transform;
            targetChanged = true;
        }

        private void handleTargetLost()
        {
            previousTarget = target;
            target = firstTarget;
            targetChanged = true;
        }

        public override void Enter()
        {
           targetChanged = false;
           lookAtPoint = target.position;
           camTranform.LookAt(lookAtPoint);
        }

        public override void Tick(in float dt)
        {
            lookAtPoint = target.position;
            camTranform.LookAt(lookAtPoint);
        }
    }


    public class CameraFSM
    {
        private Transform target;
        private Transform camTransform;

        public Transform Target{get => target; set => target = value;}
    
        private FSM_State currentStateFSM;
        public FSM_State CurrentStateFSM => currentStateFSM;

        private FSM_State currentStateFSM2;
        public FSM_State CurrentStateFSM2 => currentStateFSM2;

        public CameraFSM(Transform _t, Transform _camTransform)
        {
            target =_t;
            camTransform = _camTransform;
        }

        private IdleState idleState;
        public IdleState IdleState => idleState;

        private FollowTargetState followTargetState;
        public FollowTargetState FollowTargetState => followTargetState;

        private LookAtTargetState lookAtTargetState;
        public LookAtTargetState LookAtTargetState => lookAtTargetState;

        private LerpToTargetState lerpToTargetState;
        public LerpToTargetState LerpToTargetState => lerpToTargetState;

        private FSM fsm;
        private FSM fsm2;

        public void Init()
        {
            fsm = new FSM();
            fsm2 = new FSM();

            idleState = new IdleState(this,target,camTransform);//zooming takes place in idle state
            followTargetState = new FollowTargetState(this,target,camTransform);
            lerpToTargetState = new LerpToTargetState(this,target,camTransform);
            lookAtTargetState = new LookAtTargetState(this,target,camTransform);

            int index1 = fsm.AddState(idleState);
            int index2 = fsm.AddState(followTargetState);

            int index3 = fsm2.AddState(lookAtTargetState);
            int index4 = fsm2.AddState(lerpToTargetState);

            fsm.AddTransition(index1, index2, idleState.GetExitGuard("FollowTarget"));
            fsm.AddTransition(index2, index1, followTargetState.GetExitGuard("Idle"));
            
            fsm2.AddTransition(index3, index4, lookAtTargetState.GetExitGuard("TargetChanged"));
            fsm2.AddTransition(index4, index3, lerpToTargetState.GetExitGuard("LerpFinished"));

            idleState.Enter();
            lookAtTargetState.Enter();
        }

        public void Tick(in float dt)
        {   
            fsm.Tick(dt);
            fsm2.Tick(dt);
            currentStateFSM = fsm.CurrentState;
            currentStateFSM2 = fsm2.CurrentState;
        }
    }
}
