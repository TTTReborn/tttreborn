using System;
using System.Collections.Generic;
using System.Text.Json;

using Sandbox;

using TTTReborn.UI.VisualProgramming;

namespace TTTReborn.VisualProgramming
{
    public partial class NodeStack
    {
        private static int _currentPacketHash = -1;
        private static int _packetCount;
        private static string[] _packetData;

        [ServerCmd]
        public static void ServerRequestStack()
        {
            if (ConsoleSystem.Caller == null)
            {
                return;
            }

            To to = To.Single(ConsoleSystem.Caller);

            if (!ConsoleSystem.Caller.HasPermission("visualprogramming"))
            {
                ClientInitializeNodesFromStack(to, false);

                return;
            }

            ClientInitializeNodesFromStack(to, true, JsonSerializer.Serialize(Instance.GetJsonData()));
        }

        [ClientRpc]
        public static void ClientInitializeNodesFromStack(bool access, string jsonData = null)
        {
            if (!access)
            {
                Log.Info("No access to visual programming");

                return;
            }

            Window window = Window.Instance;

            if (window != null)
            {
                window.Delete(true);
            }

            new Window(UI.Hud.Current.GeneralHudPanel, jsonData);
        }

        public static void UploadStack(string jsonData)
        {
            int splitLength = 150;
            int splitCount = (int) MathF.Ceiling((float) jsonData.Length / splitLength);

            for (int i = 0; i < splitCount; i++)
            {
                ServerUploadPartialStack(jsonData.GetHashCode(), i, splitCount, jsonData.Substring(splitLength * i, splitLength + Math.Min(0, jsonData.Length - splitLength * (i + 1))));
            }
        }

        [ClientRpc]
        public static void ClientSendStackBuildResult()
        {
            Window window = Window.Instance;

            if (window != null)
            {
                window.BuildButton.Icon = "play_arrow";
            }
        }

        [ServerCmd]
        private static void ServerUploadPartialStack(int packetHash, int packetNum, int maxPackets, string partialStack)
        {
            if (!ConsoleSystem.Caller?.HasPermission("visualprogramming") ?? true)
            {
                return;
            }

            ProceedPartialUpload(packetHash, packetNum, maxPackets, partialStack);

            ClientSendStackBuildResult(To.Single(ConsoleSystem.Caller));
        }

        private static void ProceedPartialUpload(int packetHash, int packetNum, int maxPackets, string partialStack)
        {
            if (_currentPacketHash != packetHash)
            {
                _packetCount = 0;
                _packetData = new string[maxPackets];

                _currentPacketHash = packetHash;
            }

            if (_packetData[packetNum] != null && _packetData[packetNum].Equals(partialStack))
            {
                return;
            }

            _packetData[packetNum] = partialStack;
            _packetCount++;

            if (_packetCount == maxPackets)
            {
                _currentPacketHash = -1;

                Log.Debug("Received NodeStack");

                Dictionary<string, object> jsonDataDict = JsonSerializer.Deserialize<Dictionary<string, object>>(string.Join("", _packetData));

                if (jsonDataDict == null)
                {
                    return;
                }

                /* TODO
                Instance.MainStackNode = StackNode.GetStackNodeFromJsonData<StackNode>(jsonDataDict);

                if (Instance.MainStackNode == null)
                {
                    return;
                }
                */

                Log.Debug("Test NodeStack");

                if (Instance.Test())
                {
                    Log.Debug("NodeStack test passed");

                    Instance.Save();

                    Log.Debug("Saved NodeStack");
                }
                else
                {
                    Log.Debug("NodeStack test failed!");
                }
            }
        }

        [ServerCmd]
        public static void ServerResetStack()
        {
            if (!ConsoleSystem.Caller?.HasPermission("visualprogramming") ?? true)
            {
                return;
            }

            Instance.Init();

            ClientInitializeNodesFromStack(To.Single(ConsoleSystem.Caller), true, JsonSerializer.Serialize(Instance.GetJsonData()));
        }
    }
}
