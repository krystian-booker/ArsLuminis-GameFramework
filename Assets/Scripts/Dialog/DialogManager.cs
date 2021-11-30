using System.Collections.Generic;
using System.Linq;
using Dialog.Models;
using EventSystem.VisualEditor.Nodes.Dialog;
using UnityEngine;
using UnityEngine.Assertions;

namespace Dialog
{
    public class DialogManager : MonoBehaviour
    {
        #region Prefabs

        [SerializeField, Tooltip("Used to instantiate dialog instances when needed")]
        private GameObject dialogPrefab;

        [SerializeField, Tooltip("Used to add options to dialog instances")]
        private GameObject dialogOptionPrefab;

        #endregion

        #region Dialog Defaults

        [Tooltip("Default anchor X position on canvas, will be overwritten by dialog node if set")]
        public int defaultPositionX;

        [Tooltip("Default anchor Y position on canvas, will be overwritten by dialog node if set")]
        public int defaultPositionY;

        [Tooltip("Default dialog width on canvas, will be overwritten by dialog node if set")]
        public int defaultWidth;

        [Tooltip("Default dialog height on canvas, will be overwritten by dialog node if set")]
        public int defaultHeight;

        [Tooltip("Used to set a default time per character, will be overwritten by dialog node if set")]
        public float defaultTimePerCharacter = 0.1f;

        #endregion

        #region InstancePool

        [SerializeField, Tooltip("Dialog instances are pooled, 'n' amount of instances will be created on start")]
        private int maximumNumberOfDialogs = 6;

        [SerializeField, Tooltip("Maximum number of dialog options in the game.")]
        //This is required as dialog 'option' game objects are instantiated with the dialog pool.
        private int maximumNumberOfDialogOptions = 4;

        private List<DialogComponents> _dialogInstancePool;
        private const int PoolPositionX = -1500;
        private const int PoolPositionY = 1500;
        private GameObject _dialogPanel;

        #endregion

        private List<DialogWriter> _activeDialogWriters;

        private void Start()
        {
            //Init
            _activeDialogWriters = new List<DialogWriter>();
            _dialogInstancePool = new List<DialogComponents>();

            //GET DialogPanel from UI scene
            _dialogPanel = GameObject.Find("DialogPanel");

            //Assert
            Assert.IsNotNull(_dialogPanel, $"{nameof(DialogManager)}: dialogPanel is required for DialogManager.");
            Assert.IsNotNull(dialogPrefab, $"{nameof(DialogManager)}: dialogPrefab is required.");
            Assert.IsNotNull(dialogOptionPrefab, $"{nameof(DialogManager)}: dialogOptionPrefab is required.");

            //Create pool
            for (var i = 0; i < maximumNumberOfDialogs; i++)
            {
                PoolNewDialogInstance();
            }
        }

