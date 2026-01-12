using System.Net;
using System.Threading.Tasks;

namespace Content.Server.Corvax.Interface;

public interface IServerVPNGuardManager
{
    public void Initialize();
    public Task<bool> IsConnectionVpn(IPAddress ip);
}
