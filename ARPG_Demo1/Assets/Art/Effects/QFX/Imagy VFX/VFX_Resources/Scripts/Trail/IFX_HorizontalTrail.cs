using System;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace QFX.IFX
{
    public class IFX_HorizontalTrail : MonoBehaviour
    {
        public Material Material;
        public float LifeTime = 1;
        public bool Emit = true;
        public float[] Widths;

        public float MinVertexDistance = 0.1f;
        public int PointCount = 30;

        private Material _instanceMaterial;
        private GameObject _trailObj;
        private Mesh mesh;

        private bool _emittingDone;
        private float _lifeTimeRatio = 1;

        // Points
        private Point[] _points;
        private int _pointCnt;

        public Transform TrailObject { get; private set; }

        private void Start()
        {
            _trailObj = new GameObject("IFX_HorizontalTrail");
            _trailObj.transform.parent = null;
            _trailObj.transform.position = Vector3.zero;
            _trailObj.transform.rotation = Quaternion.identity;
            _trailObj.transform.localScale = Vector3.one;

            TrailObject = _trailObj.transform;

            var meshFilter = (MeshFilter) _trailObj.AddComponent(typeof(MeshFilter));
            mesh = meshFilter.mesh;

            _trailObj.AddComponent(typeof(MeshRenderer));
            _instanceMaterial = new Material(Material);

            _trailObj.GetComponent<Renderer>().material = _instanceMaterial;

            _points = new Point[PointCount];
        }

        private void Update()
        {
            try
            {
                UpdateTrail();
            }
            catch (Exception)
            {
                // ignored
            }
        }

        private void UpdateTrail()
        {
            if (!Emit)
                _emittingDone = true;

            if (_emittingDone)
                Emit = false;

            for (int i = _pointCnt - 1; i >= 0; i--)
            {
                var point = _points[i];
                if (point == null || point.AliveTime > LifeTime)
                {
                    _points[i] = null;
                    _pointCnt--;
                }
                else
                    break;
            }

            if (Emit)
            {
                if (_pointCnt == 0)
                {
                    _points[_pointCnt++] = new Point(transform);
                    _points[_pointCnt++] = new Point(transform);
                }

                if (_pointCnt == 1)
                    InsertPoint();

                bool add = false;
                float sqrDistance = (_points[1].position - transform.position).sqrMagnitude;
                if (sqrDistance > MinVertexDistance * MinVertexDistance)
                    add = true;

                if (add)
                {
                    if (_pointCnt == _points.Length)
                        UpdatePoints();
                    //    System.Array.Resize(ref _points, _points.Length + 50);
                    else
                        InsertPoint();
                }

                if (!add)
                    _points[0].Update(transform);
            }

            if (_pointCnt < 2)
            {
                _trailObj.GetComponent<Renderer>().enabled = false;
                return;
            }

            _trailObj.GetComponent<Renderer>().enabled = true;

            _lifeTimeRatio = 1 / LifeTime;

            if (!Emit)
                return;

            BuildMesh();
        }

        private void BuildMesh()
        {
            var vertices = new Vector3[_pointCnt * 2];
            var uvs = new Vector2[_pointCnt * 2];
            var triangles = new int[(_pointCnt - 1) * 6];

            var uvMultiplier = 1 / (_points[_pointCnt - 1].AliveTime - _points[0].AliveTime);

            for (int i = 0; i < _pointCnt; i++)
            {
                var point = _points[i];
                float ratio = point.AliveTime * _lifeTimeRatio;

                float width;
                switch (Widths.Length)
                {
                    case 0:
                        width = 1;
                        break;
                    case 1:
                        width = Widths[0];
                        break;
                    case 2:
                        width = Mathf.Lerp(Widths[0], Widths[1], ratio);
                        break;
                    default:
                    {
                        float widthRatio = ratio * (Widths.Length - 1);
                        int min = (int) Mathf.Floor(widthRatio);
                        float lerp = Mathf.InverseLerp(min, min + 1, widthRatio);
                        width = Mathf.Lerp(Widths[min], Widths[min + 1], lerp);
                        break;
                    }
                }

                _trailObj.transform.position = point.position;
                _trailObj.transform.rotation = point.rotation;

                vertices[i * 2] = _trailObj.transform.TransformPoint(-width * 0.5f, 0, 0);
                vertices[i * 2 + 1] = _trailObj.transform.TransformPoint(width * 0.5f, 0, 0);

                var uvRatio = (point.AliveTime - _points[0].AliveTime) * uvMultiplier;
                uvs[i * 2] = new Vector2(uvRatio, 0);
                uvs[i * 2 + 1] = new Vector2(uvRatio, 1);

                if (i > 0)
                {
                    // Triangles
                    int triIndex = (i - 1) * 6;
                    int vertIndex = i * 2;
                    triangles[triIndex + 0] = vertIndex - 2;
                    triangles[triIndex + 1] = vertIndex - 1;
                    triangles[triIndex + 2] = vertIndex - 0;

                    triangles[triIndex + 3] = vertIndex + 1;
                    triangles[triIndex + 4] = vertIndex + 0;
                    triangles[triIndex + 5] = vertIndex - 1;
                }
            }

            _trailObj.transform.position = Vector3.zero;
            _trailObj.transform.rotation = Quaternion.identity;

            mesh.Clear();
            mesh.vertices = vertices;
            mesh.uv = uvs;
            mesh.triangles = triangles;
        }

        private void UpdatePoints()
        {
            for (var i = _pointCnt; i > 0; i--)
                _points[i] = _points[i - 1];
        }

        private void InsertPoint()
        {
            for (var i = _pointCnt; i > 0; i--)
                _points[i] = _points[i - 1];
            _points[0] = new Point(transform);
            _pointCnt++;
        }

        private void OnDestroy()
        {
            Destroy(_trailObj.gameObject);
        }

        private class Point
        {
            public float AliveTime
            {
                get { return Time.time - _createdTime; }
            }

            public Vector3 position;
            public Quaternion rotation;

            private float _createdTime;

            public Point(Transform transform)
            {
                position = transform.position;
                rotation = transform.rotation;
                _createdTime = Time.time;
            }

            public void Update(Transform transform)
            {
                position = transform.position;
                rotation = transform.rotation;
                _createdTime = Time.time;
            }
        }
    }
}