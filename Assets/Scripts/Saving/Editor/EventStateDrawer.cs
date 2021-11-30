using Saving.Models;
using UnityEditor;
using UnityEngine;

namespace Saving.Editor
{
    [CustomPropertyDrawer(typeof(EventState))]
    public class EventStateDrawer : PropertyDrawer
    {
        private int _intValue;

        // Draw the property inside the given rect
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var rectFoldout = new Rect(position.min.x, position.min.y, position.size.x, EditorGUIUtility.singleLineHeight * 2);
            property.isExpanded = EditorGUI.Foldout(rectFoldout, property.isExpanded, label);

            var lines = 1f;
            var lineBreak = EditorGUIUtility.singleLineHeight;
            if (property.isExpanded)
            {
                lines += 0.5f;

                //ID
                var rectId = new Rect(
                    position.min.x,
                    position.min.y + lines++ * EditorGUIUtility.singleLineHeight,
                    position.size.x,
                    EditorGUIUtility.singleLineHeight
                );
                EditorGUI.PropertyField(rectId, property.FindPropertyRelative("id"));
                lines += 0.1f;

                //Description
                var rectDescription = new Rect(
                    position.min.x,
                    position.min.y + lines++ * EditorGUIUtility.singleLineHeight,
                    position.size.x,
                    EditorGUIUtility.singleLineHeight * 2
                );
                EditorGUI.PropertyField(rectDescription, property.FindPropertyRelative("description"));
                lines += 1.1f;

                //DataType
                var dataType = property.FindPropertyRelative("dataType");
                var rectDataType = new Rect(
                    position.min.x,
                    position.min.y + lines++ * EditorGUIUtility.singleLineHeight,
                    position.size.x,
                    EditorGUIUtility.singleLineHeight
                );
                EditorGUI.PropertyField(rectDataType, dataType);
                lines += 0.1f;

                //Value
                var rectDefaultValue = new Rect(
                    position.min.x,
                    position.min.y + lines++ * EditorGUIUtility.singleLineHeight,
                    position.size.x,
                    EditorGUIUtility.singleLineHeight
                );
                lines += 0.1f;

                var valueLabel = new GUIContent("Value");
                switch (dataType.enumValueIndex)
                {
                    case (int) DataType.String:
                        EditorGUI.PropertyField(rectDefaultValue, property.FindPropertyRelative("stringValue"), valueLabel);
                        ClearValues(property, "stringValue");
                        break;
                    case (int) DataType.Integer:
                        EditorGUI.PropertyField(rectDefaultValue, property.FindPropertyRelative("intValue"), valueLabel);
                        ClearValues(property, "intValue");
                        break;
                    case (int) DataType.Float:
                        EditorGUI.PropertyField(rectDefaultValue, property.FindPropertyRelative("floatValue"), valueLabel);
                        ClearValues(property, "floatValue");
                        break;
                    case (int) DataType.Boolean:
                        EditorGUI.PropertyField(rectDefaultValue, property.FindPropertyRelative("booleanValue"), valueLabel);
                        ClearValues(property, "booleanValue");
                        break;
                    case (int) DataType.Vector3:
                        EditorGUI.PropertyField(rectDefaultValue, property.FindPropertyRelative("vector3Value"), valueLabel);
                        ClearValues(property, "vector3Value");
                        break;
                }
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var totalLines = 2f;
            if (property.isExpanded)
            {
                totalLines += 4.2f;
            }

            return EditorGUIUtility.singleLineHeight * totalLines +
                   EditorGUIUtility.standardVerticalSpacing * (totalLines - 1);
        }

        private static void ClearValues(SerializedProperty property, string value)
        {
            if (value != "stringValue")
            {
                property.FindPropertyRelative("stringValue").stringValue = string.Empty;
            }

            if (value != "intValue")
            {
                property.FindPropertyRelative("intValue").intValue = 0;
            }

            if (value != "floatValue")
            {
                property.FindPropertyRelative("floatValue").floatValue = 0f;
            }

            if (value != "booleanValue")
            {
                property.FindPropertyRelative("booleanValue").boolValue = false;
            }

            if (value != "vector3Value")
            {
                property.FindPropertyRelative("vector3Value").vector3Value = Vector3.zero;
            }
        }
    }
}