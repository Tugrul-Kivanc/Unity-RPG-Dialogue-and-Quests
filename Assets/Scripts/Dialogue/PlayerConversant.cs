using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Dialogue
{
    public class PlayerConversant : MonoBehaviour
    {
        [SerializeField] private Dialogue currentDialogue;

        public string GetText()
        {
            if (currentDialogue == null)
            {
                return "Current Dialogue is Null!";
            }
            return currentDialogue.DialogueNodes[0].Text;
        }
    }

}