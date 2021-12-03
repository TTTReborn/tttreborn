using System;
using System.Collections.Generic;
using System.Text.Json;

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
        public uint Id { get; set; }
        public UVector2[] ConnectedIds { get; set; } = Array.Empty<UVector2>();

        private Vector2 _pos;

        public StackNode()
        {
            LibraryName = Utils.GetLibraryName(GetType());
        }

        public virtual void Reset()
        {
            ConnectedIds = Array.Empty<UVector2>();
        }

        internal void SetPos(float posX, float posY)
        {
            _pos = new Vector2(posX, posY);
        }

        public abstract object[] Test(params object[] input);

        public abstract object[] Evaluate(params object[] input);

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
                ["Pos"] = _pos,
            };
        }

        public virtual void LoadFromJsonData(Dictionary<string, object> jsonData)
        {
            jsonData.TryGetValue("NextNodes", out object nextNodes);

            if (nextNodes != null)
            {
                JsonElement nextNodesElement = (JsonElement) nextNodes;
                List<Dictionary<string, object>> nextNodesList = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(nextNodesElement.GetRawText());
                NextNodes = new StackNode[nextNodesList.Count];

                for (int i = 0; i < nextNodesList.Count; i++)
                {
                    NextNodes[i] = GetStackNodeFromJsonData<StackNode>(nextNodesList[i]);
                }
            }

            jsonData.TryGetValue("NodeReference", out object nodeReference);

            if (nodeReference != null)
            {
                NodeReference = nodeReference.ToString();
            }

            jsonData.TryGetValue("Pos", out object pos);

            if (pos != null)
            {
                _pos = (Vector2) pos;
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
