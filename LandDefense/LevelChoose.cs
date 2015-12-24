using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LandDefense
{
    /// <summary>
    ///     This is a game component that implements IUpdateable.
    /// </summary>
    public class LevelChoose : DrawableGameComponent
    {
        private readonly Game game;
        private readonly GraphicsDevice graphic;
        private readonly InputState inputeState;
        private readonly Rectangle[] REC = new Rectangle[5];
        private Texture2D levelbackGround;
        private Texture2D[,] leveltexture;
        private MySound mySound;
        private SpriteBatch sBatch;
        private SpriteFont sf;

        public LevelChoose(Game game, MySound mySound)
            : base(game)
        {
            this.mySound = mySound;
            this.game = game;
            LoadContent();
            inputeState = new InputState();
            graphic = game.GraphicsDevice;
            for (var i = 0; i < 5; i++)
                REC[i] = new Rectangle(i*115 + 50, 115, leveltexture[0, 0].Width/2, leveltexture[0, 0].Height/2);
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            //     mySound.PlaySound("level begain");
            levelbackGround = game.Content.Load<Texture2D>("LevelBackGround");
            leveltexture = new Texture2D[5, 2];
            sf = game.Content.Load<SpriteFont>("sFont");

            for (var i = 0; i < 5; i++)
            {
                leveltexture[i, 0] = game.Content.Load<Texture2D>(@"UIImage\level_01");
                leveltexture[i, 1] = game.Content.Load<Texture2D>(@"UIImage\level_02");
            }
            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            //  MySound.PLaySound();

            if (GameStateClass.currentGameState != GameStateClass.GameState.LevelChoose) return;

            if (inputeState.IsMenuCancel())
                GameStateClass.changeState(GameStateClass.GameState.Menu, game);
            inputeState.Update();

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            if (GameStateClass.currentGameState != GameStateClass.GameState.LevelChoose) return;
            sBatch = (SpriteBatch) Game.Services.GetService(typeof (SpriteBatch));
            sBatch.Begin();
            sBatch.Draw(levelbackGround, new Rectangle(0, 0, graphic.Viewport.Width, graphic.Viewport.Height),
                Color.White);
            for (var i = 0; i < 5; i++)
            {
                if (inputeState.IsMouseOn(REC[i]))
                {
                    sBatch.Draw(leveltexture[i, 1], REC[i], Color.White);
                    if (inputeState.IsLeftButtonClick())
                    {
                        GameStateClass.changeState(GameStateClass.GameState.MainGame, game);
                        GameStateClass.ChosedLevel = i;
                        //System.Diagnostics.Trace.WriteLine(i.ToString());
                    }
                }
                else
                    sBatch.Draw(leveltexture[i, 0], REC[i], Color.White);
                sBatch.DrawString(sf, "Level " + (i + 1),
                    new Vector2(REC[i].X + REC[i].Width/2 - 30, REC[i].Y + REC[i].Height + 10), Color.Yellow);
            }

            sBatch.End();
            base.Draw(gameTime);
        }
    }
}