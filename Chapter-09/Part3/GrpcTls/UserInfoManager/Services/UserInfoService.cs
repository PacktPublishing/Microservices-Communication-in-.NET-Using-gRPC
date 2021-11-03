using System;
using System.Linq;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Users;

namespace UserInfoManager.Services
{
    public class UserInfoService : UserManager.UserManagerBase
    {
        private readonly UserDataCache userDataCache;

        public UserInfoService(UserDataCache userDataCache)
        {
            this.userDataCache = userDataCache;
        }

        public override async Task GetAllUsers(Empty request, IServerStreamWriter<UserInfo> responseStream, ServerCallContext context)
        {
            Console.WriteLine($"Client authenticated: {context.AuthContext.IsPeerAuthenticated}");

            if (context.AuthContext.IsPeerAuthenticated)
            {
                Console.WriteLine($"Auth property name: {context.AuthContext.PeerIdentityPropertyName}");
                Console.WriteLine($"Auth property value: {context.AuthContext.Properties.FirstOrDefault()?.Value}");
            }

            foreach (var item in userDataCache.GetUsers())
            {
                await responseStream.WriteAsync(item);
            }
        }
    }
}
