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

            var rectFoldout = new Rect(position.min.x, position.min.y, position.size.x,
                EditorGUIUtility.singleLineHeight * 2);
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

                //DefaultValue
                var rectDefaultValue = new Rect(
                    position.min.x,
                    position.min.y + lines++ * EditorGUIUtility.singleLineHeight,
                    position.size.x,
                    EditorGUIUtility.singleLineHeight
                );
                lines += 0.1f;
                
                var rectValue = new Rect(
                    position.min.x,
                    position.min.y + lines++ * EditorGUIUtility.singleLineHeight,
                    position.size.x,
                    EditorGUIUtility.singleLineHeight
                );
                
                var defaultValueLabel = new GUIContent("Default Value");
                var valueLabel = new GUIContent("Value");
                switch (dataType.enumValueIndex)
                {
                    case (int)DataType.String:
                        EditorGUI.PropertyField(rectDefaultValue, property.FindPropertyRelative("defaultStringValue"),
                            defaultValueLabel);
                        // EditorGUI.PropertyField(rectValue, property.FindPropertyRelative("stringValue"), valueLabel);
                        ClearValues(property, "defaultStringValue", "stringValue");
                        break;
                    case (int)DataType.Integer:
                        EditorGUI.PropertyField(rectDefaultValue, property.FindPropertyRelative("defaultIntValue"),
                            defaultValueLabel);
                        // EditorGUI.PropertyField(rectValue, property.FindPropertyRelative("intValue"), valueLabel);
                        ClearValues(property, "defaultIntValue", "intValue");
                        break;
                    case (int)DataType.Float:
                        EditorGUI.PropertyField(rectDefaultValue, property.FindPropertyRelative("defaultFloatValue"),
                            defaultValueLabel);
                        // EditorGUI.PropertyField(rectValue, property.FindPropertyRelative("floatValue"), valueLabel);
                        ClearValues(property, "defaultFloatValue", "floatValue");
                        break;
                    case (int)DataType.Boolean:
                        EditorGUI.PropertyField(rectDefaultValue, property.FindPropertyRelative("defaultBooleanValue"),
                            defaultValueLabel);
                        // EditorGUI.PropertyField(rectValue, property.FindPropertyRelative("booleanValue"), valueLabel);
                        ClearValues(property, "defaultBooleanValue", "booleanValue");
                        break;
                    case (int)DataType.Vector3:
                        EditorGUI.PropertyField(rectDefaultValue, property.FindPropertyRelative("defaultVector3Value"),
                            defaultValueLabel);
                        // EditorGUI.PropertyField(rectValue, property.FindPropertyRelative("vector3Value"), valueLabel);
                        ClearValues(property, "defaultVector3Value", "vector3Value");
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

        private void ClearValues(SerializedProperty property, string defaultValue, string value)
        {
            if (defaultValue != "defaultStringValue" && value != "stringValue")
            {
                property.FindPropertyRelative("defaultStringValue").stringValue = string.Empty;
                property.FindPropertyRelative("stringValue").stringValue = string.Empty;
            }

            if (defaultValue != "defaultIntValue" && value != "intValue")
            {
                property.FindPropertyRelative("defaultIntValue").intValue = 0;
                property.FindPropertyRelative("intValue").intValue = 0;
            }

            if (defaultValue != "defaultFloatValue" && value != "floatValue")
            {
                property.FindPropertyRelative("defaultFloatValue").floatValue = 0f;
                property.FindPropertyRelative("floatValue").floatValue = 0f;
            }
            
            if (defaultValue != "defaultBooleanValue" && value != "booleanValue")
            {
                property.FindPropertyRelative("defaultBooleanValue").boolValue = false;
                property.FindPropertyRelative("booleanValue").boolValue = false;
            }
            
            if (defaultValue != "defaultVector3Value" && value != "vector3Value")
            {
                property.FindPropertyRelative("defaultVector3Value").vector3Value = Vector3.zero;
                property.FindPropertyRelative("vector3Value").vector3Value = Vector3.zero;
            }
        }
    }
}