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
        const string GOOD_FACE_STRING = "A";
        const string NEUTRAL_FACE_STRING = "B";
        const string BAD_FACE_STRING = "C";

        public MeshFilter Filter;
        public GameObject FacePrefab;
        public List<TMPro.TMP_Text> FaceTexts;

        public Collider EntryWall;
        public Transform SpawnPoint;
        public Transform ShowFacePoint;

        public Color HighlightColor;
        public Color BaseColor;
        public float UpThreshold;
        public float RollStopTimeLimit;

        public Rigidbody RB;
        public float MinVel;
        public float MaxVel;
        public float RandomSpread;
        public Vector3 MinAngVel;
        public Vector3 MaxAngVel;

        public float TotalShowFaceDuration;
        public AnimationCurve ShowFaceCurve;
        public float ShowFaceHoldTime;

        public float TotalMoveOffscreenDuration;
        public AnimationCurve MoveOffscreenCurve;

        private int[] shuffleIndices;

        private TMPro.TMP_Text currentUpFace;
        private float faceUpDuration;

        private AnimationCurve animCurve;
        private Vector3 animStartPosition;
        private Quaternion animStartRotation;
        private Vector3 animTargetPosition;
        private Quaternion animTargetRotation;
        private float animTotalLength;
        private float animProgress;

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
            TMPro.TMP_Text foundUpFace = null;
            foreach (var text in FaceTexts)
            {
                var isUp = Vector3.Dot((text.gameObject.transform.position - transform.position).normalized, Vector3.up) >= UpThreshold;
                if (isUp)
                {
                    text.color = HighlightColor;
                    foundUpFace = text;
                }
                else
                {
                    text.color = BaseColor;
                }
            }

            if (foundUpFace == currentUpFace)
            {
                faceUpDuration += Time.deltaTime;
            }
            else
            {
                currentUpFace = foundUpFace;
                faceUpDuration = 0;
            }
        }

        public void AssignFaces(int goodFaces, int badFaces)
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
                if (i < goodFaces)
                {
                    FaceTexts[shuffleIndices[i]].text = GOOD_FACE_STRING;
                }
                else if (i < goodFaces + badFaces)
                {
                    FaceTexts[shuffleIndices[i]].text = BAD_FACE_STRING;
                }
                else
                {
                    FaceTexts[shuffleIndices[i]].text = NEUTRAL_FACE_STRING;
                }
            }
        }

        public IEnumerator Retract(System.Action holdFinished, System.Action finished)
        {
            RB.isKinematic = true;

            yield return ShowWinningFace();
            yield return new WaitForSeconds(ShowFaceHoldTime);
            holdFinished();
            yield return MoveOffscreen();

            finished();
        }

        private IEnumerator ShowWinningFace()
        {
            animTargetPosition = ShowFacePoint.position;
            // Inverting the localrotation gets it so that the face points "forward" along the world z. Then apply 90 degree rotation on x to
            // get it facing up
            animTargetRotation =  Quaternion.Euler(90, 0, 0) *
                Quaternion.Inverse(currentUpFace.transform.localRotation);
            animCurve = ShowFaceCurve;
            animTotalLength = TotalShowFaceDuration;

            yield return RunAnimation();
        }

        private IEnumerator MoveOffscreen()
        {
            animTargetPosition = SpawnPoint.position;
            animTargetRotation = transform.rotation;
            animCurve = MoveOffscreenCurve;
            animTotalLength = TotalMoveOffscreenDuration;

            yield return RunAnimation();
        }

        private IEnumerator RunAnimation()
        {
            animProgress = 0;
            animStartPosition = transform.position;
            animStartRotation = transform.rotation;

            while (animProgress < animTotalLength)
            {
                var amt = animCurve.Evaluate(animProgress / animTotalLength);
                transform.position = Vector3.Lerp(animStartPosition, animTargetPosition, amt);
                transform.rotation = Quaternion.Lerp(animStartRotation, animTargetRotation, amt);
                yield return null;
                animProgress += Time.deltaTime;
            }
        }

        public IEnumerator Roll(bool currentPlayerX, System.Action<SquareContents> finished)
        {
            RB.isKinematic = false;

            currentUpFace = null;
            faceUpDuration = 0;

            EntryWall.isTrigger = true;
            RB.position = SpawnPoint.position;

            var forward = Quaternion.AngleAxis(Random.Range(-RandomSpread, RandomSpread), SpawnPoint.up) * SpawnPoint.forward;
            RB.velocity = forward * Random.Range(MinVel, MaxVel);
            RB.angularVelocity = new Vector3(
                Random.Range(MinAngVel.x, MaxAngVel.x),
                Random.Range(MinAngVel.y, MaxAngVel.y),
                Random.Range(MinAngVel.z, MaxAngVel.z));

            while (faceUpDuration < RollStopTimeLimit)
            {
                yield return null;
            }

            if (currentUpFace is null)
            {
                StartCoroutine(Roll(currentPlayerX, finished));
            }
            else
            {
                switch (currentUpFace.text)
                {
                    case GOOD_FACE_STRING:
                        finished(currentPlayerX ? SquareContents.X : SquareContents.O);
                        break;
                    case BAD_FACE_STRING:
                        finished(currentPlayerX ? SquareContents.O : SquareContents.X);
                        break;
                    default:
                        finished(SquareContents.Empty);
                        break;
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            EntryWall.isTrigger = false;
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
