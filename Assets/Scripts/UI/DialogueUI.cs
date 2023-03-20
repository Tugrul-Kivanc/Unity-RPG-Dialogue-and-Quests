using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Dialogue;
using TMPro;
using UnityEngine.UI;

namespace RPG.IU
{
    public class DialogueUI : MonoBehaviour
    {
        private PlayerConversant playerConversant;
        [SerializeField] private TextMeshProUGUI dialogueText;
        [SerializeField] private Button nextButton;

        private void Start()
        {
            playerConversant = GameObject.FindWithTag("Player").GetComponent<PlayerConversant>();
            nextButton.onClick.AddListener(Next);
            UpdateUI();
        }

        private void Next()
        {
            playerConversant.Next();
            UpdateUI();
        }

        private void UpdateUI()
        {
            dialogueText.text = playerConversant.GetText();
            nextButton.gameObject.SetActive(playerConversant.HasNext());
        }
    }
}
