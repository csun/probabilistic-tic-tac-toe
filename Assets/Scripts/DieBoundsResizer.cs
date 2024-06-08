using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PTTT
{
    public class DieBoundsResizer : MonoBehaviour
    {
        public Camera DieCamera;
        public Die Die;
        public Transform DieSpawn;

        public Transform Top;
        public Transform Right;
        public Transform Bottom;
        public Transform Left;

        public float MinimumExtent;
        public float DieSpreadPadding;
        public float Padding;
        Vector2Int lastScreenSize;

        public void Recalculate()
        {
            if (Screen.width == lastScreenSize.x && Screen.height == lastScreenSize.y)
            {
                return;
            }

            lastScreenSize = new(Screen.width, Screen.height);
            var worldVExtent = DieCamera.orthographicSize;
            var worldHExtent = worldVExtent * ((float)Screen.width / Screen.height);
            worldVExtent = Mathf.Max(MinimumExtent, worldVExtent);
            worldHExtent = Mathf.Max(MinimumExtent, worldHExtent);

            var paddedVExtent = worldVExtent + Padding;
            var paddedHExtent = worldHExtent + Padding;

            Top.position = new Vector3(
                Top.position.x,
                Top.position.y,
                DieCamera.transform.position.z + paddedVExtent);
            Bottom.position = new Vector3(
                Bottom.position.x,
                Bottom.position.y,
                DieCamera.transform.position.z - paddedVExtent);
            Right.position = new Vector3(
                DieCamera.transform.position.x + paddedHExtent,
                Right.position.y,
                Right.position.z);
            Left.position = new Vector3(
                DieCamera.transform.position.x - paddedHExtent,
                Left.position.y,
                Left.position.z);

            var dieMaxVTravel = (DieCamera.transform.position.z - DieSpawn.position.z) + (worldVExtent - DieSpreadPadding);
            var dieMaxHTravel = Mathf.Max(0, worldHExtent - DieSpreadPadding);
            Die.MaxSpread = Mathf.Rad2Deg * Mathf.Atan(dieMaxHTravel / dieMaxVTravel);
        }
    }
}
