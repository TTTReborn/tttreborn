using System;
using System.Text.Json;
using System.Collections.Generic;

using Sandbox;

namespace TTTReborn.VisualProgramming
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class StackNodeAttribute : LibraryAttribute
    {
        public StackNodeAttribute(string name) : base("stacknode_" + name)
        {

        }
    }

    public abstract class StackNode
    {
        public string LibraryName { get; set; }
        public string NodeReference { get; set; }
        public List<StackNode> NextNodes { get; set; } = new();

        public StackNode()
        {
            LibraryName = Utils.GetLibraryName(GetType());
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
                ["LibraryName"] = LibraryName,
                ["NodeReference"] = NodeReference,
                ["NextNodes"] = nextNodesJsonList,
            };
        }

        public virtual void LoadFromJsonData(Dictionary<string, object> jsonData)
        {
            // TODO add connect positions

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

            jsonData.TryGetValue("NodeReference", out object nodeReference);

            if (nodeReference != null)
            {
                NodeReference = nodeReference.ToString();
            }
        }

        public static T GetStackNodeFromJsonData<T>(Dictionary<string, object> jsonData) where T : StackNode
        {
            jsonData.TryGetValue("LibraryName", out object libraryName);

            if (libraryName == null)
            {
                return null;
            }

            Type type = Utils.GetTypeByLibraryName<T>(libraryName.ToString());

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
