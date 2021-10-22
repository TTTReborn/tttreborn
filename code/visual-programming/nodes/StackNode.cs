using System.Collections.Generic;

namespace TTTReborn.VisualProgramming
{
    public partial class StackNode
    {
        public List<StackNode> NextNodes = new();

        public StackNode()
        {

        }

        public virtual void Reset()
        {
            NextNodes.Clear();
        }

        public virtual object[] Build(params object[] input)
        {
            return input;
        }

        public virtual void Evaluate(params object[] input)
        {
            // TODO handle this in/by the stack at a later time
            // NodeStack.Instance.AddNode(this);

            // for (int i = 0; i < NextNodes.Count; i++)
            // {
            //     NextNodes[i].Evaluate(input.Length > i ? input[i] : null);
            // }
        }
    }
}
