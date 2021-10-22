using System.Collections.Generic;

namespace TTTReborn.VisualProgramming
{
    public partial class NodeStack
    {
        private List<StackNode> _stackNodes = new();

        public NodeStack()
        {

        }

        public void Reset()
        {
            foreach (StackNode stackNode in _stackNodes)
            {
                stackNode.Reset();
            }

            _stackNodes.Clear();
        }

        public void AddNode(StackNode stackNode)
        {
            _stackNodes.Add(stackNode);
        }
    }
}
