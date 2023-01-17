using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Inventories
{
    [Serializable]
    public class DialogueNode
    {
        [SerializeField] private string uniqueId;
        [SerializeField] private string text;
        [SerializeField] private string[] children;
    }
}
