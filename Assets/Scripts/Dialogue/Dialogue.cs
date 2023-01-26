﻿using System;
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

#if UNITY_EDITOR
        public void CreateNode(DialogueNode parentNode)
        {
            DialogueNode newNode = MakeNode(parentNode);
            Undo.RegisterCreatedObjectUndo(newNode, "Created Dialogue Node");
            Undo.RecordObject(this, "Created Dialogue Node");
            AddNode(newNode);
        }

        public void DeleteNode(DialogueNode nodeToDelete)
        {
            Undo.RecordObject(this, "Deleted Dialogue Node");
            dialogueNodes.Remove(nodeToDelete);
            OnValidate();
            DeleteLinkToChildren(nodeToDelete);
            Undo.DestroyObjectImmediate(nodeToDelete);
        }

        private static DialogueNode MakeNode(DialogueNode parentNode)
        {
            DialogueNode newNode = CreateInstance<DialogueNode>();
            newNode.name = Guid.NewGuid().ToString();
            if (parentNode != null)
            {
                parentNode.Children.Add(newNode.name);
            }

            return newNode;
        }

        private void AddNode(DialogueNode newNode)
        {
            dialogueNodes.Add(newNode);
            OnValidate();
        }

        private void DeleteLinkToChildren(DialogueNode nodeToDelete)
        {
            foreach (var node in dialogueNodes)
            {
                node.Children.Remove(nodeToDelete.name);
            }
        }
#endif

        public void OnBeforeSerialize()
        {
#if UNITY_EDITOR
            if (dialogueNodes.Count == 0)
            {
                DialogueNode newNode = MakeNode(null);
                MakeNode(newNode);
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
#endif
        }

        public void OnAfterDeserialize()
        {
            throw new NotImplementedException();
        }
    }
}
