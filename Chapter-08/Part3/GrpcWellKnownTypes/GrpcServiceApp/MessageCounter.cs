namespace GrpcServiceApp
{
    public class MessageCounter
    {
        private uint messageCount = 0;

        public uint IncrementCount()
        {
            messageCount++;
            return messageCount;
        }

        public uint GetCurrentCount()
        {
            return messageCount;
        }

        public void UpdateCount(uint count)
        {
            messageCount = count;
        }
    }
}
