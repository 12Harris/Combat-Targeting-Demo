using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Harris.Util
{
    public class GOComponents
    {

        //get the first component of gameObject that is of type T
        //also searhes in child
        public static T GetFirstExact<T, CT>(GameObject g, ref CT[] arr) where T : MonoBehaviour
        {
            //Get all components in children
            if(arr == null) { arr = g.GetComponentsInChildren<CT>(); }

            //get the first component that is of type T
            foreach (var s in arr)
            {
                if (s is T) { return s as T; }
            }

            return null;
        }
    }
}
