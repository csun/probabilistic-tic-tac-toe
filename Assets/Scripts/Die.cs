using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PTTT
{
    public class Die : MonoBehaviour
    {
        public MeshFilter Filter;
        public GameObject FacePrefab;
        public List<TMPro.TMP_Text> FaceTexts;

        public Color HighlightColor;
        public Color BaseColor;
        public float UpThreshold;

        public Rigidbody RB;
        public float MaxVel;
        public float MaxAngVel;

        private int[] shuffleIndices;

        private void Start()
        {
            shuffleIndices = new int[20];
            for (var i = 0; i < 20; i++)
            {
                shuffleIndices[i] = i;
            }
        }

        private void Update()
        {
            foreach (var text in FaceTexts)
            {
                var isUp = Vector3.Dot((text.gameObject.transform.position - transform.position).normalized, Vector3.up) >= UpThreshold;
                if (isUp)
                {
                    text.color = HighlightColor;
                }
                else
                {
                    text.color = BaseColor;
                }
            }
        }

        public void AssignFaces(int xFaces, int oFaces)
        {
            // Knuth shuffle algorithm
            for (int i = 0; i < shuffleIndices.Length; i++ )
            {
                var tmp = shuffleIndices[i];
                int r = Random.Range(i, shuffleIndices.Length);
                shuffleIndices[i] = shuffleIndices[r];
                shuffleIndices[r] = tmp;
            }

            for (var i = 0; i < shuffleIndices.Length; i++)
            {
                if (i < xFaces)
                {
                    FaceTexts[shuffleIndices[i]].text = "X";
                }
                else if (i < xFaces + oFaces)
                {
                    FaceTexts[shuffleIndices[i]].text = "O";
                }
                else
                {
                    FaceTexts[shuffleIndices[i]].text = "";
                }
            }
        }

        public IEnumerator Roll(System.Action<SquareContents> finished)
        {
            RB.velocity = new Vector3(
                Random.Range(-MaxVel, MaxVel), 0, Random.Range(-MaxVel, MaxVel));
            RB.angularVelocity = new Vector3(
                Random.Range(-MaxAngVel, MaxAngVel),
                Random.Range(-MaxAngVel, MaxAngVel),
                Random.Range(-MaxAngVel, MaxAngVel));
            
            yield return new WaitForSeconds(1f);

            finished(SquareContents.X);
        }

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
