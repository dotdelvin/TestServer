using TestServer.World;

namespace TestServer.Events
{
    /// <summary>
    ///     Provides data for the <see cref="Player.LoggedIn" /> and <see cref="Player.LoggedOut"/> events.
    /// </summary>
    public class AccountEventArgs
    {
        /// <summary>
        ///     Gets the account.
        /// </summary>
        public Account Account { get; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="AccountEventArgs" /> class.
        /// </summary>
        /// <param name="time">The account.</param>
        public AccountEventArgs(Account account)
        {
            Account = account;
        }
    }
}