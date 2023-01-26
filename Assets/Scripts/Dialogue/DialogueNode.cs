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

        public string Text
        {
            get => text;
            set
            {
                if (value != text)
                {
#if UNITY_EDITOR
                    Undo.RecordObject(this, "Update Dialogue Text");
#endif
                    text = value;
                }
            }
        }

        public List<string> Children
        {
            get
            {
#if UNITY_EDITOR
                Undo.RecordObject(this, "Updated Child Node Link");
#endif
                return children;
            }
        }

        public Rect NodeRect
        {
            get => nodeRect;
            set
            {
#if UNITY_EDITOR
                Undo.RecordObject(this, "Moved Dialogue Node");
#endif
                nodeRect = value;
            }
        }

        public void AddChild(string childId)
        {
#if UNITY_EDITOR
            Undo.RecordObject(this, "Added Child Node Link");
#endif
            children.Add(childId);
        }

        public void RemoveChild(string childId)
        {
#if UNITY_EDITOR
            Undo.RecordObject(this, "Removed Child Node Link");
#endif
            children.Remove(childId);
        }
    }
}
