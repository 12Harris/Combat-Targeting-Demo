namespace Harris
{
	using UnityEngine;
	using System;

	// Detects when a given target is visible to this object. A target is
	// visible when it's both in range and in front of the target. Both the
	// range and the angle of visibility are configurable.
	[AddComponentMenu("Sensor/Sight")]
	public partial class Sight : Sensor
	{
		public float MaxDistance => _maxDistance;
		public float Angle => _angle;

		// the object we're looking for.
		//public Transform CurrentTarget { get; private set; }

		public bool TargetIsVisible { get; private set; }

		public Vector3 LastPositionOfTarget { get; private set; }

		// If the object is more than this distance away, we can't see it.
		[Min(0f)]
		[SerializeField] private float _maxDistance = 10f;

		// The angle of our arc of visibility.
		[Range(0f, 360f)]
		[SerializeField] private float _angle = 45f;

		[SerializeField] private LayerMask _raycastLayers = -1;

		[SerializeField] private Transform[] _targets = { };

		// A property that other classes can access to determine if we can 
		// currently see our target.

		public float _degreesToTarget = 0f;
		private bool _withinArc = false;

		private Vector3 _lastHit = default;

		public LayerMask IgnorePlayer;

		public event Action<Transform> _onTargetDetected;
		public event Action<Transform> _onTargetRemoved;


		// Check to see if we can see the target every frame.
		private void Update()
		{
			CheckVisibility();

			//TargetIsVisible = CheckVisibility();
		}

		// Returns true if this object can see the specified position.
		public bool CheckVisibilityToPoint(Vector3 worldPoint)
		{

			foreach (var target in _targets)
			{
				// Calculate the direction from our location to the point
				var directionToTarget = worldPoint - transform.position;

				// Calculate the number of degrees from the forward direction.
				var degreesToTarget = Vector3.Angle(transform.forward, directionToTarget);

				// The target is within the arc if it's within half of the
				// specified angle. If it's not within the arc, it's not visible.
				var withinArc = degreesToTarget < (_angle / 2);

				if (withinArc == false)
				{
					return false;
				}

				// Figure out the distance to the target
				var distanceToTarget = directionToTarget.magnitude;

				// Take into account our maximum distance
				var rayDistance = Mathf.Min(_maxDistance, distanceToTarget);

				// Create a new ray that goes from our current location, in the 
				// specified direction
				var ray = new Ray(transform.position, directionToTarget);

				// Stores information about anything we hit
				RaycastHit hit;

				// Perform the raycast. Did it hit anything?
				if (Physics.Raycast(ray, out hit, rayDistance, _raycastLayers))
				{
					// We hit something. 
					if (hit.collider.transform == target.transform)
					{
						// It was the target itself. We can see the target point.
						//CurrentTarget = target.transform;
						return true;
					}
					// It's something between us and the target. We cannot see the 
					// target point.
					return false;
				}
				else
				{
					// There's an unobstructed line of sight between us and the
					// target point, so we can see it.
					//CurrentTarget = target.transform;
					return true;
				}
			}
			return false;
		}


		// Returns true if a straight line can be drawn between this object and
		// the target. The target must be within range, and be within the
		// visible arc.
		public void CheckVisibility()
		{
			foreach (var target in _targets)
			{
				// Compute the direction to the target
				var directionToTarget = target.transform.position - transform.position;
				//var directionToTarget = new Vector3(target.transform.position.x, transform.position.y + transform.forward.y, target.transform.position.z) - transform.position;

				// Calculate the number of degrees from the forward direction.
				_degreesToTarget = Vector3.Angle(transform.forward, directionToTarget);

				// The target is within the arc if it's within half of the
				// specified angle. If it's not within the arc, it's not visible.
				_withinArc = _degreesToTarget < (_angle / 2);

				if (_withinArc == false)
				{
					//return false;
					continue;
				}

				// Compute the distance to the point
				var distanceToTarget = directionToTarget.magnitude;

				// Our ray should go as far as the target is, or the maximum
				// distance, whichever is shorter
				var rayDistance = Mathf.Min(_maxDistance, distanceToTarget);

				// Create a ray that fires out from our position to the target
				var ray = new Ray(transform.position, directionToTarget);

				// Store information about what was hit in this variable
				RaycastHit hit;

				// Records info about whether the target is in range and not
				// occluded
				//var canSee = false;

				// Fire the raycast. Did it hit anything?

				//make sure we dont hit the "Player" or "Helper" Scene Object, we only want to check if we hit the P.Target or H.Target Object
				if (Physics.Raycast(ray, out hit, rayDistance, IgnorePlayer))
				//if (Physics.Raycast(ray, out hit, rayDistance, IgnorePlayer))
				{
					
				}


				else
				{
					
				}
			}

		}
	}
}

#if UNITY_EDITOR
namespace Harris
{
	using UnityEngine;
	using UnityEditor;


	public partial class Sight
	{
		private void OnDrawGizmos()
		{
			var visualize = false;

			if (Application.isPlaying)
			{
				//visualize = NPCManager.Instance && NPCManager.Instance.Visualize;
			}
			if (!visualize) { return; }

			var forwardPointMinusHalfAngle =
				// rotate around the Y axis by half the angle
				Quaternion.Euler(0, -Angle / 2, 0)
						// rotate the forward direction by this
						* transform.forward;

			// Draw the arc to visualise the visibility arc
			Vector3 arcStart = forwardPointMinusHalfAngle * MaxDistance;

			Handles.DrawSolidArc
			(
				transform.position, // The center of the arc
				Vector3.up,                     // The up-direction of the arc
				arcStart,                       // The point where it begins
				Angle,              // The angle of the arc
				MaxDistance         // The radius of the arc
			);


			// Draw a scale handle at the edge of the arc; if the user drags
			// it, update the arc size.

			// Reset the handle colour to full opacity
			Handles.color = Color.white;

			// Compute the position of the handle, based on the object's
			// position, the direction it's facing, and the distance
			//Vector3 handlePosition =
			//	_target.transform.position +
			//		_target.transform.forward * _target.MaxDistance;

			// Draw the handle, and store its result.
			//visibility._maxDistance = Handles.ScaleValueHandle
			//(
			//	visibility.MaxDistance,				// current value
			//	handlePosition,						// handle position
			//	visibility.transform.rotation,		// orientation
			//	1,									// size
			//	Handles.ConeHandleCap,				// cap to draw
			//	0.25f								// snap to multiples of this 
			//);								
			// if the snapping key is
			// held down
		}
	}


}
#endif