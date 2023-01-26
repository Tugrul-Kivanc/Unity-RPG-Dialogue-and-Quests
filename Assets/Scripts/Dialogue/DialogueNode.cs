using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace RPG.Dialogue
{
    public class DialogueNode : ScriptableObject
    {
        [SerializeField] private string text;
        [SerializeField] private List<string> children = new List<string>();
        [SerializeField] private Rect nodeRect = new Rect(100, 100, 200, 100);

        public List<string> Children => children;
        public Rect NodeRect => nodeRect;

        public string Text
        {
            get => text;
            set
            {
                if (value != text)
                {
#if UNITY_EDITOR
                    Undo.RecordObject(this, "Update Dialogue Text");
                    text = value;
                    EditorUtility.SetDirty(this);
#endif
                }
            }
        }

        public void AddChild(string childId)
        {
#if UNITY_EDITOR
            Undo.RecordObject(this, "Added Child Node Link");
            children.Add(childId);
            EditorUtility.SetDirty(this);
#endif
        }

        public void RemoveChild(string childId)
        {
#if UNITY_EDITOR
            Undo.RecordObject(this, "Removed Child Node Link");
            children.Remove(childId);
            EditorUtility.SetDirty(this);
#endif
        }

        public void SetPosition(Vector2 newPosition)
        {
#if UNITY_EDITOR
            Undo.RecordObject(this, "Move Dialogue Node");
            nodeRect.position = newPosition;
            EditorUtility.SetDirty(this);
#endif
        }
    }
}
