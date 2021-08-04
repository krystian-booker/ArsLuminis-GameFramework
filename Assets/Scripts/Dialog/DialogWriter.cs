using Dialog.Models;
using EventSystem.VisualEditor.Nodes.Actions;
using UnityEngine;

namespace Dialog
{
    public class DialogWriter
    {
        private float _timer;
        private int _characterIndex;
        private bool _continueClicked;

        private readonly DialogNode _dialogNode;
        private readonly DialogComponents _dialogComponents;
        private readonly float _timePerCharacter;

        private Vector3 offset;

        public DialogWriter(DialogNode dialogNode, DialogComponents dialogComponents)
        {
            _dialogNode = dialogNode;
            _dialogComponents = dialogComponents;
            _timePerCharacter = dialogNode.customTimePerCharacter
                ? dialogNode.timePerCharacter
                : GameManager.Instance.dialogManager.defaultTimePerCharacter;

            //Reset from previous runs
            _characterIndex = 0;
            _continueClicked = false;
        }

        /// <summary>
        /// Sets up the dialog window according to the configuration of the dialogNode
        /// </summary>
        public void Initialize()
        {
            var characterOriginalPosition = _dialogNode.character.transform.position;
            var canvasPosition = GameManager.Instance.mainCamera.ScreenToWorldPoint(_dialogComponents.rectTransform.position);
            offset = characterOriginalPosition + canvasPosition;
            
            _dialogComponents.dialogGameObject.SetActive(true);

            //Set text
            _dialogComponents.characterNameTMPText.text = _dialogNode.characterName;
            _dialogComponents.dialogTMPText.text = string.Empty;

            //Set size of dialog window
            var width = _dialogNode.dialogWidth != 0
                ? _dialogNode.dialogWidth
                : GameManager.Instance.dialogManager.defaultWidth;
            var height = _dialogNode.dialogHeight != 0
                ? _dialogNode.dialogHeight
                : GameManager.Instance.dialogManager.defaultHeight;
            _dialogComponents.rectTransform.sizeDelta = new Vector2(width, height);

            //Move dialog to position
            var positionX = _dialogNode.customDialogPosition
                ? _dialogNode.dialogPositionX
                : GameManager.Instance.dialogManager.defaultPositionX;
            var positionY = _dialogNode.customDialogPosition
                ? _dialogNode.dialogPositionY
                : GameManager.Instance.dialogManager.defaultPositionY;
            _dialogComponents.rectTransform.anchoredPosition = new Vector2(positionX, positionY);

            //

            
            
            Debug.Log(characterOriginalPosition);
            Debug.Log(canvasPosition);
            Debug.Log(offset);
        }

        /// <summary>
        /// </summary>
        /// <returns>True on complete</returns>
        public void Update()
        {
            UpdateDialogPosition();
            UpdateText();
        }

        /// <summary>
        /// When the text is fully displayed and the user has clicked continue
        /// then IsNodeFinished will return true
        /// </summary>
        /// <returns>bool</returns>
        public bool IsNodeFinished()
        {
            return _continueClicked && _characterIndex >= _dialogNode.text.Length;
        }

        /// <summary>
        /// Returns whether or not the text is fully displayed on screen
        /// </summary>
        /// <returns></returns>
        public bool IsTextFinished()
        {
            return _characterIndex >= _dialogNode.text.Length;
        }

        /// <summary>
        /// Will skip the typing animation and fully display the text on screen instantly
        /// </summary>
        public void SkipTextAnimation()
        {
            _characterIndex = _dialogNode.text.Length;
            _dialogComponents.dialogTMPText.text = _dialogNode.text;
        }

        /// <summary>
        /// The textWriter has fully displayed and continue has been clicked, mark as finished 
        /// </summary>
        public void MarkAsFinished()
        {
            if (!IsTextFinished())
                SkipTextAnimation();
            _continueClicked = true;
        }

        /// <summary>
        /// Returns the dialogComponents this dialogWriter instance is using
        /// </summary>
        /// <returns>DialogComponents</returns>
        public DialogComponents GetDialogComponent()
        {
            return _dialogComponents;
        }

        /// <summary>
        /// Allows the dialog box to follow the player around during movement
        /// </summary>
        private void UpdateDialogPosition()
        {
            if (_dialogNode.followCharacter)
            {
                var characterPosition = GameManager.Instance.mainCamera.WorldToScreenPoint(_dialogNode.character.transform.position);
                characterPosition += offset;
                _dialogComponents.rectTransform.position = characterPosition;
            }
        }

        /// <summary>
        /// Updates the text on the UI with a typing animation
        /// </summary>
        private void UpdateText()
        {
            if (_dialogNode.text.Length <= 0 || _characterIndex >= _dialogNode.text.Length)
                return;

            _timer -= Time.deltaTime;
            while (_timer <= 0f)
            {
                //Display next character
                _timer += _timePerCharacter;
                _characterIndex++;

                //Display all characters, change alpha on unwritten to prevent character format changes
                var text = _dialogNode.text.Substring(0, _characterIndex);
                text += $"<alpha=#00>{_dialogNode.text.Substring(_characterIndex)}";
                _dialogComponents.dialogTMPText.text = text;

                if (_characterIndex < _dialogNode.text.Length) continue;
                return;
            }
        }
    }
}