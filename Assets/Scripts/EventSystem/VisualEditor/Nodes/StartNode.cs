using EventSystem.Models;
using XNode;

//Namespace is used as path in option menu
// ReSharper disable once CheckNamespace
namespace EventSystem
{
    public class StartNode : BaseNode
    {
        [Output] public Empty exit;
    }
}