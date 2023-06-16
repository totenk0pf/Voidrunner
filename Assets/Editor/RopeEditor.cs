using System.Collections.Generic;
using System.Linq;
using Core.Logging;
using Game;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RopeBehaviour))]
[CanEditMultipleObjects]
public class RopeEditor : OdinEditor {
    private enum HandleAxis {
        X,
        Y,
        Z
    }
    
    private struct HandleData {
        public HandleAxis Axis { get; }
        public bool Start { get; }
        public int ID { get; }

        public HandleData(HandleAxis axis, bool start, int id = -1) {
            Axis  = axis;
            Start = start;
            ID    = id == -1 ? GUIUtility.GetControlID(FocusType.Passive) : id;
        }
    }

    private class RopeHandle {
        public HandleData[] Data { get; }
        public bool Start { get; }
        public Vector3 Position { get; }
        public int[] AxisIDs { get; }

        public RopeHandle(Vector3 position, bool start) {
            Position = position;
            Start     = start;
            Data = new[] {
                new HandleData(HandleAxis.X, start),
                new HandleData(HandleAxis.Y, start),
                new HandleData(HandleAxis.Z, start),
            };
            AxisIDs = new[] {
                Data[0].ID,
                Data[1].ID,
                Data[2].ID
            };
        }
    }
    
    private RopeHandle startHandle;
    private RopeHandle endHandle;
    
    private bool cached;
    private RopeBehaviour rope;
    private int nearestID;
    private RopeHandle nearestHandle;
    private Vector3 dragDir;
    
    protected virtual void OnSceneGUI() {
        if (!cached) {
            rope        = (RopeBehaviour) target;
            startHandle = new RopeHandle(rope.start, true);
            endHandle   = new RopeHandle(rope.end, false);
            cached      = true;
        }
        
        // switch (Event.current.type) {
        //     case EventType.Repaint:
        //         DrawPointGizmo(rope.GetStartPos(), startHandle, EventType.Repaint);
        //         DrawPointGizmo(rope.GetEndPos(), endHandle, EventType.Repaint);
        //         break;
        //     
        //     case EventType.Layout:
        //         DrawPointGizmo(rope.GetStartPos(), startHandle, EventType.Layout);
        //         DrawPointGizmo(rope.GetEndPos(), endHandle, EventType.Layout);
        //         break;
        //     
        //     case EventType.MouseDown:
        //         nearestID     = HandleUtility.nearestControl;
        //         nearestHandle = GetNearestHandle();
        //         dragDir       = GetAxisVector(GetActiveAxis(nearestHandle));
        //
        //         if (nearestHandle.Start) {
        //             Undo.RecordObject(rope.start, "Moved rope start");
        //         } else {
        //             
        //         }
        //         break;
        //     
        //     case EventType.MouseUp:
        //         nearestID = -1;
        //         break;
        //     
        //     case EventType.MouseDrag:
        //         if (Event.current.button == 0) {
        //             HandleDrag();
        //         }
        //         break;
        // }
        EditorGUI.BeginChangeCheck();
        Vector3 startHandlePos = Handles.PositionHandle(rope.GetStartPos(), Quaternion.identity);
        Vector3 endHandlePos = Handles.PositionHandle(rope.GetEndPos(), Quaternion.identity);
        DrawLabels(startHandlePos, $"Start\n{startHandlePos}", -0.2f);
        DrawLabels(endHandlePos, $"End\n{endHandlePos}", -0.2f);
        if (EditorGUI.EndChangeCheck()) {
            var endString = startHandlePos != rope.start ? "start" : endHandlePos != rope.end ? "end" : null;
            if (endString == null) return;
            Undo.RecordObject(rope, $"Changed {rope.name}'s {endString}");
            rope.start = startHandlePos - rope.transform.position;
            rope.end   = endHandlePos - rope.transform.position;
            rope.ResetRenderer();
        }
    }

    private void DrawPointGizmo(Vector3 point, RopeHandle handle, EventType eventType) {
        DrawArrows(point, handle.Data, eventType, 0.5f);
        var label = handle.Start ? "Start" : "End";
        DrawLabels(point, label);
    }

    private void DrawArrows(Vector3 pos, HandleData[] data, EventType eventType, float size = 1f) {
        // Vector4 drawPos = rope.transform.worldToLocalMatrix * pos;
        HandleData xData = data[0];
        HandleData yData = data[1];
        HandleData zData = data[2];
        if (IsEventRepaint()) Handles.color = GetHandleColor(xData);
        Handles.ArrowHandleCap(
            xData.ID,
            pos,
            Quaternion.identity * Quaternion.LookRotation(Vector3.right),
            size,
            eventType
        );
        if (IsEventRepaint()) Handles.color = GetHandleColor(yData);
        Handles.ArrowHandleCap(
            yData.ID,
            pos,
            Quaternion.identity * Quaternion.LookRotation(Vector3.up),
            size,
            eventType
        );
        if (IsEventRepaint()) Handles.color = GetHandleColor(zData);
        Handles.ArrowHandleCap(
            zData.ID,
            pos,
            Quaternion.identity * Quaternion.LookRotation(Vector3.forward),
            size,
            eventType
        );
    }
    
    private void DrawLabels(Vector3 pos, string text, float distance = 1f) {
        Handles.Label(pos + new Vector3(0f, distance, 0f), text);
    }

    private static bool IsEventRepaint() => Event.current.type == EventType.Repaint;
    
    private Color GetHandleColor(HandleData data) {
        Color returnColor = default;
        if (nearestID == data.ID) return Color.yellow;
        switch (data.Axis) {
            case HandleAxis.X:
                returnColor = Handles.xAxisColor;
                break;
            case HandleAxis.Y:
                returnColor = Handles.yAxisColor;
                break;
            case HandleAxis.Z:
                returnColor = Handles.zAxisColor;
                break;
        }
        return returnColor;
    }

    private RopeHandle GetNearestHandle() {
        RopeHandle nearest = startHandle.AxisIDs.Contains(nearestID) ? startHandle :
            endHandle.AxisIDs.Contains(nearestID) ? endHandle : null;
        return nearest;
    }

    private Vector3 GetAxisVector(HandleAxis axis) {
        Vector3 axisVector = new ();
        axisVector = 
            axis switch {
                HandleAxis.X => new Vector3(1f, 0f, 0f),
                HandleAxis.Y => new Vector3(0f, 1f, 0f),
                HandleAxis.Z => new Vector3(0f, 0f, 1f),
                _ => axisVector
            };
        return axisVector;
    }

    private HandleAxis GetActiveAxis(RopeHandle handle) {
        return handle.Data.First(x => x.ID == nearestID).Axis;
    }

    private void HandleDrag() {
        if (nearestHandle.Start) {
            rope.start += dragDir;
        } else {
            rope.end += dragDir;
        }
    }
}