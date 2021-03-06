﻿using SampSharp.GameMode;
using SampSharp.GameMode.Pools;
using SampSharp.GameMode.SAMP;
using System.Linq;

namespace TestServer.World
{
    /// <summary>
    ///     Represents an account.
    /// </summary>
    public class Account : IdentifiedPool<Account>
    {
        #region Fields

        private string _name;
        private string _password;
        private int _team;
        private int _skin;
        private int _money;
        private int _score;
        private float _health;
        private float _armour;
        private Color _color;
        private Place _spawn;

        #endregion

        #region Constants

        /// <summary>
        ///     Maximum number of accounts which can exist.
        /// </summary>
        public const int Max = 10000;

        /// <summary>
        ///     Maximum length of a account's password.
        /// </summary>
        public const int MaxPasswordLength = 32;

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets the name of this <see cref="Account"/>.
        /// </summary>
        public string Name
        {
            get => _name;
            set
            {
                _name = value;

                if (Player != null)
                    Player.Name = _name;
            }
        }

        /// <summary>
        ///     Gets or sets the password of this <see cref="Account"/>.
        /// </summary>
        public string Password
        {
            get => _password;
            set
            {
                if (value.Length > MaxPasswordLength)
                    return;

                _password = value;
            }
        }

        /// <summary>
        ///     Gets or sets the team of this <see cref="Account"/>.
        /// </summary>
        public int Team
        {
            get => _team;
            set
            {
                _team = value;

                if (Player != null)
                {
                    Player.Team = _team;
                    Player.SetSpawnInfo(Team, Skin, Spawn.Position, Spawn.Angle);
                }
            }
        }

        /// <summary>
        ///     Gets or sets the skin of this <see cref="Account"/>.
        /// </summary>
        public int Skin
        {
            get => _skin;
            set
            {
                _skin = value;

                if (Player != null)
                {
                    Player.Skin = _skin;
                    Player.SetSpawnInfo(Team, Skin, Spawn.Position, Spawn.Angle);
                }
            }
        }

        /// <summary>
        ///     Gets or sets the money of this <see cref="Account"/>.
        /// </summary>
        public int Money
        {
            get => _money;
            set
            {
                _money = value;

                if (Player != null)
                    Player.Money = _money;
            }
        }

        /// <summary>
        ///     Gets or sets the score of this <see cref="Account"/>.
        /// </summary>
        public int Score
        {
            get => _score;
            set
            {
                _score = value;

                if (Player != null)
                    Player.Score = _score;
            }
        }

        /// <summary>
        ///     Gets or sets the health of this <see cref="Account"/>.
        /// </summary>
        public float Health
        {
            get => _health;
            set
            {
                _health = value;

                if (Player != null)
                    Player.Health = _health;
            }
        }

        /// <summary>
        ///     Gets or sets the armour of this <see cref="Account"/>.
        /// </summary>
        public float Armour
        {
            get => _armour;
            set
            {
                _armour = value;

                if (Player != null)
                    Player.Armour = _armour;
            }
        }

        /// <summary>
        ///     Gets or sets the color of this <see cref="Account"/>.
        /// </summary>
        public Color Color
        {
            get => _color;
            set
            {
                _color = value;

                if (Player != null)
                    Player.Color = _color;
            }
        }

        /// <summary>
        ///     Gets the spawn of this <see cref="Account"/>.
        /// </summary>
        public Place Spawn
        {
            get => _spawn;
            set
            {
                _spawn = value;

                if (Player != null)
                    Player.SetSpawnInfo(Team, Skin, _spawn.Position, _spawn.Angle);
            }
        }

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

        #region Methods

        public void Init()
        {
            Team = 0;
            Skin = 1;
            Money = 0;
            Score = 1;
            Health = 100;
            Armour = 0;
            Color = Color.White;
            Spawn = new Place(new Vector3(2847.8303, 1290.8995, 11.3906), 93.0609f);
        }

        #endregion

        #region Static Methods

        public static bool Exists(string name) =>
            All.Any(account => account.Name == name);

        public static Account Create(string name, string password)
        {
            if (PoolSize == Max)
                return null;

            if (Exists(name))
                return null;

            if (password.Length > MaxPasswordLength)
                return null;

            var account = FindOrCreate(PoolSize);

            account.Name = name;
            account.Password = password;
            account.Init();

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