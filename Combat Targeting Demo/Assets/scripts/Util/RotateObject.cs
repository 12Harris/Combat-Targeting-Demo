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
        bool rotating;

        public event Action _onStartRotation;
        public event Action _onStopRotation;

        void Update()
        {
            /*if (Input.GetMouseButtonDown(0) && !rotating)
            {
                StartCoroutine(Rotate90());
            }*/
        }

        public IEnumerator Rotate(float angle, float lerpDuration = 0.5f)
        {
            rotating = true;
            float timeElapsed = 0;
            Quaternion startRotation = transform.rotation;
            Quaternion targetRotation = transform.rotation * Quaternion.Euler(0, angle, 0);
            
            _onStartRotation?.Invoke();

            while (timeElapsed < lerpDuration)
            {
                transform.rotation = Quaternion.Slerp(startRotation, targetRotation, timeElapsed / lerpDuration);
                timeElapsed += Time.deltaTime;
                yield return null;
            }
            transform.rotation = targetRotation;
            rotating = false;

            _onStopRotation?.Invoke();
        }
    }
}