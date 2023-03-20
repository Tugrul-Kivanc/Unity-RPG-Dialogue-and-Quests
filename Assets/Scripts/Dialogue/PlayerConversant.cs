using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RPG.Dialogue
{
    public class PlayerConversant : MonoBehaviour
    {
        [SerializeField] private Dialogue currentDialogue;
        private DialogueNode currentNode = null;
        private bool isChoosing = false;
        public bool IsChoosing => isChoosing;

        private void Awake()
        {
            currentNode = currentDialogue.DialogueNodes[0];
        }

        public string GetText()
        {
            if (currentNode == null)
            {
                return "Current Dialogue is Null!";
            }
            return currentNode.Text;
        }

        public IEnumerable<DialogueNode> GetChoices()
        {
            return currentDialogue.GetPlayerChildNodes(currentNode);
        }

        public void Next()
        {
            int numberOfPlayerResponses = currentDialogue.GetPlayerChildNodes(currentNode).Count();
            if (numberOfPlayerResponses > 0)
            {
                isChoosing = true;
                return;
            }

            DialogueNode[] childNodes = currentDialogue.GetAIChildNodes(currentNode).ToArray();
            int randomIndex = Random.Range(0, childNodes.Length);
            currentNode = childNodes[randomIndex];
        }

        public bool HasNext()
        {
            return currentNode.Children.Count > 0;
        }

        // public bool IsChoosing()
        // {
        //     return isChoosing;
        // }
    }

}