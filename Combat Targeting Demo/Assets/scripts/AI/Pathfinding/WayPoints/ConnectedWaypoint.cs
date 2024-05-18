// project armada

namespace Harris.AI.PathFinding
{
	using System.Collections.Generic;
	using UnityEngine;

	[AddComponentMenu("Pathfinding/Connected Waypoint")]
	public partial class ConnectedWaypoint : Waypoint
	{

		[SerializeField]
		private LayerMask ignoreEntity;

		[SerializeField]
		private List<Waypoint> ignoreWaypoints;

		//Finds a next random connected waypoint
		public ConnectedWaypoint GetNext2(ConnectedWaypoint previous)
		{
			if(_connections.Count == 0) { return null; }

			var pi = _connections.IndexOf(previous);

			//Only one waypoint and it's the previous one? Just use that.
			if (_connections.Count == 1 && pi > -1) { return previous; }

			//Otherwise, find a random one that isn't the previous one
			ConnectedWaypoint next = null;

			while(next == previous)
			{
				next = _connections[Random.Range(0, _connections.Count)];
				Debug.Log("while loop lol");
			}

			return next;
		}

		public ConnectedWaypoint GetNext(ConnectedWaypoint previous)
		{
			if(_connections.Count == 0) { return null; }

			var pi = _connections.IndexOf(previous);

			//Only one waypoint and it's the previous one? Just use that.
			if (_connections.Count == 1 && pi > -1) { return previous; }

			//Otherwise, find a random one that isn't the previous one
			ConnectedWaypoint next;
			do
			{
				next = _connections[Random.Range(0, _connections.Count)];
			}
			while(next == previous);

			return next;
		}

		[Min(0.1f)]
		[SerializeField] private float _connectivityRadius = 50f;

		//The owner of the waypoint
		//There can be only 1 owner for any connected waypoint
		//[SerializeField] private SensorTarget owner;
		//public SensorTarget Owner => owner;

		private List<ConnectedWaypoint> _connections = new List<ConnectedWaypoint>();
		public List<ConnectedWaypoint> Connections => _connections;

		private void Start()
		{
			//Grab all waypoint objects in scene, THIS IS INEFFICIENT, instead do physicy overlapsphere...
			/*foreach(var wp in FindWaypoints<ConnectedWaypoint>())
			{
				if (Vector3.Distance(transform.position, wp.transform.position) <= _connectivityRadius && wp != this)
				{
					//If there is an obstacle between the 2 waypoints then dont connect them and continue
					if(Physics.Raycast(transform.position,wp.transform.position-transform.position,(wp.transform.position-transform.position).magnitude,ignoreEntity))
						continue;
					
					//There can be only 1 owner for any connected waypoint
					//if(wp.Owner != this.Owner)	
						//continue;

					//should this waypoint be ignored?
					if(ignoreWaypoints.Contains(wp))
						continue;

					_connections.Add(wp);
				}
			}*/

			Collider[] hitColliders = Physics.OverlapSphere(transform.position, _connectivityRadius);
			foreach (var hitCollider in hitColliders)
			{
				if(hitCollider.gameObject.TryGetComponent<ConnectedWaypoint>(out ConnectedWaypoint wp) && wp != this)
				{
					Debug.Log("other waypoint detected!");
					//If there is an obstacle between the 2 waypoints then dont connect them and continue
					if(Physics.Raycast(transform.position,wp.transform.position-transform.position,(wp.transform.position-transform.position).magnitude,ignoreEntity))
					{
						Debug.Log("waypoint => continue");
						continue;
					}

					//should this waypoint be ignored?
					if(ignoreWaypoints.Contains(wp))
						continue;

					_connections.Add(wp);
				} 
			}

		}

		public void DrawConnections()
		{
			foreach(var wp in _connections)
			{
				Debug.DrawRay(transform.position + Vector3.up, (wp.transform.position - transform.position) * (wp.transform.position - transform.position).magnitude, Color.green,10f);
			}
		}

	}
}


#if UNITY_EDITOR
namespace Harris.AI.PathFinding
{
	using UnityEngine;

	public partial class ConnectedWaypoint
	{
		//private const float _GIZMO_RADIUS = _connectivityRadius;
		private static readonly Color _GIZMO_COLOR = Color.blue;

		private void OnDrawGizmos()
		{
			Gizmos.color = _GIZMO_COLOR;
			Gizmos.DrawWireSphere(transform.position,_connectivityRadius);
		}
	}
}
#endif