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
        public List<int> ConnectPositions { get; set; } = new();

        private float _posX, _posY;

        public StackNode()
        {
            LibraryName = Utils.GetLibraryName(GetType());
        }

        public virtual void Reset()
        {
            NextNodes.Clear();
        }

        internal void SetPos(float posX, float posY)
        {
            _posX = posX;
            _posY = posY;
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
                ["ConnectPositions"] = ConnectPositions,
                ["NodeReference"] = NodeReference,
                ["NextNodes"] = nextNodesJsonList,
                ["PosX"] = _posX,
                ["PosY"] = _posY
            };
        }

        public virtual void LoadFromJsonData(Dictionary<string, object> jsonData)
        {
            jsonData.TryGetValue("ConnectPositions", out object connectPosition);

            if (connectPosition != null)
            {
                ConnectPositions = JsonSerializer.Deserialize<List<int>>(((JsonElement) connectPosition).GetRawText());
            }

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

            jsonData.TryGetValue("PosX", out object posX);

            if (posX != null)
            {
                _posX = float.Parse(posX.ToString());
            }

            jsonData.TryGetValue("PosY", out object posY);

            if (posY != null)
            {
                _posY = float.Parse(posY.ToString());
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
