using System;
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
        public string Id { get; set; }
        public string LibraryName { get; set; }
        public string NodeReference { get; set; }
        public string[] ConnectionOutputIds { get; set; } = Array.Empty<string>();
        public string[] ConnectionInputIds { get; set; } = Array.Empty<string>();

        private Vector2 _pos;

        public StackNode()
        {
            LibraryName = Utils.GetLibraryName(GetType());
        }

        internal void SetPos(float posX, float posY)
        {
            _pos = new Vector2(posX, posY);
        }

        public abstract object[] Test(params object[] input);

        public abstract object[] Evaluate(params object[] input);

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
