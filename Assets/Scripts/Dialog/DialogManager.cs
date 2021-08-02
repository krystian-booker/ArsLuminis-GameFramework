using System.Collections.Generic;
using System.Linq;
using Dialog.Models;
using EventSystem.VisualEditor.Nodes.Actions;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;

namespace Dialog
{
    public class DialogManager : MonoBehaviour
    {
        [SerializeField, Tooltip("Used to instantiate dialog instances when needed")]
        private GameObject dialogPrefab;

        [SerializeField, Tooltip("Used to add options to dialog instances")]
        private GameObject dialogOptionPrefab;

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
        /// </summary>
        private void Update()
        {
            foreach (var dialogWriter in _activeDialogWriters)
            {
                dialogWriter.Update();
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

        #region PlayerInput

        /// <summary>
        /// When user has clicked continue, either finish displaying text
        /// or mark textWriter as complete depending on states
        /// </summary>
        public void ContinueClicked()
        {
            for (var i = 0; i < _activeDialogWriters.Count; i++)
            {
                var dialogWriter = _activeDialogWriters[i];
                if (dialogWriter.IsTextFinished())
                {
                    dialogWriter.MarkAsFinished();
                    ReturnDialogToPool(dialogWriter.GetDialogComponent());
                    _activeDialogWriters.RemoveAt(i);
                }
                else
                {
                    dialogWriter.SkipTextAnimation();
                }
            }
        }

        #endregion

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

            //Move dialog off screen and disable it
            dialogComponents.rectTransform.anchoredPosition = new Vector2(PoolPositionX, PoolPositionY);
            dialogGameObject.SetActive(false);

            //Add to pool
            _dialogInstancePool.Add(dialogComponents);
        }

        /// <summary>
        /// Clears dialogComponents sizing and position, returns object to the pool
        /// Disables the object
        /// </summary>
        /// <param name="dialogComponents"></param>
        private void ReturnDialogToPool(DialogComponents dialogComponents)
        {
            dialogComponents.rectTransform.sizeDelta = new Vector2(defaultWidth, defaultHeight);
            dialogComponents.rectTransform.anchoredPosition = new Vector2(PoolPositionX, PoolPositionY);
            dialogComponents.dialogGameObject.SetActive(false);
            dialogComponents.IsEnabled = false;
        }
    }
}