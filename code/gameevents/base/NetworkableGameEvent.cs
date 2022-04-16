using System;
using System.IO;

using Sandbox;

namespace TTTReborn
{
    public abstract partial class NetworkableGameEvent : GameEvent, INetworkable
    {
        public To? Receiver { get; set; } = null;

        public void RunNetworked() => RunNetworked(Receiver ?? To.Everyone);

        public virtual void RunNetworked(To to)
        {
            Assert.True(Host.IsServer);

            Receiver = to;

            base.Run();
            ClientRun(to, Name, (this as INetworkable).Write());
        }

        [ClientRpc]
        public static void ClientRun(string libraryName, byte[] bytes)
        {
            Type type = Utils.GetTypeByLibraryName<GameEvent>(libraryName);

            if (type == null)
            {
                return;
            }

            NetworkableGameEvent gameEvent = Utils.GetObjectByType<NetworkableGameEvent>(type);
            (gameEvent as INetworkable).Read(bytes);
            gameEvent.Run();
        }

        public static void RegisterNetworked<T>(T gameEvent, params GameEventScoring[] gameEventScorings) where T : NetworkableGameEvent => RegisterNetworked(To.Everyone, gameEvent, gameEventScorings);

        public static void RegisterNetworked<T>(To to, T gameEvent, params GameEventScoring[] gameEventScorings) where T : NetworkableGameEvent
        {
            gameEvent.Scoring = gameEventScorings ?? gameEvent.Scoring;

            gameEvent.ProcessRegister();
            gameEvent.RunNetworked(to);
        }

        public virtual void WriteData(BinaryWriter binaryWriter)
        {
            binaryWriter.Write(Name);
            binaryWriter.Write(CreatedAt);

            // scoring
            int count = Scoring.Length;

            binaryWriter.Write(count);

            for (int i = 0; i < count; i++)
            {
                Scoring[i].WriteData(binaryWriter);
            }
        }

        public virtual void ReadData(BinaryReader binaryReader)
        {
            Name = binaryReader.ReadString();
            CreatedAt = binaryReader.ReadSingle();

            // scoring
            int count = binaryReader.ReadInt32();
            Scoring = new GameEventScoring[count];

            for (int i = 0; i < count; i++)
            {
                Scoring[i] = new GameEventScoring(null);
                Scoring[i].ReadData(binaryReader);
            }
        }
    }
}
