using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RPG.Dialogue
{
    public class PlayerConversant : MonoBehaviour
    {
        private Dialogue currentDialogue;
        private DialogueNode currentNode = null;
        private AIConversant currentAIConversant = null;
        private float conversationRange = 3f;
        private bool isChoosing = false;
        public bool IsChoosing => isChoosing;
        public event Action onConversationUpdated;

        public void StartDialogue(Dialogue newDialogue, AIConversant newConversant)
        {
            if (IsInConversationRange(newConversant))
            {
                currentDialogue = newDialogue;
                currentNode = currentDialogue.DialogueNodes[0];
                currentAIConversant = newConversant;

                TriggerEnterAction();
                onConversationUpdated();
            }
        }

        public void Quit()
        {
            currentDialogue = null;
            TriggerExitAction();
            currentAIConversant = null;
            currentNode = null;
            isChoosing = false;
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
            TriggerEnterAction();
            isChoosing = false;
            Next();
        }

        public void Next()
        {
            int numberOfPlayerResponses = currentDialogue.GetPlayerChildNodes(currentNode).Count();
            if (numberOfPlayerResponses > 0)
            {
                isChoosing = true;
                TriggerExitAction();
                onConversationUpdated();
                return;
            }

            DialogueNode[] childNodes = currentDialogue.GetAIChildNodes(currentNode).ToArray();
            int randomIndex = UnityEngine.Random.Range(0, childNodes.Length);
            TriggerExitAction();
            currentNode = childNodes[randomIndex];
            TriggerEnterAction();
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

        public bool IsInConversationRange(AIConversant aiConversant)
        {
            return Vector3.Distance(transform.position, aiConversant.transform.position) < conversationRange;
        }

        private void TriggerEnterAction()
        {
            if (currentNode == null) return;

            TriggerAction(currentNode.OnEnterAction);
        }

        private void TriggerExitAction()
        {
            if (currentNode == null) return;

            TriggerAction(currentNode.OnExitAction);
        }

        private void TriggerAction(string action)
        {
            if (action == "") return;

            foreach (DialogueTrigger trigger in currentAIConversant.GetComponents<DialogueTrigger>())
            {
                trigger.Trigger(action);
            }
        }

        // public bool IsChoosing()
        // {
        //     return isChoosing;
        // }
    }

}