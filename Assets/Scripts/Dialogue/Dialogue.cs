using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Dialogue
{
    [CreateAssetMenu(fileName = "Default Dialogue", menuName = "Dialogue/New Dialogue", order = 0)]
    public class Dialogue : ScriptableObject
    {
        [SerializeField] private List<DialogueNode> dialogueNodes = new List<DialogueNode>();
        public List<DialogueNode> DialogueNodes => dialogueNodes;

#if UNITY_EDITOR
        private void Awake()
        {
            if (dialogueNodes.Count == 0)
            {
                dialogueNodes.Add(new DialogueNode());
            }
        }
#endif
    }
}
