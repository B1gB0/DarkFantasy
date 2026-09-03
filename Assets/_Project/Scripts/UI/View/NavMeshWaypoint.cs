using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace _Project.Scripts.UI.View
{
    public class NavMeshWaypoint : View
    {
        private readonly List<Vector3> _smoothedPath = new ();
        
        [Header("Path Settings")]
        [SerializeField] private float _pathUpdateInterval = 0.3f;
        [SerializeField] private float _smoothingAngle = 30f;
        [SerializeField] private float _transitionSpeed = 4f;
        [SerializeField] private float _heightOffset = 0.5f;

        [Header("Arrow Animation")]
        [SerializeField] private float _scrollSpeed = 0.8f;

        [Header("Line Settings")]
        [SerializeField] private float _lineWidth = 0.3f;

        private Material _materialInstance;

        private Transform _player;
        private Transform _target;

        private LineRenderer _lineRenderer;
        private NavMeshPath _path;
        private float _timer;
        private Vector2 _textureOffset;

        private List<Vector3> _previousPath = new ();
        private float _transitionProgress = 1f;

        private void Awake()
        {
            _lineRenderer = GetComponent<LineRenderer>();
            if (_lineRenderer == null)
                _lineRenderer = gameObject.AddComponent<LineRenderer>();

            _lineRenderer.startWidth = _lineWidth;
            _lineRenderer.endWidth = _lineWidth;
            
            _lineRenderer.textureMode = LineTextureMode.Tile;
            _lineRenderer.useWorldSpace = true;
            _lineRenderer.numCapVertices = 4;

            _path = new NavMeshPath();
        }

        private void Update()
        {
            if (_player == null || _target == null)
            {
                _lineRenderer.enabled = false;
                return;
            }
            
            _timer += Time.deltaTime;
            if (_timer >= _pathUpdateInterval)
            {
                _timer = 0f;
                
                CalculateAndSmoothPath();
                
                _previousPath = new List<Vector3>(_smoothedPath);
                _transitionProgress = 0f;
            }
            
            if (_transitionProgress < 1f && _previousPath.Count == _smoothedPath.Count)
            {
                _transitionProgress += Time.deltaTime * _transitionSpeed;
                if (_transitionProgress > 1f) _transitionProgress = 1f;

                for (int i = 0; i < _smoothedPath.Count; i++)
                {
                    Vector3 pos = Vector3.Lerp(_previousPath[i], _smoothedPath[i], _transitionProgress);
                    _lineRenderer.SetPosition(i, pos);
                }
            }
            else
            {
                _lineRenderer.positionCount = _smoothedPath.Count;
                _lineRenderer.SetPositions(_smoothedPath.ToArray());
            }
            
            _textureOffset.x -= _scrollSpeed * Time.deltaTime;
            _lineRenderer.material.mainTextureOffset = _textureOffset;
        }

        public void GetPlayer(Player.Core.Player player)
        {
            _player = player.transform;
        }

        public void SetTarget(Transform newTarget)
        {
            _target = newTarget;
            _timer = _pathUpdateInterval;
        }

        public void SetActive(bool active)
        {
            _lineRenderer.enabled = active;
            enabled = active;
        }

        private void CalculateAndSmoothPath()
        {
            if (NavMesh.CalculatePath(_player.position, _target.position, NavMesh.AllAreas, _path))
            {
                if (_path.corners.Length < 2)
                {
                    _lineRenderer.enabled = false;
                    return;
                }

                _smoothedPath.Clear();
                _smoothedPath.Add(_path.corners[0]);

                for (int i = 1; i < _path.corners.Length - 1; i++)
                {
                    Vector3 prev = _path.corners[i - 1];
                    Vector3 curr = _path.corners[i];
                    Vector3 next = _path.corners[i + 1];

                    Vector3 dir1 = (curr - prev).normalized;
                    Vector3 dir2 = (next - curr).normalized;
                    float angle = Vector3.Angle(dir1, dir2);

                    if (angle > _smoothingAngle)
                    {
                        int segments = Mathf.CeilToInt(angle / 15f);
                        for (int s = 1; s <= segments; s++)
                        {
                            float t = s / (float)(segments + 1);
                            Vector3 smoothedPoint = Vector3.Lerp(curr, Vector3.Lerp(prev, next, 0.5f), t);
                            _smoothedPath.Add(smoothedPoint);
                        }
                    }

                    _smoothedPath.Add(curr);
                }

                _smoothedPath.Add(_path.corners[^1]);
                
                for (int i = 0; i < _smoothedPath.Count; i++)
                {
                    Vector3 p = _smoothedPath[i];
                    p.y += _heightOffset;
                    _smoothedPath[i] = p;
                }

                _lineRenderer.positionCount = _smoothedPath.Count;
                _lineRenderer.SetPositions(_smoothedPath.ToArray());
                _lineRenderer.enabled = true;
            }
            else
            {
                _lineRenderer.enabled = false;
            }
        }
    }
}