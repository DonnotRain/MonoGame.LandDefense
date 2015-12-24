using System;
using Microsoft.Xna.Framework;

namespace LandDefense
{
    public class Bullets

    {
        public Vector2 desPosition;
        public bool IsAlive = true;
        public bool IsBombing = false;
        public int MaxDamage = 0; //塔能发射出的最大伤害，产生的实际伤害值随机产生在最小最大范围内
        public int MinDamage = 0; //塔能发射出的最小伤害
        public Vector2 Position;
        public float rotation = 0;
        public Vector2 Velocity;
        //public int k;
        public void Move()
        {
            var k = ((desPosition.Y - Position.Y)/(desPosition.X - Position.X));
            if (desPosition.X - Position.X > 0)
                Velocity.X = (float) (4/(Math.Sqrt(1 + k*k)));
            else
                Velocity.X = (float) (-4/(Math.Sqrt(1 + k*k)));
            // if (NodeList[NodeIndex].Y - Position.Y > 0)
            Velocity.Y = Velocity.X*k;
            Position += Velocity;
        }
    }
}