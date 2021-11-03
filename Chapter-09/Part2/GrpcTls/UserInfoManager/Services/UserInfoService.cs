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
            foreach (var item in userDataCache.GetUsers())
            {
                await responseStream.WriteAsync(item);
            }
        }
    }
}
