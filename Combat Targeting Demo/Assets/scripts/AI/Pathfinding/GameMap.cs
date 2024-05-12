using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Harris.AI.PathFinding
{
    public class GameMap : MonoBehaviour
    {
        private static char[,] map = new char[10,10];
        /*private static char[,] map = {{'w','w','w','w','w','w',' ','w','w','w'},
                                    {'w','w',' ',' ',' ',' ',' ','w','w','w'},
                                    {'w','w','w','w','w','w','w','w','w','w'},
                                    {'w','w','w','w','w','w','w','w','w','w'},
                                    {' ',' ',' ',' ','w','w','w','w','w','w'},
                                    {'w','w','w','w','w','w','w','w','w','w'},
                                    {'w','w','w','w','w','w','w','w','w','w'},
                                    {'w','w','w','w','w','w','w',' ','w','w'},
                                    {'w','w','w','w','w',' ',' ',' ','w','w'},
                                    {'w','w','w','w','w',' ','w','w','w','w'}};*/


        public static char[,] Map => map;

        [SerializeField]
        private LayerMask ignoreEnemies;

        private void Awake()
        {
            CreateMap();            
        }

        private void CreateMap()
        {
            /*for(int y = 0; y < map.GetLength(0);y++)
            {
                for(int x = 0; x < map.GetLength(1);x++)
                {
                    if(map[y,x] == 'w')
                    {
                        Instantiate(mapTilePrefab, new Vector3(x,0,y), Quaternion.identity);
                    }
                }
            }*/

            RaycastHit hit;

            //public static bool Raycast(Vector3 origin, Vector3 direction, out RaycastHit hitInfo, float maxDistance, int layerMask, QueryTriggerInteraction queryTriggerInteraction);

            for(int y = 0; y < 10 ;y++)
            {
                for(int x = 0; x < 10; x++)
                {
                    
                    //Debug.DrawRay(new Vector3(x+0.5f,3,-y-0.5f), -Vector3.up*4, Color.red,10);

                    if(Physics.Raycast(new Vector3(x+0.5f,3,-y-0.5f), -Vector3.up, out hit, 4, ignoreEnemies))
                    {
                        //If the ray hit a maptile then add a 'w' entry in the map
                        if(hit.transform.gameObject.TryGetComponent<MapTile>(out MapTile mapTile))
                        {
                            map[y,x] = 'w';
                        }

                        //If the ray it something else then add a 'n' entry in the map
                        else
                        {
                            map[y,x] = 'n';
                        }

                        Debug.Log("map ray hit: " + hit.transform.gameObject);
                    }

                    //If the ray did not hit anything then add a ' ' entry in the map;
                    else
                    {
                        Debug.Log("map(" + x + ", " + y + ") is empty!");
                        map[y,x] = ' '; 
                    }
                }
            }
        }
    }
}