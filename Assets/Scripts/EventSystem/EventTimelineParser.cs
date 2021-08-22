using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EventSystem.Events;
using EventSystem.Models;
using EventSystem.Models.interfaces;
using EventSystem.VisualEditor.Graphs;
using EventSystem.VisualEditor.Nodes;
using EventSystem.VisualEditor.Nodes.Animation;
using EventSystem.VisualEditor.Nodes.Audio;
using EventSystem.VisualEditor.Nodes.Camera;
using EventSystem.VisualEditor.Nodes.Dialog;
using EventSystem.VisualEditor.Nodes.Flow;
using EventSystem.VisualEditor.Nodes.Locomotion;
using EventSystem.VisualEditor.Nodes.State;
using Saving;
using Saving.Models;
using Tools;
using UnityEngine;
using UnityEngine.Assertions;
using XNode;

namespace EventSystem
{
    public class EventTimelineParser : MonoBehaviour
    {
        public EventSequenceState eventSequenceState = EventSequenceState.Awaiting;

        #region Debugger

        public bool debugger;
        public bool debugStep;
        public string description;

        #endregion

        #region Sequence

        private EventSequenceSceneGraph _eventSequenceSceneGraph;
        private List<IPauseEventExecution> _pauseEventExecutions = new List<IPauseEventExecution>();

        #endregion

        /// <summary>
        /// Start parsing the xNode timeLine.
        /// Currently this is called from Start() will be moved over to events 
        /// </summary>
        public IEnumerator StartEventSequence(EventSequenceSceneGraph eventSequenceSceneGraph)
        {
            var startNode = eventSequenceSceneGraph.graph.nodes.OfType<StartNode>().ToList();
            description = eventSequenceSceneGraph.description;

            //Assert
            Assert.IsTrue(startNode.Any(), $"{nameof(EventTimelineParser)}: Missing {nameof(StartNode)} from graph");
            Assert.IsFalse(startNode.Count > 1, $"{nameof(EventTimelineParser)}: There cannot be more than one {nameof(StartNode)} in your graph");

            //Start Sequence
            eventSequenceState = EventSequenceState.Running;
            yield return ParseNode(startNode.FirstOrDefault());
        }

        #region Timeline parser

        /// <summary>
        /// All nodes are the type of BaseNode, from there they are extended as needed.
        /// Here the specific type of the node is checked and executed as needed.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private IEnumerator ParseNode(Node node)
        {
            if (node == null)
                yield break;

            //perform action for node type
            var currentNodeType = node.GetType();
            if (currentNodeType == typeof(StartNode) || node is SkippableBaseNode { skip: true })
            {
                yield return NextNode(node);
            }
            else if (currentNodeType == typeof(ChangeVirtualCameraNode))
            {
                yield return CameraNodeExecution(node);
            }
            else if (currentNodeType == typeof(ObjectMovementNode))
            {
                yield return ObjectMovementNodeExecution(node);
            }
            else if (currentNodeType == typeof(CharacterMovementNode))
            {
                yield return CharacterMovementNodeExecution(node);
            }
            else if (currentNodeType == typeof(PlayAnimationNode))
            {
                yield return AnimationNodeExecution(node);
            }
            else if (currentNodeType == typeof(WaitNode))
            {
                yield return WaitNodeExecution(node);
            }
            else if (currentNodeType == typeof(DialogNode))
            {
                yield return DialogNodeExecution(node);
            }
            else if (currentNodeType == typeof(UpdateStateNode))
            {
                yield return UpdateStateNodeExecution(node);
            }
            else if (currentNodeType == typeof(StateBranchNode))
            {
                NextStateNodeExecution(node);
            }
            else if (currentNodeType == typeof(AutoSaveNode))
            {
                yield return AutoSaveNodeExecution(node);
            }
            else if (currentNodeType == typeof(ChangeInputActionMapNode))
            {
                yield return ChangeInputActionMapNode(node);
            }
            else if (currentNodeType == typeof(StartAudioNode))
            {
                yield return AudioNode(node);
            }
            else if (currentNodeType == typeof(StopAudioByIdNode))
            {
                yield return StopAudioById(node);
            }
            else if (currentNodeType == typeof(EndNode))
            {
                eventSequenceState = EventSequenceState.Ended;
            }
            else
            {
                Debug.LogError($"{nameof(EventTimelineParser)}: Unknown node type {currentNodeType}");
            }
        }

        /// <summary>
        /// Find a node's exit port based on our constant name "exit" 
        /// </summary>
        /// <param name="node">current executing node</param>
        private IEnumerator NextNode(Node node)
        {
            if (debugger)
            {
                yield return new WaitUntil(UserSteppedToNextNode);
                debugStep = false;
            }

            var nodePorts = node.Ports.FirstOrDefault(portNode => portNode.fieldName == "exit")?.GetConnections();
            ExecuteNodePorts(nodePorts);
        }

        /// <summary>
        /// From a list of nodePorts, execute all the linked nodes simultaneously (not threaded)
        /// </summary>
        /// <param name="nodePorts"></param>
        private void ExecuteNodePorts(List<NodePort> nodePorts)
        {
            if (nodePorts == null) return;
            foreach (var nodePort in nodePorts)
            {
                StartCoroutine(ParseNode(nodePort.node));
            }
        }

        private bool UserSteppedToNextNode()
        {
            return debugStep;
        }

        #endregion

        #region Node Executions

        /// <summary>
        /// Return when event sequence finished
        /// </summary>
        /// <returns></returns>
        public bool IsEventSequenceFinished()
        {
            return eventSequenceState == EventSequenceState.Ended;
        }

