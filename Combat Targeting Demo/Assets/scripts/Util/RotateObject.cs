// project armada

#pragma warning disable 0414

namespace Harris.Util
{
	using UnityEngine;
    using System;
    using System.Collections;

    public class RotateObject : MonoBehaviour
    {
        //float lerpDuration = 0.5f;
        private bool rotating = false;
        public bool IsRotating => rotating;

        public event Action _onStartRotation;
        public event Action _onStopRotation;
        private bool interrupt = false;
        public bool Interrupt{get => interrupt; set => interrupt = value;}
        private Quaternion targetRotation;
        private float angle = 0;

        void Update()
        {
            /*if (Input.GetMouseButtonDown(0) && !rotating)
            {
                StartCoroutine(Rotate90());
            }*/
        }

        public void SetRotationAngle(float _angle)
        {
            angle = _angle;
        }

        public IEnumerator Rotate(float _angle, float lerpDuration = 0.5f)
        {
            rotating = true;
            interrupt = false;

            float timeElapsed = 0;
            Quaternion startRotation = transform.rotation;
            targetRotation = transform.rotation * Quaternion.Euler(0, _angle, 0);
            
            _onStartRotation?.Invoke();
            angle = _angle;

            while (timeElapsed < lerpDuration && interrupt == false)
            {
                targetRotation = startRotation * Quaternion.Euler(0, angle, 0);//newly added

                transform.rotation = Quaternion.Slerp(startRotation, targetRotation, timeElapsed / lerpDuration);
                timeElapsed += Time.deltaTime;

                //yield return null;

                if(interrupt)
                {
                    //Debug.Log("rotation was interrupted!");
                }
                yield return null;
            }

            if(interrupt == false)
            {
                transform.rotation = targetRotation;
                _onStopRotation?.Invoke();
            }
            else
            {
                Debug.Log("rotation was interrupted!");
            }

            rotating = false;
        }
    }
}