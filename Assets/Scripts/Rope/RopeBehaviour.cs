using System;
using System.Collections.Generic;
using System.Linq;
using Core.Logging;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace Game {
    [Serializable]
    public class RopeVertex {
        public Vector3 prevPos;
        public Vector3 currPos;
        public Vector3 delta;

        public RopeVertex(Vector3 pos) {
            prevPos = pos;
            currPos = pos;
        }
    }
    
    [ExecuteAlways]
    [RequireComponent(typeof(LineRenderer))]
    public class RopeBehaviour : SerializedMonoBehaviour {
        [TitleGroup("Rope ends")]
        [TitleGroup("Rope ends")] [ReadOnly] public Vector3 start = new (1f, 0f, 0f);
        [TitleGroup("Rope ends")] [ReadOnly] public Vector3 end = new (-1f, 0f, 0f);
        [TitleGroup("Rope ends")] [Button("Reset ends")]
        private void ResetEnds() {
            start = new Vector3(1f, 0f, 0f);
            end   = start * -1;
            ResetRenderer();
        }

        [TitleGroup("Forces")]
        [SerializeField] private float gravity = 9.81f;
        [SerializeField] private float damping = 0.5f;
        private Vector3 _grav;
        
        [TitleGroup("Simulation settings")]
        [SerializeField] private int segmentCount = 4;
        [SerializeField] private int iterations = 16;
        [SerializeField] private float sagAmount = 0f;
        [SerializeField] private LayerMask collisionMask;
        [SerializeField] private float minActiveDelta;
        [ReadOnly] [ShowInInspector] private float segmentLength;
        
        private LineRenderer _renderer;
        private List<RopeVertex> segments = new ();
        private bool _ready;
        
        private const float _gizmoSize = 0.2f;

        private void Awake() {
            _renderer = GetComponent<LineRenderer>();
            _grav     = new Vector3(0f, gravity, 0f);
            _ready    = false;
            Setup();
        }

        private void Update() {
            if (!Application.isPlaying) {
                ResetRenderer();
                return;
            }
            Render();
        }

        private void FixedUpdate() {
            if (!Application.isPlaying || !_ready) return;
            // if (!CheckActive()) return;
            Simulate();
        }

        private void Render() {
            var ropeVertices = new Vector3[segmentCount];
            for (var i = 0; i < segmentCount; i++) {
                ropeVertices[i] = segments[i].currPos;
            }
            _renderer.positionCount = ropeVertices.Length;
            _renderer.SetPositions(ropeVertices);
        }

        private void Simulate() {
            for (var i = 0; i < segmentCount; i++) {
                RopeVertex vert = segments[i];
                vert.delta = vert.currPos - vert.prevPos;
                vert.prevPos =  vert.currPos;
                if (!CheckCollision(vert)) vert.currPos += _grav * Time.deltaTime + vert.delta;
                segments[i]  =  vert;
            }
            for (var i = 0; i < iterations; i++) {
                Constrain();
            }
        }

        private void Constrain() {
            RopeVertex first = segments[0];
            first.currPos = GetStartPos();
            segments[0]  = first;
            
            RopeVertex last = segments[^1];
            last.currPos  = GetEndPos();
            segments[^1] = last;
            
            for (var i = 0; i < segmentCount - 1; i++) {
                RopeVertex currVert = segments[i];
                RopeVertex nextVert = segments[i + 1];
            
                var dist = (currVert.currPos - nextVert.currPos).magnitude;
                var error = dist - segmentLength - sagAmount;
                Vector3 changeDir = (currVert.currPos - nextVert.currPos).normalized;
                Vector3 changeAmt = changeDir * error;
                Vector3 half = changeAmt * damping;
                if (i != 0) {
                    currVert.currPos  -= half;
                    segments[i]     =  currVert;
                    nextVert.currPos  += half;
                    segments[i + 1] =  nextVert;
                } else {
                    nextVert.currPos  += changeAmt;
                    segments[i + 1] =  nextVert;
                }
            }
        }

        private bool CheckCollision(RopeVertex vert) {
            return Physics.CheckSphere(vert.currPos, 
                                       _renderer.startWidth + 0.1f, 
                                       collisionMask,
                                       QueryTriggerInteraction.Ignore);
        }

        private bool CheckActive() {
            var deltaList = segments.Select(x => x.delta.magnitude);
            var avgDelta = deltaList.Average();
            return avgDelta > minActiveDelta;
        }

        public Vector3 GetStartPos() => transform.position + start;
        public Vector3 GetEndPos() => transform.position + end;

        public void ResetRenderer() {
            LineRenderer tempRenderer = GetComponent<LineRenderer>();
            if (tempRenderer.positionCount > 2) tempRenderer.positionCount = 0;
            var gizmoVertices = new Vector3[2];
            gizmoVertices[0] = GetStartPos();
            gizmoVertices[1] = GetEndPos();
            tempRenderer.SetPositions(gizmoVertices);
        }

        private void Setup() {
            var dist = Vector3.Distance(start, end);
            segmentLength = dist / segmentCount;
            segments.Clear();
            for (var i = 0; i < segmentCount; i++) {
                segments.Add(new RopeVertex(GetStartPos() + new Vector3(i * segmentLength + sagAmount, 0f, 0f)));
            }
            _ready = true;
        }

#if UNITY_EDITOR
        private void OnValidate() {
            if (Application.isPlaying) {
                Setup();
            } else {
                ResetRenderer();
            }
        }

        private void OnDrawGizmosSelected() {
            Gizmos.color = Color.yellow;
            Vector3 drawSize = new (_gizmoSize, _gizmoSize, _gizmoSize);
            Gizmos.DrawWireCube(GetStartPos(), drawSize);
            Gizmos.DrawWireCube(GetEndPos(), drawSize);
            var dir = (GetEndPos() - GetStartPos()).normalized;
            // Gizmos.color = Color.green;
            if (Application.isPlaying) {
                for (var i = 1; i < segmentCount - 1; i++) {
                    Gizmos.DrawWireSphere(segments[i].currPos, _gizmoSize * 0.3f);
                }
            } else {
                for (var i = 1; i < segmentCount; i++) {
                    Gizmos.DrawWireSphere(GetStartPos() + dir * (Vector3.Distance(start, end) / segmentCount) * i, _gizmoSize * 0.3f);
                }
            }
        }
#endif
    }
}