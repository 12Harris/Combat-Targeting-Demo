// project armada

namespace Harris.Player
{
	using UnityEngine;
	using UnityEngine.Events;
	using System.Collections;
	using System.Collections.Generic;
	using System;

	internal abstract class PlayerMovementState : FSM_State
	{

		#region Global Variables
		
		private PlayerCollider playerCollider;

		protected Rigidbody rb;
		protected Transform transform;
		protected LayerMask ignoreSelf;

		#endregion

		#region Movement Variables (Set externally)

		protected float airSpeed;

		protected float walkSpeed;

		protected float runSpeed;

		protected float sprintSpeed;

		protected float jumpForce;
		public float JumpForce => jumpForce;

		protected float maxSlopeAngle;
		protected float groundDecelerationSpeed;
		protected float airDecelerationSpeed;

		protected bool useGravity = true;

		#endregion


		#region Movement Variables(Set internally)

		protected float currentSpeed;

		protected bool rotating;

		protected bool moveDelay;

		protected bool isGrounded;
		public bool Is_Grounded => isGrounded;

		protected bool onSlope;
		protected bool slopeDetected;

		protected RaycastHit[] groundHitInfo;

		public RaycastHit[] GroundHitInfo => groundHitInfo;

		protected RaycastHit ledgeHitInfo;

		protected RaycastHit slopeHit;

		//Meausred from player center
		protected float groundOffset;

		public enum MoveState
		{
			//locomotion state
			idle,
			walking,
			running,
			strafeLeft,
			strafeRight,
			strafeForward,
			strafeBackward,

			//sliding state
			sliding,

			//jumping state/falling state
			airUp,
			airDown,
			walljump,

			//ledge grab state
			grabbingLedge
		}

		private MoveState currentMoveState;

		public MoveState CurrentMoveState {get => currentMoveState; set => currentMoveState = value;}

		private MoveState prevMoveState;

		public MoveState PrevMoveState  {get => prevMoveState; set => prevMoveState = value;}

		protected float maxDistanceToFall = 0.2f;

		protected SlopeInfo slopeInfo;

		//public static event Action _onLandedOnGround; //Invoked in Jump state => exit

		#endregion

		#region Other Variables
		//Dictionary of exit conditions
		private IDictionary<string, Func<bool>> exitGuards = new Dictionary<string, Func<bool>>();

		public IDictionary<string, Func<bool>> ExitGuards => exitGuards;

		#endregion

		public PlayerMovementState(ref PlayerMovement.GlobalPlayerData globalPlayerData, ref PlayerMovementData movementData) : this(ref globalPlayerData)
		{

			//Initialize player movement data from PlayerMovement script
			airSpeed = movementData.airSpeed;
			walkSpeed = movementData.walkSpeed;
			runSpeed = movementData.runSpeed;
			sprintSpeed = movementData.sprintSpeed;
			jumpForce = movementData.jumpForce;
			//maxSlopeAngle = movementData.maxSlopeAngle;
			groundDecelerationSpeed = movementData.groundDecelerationSpeed;
			airDecelerationSpeed = movementData.airDecelerationSpeed;

		}

		public Vector3 GetVelocity()
		{
			return rb.velocity;
		}

		public PlayerMovementState(ref PlayerMovement.GlobalPlayerData globalPlayerData)
		{
			//Initialize global player data from PlayerMovement script
			
			rb = globalPlayerData._rb;
			transform = globalPlayerData._transform;
			ignoreSelf = globalPlayerData._ignoreSelf;
			maxSlopeAngle = globalPlayerData._maxSlopeAngle;

			groundHitInfo = new RaycastHit[5];
			slopeHit = new RaycastHit();
		}

        public override void Enter()
        {
			
            base.Enter();
        }

		public override void Tick(in float deltaTime)
		{
	
			#region Ground Check
			
			#endregion

			#region Gravity


			#endregion

		}

		public bool SlopeDetected(ref RaycastHit slopeHit)
		{
			

		}

		/// <summary>
		/// This method checks if the player is grounded
		/// </summary>
		/// <returns></returns>
		public bool IsGrounded(ref RaycastHit[] groundHitInfo, out bool onSlope)
		{
			
			return false;
		}


		protected bool LedgeDetected()
		{
			
			return false;
		}

