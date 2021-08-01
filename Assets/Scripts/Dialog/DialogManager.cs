using System;
using System.Collections;
using System.Collections.Generic;
using EventSystem.Models;
using EventSystem.VisualEditor.Nodes.Actions;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace Dialog
{
    public class DialogManager : MonoBehaviour
    {
        public GameObject canvas;
        public GameObject dialogPrefab;
        private List<TextWriter> _textWriters;

        private void Awake()
        {
            Assert.IsNotNull(canvas, $"{nameof(DialogManager)}: canvas is required.");
            Assert.IsNotNull(dialogPrefab, $"{nameof(DialogManager)}: dialogPrefab is required.");
            _textWriters = new List<TextWriter>();
        }

        private void Update()
        {
            foreach (var textWriter in _textWriters)
            {
                var destroyInstance = textWriter.Update();
                if (destroyInstance)
                {
                    _textWriters.Remove(textWriter);
                }
            }
        }

        public TextWriter AddWriter(TMP_Text tmpText, string textToWrite, float timePerCharacter, bool invisibleCharacters)
        {
            var textWriter = new TextWriter(tmpText, textToWrite, timePerCharacter, invisibleCharacters);
            _textWriters.Add(textWriter);
            return textWriter;
        }
    }

    public class TextWriter
    {
        private TMP_Text _tmpText;

        private float _timer;
        private int _characterIndex;
        private readonly string _textToWrite;
        private readonly float _timePerCharacter;
        private readonly bool _invisibleCharacters;

        public TextWriter(TMP_Text tmpText, string textToWrite, float timePerCharacter, bool invisibleCharacters)
        {
            _tmpText = tmpText;
            _textToWrite = textToWrite;
            _timePerCharacter = timePerCharacter;
            _invisibleCharacters = invisibleCharacters;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>True on complete</returns>
        public bool Update()
        {
            _timer -= Time.deltaTime;
            while (_timer <= 0f)
            {
                //Display next character
                _timer += _timePerCharacter;
                _characterIndex++;
                var text = _textToWrite.Substring(0, _characterIndex);
                if (_invisibleCharacters)
                {
                    text += $"<alpha=#00>{_textToWrite.Substring(_characterIndex)}";
                }

                _tmpText.text = text;

                if (_characterIndex < _textToWrite.Length) continue;
                _tmpText = null;
                return true;
            }
            return false;
        }

        public bool IsActive()
        {
            return _characterIndex < _textToWrite.Length;
        }

        public void WriteAllAndDestroy()
        {
            _characterIndex = _textToWrite.Length;
            _tmpText.text = _textToWrite;
        }
    }
}