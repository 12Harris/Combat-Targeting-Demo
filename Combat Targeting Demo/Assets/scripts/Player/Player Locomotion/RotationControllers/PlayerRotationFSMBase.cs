using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Harris.Player.PlayerLocomotion.Rotation
{
    using Harris.Util;
    using Harris.Player.Combat;
    using Harris.NPC;

    public class IdleState:FSM_State
    {   
        //Status variables
        private bool turning= false;
        private PlayerRotationFSM fsm;
        private EnemyController target;

        private bool disableTargetChooser;

        public IdleState(PlayerRotationFSM _fsm)
        {
            fsm = _fsm;
            TargetChooser._onSoftLockTargetChanged += handleSoftLockTargetChanged;
            AddExitGuard("Turning", () => {return turning;});
        }

        private void  handleSoftLockTargetChanged(EnemyController e1, EnemyController e2)
        {
            if(fsm is PlayerLowerBodyRotationFSM && TargetChooser.Instance.SoftLockMode == SoftLockMode.LONGRANGE)
                return;
            
            target = e2;
            fsm.TurnState.TurnAngle = fsm.getAngleToTarget(e2);
            turning = true;
        }

        public override void Enter()
        {
            turning = false;
            target = null;
            disableTargetChooser = false;
        }

        public override void Tick(in float dt)
        {
            var move2d = PlayerRotationFSM.GetKeyboardInput();

            #region WEST

            if(PlayerMovementState.PlayerDirection == PlayerDirection.WEST && move2d.y > 0)
            {
                fsm.TurnState.TurnAngle = 90;
                turning = true;
                disableTargetChooser = true;
            }

            else if(PlayerMovementState.PlayerDirection == PlayerDirection.WEST && move2d.x > 0)
            {
                fsm.TurnState.TurnAngle = 180;
                turning = true;
                disableTargetChooser = true;
            }

            //turn 90° counter clockwise(south)
            else if(PlayerMovementState.PlayerDirection == PlayerDirection.WEST && move2d.y < 0)
            {
                fsm.TurnState.TurnAngle = -90;
                turning = true;
                disableTargetChooser = true;
            }
            #endregion

            #region NORTH
                //turn 90° counter clockwise(west)
                else if(PlayerMovementState.PlayerDirection == PlayerDirection.NORTH && move2d.x < 0)
                {
                    fsm.TurnState.TurnAngle = -90;
                    turning = true;
                    disableTargetChooser = true;
                } 

                //turn 90° clockwise(east)
                else if(PlayerMovementState.PlayerDirection == PlayerDirection.NORTH && move2d.x > 0)
                {
                    fsm.TurnState.TurnAngle = 90;
                    turning = true;
                    disableTargetChooser = true;
                }

                //turn 180° clockwise(south)
                else if(PlayerMovementState.PlayerDirection == PlayerDirection.NORTH && move2d.y < 0)
                {
                    fsm.TurnState.TurnAngle = 180;
                    turning = true;
                    disableTargetChooser = true;
                }
            #endregion

            #region EAST

                 //turn 90° counter clockwise(north)
                if(PlayerMovementState.PlayerDirection == PlayerDirection.EAST && move2d.y > 0)
                {
                    fsm.TurnState.TurnAngle = -90;//IF THIS IS HEADROTATIONCONTROLLER...
                    turning = true;
                    disableTargetChooser = true;
                }

                //turn 180° clockwise(east)
                else if(PlayerMovementState.PlayerDirection == PlayerDirection.EAST && move2d.x < 0)
                {
                    fsm.TurnState.TurnAngle = 180;
                    turning = true;
                    disableTargetChooser = true;
                }

                //turn 90°clockwise(south)
                else if(PlayerMovementState.PlayerDirection == PlayerDirection.EAST && move2d.y < 0)
                {
                    fsm.TurnState.TurnAngle = 90;
                    turning = true;
                    disableTargetChooser = true;
                }

            #endregion

            #region SOUTH
                //turn 90° clockwise(west)
                else if(PlayerMovementState.PlayerDirection == PlayerDirection.SOUTH && move2d.x < 0)
                {
                    fsm.TurnState.TurnAngle = 90;
                    turning = true;
                    disableTargetChooser = true;
                } 

                //turn 90° clockwise(east)
                else if(PlayerMovementState.PlayerDirection == PlayerDirection.SOUTH && move2d.x > 0)
                {
                    fsm.TurnState.TurnAngle = -90;
                    turning = true;
                    disableTargetChooser = true;
                }

                //turn 180° clockwise(north)
                else if(PlayerMovementState.PlayerDirection == PlayerDirection.SOUTH && move2d.y > 0)
                {
                    Debug.Log("SHOULD TURN 180!!");
                    fsm.TurnState.TurnAngle = 180;
                    turning = true;
                    disableTargetChooser = true;
                }
                #endregion
            if(turning && fsm is PlayerUpperBodyRotationFSM || fsm is PlayerHeadRotationFSM)
            fsm.TurnState.TurnAngle += fsm.getAngleBetweenHeadAndLowerBody();
        }

        public override void Exit()
        {
            if(disableTargetChooser)
            {
                TargetChooser.Instance.ChosenTarget = null;
                TargetChooser.Instance.OldTarget = null;
                TargetChooser.Instance.enabled = false;
            }
        }
    }

    public class TurnState:FSM_State
    {

        private PlayerRotationFSM fsm;
        private float turnAngle;
        public float TurnAngle {get=>turnAngle; set=>turnAngle = value;}
        private bool idle;
        private bool turnAgain;
        private bool lookAtTarget;
        private RotateObject rotator;

        public TurnState(PlayerRotationFSM _fsm)
        {
            fsm = _fsm;
            rotator = _fsm.Rotator;
            rotator._onStopRotation += handleRotationCompleted;
            TargetChooser._onSoftLockTargetChanged += handleSoftLockTargetChanged;
            AddExitGuard("LookAtTarget", () => {return lookAtTarget;});
            AddExitGuard("Idle", () => {return idle;});
            AddExitGuard("TurnAgain", () => {return turnAgain;});
        }

        private void handleSoftLockTargetChanged(EnemyController e1, EnemyController e2)
        {
            rotator.Interrupt = true;
        }

        public override void Enter()
        {
            rotator.StartCoroutine(rotator.Rotate(TurnAngle));
            idle = false;
            turnAgain = false;
            
        }

        //not true if while resetting rotation the player sights a new target
        private void handleRotationCompleted()
        {
            if(TargetChooser.Instance.ChosenTarget != null)
            {
                lookAtTarget = true;
            }
            else
            {
                idle = true;
                TargetChooser.Instance.enabled = true;
            }
        }


        public override void Tick(in float dt)
        {
            if(rotator.Interrupt == true)
            {
                turnAgain = true;
                return;
            }
        }

    }


    //reset head rotation when target lost

    public class LookAtTargetState:FSM_State
    {

        private PlayerRotationFSM fsm;
        private EnemyController target;
        public EnemyController Target{get=>target; set=>target = value;}
        private bool turning;
        private RotateObject rotator;
        private List<Vector3> worldAxis;


        public  LookAtTargetState (PlayerRotationFSM _fsm)
        {
            fsm = _fsm;
            rotator = _fsm.Rotator;
            TargetChooser._onSoftLockTargetLost+= handleSoftLockTargetLost;
            AddExitGuard("Turning", () => {return turning;});
            worldAxis = new List<Vector3>();
            worldAxis.Add(Vector3.up);
            worldAxis.Add(Vector3.down);
        }

        public override void Enter()
        {
            turning = false;
        }

        private void handleSoftLockTargetLost()
        {

            if(TargetChooser.Instance.SoftLockMode == SoftLockMode.SHORTRANGE)
            {
                var invForward = -PlayerControllerInstance.Instance.BodyTransform.forward;

                if(worldAxis.Contains(-Vector3.right))
                    worldAxis.Remove(-Vector3.right);

                if(worldAxis.Contains(-Vector3.left))
                    worldAxis.Remove(-Vector3.right);

                if(invForward.x < 0)
                {
                    worldAxis.Add(Vector3.left);
                }
                else
                {
                    worldAxis.Add(Vector3.right);
                }

                var minAngle = 360f;

                Vector3 chosenAxis = Vector3.zero;

                foreach(Vector3 axis in worldAxis)
                {
                    if (Vector3.Angle(invForward, axis) < minAngle)
                    {
                        minAngle = Vector3.Angle(invForward, axis);
                        chosenAxis = axis;
                        Debug.Log("MinAngle to axis: " + axis + " = " + minAngle);
                    }
                }


                Debug.Log("chosen axis = " + chosenAxis);

                var turnAngle = 180 - Vector3.Angle(invForward, chosenAxis);

                //Does the player need to turn left or right?
                LeftRightTest lrTest = new LeftRightTest(chosenAxis, invForward, Vector3.up);

                if (lrTest.targetIsLeft())
                    turnAngle *=-1;

                fsm.TurnState.TurnAngle = turnAngle;
            }

            turning = true;
            
        }

        public override void Exit()
        {
            TargetChooser.Instance.ChosenTarget = null;
            TargetChooser.Instance.OldTarget = null;
            TargetChooser.Instance.enabled = false;
        }

        public override void Tick(in float dt)
        {
            var v = target.transform.position;
            v.y = rotator.transform.position.y;
            rotator.transform.LookAt(v);
        }

    }

    public abstract class PlayerRotationFSM
    {
        private Transform headTransform;
        private Transform lowerBodyTransform;

        private RotateObject rotator;
        public RotateObject Rotator => rotator;

        private FSM_State currentState;
        public FSM_State CurrentState => currentState;

        public PlayerRotationFSM(Transform _hT, Transform _lbT, RotateObject _rotator)
        {
            headTransform = _hT;
            lowerBodyTransform = _lbT;
            rotator = _rotator;
        }

        private IdleState idleState;
        public IdleState IdleState => idleState;

        private TurnState turnState;
        public TurnState TurnState => turnState;

        private LookAtTargetState lookAtTargetState;
        public LookAtTargetState LookAtTargetState => lookAtTargetState;

        private FSM fsm;

        public void Init()
        {
            fsm = new FSM();

            idleState = new IdleState(this);//zooming takes place in idle state
            turnState = new TurnState(this);
            lookAtTargetState = new LookAtTargetState(this);

            int index1 = fsm.AddState(idleState);
            int index2 = fsm.AddState(turnState);
            int index3 = fsm.AddState(lookAtTargetState);

            fsm.AddTransition(index1, index2, idleState.GetExitGuard("Turning"));

            fsm.AddTransition(index2, index1, turnState.GetExitGuard("Idle"));
            fsm.AddTransition(index2, index3, turnState.GetExitGuard("LookAtTarget"));
            fsm.AddTransition(index2, index2, turnState.GetExitGuard("TurnAgain"));

            fsm.AddTransition(index3, index2, lookAtTargetState.GetExitGuard("Turning"));

            idleState.Enter();
        }

        public void Tick(in float dt)
        {   
            fsm.Tick(dt);
            currentState = fsm.CurrentState;
        }

        public static Vector2 GetKeyboardInput()
		{
			var v = Vector2.zero;

			if (Input.GetKey(KeyCode.A)) { v.x += -1f; }
			if (Input.GetKey(KeyCode.D)) { v.x += 1f; }
			if (Input.GetKey(KeyCode.W)) { v.y += 1f; }
			if (Input.GetKey(KeyCode.S)) { v.y += -1f; }

			return v.normalized;
		}

        public float getAngleToTarget(EnemyController target)
		{
			var angle= 0f;

			var headTransformForwardXZ = headTransform.forward;
			headTransformForwardXZ.y = 0;

			var dirToTargetXZ = target.transform.position - headTransform.position;
			dirToTargetXZ.y = 0;

			angle= Vector3.Angle(headTransformForwardXZ, dirToTargetXZ);

			//Does the head need to turn left or right?
			LeftRightTest lrTest = new LeftRightTest(headTransform, target.transform);

			if (lrTest.targetIsLeft())
				angle *=-1;
			
			return angle;
		}

        public float getAngleBetweenHeadAndLowerBody()
		{
			var angle= 0f;

			var headTransformForwardXZ = headTransform.forward;
			headTransformForwardXZ.y = 0;

			var bodyransformForwardXZ = lowerBodyTransform.forward;
			bodyransformForwardXZ.y = 0;

			angle= Vector3.Angle(headTransformForwardXZ, bodyransformForwardXZ);

			//Does the head need to turn left or right?
			LeftRightTest lrTest = new LeftRightTest(headTransform, lowerBodyTransform.transform);

			if (lrTest.targetIsLeft())
				angle *=-1;
			
			return angle;
		}
    }

    public class PlayerHeadRotationFSM:PlayerRotationFSM
    {
        public PlayerHeadRotationFSM(Transform _hT, Transform _lbT, RotateObject _rotator): base(_hT,_lbT,_rotator){}
    }

    public class PlayerUpperBodyRotationFSM:PlayerRotationFSM
    {
        public PlayerUpperBodyRotationFSM(Transform _hT, Transform _lbT, RotateObject _rotator): base(_hT,_lbT,_rotator){}
    }

    public class PlayerLowerBodyRotationFSM:PlayerRotationFSM
    {
        public PlayerLowerBodyRotationFSM(Transform _hT, Transform _lbT, RotateObject _rotator): base(_hT,_lbT,_rotator){}
    }
}
