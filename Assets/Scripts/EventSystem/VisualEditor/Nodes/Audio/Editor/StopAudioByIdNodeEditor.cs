using XNodeEditor;

namespace EventSystem.VisualEditor.Nodes.Audio.Editor
{
    [CustomNodeEditor(typeof(StopAudioByIdNode))]
    public class StopAudioByIdNodeEditor : NodeEditor
    {
        /// Node Width
        public override int GetWidth()
        {
            return 350;
        }
    }
}