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
        [SerializeField] private TextMeshProUGUI dialogueText;
        [SerializeField] private Button nextButton;
        [SerializeField] private GameObject AIResponse;
        [SerializeField] private Transform choiceRoot;
        [SerializeField] private GameObject choicePrefab;
        private PlayerConversant playerConversant;

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

            AIResponse.SetActive(!playerConversant.IsChoosing);
            choiceRoot.gameObject.SetActive(playerConversant.IsChoosing);

            if (playerConversant.IsChoosing)
            {
                BuildChoiceList();
            }
            else
            {
                dialogueText.text = playerConversant.GetText();
                nextButton.gameObject.SetActive(playerConversant.HasNext());
            }
        }

        private void BuildChoiceList()
        {
            RemoveChildren(choiceRoot);

            foreach (DialogueNode choice in playerConversant.GetChoices())
            {
                GameObject choiceInstance = Instantiate(choicePrefab, choiceRoot);
                var textComponent = choiceInstance.GetComponentInChildren<TextMeshProUGUI>();
                textComponent.text = choice.Text;

                Button button = choiceInstance.GetComponentInChildren<Button>();
                button.onClick.AddListener(() =>
                {
                    playerConversant.SelectChoice(choice);
                    UpdateUI();
                });
            }
        }

        private void RemoveChildren(Transform rootObject)
        {
            foreach (Transform child in rootObject)
            {
                Destroy(child.gameObject);
            }
        }
    }
}
