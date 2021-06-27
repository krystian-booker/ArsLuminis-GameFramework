using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Dialog;
using EventSystem.Events;
using EventSystem.Models;
using EventSystem.VisualEditor.Graphs;
using EventSystem.VisualEditor.Nodes.Actions;
using EventSystem.VisualEditor.Nodes.Flow;
using UnityEngine;
using XNode;

namespace EventSystem
{
    public class EventTimelineParser : MonoBehaviour
    {
        [Tooltip("Only required if the event sequence will be altering the camera state.")]
        public Camera primaryCamera;

        [Tooltip("Ability to get components from the game manager, ie: localization, dialog")]
        public GameObject gameManagerGameObject;

        private GameManager _gameManager;
        private DialogManager _dialogManager;

        private void Awake()
        {
            if (gameManagerGameObject == null)
            {
                Debug.LogError(
                    $"{nameof(EventTimelineParser)}: Missing reference to {nameof(gameManagerGameObject)} game object");
            }
        }

        private void Start()
        {
            _dialogManager = gameManagerGameObject.GetComponent<DialogManager>();
            _gameManager = gameManagerGameObject.GetComponent<GameManager>();

            //TODO: Remove, debug only.
            StartTimeLine();
        }

        /// <summary>
        /// Start parsing the xNode timeLine.
        /// Currently this is called from Start() will be moved over to events 
        /// </summary>
        private void StartTimeLine()
        {
            var essg = gameObject.GetComponent<EventSequenceSceneGraph>();
            var startNode = essg.graph.nodes.Where(x => x.GetType() == typeof(StartNode)).ToList();
            if (!startNode.Any())
            {
                Debug.LogError($"{nameof(EventTimelineParser)}: Missing {nameof(StartNode)} from graph");
                return;
            }

            if (startNode.Count > 1)
            {
                Debug.LogError(
                    $"{nameof(EventTimelineParser)}: There cannot be more than one {nameof(StartNode)} in your graph");
                return;
            }

            StartCoroutine(ParseNode(startNode.FirstOrDefault()));
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
            if (currentNodeType == typeof(StartNode) || currentNodeType == typeof(EndNode) ||
                node is BaseNodeExtended {skip: true})
            {
                NextNode(node);
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
            else if (currentNodeType == typeof(StateNode))
            {
                NextStateNodeExecution(node);
            }
            else if (currentNodeType == typeof(StartNode) || currentNodeType == typeof(EndNode))
            {
                NextNode(node);
            }
            else
            {
                Debug.LogError($"{nameof(EventTimelineParser)}: Unknown node type {currentNodeType}");
            }
        }

        /// <summary>
        /// Runs the camera execution based on the camera node parameters 
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private IEnumerator CameraNodeExecution(Node node)
        {
            var cameraExecution = new CameraExecution(primaryCamera);
            StartCoroutine(cameraExecution.Execute(node));
            yield return new WaitUntil(cameraExecution.IsFinished);
            NextNode(node);
        }

        /// <summary>
        /// Runs the objectMovement execution based on the objectMovement node parameters 
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private IEnumerator ObjectMovementNodeExecution(Node node)
        {
            var objectMovementExecution = new ObjectMovementExecution();
            StartCoroutine(objectMovementExecution.Execute(node));
            yield return new WaitUntil(objectMovementExecution.IsFinished);
            NextNode(node);
        }

        /// <summary>
        /// Runs the characterMovement execution based on the characterMovement node parameters
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private IEnumerator CharacterMovementNodeExecution(Node node)
        {
            var characterMovementExecution = new CharacterMovementExecution();
            StartCoroutine(characterMovementExecution.Execute(node));
            yield return new WaitUntil(characterMovementExecution.IsFinished);
            NextNode(node);
        }

        /// <summary>
        /// Runs the animation execution based on the animation node parameters
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private IEnumerator AnimationNodeExecution(Node node)
        {
            var animationExecution = new AnimationExecution();
            StartCoroutine(animationExecution.Execute(node));
            yield return new WaitUntil(animationExecution.IsFinished);
            NextNode(node);
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
            NextNode(node);
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
                _gameManager.gameState.states.FirstOrDefault(eventStateValue =>
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
        /// Executes the configuration of the current dialogNode
        /// If this dialog has options, the node attached to the selected option will be ran
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private IEnumerator DialogNodeExecution(Node node)
        {
            var dialogNode = node as DialogNode;
            _dialogManager.StartDialog(dialogNode);
            yield return new WaitUntil(_dialogManager.IsContinueClicked);
            if (dialogNode.options.Count > 0)
            {
                var selectedOptionIndex = _dialogManager.GetSelectedOption();
                var dynamicPorts = dialogNode.DynamicPorts.ToList();
                var optionNode = dynamicPorts[selectedOptionIndex];
                var selectedNodes = optionNode.GetConnections();
                ExecuteNodePorts(selectedNodes);
            }
            else
            {
                NextNode(node);
            }
        }

        /// <summary>
        /// Find a node's exit port based on our constant name "exit" 
        /// </summary>
        /// <param name="node">current executing node</param>
        private void NextNode(Node node)
        {
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
                var baseNode = nodePort.node as BaseNode;
                if (baseNode == null || baseNode is {started: true})
                    return;

                baseNode.started = true;
                StartCoroutine(ParseNode(nodePort.node));
            }
        }
    }
}