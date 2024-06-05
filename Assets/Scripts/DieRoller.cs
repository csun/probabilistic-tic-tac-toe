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

        private int tmpTick;

        private void FixedUpdate()
        {
            tmpTick = (tmpTick + 1) % 360;
            if (tmpTick == 0)
            {
                Roll();
            }
        }
        private void Roll()
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
