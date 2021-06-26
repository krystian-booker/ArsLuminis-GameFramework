using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EventSystem.Events;
using EventSystem.Models;
using EventSystem.VisualEditor.Graphs;
using EventSystem.VisualEditor.Nodes.Actions;
using EventSystem.VisualEditor.Nodes.Flow;
using Managers;
using UnityEngine;
using XNode;

namespace EventSystem
{
    public class EventTimelineParser : MonoBehaviour
    {
        [Tooltip("Only required if the event sequence will be altering the camera state.")]
        public Camera primaryCamera;

        [Tooltip("Ability to get components from the game manager, ie: localization, dialog")]
        public GameObject gameManager;

        private DialogManager _dialogManager;

        private void Awake()
        {
            if (gameManager == null)
            {
                Debug.LogError(
                    $"{nameof(EventTimelineParser)}: Missing reference to {nameof(gameManager)} game object");
            }
        }

        private void Start()
        {
            _dialogManager = gameManager.GetComponent<DialogManager>();

            StartTimeLine();
        }

        private void StartTimeLine()
        {
            var essg = gameObject.GetComponent<EventSequenceSceneGraph>();
            var startNode = essg.graph.nodes.Where(x => x.GetType() == typeof(StartNode)).ToList();
            if (!startNode.Any())
            {
                Debug.LogError($"{nameof(EventTimelineParser)}: Missing {nameof(StartNode)} from graph");
            }

            if (startNode.Count > 1)
            {
                Debug.LogError(
                    $"{nameof(EventTimelineParser)}: There cannot be more than one {nameof(StartNode)} in your graph");
            }

            StartCoroutine(ParseNode(startNode.FirstOrDefault()));
        }

        private IEnumerator ParseNode(Node node)
        {
            if (node == null)
                yield return null;

            //perform action for node type
            var currentNodeType = node.GetType();
            if (currentNodeType != typeof(StartNode) && currentNodeType != typeof(EndNode) &&
                node is BaseNode {skip: true})
            {
                NextNode(node);
            }
            else if (currentNodeType == typeof(CameraNode))
            {
                var cameraExecution = new CameraExecution(primaryCamera);
                StartCoroutine(cameraExecution.Execute(node));
                yield return new WaitUntil(cameraExecution.IsFinished);
                NextNode(node);
            }
            else if (currentNodeType == typeof(ObjectMovementNode))
            {
                var objectMovementExecution = new ObjectMovementExecution();
                StartCoroutine(objectMovementExecution.Execute(node));
                yield return new WaitUntil(objectMovementExecution.IsFinished);
                NextNode(node);
            }
            else if (currentNodeType == typeof(CharacterMovementNode))
            {
                var characterMovementExecution = new CharacterMovementExecution();
                StartCoroutine(characterMovementExecution.Execute(node));
                yield return new WaitUntil(characterMovementExecution.IsFinished);
                NextNode(node);
            }
            else if (currentNodeType == typeof(AnimationNode))
            {
                var animationExecution = new AnimationExecution();
                StartCoroutine(animationExecution.Execute(node));
                yield return new WaitUntil(animationExecution.IsFinished);
                NextNode(node);
            }
            else if (currentNodeType == typeof(WaitNode))
            {
                var waitNode = node as WaitNode;
                yield return new WaitForSeconds(waitNode ? waitNode.delayTime : 0);
                NextNode(node);
            }
            else if (currentNodeType == typeof(DialogNode))
            {
                var dialogNode = node as DialogNode;
                _dialogManager.StartDialog(dialogNode);
                yield return new WaitUntil(_dialogManager.IsContinueClicked);
                NextNode(node);
            }
            else if (currentNodeType == typeof(StateNode))
            {
                NextStateNode(node);
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

        private void NextNode(Node node)
        {
            var nodePorts = node.Ports.FirstOrDefault(portNode => portNode.fieldName == "exit")?.GetConnections();
            ExecuteNodePorts(nodePorts);
        }

        private void NextStateNode(Node node)
        {
            //Get event state
            var currentEventState = false;
            var stateNode = node as StateNode;
            if (stateNode == null)
                return;

            //TODO: Complete functionality, this doesn't currently work.
            var eventState = new EventStates();
            var eventStateProps = typeof(EventStates).GetProperty(stateNode.eventState);
            if (eventStateProps == null)
                return;
            
            currentEventState = (bool) eventStateProps.GetValue(eventState, null);

            //Execute port based on state
            var nodePorts = node.Ports
                .FirstOrDefault(portNode => portNode.fieldName == (currentEventState ? "stateTrue" : "stateFalse"))
                ?.GetConnections();
            ExecuteNodePorts(nodePorts);
        }

        private void ExecuteNodePorts(List<NodePort> nodePorts)
        {
            if (nodePorts == null) return;
            foreach (var portNode in nodePorts)
            {
                var baseNode = portNode.node as BaseNode;
                if (baseNode == null || baseNode is {started: true}) continue;

                baseNode.started = true;
                StartCoroutine(ParseNode(portNode.node));
            }
        }
    }
}