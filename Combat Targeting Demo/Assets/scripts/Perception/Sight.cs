namespace Harris.Perception
{

	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using System;
	using Harris.Util;
	//using Harris.NPC;

	// BEGIN enemy_visibility
	#if UNITY_EDITOR
	using UnityEditor;
	#endif

	[AddComponentMenu("Sensor/Sight")]

	// Detects when a given target is visible to this object. A target is
	// visible when it's both in range and in front of the target. Both the
	// range and the angle of visibility are configurable.
	public class Sight : Sensor
	{
		// The object we're looking for.
		public SensorTarget target = null;

		[SerializeField]
		private List<SensorTarget> targets;

		// If the object is more than this distance away, we can't see it.
		public float maxDistance = 10f;

		// The horizontal angle of our arc of visibility.
		[Range(0f, 360f)]
		public float horAngle = 45f;

		// If true, visualise changes in visilibity by changing material colour
		[SerializeField] bool visualize = true;

		// A property that other classes can access to determine if we can 
		// currently see our target.
		public bool targetIsVisible { get; private set; }


		public event Action<SensorTarget> _onTargetDetected;
		public event Action<SensorTarget> _onTargetLost;

		private float checkVisibilityTimer = 0f;
		private float checkVisibilityTimerInterval = 0.0f;

		[SerializeField]
		private LayerMask ignore;

		[SerializeField]
		private SensorTarget self;


		// Check to see if we can see the target every frame.
		void Update()
		{
			checkVisibilityTimer += Time.deltaTime;

			if (checkVisibilityTimer > checkVisibilityTimerInterval)
			{
				//targetIsVisible = CheckVisibility();
				CheckVisibility();
				checkVisibilityTimer = 0f;
			}

			if (visualize) {
				// Update our colour; yellow if we can see the target, white if
				// we can't
				var color = targetIsVisible ? Color.yellow : Color.white;

				//GetComponent<Renderer>().material.color = color;
			}

		}

		// Returns true if this object can see the specified position.
		public bool CheckVisibilityToPoint(Vector3 worldPoint) {

			// Calculate the direction from our location to the point
			var directionToTarget = worldPoint - transform.position;

			// Calculate the number of degrees from the forward direction.
			var degreesToTarget = 
				Vector3.Angle(transform.forward, directionToTarget);

			// The target is within the arc if it's within half of the
			// specified angle. If it's not within the arc, it's not visible.
			var withinArc = degreesToTarget < (horAngle / 2);

			if (withinArc == false)
			{
				return false;
			}

			// Figure out the distance to the target
			var distanceToTarget = directionToTarget.magnitude;

			// Take into account our maximum distance
			var rayDistance = Mathf.Min(maxDistance, distanceToTarget);

			// Create a new ray that goes from our current location, in the 
			// specified direction
			var ray = new Ray(transform.position, directionToTarget);

			// Stores information about anything we hit
			RaycastHit hit;

			// Perform the raycast. Did it hit anything?
			if (Physics.Raycast(ray, out hit, rayDistance)) {
				// We hit something. 
				if (hit.collider.transform == target) {
					// It was the target itself. We can see the target point.
					return true;
				}
				// It's something between us and the target. We cannot see the 
				// target point.
				return false;
			} else {
				// There's an unobstructed line of sight between us and the
				// target point, so we can see it.
				return true;
			}
		}

		//targets contains all targets within the visibility radius of the player
		//it would be extreme overhead and totally inefficient to check for all targets that exist in the game
		//it would only slow down the game
		void GetNearbyTargets(float radius)
		{
			targets.Clear();
			Collider[] hitColliders = Physics.OverlapSphere(transform.position, maxDistance);
			foreach (var hitCollider in hitColliders)
			{
				SensorTarget target = hitCollider.transform.GetComponentInChildren<SensorTarget>();
				if (target != null && target != self)
					targets.Add(target);
			}
		}

		// Returns true if a straight line can be drawn between this object and
		// the target. The target must be within range, and be within the
		// visible arc.
		public void CheckVisibility()
		{        
			//targetsSensed contains all targets that are in the fov of the player

			//get all targets in direct vicinity to player
			GetNearbyTargets(maxDistance);

			var rotationAngle1 = Quaternion.AngleAxis(-horAngle / 2, Vector3.up);
			var rotationAngle2 = Quaternion.AngleAxis(horAngle / 2, Vector3.up);

			Vector3 leftBorder = transform.forward;
			leftBorder = rotationAngle1 * leftBorder;

			Vector3 rightBorder = transform.forward;
			rightBorder = rotationAngle2 * rightBorder;

			//Debug.DrawRay(Vector3 start, Vector3 dir, Color color = Color.white, float duration = 0.0f, bool depthTest = true);
			Debug.DrawRay(transform.position, leftBorder,Color.green);
			Debug.DrawRay(transform.position, rightBorder,Color.green);


			foreach(SensorTarget sensorTarget in targets)
			{
				Transform closestTargetPoint1 = null;//for left vector
				Transform closestTargetPoint2 = null;//for right vector
				float minAngle1, minAngle2;
				
				minAngle1 = minAngle2 = 360;
				//var sensorTarget = target.gameObject.GetComponentInChildren<SensorTarget>();
				

				//get the closes visibility points on the sensorTarget
				foreach(Transform point in sensorTarget.VisibilityPoints)
				{
					point.gameObject.GetComponent<Renderer>().material.color = Color.white;
					var pointXZ = point.position;
					//pointXZ.y = 0;
					if (Vector3.Angle(pointXZ-transform.position, leftBorder ) < minAngle1)
					{
						minAngle1 = Vector3.Angle(pointXZ-transform.position, leftBorder );
						closestTargetPoint1 = point;
					}
					
					if (Vector3.Angle(pointXZ-transform.position, rightBorder) < minAngle2)
					{
						minAngle2 = Vector3.Angle(pointXZ-transform.position, rightBorder);
						closestTargetPoint2 = point;
					}
				}

				closestTargetPoint1.gameObject.GetComponent<Renderer>().material.color = Color.red;
				closestTargetPoint2.gameObject.GetComponent<Renderer>().material.color = Color.green;

				var dirToClosestTargetPoint1 = closestTargetPoint1.position - transform.position;
				dirToClosestTargetPoint1.y = 0;

				var dirToClosestTargetPoint2 = closestTargetPoint2.position - transform.position;
				dirToClosestTargetPoint2.y = 0;

				//Debug.DrawRay(transform.position, dirToClosestEnemyPoint1,Color.green);
				//Debug.DrawRay(transform.position, dirToClosestEnemyPoint2,Color.green);

				// Compute the horizontal direction to the target
				//var directionToTargetXZ = target.transform.position - transform.position;
				//directionToTargetXZ.y = 0;

				var forwardXZ = transform.forward;
				forwardXZ.y = 0;

				// Calculate the number of degrees from the horizontal forward direction.
				//var horDegreesToTarget = 
					//Vector3.Angle(transform.forward, directionToTarget);
					//Vector3.Angle(forwardXZ, directionToTargetXZ);

				
				var horDegreesToClosestTargetPoint1 = Vector3.Angle(forwardXZ, dirToClosestTargetPoint1 );
				var horDegreesToClosestTargetPoint2 = Vector3.Angle(forwardXZ, dirToClosestTargetPoint2 );

				// The target is within the arc if it's within half of the
				// specified horizontal angle AND if . If it's not within the arc, it's not visible.
				//var withinArc = horDegreesToTarget < (horAngle / 2);
				
				//var withinArc = horDegreesToClosestEnemyPoint1 < (horAngle / 2);
				var withinArc = horDegreesToClosestTargetPoint1 < (horAngle / 2) || horDegreesToClosestTargetPoint2 < (horAngle / 2);

				if (withinArc == false) {
					//return false;
					if(_targetsSensed.Contains(sensorTarget))
					{
						_targetsSensed.Remove(sensorTarget);
						_onTargetLost.Invoke(sensorTarget);
					}
					continue;
				}

				//left ray OR right ray(direction to enemy) must intersect enemy
				
				// Compute the HORIZONTAL distance to the point
				//var horDistanceToTarget = directionToTargetXZ.magnitude;

				// Our ray should go as far as the target is, or the maximum
				// distance, whichever is shorter
				//var rayDistance = Mathf.Min(maxDistance, horDistanceToTarget);

				// Create a ray that fires out from our position to the target
				//var ray = new Ray(transform.position, directionToTargetXZ);

				// Compute the HORIZONTAL distance to the closes enemy point1
				var horDistanceToCTP1 = dirToClosestTargetPoint1.magnitude;
				var horDistanceToCTP2 = dirToClosestTargetPoint2.magnitude;

				// Our ray should go as far as the target is, or the maximum
				// distance, whichever is shorter
				var rayDistance1 = Mathf.Min(maxDistance, horDistanceToCTP1);
				var rayDistance2 = Mathf.Min(maxDistance, horDistanceToCTP2);

				// Create a ray that fires out from our position to the target
				var ray1 = new Ray(transform.position, dirToClosestTargetPoint1);
				var ray2 = new Ray(transform.position, dirToClosestTargetPoint2);

				// Store information about what was hit in this variable
				RaycastHit hit;

				// Records info about whether the target is in range and not
				// occluded
				var canSee = false;

				// Fire the raycasts. Did they hit anything?

				//Debug.DrawRay(transform.position, dirToClosestTargetPoint1*rayDistance1, Color.red);
				//Debug.DrawRay(transform.position, dirToClosestTargetPoint2*rayDistance2, Color.red);

				if (Physics.Raycast(ray1, out hit, rayDistance1, ignore) || (Physics.Raycast(ray2, out hit, rayDistance2, ignore)))
				{
					// Did the ray hit our target?
					//if (hit.collider.transform == target || hit.collider.transform.parent == target)

					SensorTarget target = hit.collider.transform.parent.GetComponentInChildren<SensorTarget>();
					//if (hit.collider.transform.parent.TryGetComponent<SensorTarget>(out SensorTarget target))
					if(target != null)
					{
						// Then we can see it (that is, the ray didn't hit an
						// obstacle in between us and the target)
						canSee = true;

						if(!_targetsSensed.Contains(target))
						{
							_targetsSensed.Add(target);
							_onTargetDetected?.Invoke(target);
						}
					}
					else
					{
						Debug.Log("sensortarget is null!");
					}

					Debug.Log("parent = " + hit.collider.transform.parent);
					Debug.Log("Ray hit: " + hit.collider.transform.gameObject);

					// Visualise the ray.
					Debug.DrawLine(transform.position, hit.point);

				}
				else
				{
					if(_targetsSensed.Contains(sensorTarget))
					{
						_targetsSensed.Remove(sensorTarget);
						_onTargetLost.Invoke(sensorTarget);
					}
					// The ray didn't hit anything. This means that it reached the
					// maximum distance, and stopped, which means we didn't hit our
					// target. It must be out of range.

					// Visualise the rays.
					//Debug.DrawRay(transform.position, 
								//directionToTargetXZ.normalized * rayDistance1);
					//Debug.Log("Ray didnt hit anything!");

				}
			}

			// Is it visible?
			//return canSee;

		}
	}

	#if UNITY_EDITOR
	// A custom editor for the EnemyVisibility class. Visualises and allows
	// editing the visible range.
	[CustomEditor(typeof(Sight))]
	public class SightEditor : Editor {

		// Called when Unity needs to draw the Scene view. 
		private void OnSceneGUI()
		{
			// Get a reference to the EnemyVisibility script we're looking at
			var visibility = target as Sight;

			// Start drawing at 10% opacity
			Handles.color = new Color(1, 1, 1, 0.1f);

			// Drawing an arc sweeps from the point you give it. We want to
			// draw the arc such that the middle of the arc is in front of the
			// object, so we'll take the forward direction and rotate it by
			// half the angle.

			var forwardPointMinusHalfAngle = 
				// rotate around the Y axis by half the angle
				Quaternion.Euler(0, -visibility.horAngle / 2, 0) 
						// rotate the forward direction by this
						* visibility.transform.forward;

			// Draw the arc to visualise the visibility arc
			Vector3 arcStart = 
				forwardPointMinusHalfAngle * visibility.maxDistance;

			Handles.DrawSolidArc(
				visibility.transform.position, // The center of the arc
				Vector3.up,                    // The up-direction of the arc
				arcStart,                      // The point where it begins
				visibility.horAngle,              // The angle of the arc
				visibility.maxDistance         // The radius of the arc
			);


			// Draw a scale handle at the edge of the arc; if the user drags
			// it, update the arc size.

			// Reset the handle colour to full opacity
			Handles.color = Color.white;

			// Compute the position of the handle, based on the object's
			// position, the direction it's facing, and the distance
			Vector3 handlePosition = 
				visibility.transform.position + 
					visibility.transform.forward * visibility.maxDistance;

			// Draw the handle, and store its result.
			visibility.maxDistance = Handles.ScaleValueHandle(
				visibility.maxDistance,         // current value
				handlePosition,                 // handle position
				visibility.transform.rotation,  // orientation
				1,                              // size
				Handles.ConeHandleCap,          // cap to draw
				0.25f);                         // snap to multiples of this 
												// if the snapping key is
												// held down
		}
	}
	#endif
	// END sight
}