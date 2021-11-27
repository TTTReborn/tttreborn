using System.Collections.Generic;

namespace TTTReborn.VisualProgramming
{
    public abstract class StackNode
    {
        public string Name { get; set; }
        public List<StackNode> NextNodes { get; set; } = new();

        public StackNode()
        {
            Name = Utils.GetTypeName(GetType());
        }

        public virtual void Reset()
        {
            NextNodes.Clear();
        }

        public abstract object[] Test(params object[] input);

        public virtual void Evaluate(params object[] input)
        {
            // TODO handle this in/by the stack at a later time

            // for (int i = 0; i < NextNodes.Count; i++)
            // {
            //     NextNodes[i].Evaluate(input.Length > i ? input[i] : null);
            // }
        }

        public virtual Dictionary<string, object> GetJsonData(List<StackNode> proceedNodes = null)
        {
            if (proceedNodes != null)
            {
                proceedNodes.Add(this);
            }

            List<Dictionary<string, object>> nextNodesJsonList = new();

            foreach (StackNode stackNode in NextNodes)
            {
                nextNodesJsonList.Add(stackNode.GetJsonData(proceedNodes));
            }

            return new Dictionary<string, object>()
            {
                ["Name"] = Name,
                ["NextNodes"] = nextNodesJsonList,
            };
        }
    }
}
