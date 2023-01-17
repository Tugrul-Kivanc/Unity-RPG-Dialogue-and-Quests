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
        public string Text => text;
    }
}
