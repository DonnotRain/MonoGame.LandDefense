using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LandDefense.Models
{
    public class EnemiesInfor
    {
        private readonly Vector2[] NodeList;
        private readonly Random random = new Random();
        private readonly float VeloCity;
        public int AgainstSpells; //魔法抗性      
        public int AllLifeNum = 1;
        public int armor; //护甲值。
        public string enemyName;
        public Texture2D enemytexture;
        public float Interval = 0; //攻击的时间间隔,毫秒为单位
        public bool IsAlive = true;
        public bool IsFlying = false; //是否为飞行生物
        private bool ISopposite;
        public int lifeNum;
        public int MaxDamage = 0; //此敌人最大伤害，产生的实际伤害值随机产生在最小最大范围内
        public int MinDamage = 0; //敌人最小伤害
        private int NodeIndex = 1;
        private int pic_Height;
        private int pic_Width;
        private int pic_X;
        private int pic_Y;
        public Vector2 Position;
        public int range = 0; //攻击范围，以像素点为单位
        private int StepDuration;
        public Texture2D[] texture;
        public Vector2 Velocity;

        public EnemiesInfor(Texture2D[] texture, Vector2[] NodeList)
        {
            this.texture = texture;
            this.NodeList = NodeList;
            VeloCity = (float)(random.Next(1) / 2 + 0.5);
            //  AllLifeNum=
            // this.Position = NodeList[0];
        }

        public void Move(GameTime gameTime)
        {
            // AllLifeNum = lifeNum;
            //  System.Diagnostics.Trace.WriteLine("Enemylifenum:" + lifeNum);
            Position += Velocity;


            StepDuration += Convert.ToInt16(gameTime.ElapsedGameTime.TotalMilliseconds);
            if (NodeIndex == NodeList.Length - 1)
            {
                IsAlive = false;
                GameStateClass.AllLifeNum -= 1;
                GameStateClass.AllPowerNum += 50;

                //Console.WriteLine("这个敌人成功闯出");
            }
            if ((int)NodeList[NodeIndex].X != (int)Position.X)
            {
                var k = (NodeList[NodeIndex].Y - Position.Y) / (NodeList[NodeIndex].X - Position.X);
                //VeloCity = 100;
                if (NodeList[NodeIndex].X - Position.X > 0)
                    Velocity.X = (float)(VeloCity / (Math.Sqrt(1 + k * k)));
                else
                    Velocity.X = (float)(-VeloCity / (Math.Sqrt(1 + k * k)));
                // if (NodeList[NodeIndex].Y - Position.Y > 0)
                Velocity.Y = Velocity.X * k;
            }
            var distance = (int)(NodeList[NodeIndex].Y - Position.Y) * (int)(NodeList[NodeIndex].Y - Position.Y) +
                           (int)(NodeList[NodeIndex].X
                                  - Position.X) * (int)(NodeList[NodeIndex].X - Position.X);
            if (distance < 10)
            {
                NodeIndex++;
                // System.Diagnostics.Trace.WriteLine(TargetIndex.ToString() + targetNode[TargetIndex].X + "," + targetNode[TargetIndex].Y);
            }
            if (StepDuration > 80)
            {
                pic_X++;
                StepDuration = 0;
            }
            if (Velocity.X < 0)
                ISopposite = true;
            else
                ISopposite = false;
            switch (enemyName)
            {
                case "小恶魔":
                    if (pic_X == 6)
                        pic_X = 0;
                    if (Velocity.Y > 0 && Math.Abs(Velocity.X) > Math.Abs(Velocity.Y))
                        pic_Y = 0;

                    else if (Velocity.Y > 0 && Math.Abs(Velocity.X) <= Math.Abs(Velocity.Y))
                    {
                        pic_Y = 2;
                    }
                    if (Velocity.Y < 0 && Math.Abs(Velocity.X) > Math.Abs(Velocity.Y))
                        pic_Y = 0;
                    else if (Velocity.Y < 0 && Math.Abs(Velocity.X) <= Math.Abs(Velocity.Y))
                        pic_Y = 1;

                    //  pic_Y = 0;
                    break;
                case "滴水兽":
                    if (pic_X == 6)
                        pic_X = 0;
                    if (Velocity.Y > 0 && Math.Abs(Velocity.X) > Math.Abs(Velocity.Y))
                        pic_Y = 0;

                    else if (Velocity.Y > 0 && Math.Abs(Velocity.X) <= Math.Abs(Velocity.Y))
                    {
                        pic_Y = 2;
                    }
                    if (Velocity.Y < 0 && Math.Abs(Velocity.X) > Math.Abs(Velocity.Y))
                        pic_Y = 0;
                    else if (Velocity.Y < 0 && Math.Abs(Velocity.X) <= Math.Abs(Velocity.Y))
                        pic_Y = 1;

                    //  pic_Y = 0;
                    break;
                case "暗骑士":
                    if (pic_X == 12)
                        pic_X = 0;
                    if (Velocity.Y > 0 && Math.Abs(Velocity.X) > Math.Abs(Velocity.Y))
                        pic_Y = 0;

                    else if (Velocity.Y > 0 && Math.Abs(Velocity.X) <= Math.Abs(Velocity.Y))
                    {
                        pic_Y = 2;
                    }
                    if (Velocity.Y < 0 && Math.Abs(Velocity.X) > Math.Abs(Velocity.Y))
                        pic_Y = 0;
                    else if (Velocity.Y < 0 && Math.Abs(Velocity.X) <= Math.Abs(Velocity.Y))
                        pic_Y = 1;

                    //  pic_Y = 0;
                    break;

                case "暗黑屠夫":
                    if (pic_X == 11)
                        pic_X = 0;
                    if (Velocity.Y > 0 && Math.Abs(Velocity.X) > Math.Abs(Velocity.Y))
                        pic_Y = 0;

                    else if (Velocity.Y > 0 && Math.Abs(Velocity.X) <= Math.Abs(Velocity.Y))
                    {
                        pic_Y = 2;
                    }
                    if (Velocity.Y < 0 && Math.Abs(Velocity.X) > Math.Abs(Velocity.Y))
                        pic_Y = 0;
                    else if (Velocity.Y < 0 && Math.Abs(Velocity.X) <= Math.Abs(Velocity.Y))
                        pic_Y = 1;

                    //  pic_Y = 0;
                    break;

                case "亡命徒":
                    if (pic_X == 12)
                        pic_X = 0;
                    if (Velocity.Y > 0 && Math.Abs(Velocity.X) > Math.Abs(Velocity.Y))
                        pic_Y = 0;

                    else if (Velocity.Y > 0 && Math.Abs(Velocity.X) <= Math.Abs(Velocity.Y))
                    {
                        pic_Y = 2;
                    }
                    if (Velocity.Y < 0 && Math.Abs(Velocity.X) > Math.Abs(Velocity.Y))
                        pic_Y = 0;
                    else if (Velocity.Y < 0 && Math.Abs(Velocity.X) <= Math.Abs(Velocity.Y))
                        pic_Y = 1;

                    //  pic_Y = 0;
                    break;
            }
        }

        public void Draw(SpriteBatch sBatch, GameTime gameTime)

        {
            var blood_X = 0;
            var blood_Y = 0;

            switch (enemyName)
            {
                case "小恶魔":
                    enemytexture = texture[0];
                    pic_Width = 48;
                    pic_Height = 53;
                    var desRec1 = new Rectangle((int)Position.X - pic_Width / 2, (int)Position.Y - pic_Height / 2,
                        pic_Width, pic_Height);
                    var ResRec1 = new Rectangle(pic_Width * pic_X, pic_Height * pic_Y, pic_Width, pic_Height);
                    if (!ISopposite)
                        sBatch.Draw(enemytexture, desRec1, ResRec1, Color.White, 0, Vector2.Zero, SpriteEffects.None,
                            0.5f);
                    else
                        sBatch.Draw(enemytexture, desRec1, ResRec1, Color.White, 0, Vector2.Zero,
                            SpriteEffects.FlipHorizontally, 0.5f);

                    blood_X = (int)Position.X - desRec1.Width / 2;
                    blood_Y = (int)Position.Y - desRec1.Height / 2;
                    break;
                case "滴水兽":
                    enemytexture = texture[1];
                    pic_Width = 47;
                    pic_Height = 47;
                    var desRec2 = new Rectangle((int)Position.X - pic_Width / 2, (int)Position.Y - pic_Height / 2,
                        pic_Width, pic_Height);
                    var ResRec2 = new Rectangle(pic_Width * pic_X, pic_Height * pic_Y, pic_Width, pic_Height);
                    if (!ISopposite)
                        sBatch.Draw(enemytexture, desRec2, ResRec2, Color.White, 0, Vector2.Zero, SpriteEffects.None,
                            0.5f);
                    else
                        sBatch.Draw(enemytexture, desRec2, ResRec2, Color.White, 0, Vector2.Zero,
                            SpriteEffects.FlipHorizontally, 0.5f);
                    blood_X = (int)Position.X - desRec2.Width / 2;
                    blood_Y = (int)Position.Y - desRec2.Height / 2;
                    break;
                case "暗骑士":
                    enemytexture = texture[2];
                    pic_Width = 44;
                    pic_Height = 35;
                    var desRec3 = new Rectangle((int)Position.X - pic_Width / 2, (int)Position.Y - pic_Height / 2,
                        pic_Width, pic_Height);
                    var ResRec3 = new Rectangle(pic_Width * pic_X, pic_Height * pic_Y, pic_Width, pic_Height);
                    if (!ISopposite)
                        sBatch.Draw(enemytexture, desRec3, ResRec3, Color.White, 0, Vector2.Zero, SpriteEffects.None,
                            0.5f);
                    else
                        sBatch.Draw(enemytexture, desRec3, ResRec3, Color.White, 0, Vector2.Zero,
                            SpriteEffects.FlipHorizontally, 0.5f);
                    blood_X = (int)Position.X - desRec3.Width / 2;
                    blood_Y = (int)Position.Y - desRec3.Height / 2;
                    break;
                case "暗黑屠夫":
                    enemytexture = texture[3];
                    pic_Width = 47;
                    pic_Height = 46;

                    var desRec4 = new Rectangle((int)Position.X - pic_Width / 2, (int)Position.Y - pic_Height / 2,
                        pic_Width, pic_Height);
                    var ResRec4 = new Rectangle(pic_Width * pic_X, pic_Height * pic_Y, pic_Width, pic_Height);
                    if (!ISopposite)
                        sBatch.Draw(enemytexture, desRec4, ResRec4, Color.White, 0, Vector2.Zero, SpriteEffects.None,
                            0.5f);
                    else
                        sBatch.Draw(enemytexture, desRec4, ResRec4, Color.White, 0, Vector2.Zero,
                            SpriteEffects.FlipHorizontally, 0.5f);
                    blood_X = (int)Position.X - desRec4.Width / 2;
                    blood_Y = (int)Position.Y - desRec4.Height / 2;
                    break;
                case "亡命徒":
                    enemytexture = texture[4];

                    pic_Width = 49;
                    pic_Height = 40;
                    var desRec5 = new Rectangle((int)Position.X - pic_Width / 2, (int)Position.Y - pic_Height / 2,
                        pic_Width, pic_Height);
                    var ResRec5 = new Rectangle(pic_Width * pic_X, pic_Height * pic_Y, pic_Width, pic_Height);
                    if (!ISopposite)
                        sBatch.Draw(enemytexture, desRec5, ResRec5, Color.White, 0, Vector2.Zero, SpriteEffects.None,
                            0.5f);
                    else
                        sBatch.Draw(enemytexture, desRec5, ResRec5, Color.White, 0, Vector2.Zero,
                            SpriteEffects.FlipHorizontally, 0.5f);
                    blood_X = (int)Position.X - desRec5.Width / 2;
                    blood_Y = (int)Position.Y - desRec5.Height / 2;
                    break;
            }
            var rate = lifeNum / (double)AllLifeNum;
            var BloodDesRec1 = new Rectangle(blood_X + 20, blood_Y, 12, 2);
            var BloodDesRec2 = new Rectangle(blood_X + 20, blood_Y, (int)(12 * rate), 2);

            // Rectangle BloodDesRec2 = new Rectangle(blood_X + 20, blood_Y, 3, 2);
            sBatch.Draw(texture[6], BloodDesRec1, new Rectangle(0, 0, texture[6].Width, texture[6].Height), Color.White,
                0, Vector2.Zero, SpriteEffects.None, 0.5f); //红
            sBatch.Draw(texture[5], BloodDesRec2, new Rectangle(0, 0, texture[6].Width, texture[6].Height), Color.White,
                0, Vector2.Zero, SpriteEffects.None, 0.4f); //绿
        }
    }
}