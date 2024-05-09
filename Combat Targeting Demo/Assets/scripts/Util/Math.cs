// project armada
using UnityEngine;
using System.Collections;

namespace Harris.Util
{
    public class LeftRightTest {
        private Vector3 heading;

        private Vector3 fwd;

        private Vector3 up;

        private float dirNum;

        public LeftRightTest(Transform _self, Transform _target) : this(_self.forward,
                                                                    _target.position - _self.transform.position,
                                                                    _self.up)
        {
        }

        public LeftRightTest(Vector3 _fwd, Vector3 _heading, Vector3 _up)
        {
            fwd = _fwd;
            heading = _heading;
            up = _up;
        }

        public bool targetIsLeft()
        {
            return (dirNum = AngleDir(fwd, heading, up)) == -1;
        }
        
        private float AngleDir(Vector3 fwd, Vector3 targetDir, Vector3 up) {
            Vector3 perp = Vector3.Cross(fwd, targetDir);
            float dir = Vector3.Dot(perp, up);
            
            if (dir > 0f) {
                return 1f;
            } else if (dir < 0f) {
                return -1f;
            } else {
                return 0f;
            }
        }
    }
}