        /// <summary>
        /// Updates all dialog writers
        /// For timed dialogs, checks if complete/removes if complete
        /// </summary>
        private void Update()
        {
            //Iterate over list in reverse to allow us to remove elements without breaking indices 
            for (var i = _activeDialogWriters.Count - 1; i >= 0; i--)
            {
                _activeDialogWriters[i].Update();
                if (_activeDialogWriters[i].IsTimedDialog() && _activeDialogWriters[i].HasDisplayedForRequiredTime() ||
                    _activeDialogWriters[i].IsOptionDialog() && _activeDialogWriters[i].IsOptionSelected())
                {
                    ReturnDialogToPool(_activeDialogWriters[i].GetDialogComponent());
                    _activeDialogWriters.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// Used to create a new display dialog on screen from a DialogNode 
        /// </summary>
        /// <param name="dialogNode"></param>
        public DialogWriter NewDialog(DialogNode dialogNode)
        {
            var dialogInstance = GetAvailableDialog();

            //Initialize and update UI
            var dialogWriter = new DialogWriter(dialogNode, dialogInstance);
            dialogWriter.Initialize();
            _activeDialogWriters.Add(dialogWriter);
            return dialogWriter;
        }

        /// <summary>
        /// When user has clicked continue, either finish displaying text
        /// or mark textWriter as complete depending on states
        /// </summary>
        public void ContinueClicked()
        {
            //Iterate over list in reverse to allow us to remove elements without breaking indices 
            for (var i = _activeDialogWriters.Count - 1; i >= 0; i--)
            {
                if (_activeDialogWriters[i].IsTimedDialog())
                    return;

                if (_activeDialogWriters[i].IsTextFinished())
                {
                    if (_activeDialogWriters[i].IsOptionDialog())
                        return;

                    _activeDialogWriters[i].MarkAsFinished();
                    ReturnDialogToPool(_activeDialogWriters[i].GetDialogComponent());
                    _activeDialogWriters.RemoveAt(i);
                }
                else
                {
                    _activeDialogWriters[i].SkipTextAnimation();
                }
            }
        }

        /// <summary>
        /// Gets next available dialog from the pool, marks as enabled to prevent other dialogs from using the same
        /// instance.
        /// </summary>
        /// <returns>DialogComponents instance</returns>
        private DialogComponents GetAvailableDialog()
        {
            var dialogInstance = _dialogInstancePool.FirstOrDefault(x => !x.IsEnabled);
            Assert.IsNotNull(dialogInstance,
                $"{nameof(DialogManager)}: No dialogInstances were available in the pool, Increase the amount of 'maximumNumberOfDialog' available.");

            //Mark as enabled, prevent other dialogs from grabbing our instance
            dialogInstance.IsEnabled = true;
            return dialogInstance;
        }

        /// <summary>
        /// Creates a new instance of the dialog, GETS needed components
        /// Adds the instance and component references to the pool 
        /// </summary>
        private void PoolNewDialogInstance()
        {
            //Create and get
            var dialogGameObject = Instantiate(dialogPrefab, _dialogPanel.transform);
            var dialogComponents = dialogGameObject.GetComponent<DialogComponents>();

            //Add dialog options
            dialogComponents.optionInstances = new List<OptionComponents>();
            CreateDialogOptions(dialogComponents);

            //Move dialog off screen and disable it
            dialogComponents.rectTransform.anchoredPosition = new Vector2(PoolPositionX, PoolPositionY);
            dialogGameObject.SetActive(false);

            //Add to pool
            _dialogInstancePool.Add(dialogComponents);
        }

        /// <summary>
        /// Creates 'n' amount of options on dialog instances
        /// </summary>
        /// <param name="dialogComponents"></param>
        private void CreateDialogOptions(DialogComponents dialogComponents)
        {
            for (var i = 0; i < maximumNumberOfDialogOptions; i++)
            {
                //Create
                var dialogOptionGO = Instantiate(dialogOptionPrefab, dialogComponents.optionsPanel.transform);
                var optionComponents = dialogOptionGO.GetComponent<OptionComponents>();

                //Ref
                dialogOptionGO.SetActive(false);
                dialogComponents.optionInstances.Add(optionComponents);
            }
        }

        /// <summary>
        /// Clears dialogComponents sizing and position, returns object to the pool
        /// Disables the object
        /// </summary>
        /// <param name="dialogComponents"></param>
        private void ReturnDialogToPool(DialogComponents dialogComponents)
        {
            //Pool position
            dialogComponents.rectTransform.sizeDelta = new Vector2(defaultWidth, defaultHeight);
            dialogComponents.rectTransform.anchoredPosition = new Vector2(PoolPositionX, PoolPositionY);

            //Clear text
            dialogComponents.dialogTMPText.text = string.Empty;
            dialogComponents.characterNameTMPText.text = string.Empty;

            //Clear options
            foreach (var option in dialogComponents.optionInstances)
            {
                option.optionTMPText.text = string.Empty;
                option.gameObject.SetActive(false);
            }

            //Disable
            dialogComponents.optionsPanel.SetActive(false);
            dialogComponents.gameObject.SetActive(false);
            dialogComponents.IsEnabled = false;
        }
    }
}