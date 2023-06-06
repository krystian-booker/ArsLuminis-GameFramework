using UnityEngine;
using XNode;
using XNodeEditor;

[CustomNodeEditor(typeof(Node))]
public class StartNodeEditor : NodeEditor 
{
    public override GUIStyle GetBodyStyle()
    {
        GUIStyle bodyStyle = new GUIStyle(base.GetBodyStyle());
        bodyStyle.normal.textColor = Color.black; // Change to the color you want for the text
        return bodyStyle;
    }
}