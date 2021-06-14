using System.Collections;
using System.Linq;
using EventSystem.Events;
using EventSystem.Models;
using EventSystem.VisualEditor.Graphs;
using UnityEngine;
using XNode;

namespace EventSystem
{
    public class EventTimelineParser : MonoBehaviour
    {
        [Tooltip("Only required if the event sequence will be altering the camera state.")]
        public Camera primaryCamera;

        private const string FieldName = "exit";

        private void Start()
        {
            StartTimeLine();
        }

        private void StartTimeLine()
        {
            var essg = gameObject.GetComponent<EventSequenceSceneGraph>();
            var startNode = essg.graph.nodes.FirstOrDefault(x => x.GetType() == typeof(StartNode));
            if (startNode == null)
            {
                Debug.LogError($"{nameof(EventTimelineParser)}: Missing {nameof(StartNode)} from graph");
            }

            StartCoroutine(ParseNode(startNode));
        }

        private IEnumerator ParseNode(Node node)
        {
            if (node == null)
                yield return null;

            //perform action for node type
            var currentNodeType = node.GetType();
            if (currentNodeType == typeof(CameraNode))
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
            else if (currentNodeType == typeof(StartNode) || currentNodeType == typeof(EndNode))
            {
                NextNode(node);
            }
        }

        private void NextNode(Node node)
        {
            var nodePorts = node.Ports.FirstOrDefault(portNode => portNode.fieldName == FieldName)?.GetConnections();
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