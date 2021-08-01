using System.Collections.Generic;

namespace StatusMicroservice
{
    public interface IStateStore
    {
        IEnumerable<(string ClientName, ClientStatus ClientStatus)> GetAllStatuses();
        ClientStatus GetStatus(string clientName);
        bool UpdateStatus(string clientName, ClientStatus status);
    }

    internal class StateStore : IStateStore
    {
        private Dictionary<string, ClientStatus> statuses;

        public StateStore()
        {
            statuses = new Dictionary<string, ClientStatus>();
        }

        public IEnumerable<(string ClientName, ClientStatus ClientStatus)> GetAllStatuses()
        {
            var returnedStatuses = new List<(string ClientName, ClientStatus ClientStatus)>();

            foreach (var record in statuses)
            {
                returnedStatuses.Add((record.Key, record.Value));
            }

            return returnedStatuses;
        }

        public ClientStatus GetStatus(string clientName)
        {
            if (!statuses.ContainsKey(clientName))
            {
                return ClientStatus.OFFLINE;
            }

            return statuses[clientName];
        }

        public bool UpdateStatus(string clientName, ClientStatus status)
        {
            statuses[clientName] = status;

            return true;
        }
    }
}