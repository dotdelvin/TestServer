using SampSharp.GameMode.Pools;
using System.Linq;

namespace TestServer.World
{
    /// <summary>
    ///     Represents an account.
    /// </summary>
    public class Account : IdentifiedPool<Account>
    {
        #region Constants

        /// <summary>
        ///     Maximum number of accounts which can exist.
        /// </summary>
        public const int Max = 10000;

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets the name of this <see cref="Account"/>.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Gets or sets the password of this <see cref="Account"/>.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        ///     Gets or sets the skin of this <see cref="Account"/>.
        /// </summary>
        public int Skin { get; set; }

        /// <summary>
        ///     Gets or sets the money of this <see cref="Account"/>.
        /// </summary>
        public int Money { get; set; }

        /// <summary>
        ///     Gets or sets the score of this <see cref="Account"/>.
        /// </summary>
        public int Score { get; set; }

        /// <summary>
        ///     Gets or sets the health of this <see cref="Account"/>.
        /// </summary>
        public float Health { get; set; }

        /// <summary>
        ///     Gets or sets the armour of this <see cref="Account"/>.
        /// </summary>
        public float Armour { get; set; }

        /// <summary>
        ///     Gets the <see cref="World.Player"/> is currently using this <see cref="Account"/>.
        /// </summary>
        public Player Player =>
            Player.All.SingleOrDefault(player => player.Account == this);

        #endregion

        #region Static Properties

        /// <summary>
        ///     Gets the size of the accounts pool.
        /// </summary>
        public static int PoolSize { get; private set; }

        #endregion

        #region Static Methods

        public static bool Exists(string name) =>
            All.Any(account => account.Name == name);

        public static Account Find(string name) =>
            All.SingleOrDefault(account => account.Name == name);

        public static Account Create(string name, string password)
        {
            if (PoolSize == Max)
                return null;

            if (Exists(name))
                return null;

            var account = FindOrCreate(PoolSize);

            account.Name = name;
            account.Password = password;

            // Sets default values.
            account.Skin = 1;
            account.Money = 0;
            account.Score = 1;
            account.Health = 100;
            account.Armour = 0;

            PoolSize++;

            return account;
        }

        #endregion

        #region Override Methods

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            PoolSize--;
        }

        #endregion
    }
}