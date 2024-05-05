using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Harris.Physics
{

    [RequireComponent(typeof(CapsuleCollider))]
    internal class HumanoidCollider : MonoBehaviour
    {

        private Vector3 bottomPoint;
        public Vector3 BottomPoint => bottomPoint;

        public float Height => (collider as CapsuleCollider).height;
        public float Radius => (collider as CapsuleCollider).radius;

        [SerializeField]
        private Collider collider;
        // Start is called before the first frame update
        void Start()
        {
            bottomPoint = Vector3.zero;
        }

        void setRadius(float r)
        {
            (collider as CapsuleCollider).radius = r;
        }

        void setHeight(float h)
        {
            (collider as CapsuleCollider).height = h;
        }

        // Update is called once per frame
        void Update()
        {
            //Update the collider bottom point
            bottomPoint = transform.TransformPoint((collider as CapsuleCollider).center )- Vector3.up * (collider as CapsuleCollider).height/2;
        }
    }
}
