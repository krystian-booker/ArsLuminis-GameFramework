using System.Collections;
using EventSystem.VisualEditor.Nodes.Actions;
using TMPro;
using UnityEngine;

namespace Managers
{
    public class DialogManager : MonoBehaviour
    {
        public TMP_Text nameText;
        public TMP_Text dialogText;
        public GameObject dialogBox;

        public float typeSpeed = 0.05f;
        private bool _continueClicked;
        private bool _hideDialogBoxOnComplete;

        //TODO: Add customization for dialog behaviour to the manager and dialogNode. Works for now as proof of concept
        private void Awake()
        {
            dialogBox.SetActive(false);
        }

        public void StartDialog(DialogNode dialogNode)
        {
            //Default states
            dialogBox.SetActive(true);
            _continueClicked = false;
            
            //Node state
            _hideDialogBoxOnComplete = dialogNode.hideAfter;
            nameText.text = dialogNode.characterName;
            StartCoroutine(TypeSentence(dialogNode.text));
        }

        private IEnumerator TypeSentence(string sentence)
        {
            dialogText.text = "";
            foreach (var letter in sentence.ToCharArray())
            {
                dialogText.text += letter;
                yield return new WaitForSeconds(typeSpeed);
            }
        }

        public void ContinueClicked()
        {
            _continueClicked = true;
        }

        public bool IsContinueClicked()
        {
            if (_continueClicked && _hideDialogBoxOnComplete)
            {
                dialogBox.SetActive(false);
            }
            return _continueClicked;
        }
    }
}