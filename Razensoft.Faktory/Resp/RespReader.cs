using System;
using System.IO;
using System.Threading.Tasks;

namespace Razensoft.Faktory.Resp
{
    public class RespReader
    {
        private readonly StreamReader streamReader;

        public RespReader(StreamReader streamReader) => this.streamReader = streamReader;

        public async Task<RespMessage> ReadAsync()
        {
            var typeCharBuffer = new char[1];
            var messageArrived = false;
            while (!messageArrived)
                messageArrived = await streamReader.ReadAsync(typeCharBuffer, 0, 1) == 1;
            switch (typeCharBuffer[0])
            {
                case SimpleStringMessage.TypeDescriptor:
                    return await Deserialize<SimpleStringMessage>();
                case ErrorMessage.TypeDescriptor:
                    return await Deserialize<ErrorMessage>();
                case IntegerMessage.TypeDescriptor:
                    return await Deserialize<IntegerMessage>();
                case BulkStringMessage.TypeDescriptor:
                    return await Deserialize<BulkStringMessage>();
                case ArrayMessage.TypeDescriptor:
                    return await Deserialize<ArrayMessage>();
                default:
                    throw new ArgumentException($"Unknown type descriptor {typeCharBuffer[0]}");
            }
        }

        private async Task<T> Deserialize<T>() where T : RespMessage, new()
        {
            var message = new T();
            await message.DeserializeAsync(streamReader);
            return message;
        }
    }
}