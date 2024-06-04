using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PTTT
{
    public class DieRoller : MonoBehaviour
    {
        public Rigidbody RB;
        public float MaxVel;
        public float MaxAngVel;

        private void OnMouseDown()
        {
            RB.velocity = new Vector3(
                Random.Range(-MaxVel, MaxVel), 0, Random.Range(-MaxVel, MaxVel));
            RB.angularVelocity = new Vector3(
                Random.Range(-MaxAngVel, MaxAngVel),
                Random.Range(-MaxAngVel, MaxAngVel),
                Random.Range(-MaxAngVel, MaxAngVel));
        }
    }
}
