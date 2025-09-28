using LogAnalytics.Shared;

namespace LogAnalytics.Server.Services
{


    public class ServerEnvironmentInfoService : IEnvironmentInfoService
    {
        public string GetMode() => "Server";
    }
}
