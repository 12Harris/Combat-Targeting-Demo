using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Harris.AI
{
    public class GameMap : MonoBehaviour
    {
        [SerializeField]
        private GameObject mapTilePrefab;

        private static char[,] map = {{'w','w','w','w','w','w',' ','w','w','w'},
                                    {'w','w',' ',' ',' ',' ',' ','w','w','w'},
                                    {'w','w','w','w','w','w','w','w','w','w'},
                                    {'w','w','w','w','w','w','w','w','w','w'},
                                    {' ',' ',' ',' ','w','w','w','w','w','w'},
                                    {'w','w','w','w','w','w','w','w','w','w'},
                                    {'w','w','w','w','w','w','w','w','w','w'},
                                    {'w','w','w','w','w','w','w',' ','w','w'},
                                    {'w','w','w','w','w',' ',' ',' ','w','w'},
                                    {'w','w','w','w','w',' ','w','w','w','w'}};


        /*private static char[,] map = {{'w','w','w','w','w','w','w','w','w','w'},
                                    {'w','w','w','w','w','w','w','w','w','w'},
                                    {'w','w','w','w','w','w','w','w','w','w'},
                                    {'w','w','w','w','w','w','w','w','w','w'},
                                    {'w','w','w','w','w','w','w','w','w','w'},
                                    {'w','w','w','w','w','w','w','w','w','w'},
                                    {'w','w','w','w','w','w','w','w','w','w'},
                                    {'w','w','w','w','w','w','w','w','w','w'},
                                    {'w','w','w','w','w','w','w','w','w','w'},
                                    {'w','w','w','w','w','w','w','w','w','w'}};*/

        public static char[,] Map => map;

        private void Awake()
        {
            CreateMap();            
        }

        private void CreateMap()
        {
            for(int y = 0; y < map.GetLength(0);y++)
            {
                for(int x = 0; x < map.GetLength(1);x++)
                {
                    if(map[y,x] == 'w')
                    {
                        Instantiate(mapTilePrefab, new Vector3(x,0,y), Quaternion.identity);
                    }
                }
            }
        }
    }
}