using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LandDefense
{
    /// <summary>
    ///     This is a game component that implements IUpdateable.
    /// </summary>
    public class FpsCounter : DrawableGameComponent
    {
        private static int frames;
        private static int milliseconds;
        private readonly Game game;
        private SpriteBatch sBatch;
        //   GraphicsDevice graphic;

        private SpriteFont sf;

        public FpsCounter(Game game)
            : base(game)
        {
            this.game = game;
            LoadContent();
        }

        /// <summary>
        ///     得到当前的FPS
        /// </summary>
        public static float Fps { get; private set; }

        /// <summary>
        ///     设置/获取用于计算FPS的时间间隔（毫秒）
        ///     默认值1000
        /// </summary>
        public static int Interval { get; set; } = 1000;

        /// <summary>
        ///     在每一帧调用
        /// </summary>
        /// <param name="time">自从上一帧经过的时间</param>
        /// <returns>如果经过的时间大于设置的间隔时间就返回true</returns>
        public static bool NewFrame(GameTime time)
        {
            frames++;
            milliseconds += time.ElapsedGameTime.Milliseconds;
            if (milliseconds >= Interval)
            {
                Fps = frames*1000.0f/milliseconds;
                frames = 0;
                milliseconds -= Interval;
                return true;
            }
            return false;
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            sf = game.Content.Load<SpriteFont>("sFont");

            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            NewFrame(gameTime);
            sBatch = (SpriteBatch) Game.Services.GetService(typeof (SpriteBatch));

            sBatch.Begin();
            sBatch.DrawString(sf, "FPS:" + ((int) Fps), new Vector2(30, 32), Color.White);
            sBatch.DrawString(sf, "FPS:" + ((int) Fps), new Vector2(30, 30), Color.Yellow);

            sBatch.End();
            base.Draw(gameTime);
        }
    }
}