using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EventSystem.Events;
using EventSystem.Models;
using EventSystem.Models.interfaces;
using EventSystem.VisualEditor.Graphs;
using EventSystem.VisualEditor.Nodes.Actions;
using EventSystem.VisualEditor.Nodes.Flow;
using EventSystem.VisualEditor.Nodes.State;
using Saving;
using UnityEngine;
using UnityEngine.Assertions;
using XNode;

namespace EventSystem
{
    public class EventTimelineParser : MonoBehaviour
    {
        public EventSequenceState eventSequenceState = EventSequenceState.Awaiting;
        private EventSequenceSceneGraph _eventSequenceSceneGraph;

        //TODO: Finish this functionality. Replace with in game UI
        public bool debugger;
        [HideInInspector] public bool step;
        
        private List<IPauseEventExecution> _pauseEventExecutions = new List<IPauseEventExecution>();
        
        /// <summary>
        /// Start parsing the xNode timeLine.
        /// Currently this is called from Start() will be moved over to events 
        /// </summary>
        public IEnumerator StartEventSequence(EventSequenceSceneGraph eventSequenceSceneGraph)
        {
            _eventSequenceSceneGraph = eventSequenceSceneGraph;
            var startNode = _eventSequenceSceneGraph.graph.nodes.Where(x => x.GetType() == typeof(StartNode)).ToList();
            if (!startNode.Any())
            {
                Debug.LogError($"{nameof(EventTimelineParser)}: Missing {nameof(StartNode)} from graph");
                yield return null;
            }

            if (startNode.Count > 1)
            {
                Debug.LogError(
                    $"{nameof(EventTimelineParser)}: There cannot be more than one {nameof(StartNode)} in your graph");
                yield return null;
            }

            eventSequenceState = EventSequenceState.Started;
            yield return ParseNode(startNode.FirstOrDefault());
        }

        /// <summary>
        /// All nodes are the type of BaseNode, from there they are extended as needed.
        /// Here the specific type of the node is checked and executed as needed.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private IEnumerator ParseNode(Node node)
        {
            if (node == null)
                yield return null;

            //perform action for node type
            var currentNodeType = node.GetType();
            if (currentNodeType == typeof(StartNode) || node is BaseNodeExtended {skip: true})
            {
                yield return NextNode(node);
            }
            else if (currentNodeType == typeof(CameraNode))
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
            else if (currentNodeType == typeof(AnimationNode))
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
            else if (currentNodeType == typeof(StateNode))
            {
                NextStateNodeExecution(node);
            }
            else if (currentNodeType == typeof(AutoSaveNode))
            {
                yield return AutoSaveNodeExecution(node);
            }
            else if (currentNodeType == typeof(InputActionMapNode))
            {
                yield return InputActionMapNode(node);
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
        }

        /// <summary>
        /// Runs the camera execution based on the camera node parameters 
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private IEnumerator CameraNodeExecution(Node node)
        {
            var cameraExecution = new CameraExecution(GameManager.Instance.mainCamera);
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
            
            var dialogWriter = GameManager.Instance.dialogManager.NewDialog(dialogNode);
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

            var eventStateValues =
                GameManager.Instance.gameState.states.FirstOrDefault(x => x.name == updateStateNode.eventState);

            if (eventStateValues != null)
                eventStateValues.complete = updateStateNode.stateComplete;

            yield return NextNode(node);
        }

        /// <summary>
        /// From the state defined in the node, get the current saves state value.
        /// Select next node from the state value retrieved.
        /// </summary>
        /// <param name="node"></param>
        private void NextStateNodeExecution(Node node)
        {
            var stateNode = node as StateNode;
            if (stateNode == null)
                return;

            var eventState =
                GameManager.Instance.gameState.states.FirstOrDefault(eventStateValue =>
                    eventStateValue.name == stateNode.eventState);
            if (eventState == null)
            {
                Debug.LogError(
                    $"{nameof(EventTimelineParser)}: Unable to find the state '{stateNode.eventState}' in gameManager states");
                return;
            }

            //Port selection
            var portName = eventState.complete ? "stateTrue" : "stateFalse";

            //Execute port based on state
            var nodePort = node.Ports.FirstOrDefault(portNode => portNode.fieldName == portName);
            if (nodePort == null)
            {
                Debug.LogError($"{nameof(EventTimelineParser)}: Unable to find node port for {portName}");
                return;
            }

            var nodePorts = nodePort.GetConnections();
            ExecuteNodePorts(nodePorts);
        }

        /// <summary>
        /// Updates the auto save file with the active state system
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private IEnumerator AutoSaveNodeExecution(Node node)
        {
            SaveManager.SaveGame(GameManager.Instance.gameState, true);
            yield return NextNode(node);
        }

        /// <summary>
        /// Updates the active input action map
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private IEnumerator InputActionMapNode(Node node)
        {
            var inputActionMapNode = node as InputActionMapNode;
            GameManager.Instance.inputManager.ChangeActionMap(inputActionMapNode.actionMap);
            yield return NextNode(node);
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
                step = false;
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
            return step;
        }
    }
}