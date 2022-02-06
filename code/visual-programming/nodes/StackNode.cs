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
        public static readonly List<StackNode> StackNodeList = new();

        public string Id { get; set; }
        public string LibraryName { get; set; }
        public string NodeReference { get; set; }
        public string[] ConnectionOutputIds { get; set; } = Array.Empty<string>();
        public string[] ConnectionInputIds { get; set; } = Array.Empty<string>();
        public object[] PreparedInputData { get; set; } = null;

        private Vector2 _pos;

        public StackNode()
        {
            LibraryName = Utils.GetLibraryName(GetType());

            StackNodeList.Add(this);
        }

        public static StackNode GetById(string id)
        {
            foreach (StackNode stackNode in StackNodeList)
            {
                if (stackNode.Id == id)
                {
                    return stackNode;
                }
            }

            return null;
        }

        public virtual void Delete()
        {
            StackNodeList.Remove(this);
        }

        public virtual void Reset()
        {
            ResetData();

            _pos = Vector2.Zero;
        }

        public virtual void ResetData()
        {
            ConnectionOutputIds = Array.Empty<string>();
            ConnectionInputIds = Array.Empty<string>();
            PreparedInputData = null;
        }

        internal void SetPos(float posX, float posY)
        {
            _pos = new Vector2(posX, posY);
        }

        public abstract object[] Test(object[] input);

        public abstract object[] Evaluate(object[] input);

        public virtual Dictionary<string, object> GetJsonData()
        {
            return new Dictionary<string, object>()
            {
                ["Id"] = Id,
                ["LibraryName"] = LibraryName,
                ["NodeReference"] = NodeReference,
                ["ConnectionInputIds"] = ConnectionInputIds,
                ["ConnectionOutputIds"] = ConnectionOutputIds,
                ["Pos"] = _pos,
            };
        }

        public virtual void LoadFromJsonData(Dictionary<string, object> jsonData)
        {
            jsonData.TryGetValue("Id", out object id);

            if (id != null)
            {
                Id = id.ToString();
            }

            jsonData.TryGetValue("NodeReference", out object nodeReference);

            if (nodeReference != null)
            {
                NodeReference = nodeReference.ToString();
            }

            jsonData.TryGetValue("ConnectionInputIds", out object connectionInputIds);

            if (connectionInputIds != null)
            {
                ConnectionInputIds = JsonSerializer.Deserialize<string[]>(((JsonElement) connectionInputIds).GetRawText());
            }

            jsonData.TryGetValue("ConnectionOutputIds", out object connectionOutputIds);

            if (connectionOutputIds != null)
            {
                ConnectionOutputIds = JsonSerializer.Deserialize<string[]>(((JsonElement) connectionOutputIds).GetRawText());
            }

            jsonData.TryGetValue("Pos", out object pos);

            if (pos != null)
            {
                _pos = JsonSerializer.Deserialize<Vector2>(((JsonElement) pos).GetRawText());
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
