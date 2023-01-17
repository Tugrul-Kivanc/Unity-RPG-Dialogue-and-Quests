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
        public string UniqueId { get { return uniqueId; } set { uniqueId = value; } }
        public string Text { get { return text; } set { text = value; } }
    }
}
