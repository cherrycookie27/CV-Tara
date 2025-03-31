using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Ink.Runtime;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public Rigidbody2D rb;
    public TextAsset inkAsset;
    private Story _inkStory;
    public GameObject dialoguebox;

    // UI elements using TextMeshPro
    public TextMeshProUGUI dialogueText; // TextMeshProUGUI component for displaying dialogue
    public GameObject choiceButtonPrefab;
    public Transform choiceButtonContainer;

    // To store the player's choice
    private Coroutine typingCoroutine;
    public float letterDelay = 0.02f;
    private int _selectedChoice = -1;

    private bool isTyping = false;
    private bool dialogueIsPlaying;

    private void Start()
    {
        dialoguebox.SetActive(false);
        dialogueIsPlaying = false;
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.E) && !dialogueIsPlaying)
        {
            StartDialogue();
        }

        if (!dialogueIsPlaying)
        {
            return;
        }

        // Continue dialogue or close the dialogue bar
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
        {
            if (isTyping)
            {
                FinishTyping();
            }
            else if (_inkStory.canContinue && _inkStory.currentChoices.Count == 0)
            {
                ContinueStory();
            }
            else if (!_inkStory.canContinue && _inkStory.currentChoices.Count == 0)
            {
                dialoguebox.SetActive(false);
                rb.constraints = RigidbodyConstraints2D.None;
            }
        }
    }

    public void StartDialogue(/*Story story??*/) //use this to start the story from other places, make sure it starts a specific dilogue part from a specific inkAsset
    {
        _inkStory = new Story(inkAsset.text);
        dialogueIsPlaying = true;
        dialoguebox.SetActive(true);
        rb.constraints = RigidbodyConstraints2D.FreezeAll;

        ContinueStory();
    }

    #region Story Continue
    private void ContinueStory()
    {
        if (_inkStory.canContinue)
        {
            // Stop any ongoing typing coroutine
            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
            }

            // Start typing the next line of dialogue
            string nextLine = _inkStory.Continue();
            typingCoroutine = StartCoroutine(TypeText(nextLine));

            ClearChoices();
            CreateChoices();
        }
        else if (_inkStory.currentChoices.Count > 0 && _selectedChoice >= 0)
        {
            // Choose the selected choice
            _inkStory.ChooseChoiceIndex(_selectedChoice);
            _selectedChoice = -1; // Reset after choosing

            // Clear previous dialogue text and choices
            dialogueText.text = "";
            ClearChoices();
            CreateChoices();

            ContinueStory();
        }
        else if (_inkStory.currentChoices.Count == 0 && !_inkStory.canContinue)
        {
            dialoguebox.SetActive(false);
            ClearChoices();
            dialogueText.text = "";
        }
    }
    private void FinishTyping()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }
        dialogueText.text = _inkStory.currentText;
        isTyping = false;
    }

    private IEnumerator TypeText(string text)
    {
        dialogueText.text = ""; // Clear the current text
        isTyping = true;

        foreach (char letter in text)
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(letterDelay);
        }

        isTyping = false;
        typingCoroutine = null; // Reset coroutine reference after finishing
    }

    #endregion

    #region Choices and buttons
    public void SetChoice(int choiceIndex)
    {
        _selectedChoice = choiceIndex;

        ContinueStory();
    }

    private void CreateChoices()
    {
        if (_inkStory.currentChoices.Count > 0)
        {
            for (int i = 0; i < _inkStory.currentChoices.Count; i++)
            {
                Choice choice = _inkStory.currentChoices[i];
                CreateChoiceButton(choice.text, i);
            }
        }
    }

    private void CreateChoiceButton(string choiceText, int choiceIndex)
    {
        GameObject buttonObject = Instantiate(choiceButtonPrefab, choiceButtonContainer);
        TextMeshProUGUI buttonText = buttonObject.GetComponentInChildren<TextMeshProUGUI>();
        buttonText.text = choiceText;

        Button button = buttonObject.GetComponent<Button>();
        button.onClick.AddListener(() => SetChoice(choiceIndex));
    }
    private void ClearChoices()
    {
        foreach (Transform child in choiceButtonContainer)
        {
            Destroy(child.gameObject);
        }
    }
    #endregion
}
