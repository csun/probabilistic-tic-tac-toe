using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PTTT
{
    public class DieFaceGenerator : MonoBehaviour
    {
        public MeshFilter Filter;
        public GameObject FacePrefab;


#if UNITY_EDITOR
        [ContextMenu("Place Faces")]
        public void PlaceFaces()
        {
            var mesh = Filter.sharedMesh;
            for (var i = 0; i < mesh.triangles.Length; i += 3)
            {
                var p1 = mesh.vertices[i] * 100;
                var p2 = mesh.vertices[i + 1] * 100;
                var p3 = mesh.vertices[i + 2] * 100;

                var center = (p1 + p2 + p3) / 3;
                var normal = Vector3.Cross(p2 - p1, p3 - p1).normalized;
                var offsetCenter = center + (normal * 0.001f);

                var instantiated = (GameObject)PrefabUtility.InstantiatePrefab(FacePrefab);
                instantiated.transform.parent = transform;
                instantiated.transform.position = offsetCenter;
                instantiated.transform.rotation = Quaternion.LookRotation(-normal, (p1 - center).normalized);
            }
        }
#endif
    }
}