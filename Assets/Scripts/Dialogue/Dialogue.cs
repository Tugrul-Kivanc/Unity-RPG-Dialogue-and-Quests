using System;
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
        private Dictionary<string, DialogueNode> nodeLookup = new Dictionary<string, DialogueNode>();

#if UNITY_EDITOR
        private void Awake()
        {
            if (dialogueNodes.Count == 0)
            {
                DialogueNode rootNode = new DialogueNode();
                rootNode.UniqueId = Guid.NewGuid().ToString();
                dialogueNodes.Add(rootNode);
            }

            OnValidate();
        }
#endif

        private void OnValidate()
        {
            nodeLookup.Clear();
            foreach (var node in dialogueNodes)
            {
                nodeLookup[node.UniqueId] = node;
            }
        }

        public DialogueNode GetRootNode()
        {
            return dialogueNodes[0];
        }

        public IEnumerable<DialogueNode> GetAllChildren(DialogueNode parentNode)
        {
            foreach (string childId in parentNode.Children)
            {
                if (nodeLookup.ContainsKey(childId))
                {
                    yield return nodeLookup[childId];
                }
            }
        }

        public void CreateNode(DialogueNode parentNode)
        {
            DialogueNode newNode = new DialogueNode();
            newNode.UniqueId = Guid.NewGuid().ToString();
            parentNode.Children.Add(newNode.UniqueId);
            dialogueNodes.Add(newNode);
            OnValidate();
        }

        public void DeleteNode(DialogueNode nodeToDelete)
        {
            dialogueNodes.Remove(nodeToDelete);
            DeleteLinkToChildren(nodeToDelete);
            OnValidate();
        }

        private void DeleteLinkToChildren(DialogueNode nodeToDelete)
        {
            foreach (var node in dialogueNodes)
            {
                node.Children.Remove(nodeToDelete.UniqueId);
            }
        }
    }
}
