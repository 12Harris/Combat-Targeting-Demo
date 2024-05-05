using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Harris.Player.PlayerLocomotion
{
    using Harris.Util;

    public class PlayerMovementState:FSM_State
    {
        private Rigidbody rb;
        public Rigidbody RB => rb;

        protected float groundSpeed = 5f;

        protected float groundDeceleration = 0.9f;

        public PlayerMovementState()
        {
            rb = PlayerControllerInstance.Instance.transform.GetComponentInChildren<Rigidbody>();
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

        public virtual void decelerateOnGround()
        {
            if (rb.velocity == Vector3.zero)
                return;
            rb.velocity = rb.velocity * groundDeceleration;

        }
    }

    public class PlayerMovement : MonoBehaviour
    {
        private FSM playerMovementFSM = new FSM();

        void Awake()
        {
            //CAMERA LOOK AT FSM
			PlayerMovementState idleState = new IdleState();//zooming takes place in idle state
			PlayerMovementState moveState = new MoveState();
;
			int index1 = playerMovementFSM.AddState(idleState);
			int index2 = playerMovementFSM.AddState(moveState);
			
            playerMovementFSM.AddTransition(index1, index2, idleState.GetExitGuard("Moving"));
            playerMovementFSM.AddTransition(index2, index1, moveState.GetExitGuard("Idle"));
			
			//switch from switchtarget to zoom_lookat

			idleState.Enter();
        }

        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            playerMovementFSM.Tick(Time.deltaTime);
        }

        void LateUpdate()
        {
            playerMovementFSM.LateTick(Time.deltaTime);
        }
    }
}