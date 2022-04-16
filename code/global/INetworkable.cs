using System.IO;

namespace TTTReborn.Globals
{
    public partial interface INetworkable
    {
        void WriteData(BinaryWriter binaryWriter);

        public byte[] Write()
        {
            using MemoryStream memoryStream = new();

            using (BinaryWriter binaryWriter = new(memoryStream))
            {
                WriteData(binaryWriter);
            }

            memoryStream.Flush();

            return memoryStream.GetBuffer();
        }

        void ReadData(BinaryReader binaryReader);

        public void Read(byte[] bytes)
        {
            using MemoryStream memoryStream = new(bytes);
            using BinaryReader binaryReader = new(memoryStream);

            ReadData(binaryReader);
        }
    }
}
