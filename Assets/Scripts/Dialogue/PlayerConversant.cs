using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RPG.Dialogue
{
    public class PlayerConversant : MonoBehaviour
    {
        [SerializeField] private Dialogue testDialogue;
        private Dialogue currentDialogue;
        private DialogueNode currentNode = null;
        private bool isChoosing = false;
        public bool IsChoosing => isChoosing;
        public event Action onConversationUpdated;

        private void Awake()
        {

        }

        private IEnumerator Start()
        {
            yield return new WaitForSeconds(2);
            StartDialogue(testDialogue);
        }

        public void StartDialogue(Dialogue newDialogue)
        {
            currentDialogue = newDialogue;
            currentNode = currentDialogue.DialogueNodes[0];
            onConversationUpdated();
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

        public void SelectChoice(DialogueNode chosenNode)
        {
            currentNode = chosenNode;
            isChoosing = false;
            Next();
        }

        public void Next()
        {
            int numberOfPlayerResponses = currentDialogue.GetPlayerChildNodes(currentNode).Count();
            if (numberOfPlayerResponses > 0)
            {
                isChoosing = true;
                onConversationUpdated();
                return;
            }

            DialogueNode[] childNodes = currentDialogue.GetAIChildNodes(currentNode).ToArray();
            int randomIndex = UnityEngine.Random.Range(0, childNodes.Length);
            currentNode = childNodes[randomIndex];
            onConversationUpdated();
        }

        public bool HasNext()
        {
            return currentNode.Children.Count > 0;
        }

        public bool IsActive()
        {
            return currentDialogue != null;
        }

        // public bool IsChoosing()
        // {
        //     return isChoosing;
        // }
    }

}