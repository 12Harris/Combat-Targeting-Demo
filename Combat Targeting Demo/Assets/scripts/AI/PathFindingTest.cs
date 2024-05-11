using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Harris.AI
{
    public class PathFindingTest : MonoBehaviour
    {

        private Pathfinding pathfinding;
        private List<PathNode> path;
        // Start is called before the first frame update
        void Start()
        {
            pathfinding = new Pathfinding();
            List<PathNode> path = pathfinding.FindPath(0,0,6,7);
        }

        // Update is called once per frame
        void Update()
        {
            if(path != null)
            {
                for(int i=0; i < path.Count - 1; i++)
                {
                    //public static void DrawLine(Vector3 start, Vector3 end, Color color = Color.white, float duration = 0.0f, bool depthTest = true);
                    Debug.DrawLine(new Vector3(path[i].X,1,path[i].Y), new Vector3(path[i+1].X,1,path[i+1].Y),Color.red);
                }
            }
        }
    }
}
