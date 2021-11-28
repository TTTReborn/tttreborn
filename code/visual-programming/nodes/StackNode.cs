using System;
using System.Text.Json;

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

        public virtual void LoadFromJsonData(Dictionary<string, object> jsonData)
        {
            jsonData.TryGetValue("NextNodes", out object nextNodes);

            if (nextNodes != null)
            {
                JsonElement nextNodesElement = (JsonElement) nextNodes;
                List<Dictionary<string, object>> nextNodesList = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(nextNodesElement.GetRawText());

                for (int i = 0; i < nextNodesList.Count; i++)
                {
                    StackNode stackNode = GetStackNodeFromJsonData<StackNode>(nextNodesList[i]);

                    if (stackNode == null)
                    {
                        continue;
                    }

                    NextNodes.Add(stackNode);
                }
            }
        }

        public static T GetStackNodeFromJsonData<T>(Dictionary<string, object> jsonData) where T : StackNode
        {
            jsonData.TryGetValue("Name", out object name);

            if (name == null)
            {
                return null;
            }

            Type type = Utils.GetTypeByLibraryName<T>(name.ToString());

            if (type == null)
            {
                return null;
            }

            T stackNode = Utils.GetObjectByType<T>(type);

            if (stackNode == null)
            {
                return null;
            }

            stackNode.LoadFromJsonData(jsonData);

            return stackNode;
        }
    }
}
