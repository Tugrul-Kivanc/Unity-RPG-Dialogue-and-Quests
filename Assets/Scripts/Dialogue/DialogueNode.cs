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
        [SerializeField] private string[] children;
        [SerializeField] private Rect nodeRect = new Rect(0, 0, 200, 100);
        public string UniqueId { get { return uniqueId; } set { uniqueId = value; } }
        public string Text { get { return text; } set { text = value; } }
        public string[] Children => children;
        public Rect NodeRect { get => nodeRect; set => nodeRect = value; }
    }
}
