using SampSharp.GameMode;

namespace TestServer
{
    /// <summary>
    ///     Represents a place.
    /// </summary>
    public struct Place
    {
        /// <summary>
        ///      Gets the position of this <see cref="Place"/>.
        /// </summary>
        public Vector3 Position { get; }

        /// <summary>
        ///      Gets the angle of this <see cref="Place"/>.
        /// </summary>
        public float Angle { get; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Place" /> struct.
        /// </summary>
        /// <param name="position">The position of the spawn location.</param>
        /// <param name="angle">The angle of the spawn location.</param>
        public Place(Vector3 position, float angle)
        {
            Position = position;
            Angle = angle;
        }
    }
}