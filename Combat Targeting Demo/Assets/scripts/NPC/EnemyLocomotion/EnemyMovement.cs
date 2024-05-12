using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Harris.NPC.EnemyLocomotion
{
    using Harris.Util;
    using Harris.NPC;
    using Harris.AI.PathFinding;

    public enum EnemyDirection
    {
        WEST,
        NORTH,
        EAST,
        SOUTH
    }

    public class EnemyMovementState:FSM_State
    {
        private Rigidbody rb;
        public Rigidbody RB => rb;  

        protected float groundSpeed = 2f;
        protected float groundDeceleration = 0.9f;

        private static EnemyDirection enemyDirection = EnemyDirection.NORTH;

        public static EnemyDirection EnemyDirection{get =>  enemyDirection; set => enemyDirection = value;}

        //reference to the player movement fsm
        private EnemyMovement enemyMovement;

        public EnemyMovement EnemyMovement => enemyMovement;
        public EnemyMovementState(EnemyMovement _enemyMovement)
        {
            //rb = PlayerControllerInstance.Instance.transform.GetComponentInChildren<Rigidbody>();
            rb = EnemyControllerInstance.Instance.transform.GetComponent<Rigidbody>();
            enemyMovement = _enemyMovement;
        }


        public virtual void decelerateOnGround()
        {
            if (rb.velocity == Vector3.zero)
                return;
            rb.velocity = rb.velocity * groundDeceleration;

        }

        public override void Tick(in float deltaTime)
        {
            base.Tick(in deltaTime);

        }
    }

    public class EnemyMovement : MonoBehaviour
    {
        private FSM enemyMovementFSM = new FSM();

        private IdleState idleState;
        private TurnState turnState;
        private PatroleState patroleState;
        private ChaseState chaseState;
  
        public IdleState IdleState => idleState;
        public TurnState TurnState => turnState;
        public PatroleState PatroleState => patroleState;
        public ChaseState ChaseState => chaseState;

        [SerializeField]
        private ConnectedWaypoint startWaypoint;
        public ConnectedWaypoint StartWaypoint => startWaypoint;

        private ConnectedWaypoint nextWaypoint;
        public ConnectedWaypoint NextWaypoint {get => nextWaypoint; set => nextWaypoint = value;}

        void Start()
        {
            
            nextWaypoint = startWaypoint;

			idleState = new IdleState(this);
            turnState = new TurnState(this);
			patroleState = new PatroleState(this);
            chaseState = new ChaseState(this);
 
			int index1 = enemyMovementFSM.AddState(idleState);
			int index2 = enemyMovementFSM.AddState(turnState);
			int index3 = enemyMovementFSM.AddState(patroleState);
            int index4 = enemyMovementFSM.AddState(chaseState);

            enemyMovementFSM.AddTransition(index1, index2, idleState.GetExitGuard("Turning"));

            enemyMovementFSM.AddTransition(index2, index3, turnState.GetExitGuard("Patroling"));
            enemyMovementFSM.AddTransition(index2, index4, turnState.GetExitGuard("Chasing"));

            enemyMovementFSM.AddTransition(index3, index1, patroleState.GetExitGuard("Idle"));
            enemyMovementFSM.AddTransition(index3, index2, patroleState.GetExitGuard("Target Detected"));

            enemyMovementFSM.AddTransition(index4, index1, chaseState.GetExitGuard("TargetLost"));
 

			idleState.Enter();
        }

        // Start is called before the first frame update
        /*void Awake()
        {
            
        }*/

        // Update is called once per frame
        void Update()
        {
            enemyMovementFSM.Tick(Time.deltaTime);
        }

        void LateUpdate()
        {
            enemyMovementFSM.LateTick(Time.deltaTime);
        }
    }
}