#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace RPG.Dialogue.Editor
{
    public class DialogueEditor : EditorWindow
    {
        private Dialogue selectedDialogue = null;
        private GUIStyle nodeStyle;
        private DialogueNode nodeToDrag = null;
        private Vector2 draggingOffset;

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
                    OnGUINode(dialogueNode);
                }
            }
        }

        private void ProcessMouseEvent()
        {
            bool isDragging = nodeToDrag != null;

            if (Event.current.type == EventType.MouseDown && !isDragging)
            {
                nodeToDrag = GetNodeAtPoint(Event.current.mousePosition);
                if (draggingOffset != null)
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

        private void OnGUINode(DialogueNode dialogueNode)
        {
            GUILayout.BeginArea(dialogueNode.NodeRect, nodeStyle);

            EditorGUI.BeginChangeCheck();

            EditorGUILayout.LabelField("Node " + dialogueNode.UniqueId + ":", EditorStyles.boldLabel);
            string newUniqueId = EditorGUILayout.TextField(dialogueNode.UniqueId);
            string newText = EditorGUILayout.TextField(dialogueNode.Text);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(selectedDialogue, "Update Dialogue Text");

                dialogueNode.UniqueId = newUniqueId;
                dialogueNode.Text = newText;
            }

            foreach (DialogueNode childNode in selectedDialogue.GetAllChildren(dialogueNode))
            {
                EditorGUILayout.LabelField(childNode.Text);
            }

            GUILayout.EndArea();
        }
    }
}
#endif