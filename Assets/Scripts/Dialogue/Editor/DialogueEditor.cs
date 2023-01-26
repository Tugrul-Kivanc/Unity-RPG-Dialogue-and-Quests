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
        private Vector2 scrollViewPosition;
        private const float editorSize = 4000f;
        private const float backgroundSize = 50f;
        [NonSerialized] private GUIStyle nodeStyle;
        [NonSerialized] private DialogueNode nodeToDrag = null;
        [NonSerialized] private DialogueNode creatingNode = null;
        [NonSerialized] private DialogueNode deletingNode = null;
        [NonSerialized] private DialogueNode linkingNode = null;
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

                scrollViewPosition = EditorGUILayout.BeginScrollView(scrollViewPosition);
                Rect canvas = GUILayoutUtility.GetRect(editorSize, editorSize);
                Texture2D backgroundTexture = Resources.Load("background") as Texture2D;
                GUI.DrawTextureWithTexCoords(canvas, backgroundTexture, new Rect(0, 0, editorSize / backgroundSize, editorSize / backgroundSize));

                foreach (DialogueNode dialogueNode in selectedDialogue.DialogueNodes)
                {
                    DrawNode(dialogueNode);
                }

                foreach (DialogueNode dialogueNode in selectedDialogue.DialogueNodes)
                {
                    DrawNodeConnections(dialogueNode);
                }

                EditorGUILayout.EndScrollView();

                if (creatingNode != null)
                {
                    selectedDialogue.CreateNode(creatingNode);
                    creatingNode = null;
                }

                if (deletingNode != null)
                {
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
                nodeToDrag = GetNodeAtPoint(Event.current.mousePosition + scrollViewPosition);
                if (draggingOffset != null && nodeToDrag != null)
                {
                    draggingOffset = nodeToDrag.NodeRect.position - Event.current.mousePosition;
                    Selection.activeObject = nodeToDrag;
                }
                else
                {
                    Selection.activeObject = selectedDialogue;
                }
            }
            else if (Event.current.type == EventType.MouseDrag && isDragging)
            {
                MoveNode(nodeToDrag);
                GUI.changed = true;
            }
            else if (Event.current.type == EventType.MouseDrag && !isDragging)
            {
                scrollViewPosition -= Event.current.delta;
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

            dialogueNode.Text = EditorGUILayout.TextField(dialogueNode.Text);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Add"))
            {
                creatingNode = dialogueNode;
            }

            if (GUILayout.Button("Delete"))
            {
                deletingNode = dialogueNode;
            }

            DrawLinkButtons(dialogueNode);
            GUILayout.EndHorizontal();

            GUILayout.EndArea();
        }

        private void DrawLinkButtons(DialogueNode dialogueNode)
        {
            if (linkingNode == null)
            {
                if (GUILayout.Button("Link"))
                {
                    linkingNode = dialogueNode;
                }
            }
            else if (linkingNode == dialogueNode)
            {
                if (GUILayout.Button("Cancel"))
                {
                    linkingNode = null;
                }
            }
            else if (linkingNode.Children.Contains(dialogueNode.name))
            {
                if (GUILayout.Button("Unlink"))
                {
                    linkingNode.RemoveChild(dialogueNode.name);
                    linkingNode = null;
                }
            }
            else
            {
                if (GUILayout.Button("Child"))
                {
                    linkingNode.AddChild(dialogueNode.name);
                    linkingNode = null;
                }
            }
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