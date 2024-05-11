using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Harris.Player.PlayerLocomotion
{
    using Harris.Util;
    using Harris.NPC;
    using Harris.Player.Combat;

    public enum PlayerDirection
    {
        WEST,
        NORTH,
        EAST,
        SOUTH
    }

    public class PlayerMovementState:FSM_State
    {
        private Rigidbody rb;
        public Rigidbody RB => rb;  

        protected float groundSpeed = 5f;
        protected float strafeSpeed = 2f;

        protected float groundDeceleration = 0.9f;

        private static PlayerDirection playerDirection = PlayerDirection.NORTH;

        public static PlayerDirection PlayerDirection{get =>  playerDirection; set => playerDirection = value;}

        //reference to the player movement fsm
        private PlayerMovement playerMovement;

        public PlayerMovement PlayerMovement => playerMovement;

        private static Vector2 move2d;
        public static Vector2 Move2d => move2d;

        public PlayerMovementState(PlayerMovement _playerMovement)
        {
            //rb = PlayerControllerInstance.Instance.transform.GetComponentInChildren<Rigidbody>();
            rb = PlayerControllerInstance.Instance.transform.GetComponent<Rigidbody>();
            playerMovement = _playerMovement;
        }

        /// <summary>
		/// This method retrieves the 2d movement vector
		/// </summary>
		/// <returns>
		/// 2D movement vector
		/// </returns>
		public static Vector2 GetMovementInput()
		{
			var v = Vector2.zero;

			if (Input.GetKey(KeyCode.A)) { v.x += -1f; }
			if (Input.GetKey(KeyCode.D)) { v.x += 1f; }
			if (Input.GetKey(KeyCode.W)) { v.y += 1f; }
			if (Input.GetKey(KeyCode.S)) { v.y += -1f; }

			return v.normalized;
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

            move2d = PlayerMovementState.GetMovementInput();


            if(move2d.magnitude > 0f && (this is IdleState || this is MoveState))
            {
                #region WEST
                //turn 90° clockwise(north)
                if(PlayerMovementState.PlayerDirection == PlayerDirection.WEST && move2d.y > 0)
                {
                    PlayerMovement.TurnState.TurnAngle = 90;

                    if (this is IdleState)
                        (this as IdleState).Turning = true;
                    else
                        (this as MoveState).Turning = true;
    
                }

                //turn 180° clockwise(east)
                else if(PlayerMovementState.PlayerDirection == PlayerDirection.WEST && move2d.x > 0)
                {
                    PlayerMovement.TurnState.TurnAngle = 180;
                    if (this is IdleState)
                        (this as IdleState).Turning = true;
                    else
                        (this as MoveState).Turning = true;
                }

                //turn 90° counter clockwise(south)
                else if(PlayerMovementState.PlayerDirection == PlayerDirection.WEST && move2d.y < 0)
                {
                    PlayerMovement.TurnState.TurnAngle = -90;
                    if (this is IdleState)
                        (this as IdleState).Turning = true;
                    else
                        (this as MoveState).Turning = true;
                }
                #endregion


                #region NORTH
                //turn 90° counter clockwise(west)
                else if(PlayerMovementState.PlayerDirection == PlayerDirection.NORTH && move2d.x < 0)
                {
                    PlayerMovement.TurnState.TurnAngle = -90;
                    if (this is IdleState)
                        (this as IdleState).Turning = true;
                    else
                        (this as MoveState).Turning = true;
                } 

                //turn 90° clockwise(east)
                else if(PlayerMovementState.PlayerDirection == PlayerDirection.NORTH && move2d.x > 0)
                {
                    PlayerMovement.TurnState.TurnAngle = 90;
                    if (this is IdleState)
                        (this as IdleState).Turning = true;
                    else
                        (this as MoveState).Turning = true;
                }

                //turn 180° clockwise(south)
                else if(PlayerMovementState.PlayerDirection == PlayerDirection.NORTH && move2d.y < 0)
                {
                    PlayerMovement.TurnState.TurnAngle = 180;
                    if (this is IdleState)
                        (this as IdleState).Turning = true;
                    else
                        (this as MoveState).Turning = true;
                }
                #endregion

                #region EAST

                 //turn 90° counter clockwise(north)
                if(PlayerMovementState.PlayerDirection == PlayerDirection.EAST && move2d.y > 0)
                {
                    PlayerMovement.TurnState.TurnAngle = -90;
                    if (this is IdleState)
                        (this as IdleState).Turning = true;
                    else
                        (this as MoveState).Turning = true;
                }

                //turn 180° clockwise(east)
                else if(PlayerMovementState.PlayerDirection == PlayerDirection.EAST && move2d.x < 0)
                {
                    PlayerMovement.TurnState.TurnAngle = 180;
                    if (this is IdleState)
                        (this as IdleState).Turning = true;
                    else
                        (this as MoveState).Turning = true;
                }

                //turn 90°clockwise(south)
                else if(PlayerMovementState.PlayerDirection == PlayerDirection.EAST && move2d.y < 0)
                {
                    PlayerMovement.TurnState.TurnAngle = 90;
                    if (this is IdleState)
                        (this as IdleState).Turning = true;
                    else
                        (this as MoveState).Turning = true;
                }

                #endregion
                

                #region SOUTH
                //turn 90° clockwise(west)
                else if(PlayerMovementState.PlayerDirection == PlayerDirection.SOUTH && move2d.x < 0)
                {
                    PlayerMovement.TurnState.TurnAngle = 90;
                    if (this is IdleState)
                        (this as IdleState).Turning = true;
                    else
                        (this as MoveState).Turning = true;
                } 

                //turn 90° clockwise(east)
                else if(PlayerMovementState.PlayerDirection == PlayerDirection.SOUTH && move2d.x > 0)
                {
                    PlayerMovement.TurnState.TurnAngle = -90;
                    if (this is IdleState)
                        (this as IdleState).Turning = true;
                    else
                        (this as MoveState).Turning = true;
                }

                //turn 180° clockwise(north)
                else if(PlayerMovementState.PlayerDirection == PlayerDirection.SOUTH && move2d.y > 0)
                {
                    Debug.Log("SHOULD TURN 180!!");
                    PlayerMovement.TurnState.TurnAngle = 180;
                   if (this is IdleState)
                        (this as IdleState).Turning = true;
                    else
                        (this as MoveState).Turning = true;
                }
                #endregion

                else if (this is IdleState)
                {
                    (this as IdleState).Moving = true;
                }
            }
        }
    }

    public class PlayerMovement : MonoBehaviour
    {
        private FSM playerMovementFSM = new FSM();

        private IdleState idleState;
        private TurnState turnState;
        private MoveState moveState;
        private EncircleTargetState encircleTargetState ;

        public IdleState IdleState => idleState;
        public TurnState TurnState => turnState;
        public MoveState MoveState => moveState;

        private bool encircleTarget;

        public bool EncircleTarget => encircleTarget;

        void Awake()
        {

            TargetChooser._onSoftLockTargetChanged += handleSoftLockTargetChanged;
			TargetChooser._onSoftLockTargetLost += handleSoftLockTargetLost;

			idleState = new IdleState(this);//zooming takes place in idle state
            turnState = new TurnState(this);
			moveState = new MoveState(this);
            encircleTargetState = new EncircleTargetState(this);

			int index1 = playerMovementFSM.AddState(idleState);
			int index2 = playerMovementFSM.AddState(turnState);
			int index3 = playerMovementFSM.AddState(moveState);
            int index4 = playerMovementFSM.AddState(encircleTargetState);


            //First calculate rotation angle in turn state => enter
            playerMovementFSM.AddTransition(index1, index2, idleState.GetExitGuard("Turning"));//(also towards near target)
            playerMovementFSM.AddTransition(index1, index3, idleState.GetExitGuard("Moving"));

            playerMovementFSM.AddTransition(index2, index1, turnState.GetExitGuard("Idle"));
            playerMovementFSM.AddTransition(index2, index3, turnState.GetExitGuard("Moving"));
            playerMovementFSM.AddTransition(index2, index4, turnState.GetExitGuard("EncircleTarget"));//turn state => encircleTarget state

            playerMovementFSM.AddTransition(index3, index1, moveState.GetExitGuard("Idle"));
			playerMovementFSM.AddTransition(index3, index2, moveState.GetExitGuard("Turning"));//(also towards near target)

			playerMovementFSM.AddTransition(index4, index2, encircleTargetState.GetExitGuard("Turning"));


			idleState.Enter();
        }

        private void handleSoftLockTargetChanged(Enemy oldEnemy, Enemy newEnemy)
        {
            if(TargetChooser.Instance.SoftLockMode == SoftLockMode.SHORTRANGE)
            {
                encircleTarget = true;
            }
        }

        private void handleSoftLockTargetLost()
        {
            if(TargetChooser.Instance.SoftLockMode == SoftLockMode.SHORTRANGE)
                encircleTarget = false;
        }

        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            if(Vector3.Angle(PlayerControllerInstance.Instance.BodyTransform.forward, -Vector3.right) < 1f)
                PlayerMovementState.PlayerDirection = PlayerDirection.WEST;

            else if(Vector3.Angle(PlayerControllerInstance.Instance.BodyTransform.forward, Vector3.forward) < 1f)
            {
                //Debug.Log("player direction is north??? ");
                PlayerMovementState.PlayerDirection = PlayerDirection.NORTH;
            }

            else if(Vector3.Angle(PlayerControllerInstance.Instance.BodyTransform.forward, Vector3.right) < 1f)
                PlayerMovementState.PlayerDirection = PlayerDirection.EAST;

            else if(Vector3.Angle(PlayerControllerInstance.Instance.BodyTransform.forward, -Vector3.forward) < 1f)
                PlayerMovementState.PlayerDirection = PlayerDirection.SOUTH;


            Debug.Log("player direction = " + PlayerMovementState.PlayerDirection);

            playerMovementFSM.Tick(Time.deltaTime);

            
        }

        void LateUpdate()
        {
            playerMovementFSM.LateTick(Time.deltaTime);
        }
    }
}