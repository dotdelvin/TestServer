using SampSharp.GameMode.Controllers;
using SampSharp.GameMode.Tools;
using TestServer.World;

namespace TestServer.Controllers
{
    [Controller]
    public class AccountController : Disposable, ITypeProvider
    {
        public void RegisterTypes() =>
            Account.Register<Account>();

        protected override void Dispose(bool disposing)
        {
            foreach (var account in Account.All)
                account.Dispose();
        }
    }
}