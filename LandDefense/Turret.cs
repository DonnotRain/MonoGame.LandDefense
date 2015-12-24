using Microsoft.Xna.Framework;

namespace LandDefense
{
    public class Turret
    {
        private readonly float acceleration = 0.5f;
        public Vector2 desPosition;
        public bool IsAlive = true;
        public bool IsBombing;
        public int MaxDamage = 0; //塔能发射出的最大伤害，产生的实际伤害值随机产生在最小最大范围内
        public int MinDamage = 0; //塔能发射出的最小伤害
        public Vector2 Position;
        public Vector2 Velocity;

        public void Move()
        {
            if (desPosition.X == Position.X)
                IsBombing = true;
            Velocity.Y += acceleration;
            Position.X += Velocity.X;
            Position.Y = Position.Y + Velocity.Y + 0.5f*acceleration;
        }
    }
}