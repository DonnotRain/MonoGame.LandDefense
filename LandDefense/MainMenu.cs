using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LandDefense
{
    public class MainMenu : DrawableGameComponent
    {
        private readonly Game game;
        private readonly InputState inputState;
        private readonly Rectangle PlayRec;
        private Texture2D backGround, Play_01, Play_02;
        private int MouseStateTest;
        private MySound mysound;
        private SpriteBatch sBatch;
        private SpriteFont sf;

        public MainMenu(Game game, MySound mySound)
            : base(game)
        {
            mysound = mySound;
            this.game = game;
            LoadContent();
            inputState = new InputState();
            PlayRec = new Rectangle(300, 400, Play_01.Width, Play_01.Height);
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            // ContentManager content = Game.Content;
            //   mysound.PLaySound(1);

            base.LoadContent();
            backGround = game.Content.Load<Texture2D>("StartPage");
            sf = game.Content.Load<SpriteFont>("sFont");
            Play_01 = game.Content.Load<Texture2D>(@"UIImage\Play_01");
            Play_02 = game.Content.Load<Texture2D>(@"UIImage\Play_02");
            //  System.Diagnostics.Trace.WriteLine("Spritebatch");
        }

        public override void Update(GameTime gameTime)
        {
            if (GameStateClass.currentGameState != GameStateClass.GameState.Menu) return;

            if (inputState.IsLeftButtonClick())
                MouseStateTest++;
            if (inputState.IsMenuCancel())
                game.Exit();
            inputState.Update();
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            if (GameStateClass.currentGameState != GameStateClass.GameState.Menu) return;
            sBatch = (SpriteBatch) Game.Services.GetService(typeof (SpriteBatch));
            sBatch.Begin();
            sBatch.Draw(backGround, new Rectangle(0, 0, 700, 600), Color.White);

            if (inputState.IsMouseOn(PlayRec))
            {
                sBatch.Draw(Play_02, PlayRec, Color.White);
                if (inputState.IsLeftButtonClick())
                {
                    GameStateClass.changeState(GameStateClass.GameState.LevelChoose, game);
                }
            }
            else
            {
                sBatch.Draw(Play_01, PlayRec, Color.White);
            }
            //sBatch.DrawString(sf, inputState.CurrentMouseState.X.ToString() + ","
            //    + inputState.CurrentMouseState.Y.ToString(), new Vector2(inputState.CurrentMouseState.X, inputState.CurrentMouseState.Y), Color.Yellow);

            sBatch.End();
            base.Draw(gameTime);
        }
    }
}