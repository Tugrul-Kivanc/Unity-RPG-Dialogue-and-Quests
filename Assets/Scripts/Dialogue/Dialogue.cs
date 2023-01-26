using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace RPG.Dialogue
{
    [CreateAssetMenu(fileName = "Default Dialogue", menuName = "Dialogue/New Dialogue", order = 0)]
    public class Dialogue : ScriptableObject, ISerializationCallbackReceiver
    {
        [SerializeField] private List<DialogueNode> dialogueNodes = new List<DialogueNode>();
        public List<DialogueNode> DialogueNodes => dialogueNodes;
        private Dictionary<string, DialogueNode> nodeLookup = new Dictionary<string, DialogueNode>();

#if UNITY_EDITOR
        private void Awake()
        {
            OnValidate();
        }
#endif

        private void OnValidate()
        {
            nodeLookup.Clear();
            foreach (var node in dialogueNodes)
            {
                nodeLookup[node.name] = node;
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
            DialogueNode newNode = CreateInstance<DialogueNode>();
            newNode.name = Guid.NewGuid().ToString();
            Undo.RegisterCreatedObjectUndo(newNode, "Created Dialogue Node");
            if (parentNode != null)
            {
                parentNode.Children.Add(newNode.name);
            }
            dialogueNodes.Add(newNode);
            OnValidate();
        }

        public void DeleteNode(DialogueNode nodeToDelete)
        {
            dialogueNodes.Remove(nodeToDelete);
            DeleteLinkToChildren(nodeToDelete);
            OnValidate();
            Undo.DestroyObjectImmediate(nodeToDelete);
        }

        private void DeleteLinkToChildren(DialogueNode nodeToDelete)
        {
            foreach (var node in dialogueNodes)
            {
                node.Children.Remove(nodeToDelete.name);
            }
        }

        public void OnBeforeSerialize()
        {
            if (dialogueNodes.Count == 0)
            {
                CreateNode(null);
            }

            if (!String.IsNullOrEmpty(AssetDatabase.GetAssetPath(this)))
            {
                foreach (var node in dialogueNodes)
                {
                    if (String.IsNullOrEmpty(AssetDatabase.GetAssetPath(node)))
                    {
                        AssetDatabase.AddObjectToAsset(node, this);
                    }
                }
            }
        }

        public void OnAfterDeserialize()
        {
            throw new NotImplementedException();
        }
    }
}
