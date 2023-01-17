#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace RPG.Dialogue.Editor
{
    public class DialogueEditor : EditorWindow
    {
        private Dialogue selectedDialogue = null;

        private void OnEnable()
        {
            Selection.selectionChanged += OnSelectionChanged;
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
                foreach (DialogueNode dialogueNode in selectedDialogue.DialogueNodes)
                {
                    EditorGUI.BeginChangeCheck();

                    EditorGUILayout.LabelField("Node " + dialogueNode.UniqueId + ":");
                    string newUniqueId = EditorGUILayout.TextField(dialogueNode.UniqueId);
                    string newText = EditorGUILayout.TextField(dialogueNode.Text);

                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(selectedDialogue, "Update Dialogue Text");

                        dialogueNode.UniqueId = newUniqueId;
                        dialogueNode.Text = newText;
                    }
                }
            }
        }
    }
}
#endif