		/// <summary>
		/// This method calculates the slope down direction given its slopeNormal
		/// </summary>
		/// <param name="slopeNormal"></param>
		/// <returns> The slope down direction</returns>
		protected Vector3 GetSlopeDownDirection(Vector3 slopeNormal)
		{
			//Calculate the left vector perpendicular to the slope Normal
			//Important: The cross product of 2 same vectors yield always a zero Vector!
			var left = Vector3.Cross(slopeNormal, Vector3.up);

			//Calculate the slope Direction Vector(downwards)
			var slope = Vector3.Cross(slopeNormal, left);

			//Normalize it
			slope = slope.normalized;

			//Make sure the perpendicular vector points in the downward direction of the slope
			if(slope.y > 0)
				slope*=-1;

			return slope;
		}

		protected bool LookingDownSlope(Vector3 slopeNormal)
		{
			return false;
		}


		/// <summary>
		/// This method checks whether the player detected a slope or not
		/// </summary>
		/// <returns></returns>
		private bool SlopeDetected(out Vector3 slopeNormal)
		{
			
			return false;
		}


		/// <summary>
		/// This method retrieves the 2d movement vector
		/// </summary>
		/// <returns>
		/// 2D movement vector
		/// </returns>
		public static Vector2 GetMovement()
		{
			var v = Vector2.zero;

			if (Input.GetKey(KeyCode.A)) { v.x += -1f; }
			if (Input.GetKey(KeyCode.D)) { v.x += 1f; }
			if (Input.GetKey(KeyCode.W)) { v.y += 1f; }
			if (Input.GetKey(KeyCode.S)) { v.y += -1f; }

			return v.normalized;
		}


		/// <summary>
		/// This method retrieves the 3d movement vector
		/// </summary>
		/// <returns>
		/// 3D movement vector
		/// </returns>
		public static Vector3 GetMovement3D(Vector2 movement2D)
		{
			//Convert 2d movement vector to 3d movement vector
			//and map the y component to the z component
			//since the player only moves on the XZ-Plane
			var move3d = new Vector3(movement2D.x,0,movement2D.y);

			//transform the 3d movement vector so the player  moves in the direction the camera is facing
			move3d = Quaternion.AngleAxis(CameraController.Instance.transform.rotation.eulerAngles.y, Vector3.up) * move3d;
			return move3d.normalized;
		}

		protected void AddExitGuard(string name, Func<bool> exitGuard)
		{
			if (!exitGuards.ContainsKey(name))
			{
				exitGuards.Add(name, exitGuard);
			}
		}

		public Func<bool> GetExitGuard(string name)
		{
			if (!exitGuards.ContainsKey(name)) return null;
			return exitGuards[name];
		}
	}

	internal sealed class StrafeState: PlayerMovementState
	{
		public static event Action _onEnter;
		public static event Action _onExit;

		private float turnSpeed = 10f;

		private bool tabKeyPressed;

		private bool spaceKeyPressed;

		private bool switchingTarget;

		private Quaternion switchTargetStartRotation;

		public Quaternion spineBoneRotation;
		private float timeElapsed = 0f;
		

		public StrafeState(ref PlayerMovement.GlobalPlayerData globalPlayerData, PlayerMovementData playerMovementData) : base(ref globalPlayerData, ref playerMovementData)
		{
			
			AddExitGuard("StoppedStrafing", () => {return tabKeyPressed;});
			AddExitGuard("StartedJumping", () => {return spaceKeyPressed;});
			SoftLock._onSoftLockTargetChanged += handleSoftLockTargetChanged;
		}

		public override void Enter()
		{
			Debug.Log("Entering player movement strafe state");
			_onEnter?.Invoke();
		}

		private void handleSoftLockTargetChanged(Transform newTarget)
		{
			//return if player is currently aiming at target
			if(switchingTarget)
				return;

			switchingTarget = true;

			switchTargetStartRotation = spineBone.rotation;
		}

	    public override void Tick(in float deltaTime)
        {
			base.Tick(in deltaTime);

			tabKeyPressed = Input.GetKeyDown(KeyCode.Tab);

			spaceKeyPressed = Input.GetKeyDown(KeyCode.Space);

			float yCam = CameraController.Instance.transform.rotation.eulerAngles.y;
			transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0,yCam,0), turnSpeed * Time.deltaTime);

