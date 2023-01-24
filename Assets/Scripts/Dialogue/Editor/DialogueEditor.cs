﻿#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace RPG.Dialogue.Editor
{
    public class DialogueEditor : EditorWindow
    {
        private Dialogue selectedDialogue = null;
        [NonSerialized] private GUIStyle nodeStyle;
        [NonSerialized] private DialogueNode nodeToDrag = null;
        [NonSerialized] private DialogueNode creatingNode = null;
        [NonSerialized] private DialogueNode deletingNode = null;
        [NonSerialized] private Vector2 draggingOffset = new Vector2(0f, 0f);

        private void OnEnable()
        {
            Selection.selectionChanged += OnSelectionChanged;

            nodeStyle = new GUIStyle();
            nodeStyle.normal.background = Texture2D.grayTexture;
            nodeStyle.padding = new RectOffset(20, 20, 20, 20);
            nodeStyle.border = new RectOffset(12, 12, 12, 12);
        }

        private void OnDisable()
        {
            Selection.selectionChanged -= OnSelectionChanged;
        }

        private void OnSelectionChanged()
        {
            Dialogue newDialogue = Selection.activeObject as Dialogue;

            if (newDialogue != null)
            {
                selectedDialogue = newDialogue;
            }
        }

        [MenuItem("RPG Project/Dialogue Editor")]
        private static void ShowWindow()
        {
            var window = GetWindow<DialogueEditor>();
            window.titleContent = new GUIContent("Dialogue Editor");
            window.Show();
        }

        [OnOpenAsset(1)]
        public static bool OnOpenDialogueAsset(int instanceId, int line)
        {
            bool isDialogue = EditorUtility.InstanceIDToObject(instanceId) is Dialogue;
            if (!isDialogue) return false;

            ShowWindow();
            return true;
        }

        private void OnGUI()
        {
            if (selectedDialogue == null) EditorGUILayout.LabelField("No dialogue selected.");
            else
            {
                ProcessMouseEvent();
                foreach (DialogueNode dialogueNode in selectedDialogue.DialogueNodes)
                {
                    DrawNode(dialogueNode);
                }

                foreach (DialogueNode dialogueNode in selectedDialogue.DialogueNodes)
                {
                    DrawNodeConnections(dialogueNode);
                }

                if (creatingNode != null)
                {
                    Undo.RecordObject(selectedDialogue, "Created Dialogue Node");
                    selectedDialogue.CreateNode(creatingNode);
                    creatingNode = null;
                }

                if (deletingNode != null)
                {
                    Undo.RecordObject(selectedDialogue, "Deleted Dialogue Node");
                    selectedDialogue.DeleteNode(deletingNode);
                    deletingNode = null;
                }

            }
        }

        private void ProcessMouseEvent()
        {
            bool isDragging = nodeToDrag != null;

            if (Event.current.type == EventType.MouseDown && !isDragging)
            {
                nodeToDrag = GetNodeAtPoint(Event.current.mousePosition);
                if (draggingOffset != null && nodeToDrag != null)
                {
                    draggingOffset = nodeToDrag.NodeRect.position - Event.current.mousePosition;
                }
            }
            else if (Event.current.type == EventType.MouseDrag && isDragging)
            {
                Undo.RecordObject(selectedDialogue, "Move Dialogue Node");
                MoveNode(nodeToDrag);
                GUI.changed = true;
            }
            else if (Event.current.type == EventType.MouseUp && isDragging)
            {
                nodeToDrag = null;
            }
        }

        private void MoveNode(DialogueNode nodeToDrag)
        {
            var rect = nodeToDrag.NodeRect;
            rect.position = Event.current.mousePosition + draggingOffset;
            nodeToDrag.NodeRect = rect;
        }

        private DialogueNode GetNodeAtPoint(Vector2 point)
        {
            DialogueNode foundNode = null;
            foreach (var node in selectedDialogue.DialogueNodes)
            {
                if (node.NodeRect.Contains(point)) foundNode = node;
            }
            return foundNode;
        }

        private void DrawNode(DialogueNode dialogueNode)
        {
            GUILayout.BeginArea(dialogueNode.NodeRect, nodeStyle);

            EditorGUI.BeginChangeCheck();

            string newText = EditorGUILayout.TextField(dialogueNode.Text);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(selectedDialogue, "Update Dialogue Text");

                dialogueNode.Text = newText;
            }

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Add"))
            {
                creatingNode = dialogueNode;
            }

            if (GUILayout.Button("Delete"))
            {
                deletingNode = dialogueNode;
            }
            GUILayout.EndHorizontal();

            GUILayout.EndArea();
        }

        private void DrawNodeConnections(DialogueNode dialogueNode)
        {
            float BezierLineWidth = 2f;
            Vector3 startPosition = new Vector2(dialogueNode.NodeRect.xMax, dialogueNode.NodeRect.center.y);

            foreach (var childNode in selectedDialogue.GetAllChildren(dialogueNode))
            {
                Vector3 endPosition = new Vector2(childNode.NodeRect.xMin, childNode.NodeRect.center.y);

                Vector3 controlPointOffset = endPosition - startPosition;
                controlPointOffset.y = 0;
                controlPointOffset.x *= 0.75f;

                Handles.DrawBezier(
                    startPosition, endPosition,
                    startPosition + controlPointOffset, endPosition - controlPointOffset,
                    Color.blue, null, BezierLineWidth);
            }
        }
    }
}
#endif