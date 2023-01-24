using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Dialogue
{
    [Serializable]
    public class DialogueNode
    {
        [SerializeField] private string uniqueId;
        [SerializeField] private string text;
        [SerializeField] private List<string> children = new List<string>();
        [SerializeField] private Rect nodeRect = new Rect(100, 100, 200, 100);
        public string UniqueId { get { return uniqueId; } set { uniqueId = value; } }
        public string Text { get { return text; } set { text = value; } }
        public List<string> Children => children;
        public Rect NodeRect { get => nodeRect; set => nodeRect = value; }
    }
}
