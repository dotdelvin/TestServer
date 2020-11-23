using SampSharp.GameMode;

namespace TestServer
{
    public struct Place
    {
        public Vector3 Position { get; }
        public float Angle { get; }

        public Place(Vector3 position, float angle)
        {
            Position = position;
            Angle = angle;
        }
    }
}