        /// <summary>
        /// Pause all IPauseEventExecution events
        /// </summary>
        public void PauseEventSequence()
        {
            foreach (var pauseExecution in _pauseEventExecutions)
            {
                pauseExecution.PauseExecution();
            }

            eventSequenceState = EventSequenceState.Paused;
        }

        /// <summary>
        /// Resume all paused IPauseEventExecution
        /// </summary>
        public void ResumeEventSequence()
        {
            foreach (var pauseExecution in _pauseEventExecutions)
            {
                pauseExecution.ResumeExecution();
            }

            eventSequenceState = EventSequenceState.Running;
        }

        /// <summary>
        /// Runs the camera execution based on the camera node parameters 
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private IEnumerator CameraNodeExecution(Node node)
        {
            var cameraExecution = new ChangeVirtualCameraExecution(Systems.MainCamera);
            cameraExecution.Execute(node);
            yield return new WaitUntil(cameraExecution.IsFinished);
            yield return NextNode(node);
        }

        /// <summary>
        /// Runs the objectMovement execution based on the objectMovement node parameters 
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private IEnumerator ObjectMovementNodeExecution(Node node)
        {
            var objectMovementExecution = new ObjectMovementExecution();
            objectMovementExecution.Execute(node);

            _pauseEventExecutions.Add(objectMovementExecution);
            yield return new WaitUntil(objectMovementExecution.IsFinished);
            _pauseEventExecutions.Remove(objectMovementExecution);

            yield return NextNode(node);
        }

        /// <summary>
        /// Runs the characterMovement execution based on the characterMovement node parameters
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private IEnumerator CharacterMovementNodeExecution(Node node)
        {
            var characterMovementExecution = new CharacterMovementExecution();
            characterMovementExecution.Execute(node);

            _pauseEventExecutions.Add(characterMovementExecution);
            yield return new WaitUntil(characterMovementExecution.IsFinished);
            _pauseEventExecutions.Remove(characterMovementExecution);

            yield return NextNode(node);
        }

        /// <summary>
        /// Runs the animation execution based on the animation node parameters
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private IEnumerator AnimationNodeExecution(Node node)
        {
            var animationExecution = new AnimationExecution();
            animationExecution.Execute(node);
            yield return new WaitUntil(animationExecution.IsFinished);
            yield return NextNode(node);
        }

        /// <summary>
        /// Waits n seconds defined in the node
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private IEnumerator WaitNodeExecution(Node node)
        {
            var waitNode = node as WaitNode;
            yield return new WaitForSeconds(waitNode ? waitNode.delayTime : 0);
            yield return NextNode(node);
        }

        /// <summary>
        /// Executes the configuration of the current dialogNode
        /// If this dialog has options, the node attached to the selected option will be ran
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private IEnumerator DialogNodeExecution(Node node)
        {
            var dialogNode = node as DialogNode;
            Assert.IsNotNull(dialogNode);

            var dialogWriter = Systems.DialogManager.NewDialog(dialogNode);
            yield return new WaitUntil(dialogWriter.IsNodeFinished);

            if (dialogNode.options.Count > 0)
            {
                var selectedOptionIndex = dialogWriter.GetSelectedOption();
                var dynamicPorts = dialogNode.DynamicPorts.ToList();
                var optionNode = dynamicPorts[selectedOptionIndex];
                var selectedNodes = optionNode.GetConnections();
                ExecuteNodePorts(selectedNodes);
            }
            else
            {
                yield return NextNode(node);
            }
        }

        /// <summary>
        /// Updates the selected state system
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private IEnumerator UpdateStateNodeExecution(Node node)
        {
            var updateStateNode = node as UpdateStateNode;
            Assert.IsNotNull(updateStateNode);
            Systems.SaveManager.UpdateState(updateStateNode);
            yield return NextNode(node);
        }

        /// <summary>
        /// From the state defined in the node, get the current saves state value.
        /// Select next node from the state value retrieved.
        /// </summary>
        /// <param name="node"></param>
        private void NextStateNodeExecution(Node node)
        {
            var nodePorts = SaveManager.ExecuteStateBranchNode(node);
            ExecuteNodePorts(nodePorts);
        }

        /// <summary>
        /// Updates the auto save file with the active state system
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private IEnumerator AutoSaveNodeExecution(Node node)
        {
            Systems.SaveManager.SaveGame(true);
            yield return NextNode(node);
        }

        /// <summary>
        /// Updates the active input action map
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private IEnumerator ChangeInputActionMapNode(Node node)
        {
            var inputActionMapNode = node as ChangeInputActionMapNode;
            Assert.IsNotNull(inputActionMapNode);
            Systems.InputManager.ChangeActionMap(inputActionMapNode.actionMap);
            yield return NextNode(node);
        }

        /// <summary>
        /// Plays the set audio on the node
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private IEnumerator AudioNode(Node node)
        {
            var audioExecution = new StartAudioExecution();
            audioExecution.Execute(node);
            yield return new WaitUntil(audioExecution.IsFinished);
            yield return NextNode(node);
        }

        /// <summary>
        /// Plays the set audio on the node
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private IEnumerator StopAudioById(Node node)
        {
            var stopAudioById = node as StopAudioByIdNode;
            Assert.IsNotNull(stopAudioById);
            Systems.AudioManager.StopActiveAudioSource(stopAudioById.audioNodeId);
            yield return NextNode(node);
        }

        #endregion
    }
}