			var move2d = GetMovement();

			currentSpeed = runSpeed;

			if(move2d.x == -1)
			{
				CurrentMoveState = MoveState.strafeLeft;
				rb.velocity = -transform.right* currentSpeed;	
			}

			else if(move2d.x == 1)
			{
				CurrentMoveState = MoveState.strafeRight;
				rb.velocity = transform.right* currentSpeed;
			}

			else if(move2d.y == 1)
			{
				CurrentMoveState = MoveState.strafeForward;
				rb.velocity = transform.forward* currentSpeed;
			}

			else if(move2d.y == -1)
			{
				CurrentMoveState = MoveState.strafeBackward;
				rb.velocity = -transform.forward* currentSpeed;
			}

			else
			{
				CurrentMoveState = MoveState.idle;
			}
		}

		
        public override void LateTick(in float deltaTime)
        {

        }

		private void FollowTarget(Transform target)
		{
			spineBone.LookAt(target);

		}

		public override void Exit()
		{
			_onExit?.Invoke();
		}

	}

	internal sealed class JumpState : PlayerMovementState
	{
		public static event Action _onStartJump; //Invoked in Jump state => enter
		private float camEulerAnglesY;
		private bool landed;

		private float maxShoulderRotation = 20f;
		private float currentShoulderRotation = 0f;
		private bool moveXZ;

		public JumpState(ref PlayerMovement.GlobalPlayerData globalPlayerData, PlayerMovementData playerMovementData) : base(ref globalPlayerData, ref playerMovementData)
		{
			
			//AddExitGuard("LandedOnSteepSlope",  () => {return landedOnSteepSlope;});
			AddExitGuard("LandedOnSteepSlope",  () => {return landedOnSteepSlope;});

			//AddExitGuard("LandedFlat", LandedFlat());
			AddExitGuard("LandedFlat", () => {return landedFlat;});		

			//AddExitGuard("GrabbedLedge", () => {return jumpingToLedge && distanceToLedge < 0.1f;});	
		}

		public override void Enter()
		{
			transform.GetComponent<PlayerMovement>().Fsm.CurrentState = this;
			landed = false;
			_onStartJump?.Invoke();
			
			camEulerAnglesY = CameraController.Instance.transform.rotation.eulerAngles.y;

			rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

			CurrentMoveState = MoveState.airUp;

		}

        public override void Tick(in float deltaTime)
        {
			base.Tick(in deltaTime);

			
			Quaternion desiredRotation = Quaternion.identity;
			var move2d = GetMovement();

			//Check if we want to move
			if (!Mathf.Approximately(move2d.magnitude, 0f))
			{	
				moveXZ = true;
				//Get 3d Movement vector
				var move3d = new Vector3(move2d.x,0,move2d.y);
				move3d = Quaternion.AngleAxis(camEulerAnglesY, Vector3.up) * move3d;

				//Calculate the desired targetRotation
				desiredRotation = Quaternion.LookRotation(move3d, Vector3.up);

				//if while moving the camera angle changes or we press another movement key the player will rotate towards its new movement direction
				if(transform.rotation != desiredRotation)
				{
					rotating = true;
				}

				//Once we reached the desired target rotation we can begin moving
				if(!rotating)
				{
					moveDelay = false;
					MovePlayer();
				}
			}
			
			//Set xz velocity to zero(caused by faulty gravity)
			if(!moveXZ)
			{
				rb.velocity = new Vector3(0,rb.velocity.y,0);
			}

			if(rotating)
			{
				rb.velocity = new Vector3(0,rb.velocity.y,0);
				if(moveDelay == false)
				{	
					moveDelay = true;
					transform.GetComponent<PlayerMovement>().StartCoroutine(transform.GetComponent<PlayerMovement>().RotateTowards(desiredRotation, 20f));
				}
			}
        }

		//Used for overriding animations
        public override void LateTick(in float deltaTime)
        {

        }

		/// <summary>
		/// Move Player while jumping
		/// </summary>
		private void MovePlayer()
		{
			rb.velocity = new Vector3(transform.forward.x * airSpeed ,rb.velocity.y,transform.forward.z * airSpeed);

		}

        public override void Exit()
        {
            Debug.Log("Exiting jump state!");
			moveXZ = false;
			CurrentMoveState = MoveState.idle;
			//(collider as CapsuleCollider).height = 1.8f;
        }

	}

	internal sealed class FallingState :PlayerMovementState
	{

		public static event Action _onLandedOnGround;
		public static event Action _onStartedFalling;
		private bool landedOnGround;
		public FallingState(ref PlayerMovement.GlobalPlayerData globalPlayerData, PlayerMovementData playerMovementData) : base(ref globalPlayerData,ref playerMovementData)
		{
			AddExitGuard("Landed", () => {return landedOnGround;});	
		}

		public override void Enter()
		{
			
			//Time.timeScale = 0;
			//transform.position += transform.forward*0.1f;
			transform.GetComponent<PlayerMovement>().Fsm.CurrentState = this;
			Debug.Log("ENTERING FALLING STATE!");
			CurrentMoveState = MoveState.airDown;
			animator.SetBool("IsFalling", true);

			rb.velocity = new Vector3(0,rb.velocity.y,0);
			landedOnGround = false;
			_onStartedFalling?.Invoke();
			
		}

        public override void Tick(in float deltaTime)
        {
			Debug.Log("Ticking falling state");

            base.Tick(deltaTime);
			
			if(isGrounded)
			{
				_onLandedOnGround?.Invoke();
				landedOnGround = true;
			}
        }

        public override void Exit()
        {
            base.Exit();

			//PlayerBottom.position -= Vector3.up * 0.5f;
			animator.SetBool("IsFalling", false);
			animator.SetTrigger("LandedOnGround");			
        }

	}

	internal sealed class LocomotionState : PlayerMovementState
	{
			
		private string _speedParameter = "Speed";
		private int _speedHash = -1;
		private float distanceToGround;
		private bool movingDownSlope;
		private bool movingUpSlope;
		private int test = 0;

		private Vector3 slopeHitpoint;

		private bool startedMovementCorrection;

		public static event Action _onLandedOnGround;

		private Vector3 desiredMoveDirection;

		private bool upperSlopeSteeper;
		private bool lowerSlopeSteeper;

		private bool willFall;

		private bool startedFalling;

		private bool startedStrafing;

		private float currentStepHeight = 0f;

		public static event Action _onEnter;

		private bool switchingTarget;

		private Quaternion switchTargetStartRotation;

		private float timeElapsed = 0f;

		public static LocomotionState Instance;

		public Quaternion spineBoneRotation;

		private bool tabKeyPressed;

		public LocomotionState(ref PlayerMovement.GlobalPlayerData globalPlayerData, PlayerMovementData playerMovementData) : base(ref globalPlayerData,ref playerMovementData)
		{
			_speedHash = Animator.StringToHash("Speed");
			AddExitGuard("Falling", () => {return startedFalling;});	
			AddExitGuard("Strafing", () => {return tabKeyPressed;});	
			Instance = this;
		}

		public override void Enter()
		{
			Debug.Log("Entering locomotion state");	
			startedFalling = false;
			CurrentMoveState = MoveState.idle;
			transform.GetComponent<PlayerMovement>().Fsm.CurrentState = this;
			tabKeyPressed = false;
			_onEnter?.Invoke();
			
		}

		public override void Exit()
		{
			Debug.Log("Exiting locomotion state");
		}

		#region Tick
		public override void Tick(in float deltaTime)
		{
			
		}

		public override void LateTick(in float deltaTime)
		{

			base.LateTick(in deltaTime);
		
		}

		/// <summary>
		/// Decelerates the player when he is grounded
		/// </summary>
		protected void DecelerateOnGround()
		{

		}

		/// <summary>
		/// In this method we move the player
		/// Movement happens both while grounded and airborn
		/// </summary>
		/// <param name="move3d"></param>
		/// <param name="isGrounded"></param>
		private void MovePlayer(ref bool upperSlopeSteeper, ref Vector3 desiredMoveDirection)
		{

		}	

		/// <summary>
		/// This method calculates the player movement direction when
		/// moving on a slope
		/// </summary>
		/// <param name="slopeInfo"></param>
		/// <returns></returns>
		private Vector3 GetSlopeMoveDirection(Vector3 slopeNormal)
		{

		}

	}

	public struct SlopeInfo
	{
		public Vector3 normal;
	}

	internal class PlayerMovement : MonoBehaviour
	{
		// fsm governing
		private FSM _fsm = new FSM();

		public FSM Fsm => _fsm;
		private bool spaceBarPressed;

		public static event Action _onRotationEnded;
		public static event Action _onEnableGravity;

		public Transform leftFootTip, leftFootHeel;
		public Transform rightFootTip, rightFootHeel;

		[SerializeField] private PlayerMovementData playerMovementData;

		[Serializable]
		public struct GlobalPlayerData
		{
			public Animator _animator;
			public PlayerCollider _collider;
			public Rigidbody _rb;
			public Transform _transform;
			public LayerMask _ignoreSelf;
			public float _maxSlopeAngle;

		}

		[SerializeField] private GlobalPlayerData globalPlayerData;

		public GlobalPlayerData GPlayerData => globalPlayerData;

		public static PlayerMovement Instance;


		private void Awake()
		{

			QualitySettings.vSyncCount = 0;
			Application.targetFrameRate = 60;

			Instance = this;

			PlayerMovementState s1 = new LocomotionState(ref globalPlayerData, playerMovementData);
			PlayerMovementState s2 = new JumpState(ref globalPlayerData, playerMovementData);
			PlayerMovementState s3 = new SlideState(50f,ref globalPlayerData);
			PlayerMovementState s4 = new LedgeGrabState(ref globalPlayerData);
			PlayerMovementState s5 = new FallingState(ref globalPlayerData, playerMovementData);
			PlayerMovementState s6 = new StrafeState(ref globalPlayerData, playerMovementData);

			// register states
			int index1 = _fsm.AddState(s1);
			int index2 = _fsm.AddState(s2);
			int index3 = _fsm.AddState(s3);
			int index4 = _fsm.AddState(s4);
			int index5 = _fsm.AddState(s5);
			int index6 = _fsm.AddState(s6);

			// register exits
			_fsm.AddTransition(index1, index2, OnSpaceBarPressed()); // LOCOMOTION STATE => JUMP STATE

			/*_fsm.AddTransition(index1, index5, s1.GetExitGuard("Falling")); // LOCOMOTION STATE => FALLING STATE

			_fsm.AddTransition(index1, index6, s1.GetExitGuard("Strafing")); // LOCOMOTION STATE => STRAFE STATE

			_fsm.AddTransition(index6, index1, s6.GetExitGuard("StoppedStrafing")); //STRAFE STATE => LOCOMOTION STATE

			_fsm.AddTransition(index6, index2, s6.GetExitGuard("StartedJumping")); //STRAFE STATE => Jump State

			_fsm.AddTransition(index5, index1, s5.GetExitGuard("Landed")); // FALLING STATE => LOCOMOTION STATE*/
			
			_fsm.AddTransition(index2, index1, s2.GetExitGuard("Landed")); //JUMP STATE => LOCOMOTION STATE

			/*_fsm.AddTransition(index2, index1, s2.GetExitGuard("LandedOnSteepSlope"));//JUMP STATE => LOCOMOTION STATE

			//_fsm.AddTransition(index2, index3, s2.GetExitGuard("LandedOnSteepSlope"));//JUMP STATE => SLIDE STATE

			_fsm.AddTransition(index2, index3, s2.GetExitGuard("LandedOnSteepSlope"));//JUMP STATE => SLIDE STATE

			_fsm.AddTransition(index2, index4, s2.GetExitGuard("GrabbedLedge"));//JUMP STATE => LEDGE GRAB STATE

			_fsm.AddTransition(index3,index1, s3.GetExitGuard("StoppedSliding"));//SLIDING STATE => LOCOMOTION STATE*/

			_fsm.CurrentState = s1;

			s1.Enter();
		}

		private void Update()
		{
			if(Application.targetFrameRate != 60)
             Application.targetFrameRate = 60;

			spaceBarPressed = Input.GetKeyDown(KeyCode.Space);

			// update fsm instance
			_fsm.Tick(Time.deltaTime);

			//spaceBarPressed = Input.GetKeyDown(KeyCode.Space);

		}

		private void LateUpdate()
		{
			_fsm.LateTick(Time.deltaTime);
		}

		private System.Func<bool> OnSpaceBarPressed()
		{
			System.Func<bool> fn = () =>
			{
				return spaceBarPressed;
			};
			return fn;
		}
	}
	#endregion
}