using EventSystem.Models;
using XNode;

//Namespace is used as path in option menu
// ReSharper disable once CheckNamespace
namespace EventSystem
{
    public class EndNode : BaseNode
    {
        [Input] public Empty entry;
    }
}