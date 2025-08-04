using System.Collections.Generic;

using UnityEngine;

namespace IGI.Enemy
{
    [ExecuteInEditMode]
    public class FieldOfView : MonoBehaviour
    {
        [Range(.1f, 180)]
        [SerializeField] private float viewAngle = 50f;
        [Range(.1f, 10)]
        [SerializeField] private float viewRadius = 10f;
        [Min(1)]
        [SerializeField] private int segment = 10;
        [SerializeField] private LayerMask targetLayer, nonTargetLayer;
        [SerializeField] private Transform headBone;
        [SerializeField] private Vector3 offset;

        public Transform TargetOnCaught { get; private set; }
        public Transform TargetNoDelete { get; private set; }

        private Mesh viewMesh;
        private MeshFilter viewMeshFilter;
        public MeshRenderer ViewMeshRenderer { get; private set; }
        private Collider[] targetsBuffer = new Collider[5];

        private void Awake()
        {
            viewMeshFilter = GetComponent<MeshFilter>();
            ViewMeshRenderer = GetComponent<MeshRenderer>();
            viewMesh = new();
            viewMesh.name = "Field Of View";
            viewMeshFilter.mesh = viewMesh;
        }

        private void Update()
        {
            Quaternion rotationHead = Quaternion.Euler(new(0, headBone.eulerAngles.y, 0));
            transform.SetPositionAndRotation(headBone.position + offset, rotationHead);
            DrawFieldOfView();
            DetectTarget();
        }

        private void DrawFieldOfView()
        {
            float stepAngleSize = viewAngle / segment;

            List<Vector3> viewPoints = new();

            for (int i = 0; i <= segment; i++)
            {
                float angle = -viewAngle / 2 + stepAngleSize * i;
                Vector3 dir = DirFromAngle(angle);
                Vector3 point;

                if (Physics.Raycast(transform.position, dir, out RaycastHit hit, viewRadius))
                {
                    point = hit.point;
                }
                else
                {
                    point = transform.position + dir * viewRadius;
                }

                viewPoints.Add(transform.InverseTransformPoint(point));
            }

            int vertexCount = viewPoints.Count + 1;
            Vector3[] vertices = new Vector3[vertexCount];
            int[] triangles = new int[(vertexCount - 2) * 3];

            vertices[0] = Vector3.zero;
            for (int i = 0; i < viewPoints.Count; i++)
            {
                vertices[i + 1] = viewPoints[i];
            }

            for (int i = 0; i < viewPoints.Count - 1; i++)
            {
                int triIdx = i * 3;
                triangles[triIdx] = 0;
                triangles[triIdx + 1] = i + 1;
                triangles[triIdx + 2] = i + 2;
            }

            viewMesh.Clear();
            viewMesh.vertices = vertices;
            viewMesh.triangles = triangles;
            viewMesh.RecalculateNormals();
        }

        private Vector3 DirFromAngle(float angleInDegrees)
        {
            return Quaternion.Euler(0, angleInDegrees, 0) * transform.forward;
        }

        private void DetectTarget()
        {
            TargetOnCaught = null;

            int count = Physics.OverlapSphereNonAlloc(
            transform.position,
            viewRadius,
            targetsBuffer,
            targetLayer
        );

            //int visibleCount = 0;

            for (int i = 0; i < count; i++)
            {
                Transform target = targetsBuffer[i].transform;
                Vector3 offset = target.position - transform.parent.position;
                float sqrDist = offset.sqrMagnitude;

                if (sqrDist <= viewRadius * viewRadius)
                {
                    Vector3 dirToTarget = offset.normalized;

                    if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle * 0.5f)
                    {
                        if (!Physics.Raycast(transform.position, dirToTarget, Mathf.Sqrt(sqrDist), nonTargetLayer))
                        {
                            TargetOnCaught = target;
                            TargetNoDelete = target;
                            //Debug.Log(target.name);
                            break;
                        }
                    }
                }
            }
        }
    }
}