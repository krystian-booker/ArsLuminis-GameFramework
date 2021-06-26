using System.Collections;
using System.Collections.Generic;
using EventSystem.Models;
using EventSystem.VisualEditor.Nodes.Actions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Managers
{
    public class DialogManager : MonoBehaviour
    {
        [Header("Dialog UI links")]
        [Tooltip("A gameobject that contains all child components of the dialogUI")]
        public GameObject dialogBox;

        [Tooltip("TextMeshPro: The character name section of the dialogUI. Not required.")]
        public TMP_Text nameText;
        
        [Tooltip("TextMeshPro: The text section of the dialogUI. Required.")]
        public TMP_Text dialogText;
        
        [Tooltip("Button - TextMeshPro: Continue button. Required.")]
        public GameObject continueButton;

        [Tooltip("The option buttons for the dialog, the amount set here is the maximum that will be allowed for dialogNodes.")]
        public List<GameObject> options;

        [Header("Dialog customizations")]
        [Tooltip("The speed that the dialog text will be typed at. 0 for instant. 0.05 recommended")]
        public float typeSpeed = 0.05f;
        
        private bool _continueClicked;
        private bool _hideDialogBoxOnComplete;
        private int _selectedOptionIndex = -1; //Used to track which option was selected
        
        private List<DialogOption> _dialogOptions; //DialogOptions from the DialogNode
        private List<TMP_Text> _optionText; //Synced with options, contains text gameobject
        
        private Coroutine _previousSentenceState;

        private void Awake()
        {
            dialogBox.SetActive(false);

            //Initialize options 
            _optionText = new List<TMP_Text>();
            for (var i = 0; i < options.Count; i++)
            {
                _optionText.Add(options[i].GetComponentInChildren<TMP_Text>());
                var button = options[i].GetComponent<Button>();
                var buttonIndex = i; //This is required, passing i instead will pass by reference
                button.onClick.AddListener(() => OptionClicked(buttonIndex));
            }
        }

        /// <summary>
        /// Called from the EventTimelineParser, when a dialogNode is hit.
        /// This will setup the dialog UI to the required states and begin the text
        /// </summary>
        /// <param name="dialogNode"></param>
        public void StartDialog(DialogNode dialogNode)
        {
            InitializeDialog(dialogNode);

            //Node states
            _dialogOptions = dialogNode.options;
            _hideDialogBoxOnComplete = dialogNode.hideUIOnComplete;

            if (nameText != null && !string.IsNullOrEmpty(dialogNode.characterName))
            {
                nameText.text = dialogNode.characterName;
            }
            _previousSentenceState = StartCoroutine(TypeSentence(dialogNode.text));
        }

        /// <summary>
        /// Called from the UI when the next button is clicked
        /// </summary>
        public void ContinueClicked()
        {
            _continueClicked = true;
        }
        
        /// <summary>
        /// Called from the UI when an Option button is clicked
        /// </summary>
        /// <param name="optionIndex"></param>
        public void OptionClicked(int optionIndex)
        {
            _continueClicked = true; //Option clicked...
            _selectedOptionIndex = optionIndex;
        }
        
        /// <summary>
        /// Called from the EventTimelineParser. This lets the timeline know when the dialog has finished.
        /// Hides the dialog UI, if set on the dialogNode
        /// </summary>
        /// <returns></returns>
        public bool IsContinueClicked()
        {
            if (_continueClicked && _hideDialogBoxOnComplete)
            {
                dialogBox.SetActive(false);
            }

            return _continueClicked;
        }

        public int GetSelectedOption()
        {
            return _selectedOptionIndex;
        }

        /// <summary>
        /// Stops previous coroutines, needed to prevent multiple coroutines writing to same text box.
        /// Defaults the states from previous runs.
        /// Sets up the buttons as required.
        /// </summary>
        /// <param name="dialogNode"></param>
        private void InitializeDialog(DialogNode dialogNode)
        {
            //Stop previous state
            if (_previousSentenceState != null)
            {
                StopCoroutine(_previousSentenceState);
                _previousSentenceState = null;
            }
            
            //Default states
            dialogBox.SetActive(true);
            continueButton.SetActive(true);
            _continueClicked = false;
            _selectedOptionIndex = -1;
            
            //Options setup
            if (dialogNode.options != null && dialogNode.options.Count > 0)
            {
                _dialogOptions = dialogNode.options;
                continueButton.SetActive(false);
            }
            else
            {
                foreach (var option in options)
                {
                    option.SetActive(false);
                }
            }
        }

        /// <summary>
        /// Type the sentence passed in from the dialogNode
        /// Create options once completed.
        /// </summary>
        /// <param name="sentence"></param>
        /// <returns></returns>
        private IEnumerator TypeSentence(string sentence)
        {
            if (typeSpeed == 0)
            {
                dialogText.text = sentence;
                yield return null;
            }
            else
            {
                dialogText.text = "";
                foreach (var letter in sentence.ToCharArray())
                {
                    dialogText.text += letter;
                    yield return new WaitForSeconds(typeSpeed);
                }
            }
            CreateOptions();
        }

        /// <summary>
        /// You are required to create an equal or greater amount of options UI buttons than you set in the dialogNodes.
        /// Links the dialogOption to the same index dialog option UI
        /// </summary>
        private void CreateOptions()
        {
            if (_dialogOptions.Count > options.Count)
            {
                Debug.LogError(
                    $"{nameof(DialogManager)}: Dialog node has '{_dialogOptions.Count}' options defined but only '{options.Count}' have been defined on the DialogManager.");
            }

            for (var i = 0; i < _dialogOptions.Count; i++)
            {
                options[i].SetActive(true);
                _optionText[i].text = _dialogOptions[i].text;
            }
        }
    }
}