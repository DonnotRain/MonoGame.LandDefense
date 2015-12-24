using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LandDefense
{
    public class GameMain : Game
    {
        private readonly GraphicsDeviceManager graphics;
        private AboutPage aboutPage;
        private FpsCounter fps;
        private GamePlayScreen gamePlayScreen;
        private InputState inputState;
        private LevelChoose levelChoose;
        //声明主菜单，关卡选择，关于，及游戏主画面
        private MainMenu mainMenu;
        private MySound mysound;
        private int newLevelIndex;
        private int oldLevelIndex;
        private SpriteBatch spriteBatch;

        public GameMain()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            GameStateClass.changeState(GameStateClass.GameState.Menu, this);
            graphics.PreferredBackBufferWidth = 700;
            graphics.PreferredBackBufferHeight = 600;
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            base.Initialize();
            //mysound = new MySound();

            //mysound.ae = new AudioEngine("Content\\music\\myMusic.xgs");

            //mysound.sb = new SoundBank(mysound.ae, "Content\\music\\Sound Bank.xsb");
            //mysound.wb = new WaveBank(mysound.ae, "Content\\music\\Wave Bank.xwb");
            inputState = new InputState();
            newLevelIndex = GameStateClass.ChosedLevel;

            //初始化各画面，并加入显示序列中
            fps = new FpsCounter(this);
            mainMenu = new MainMenu(this, mysound);
            gamePlayScreen = new GamePlayScreen(this, mysound);
            levelChoose = new LevelChoose(this, mysound);
            aboutPage = new AboutPage(this);
            Components.Add(mainMenu);
            Components.Add(gamePlayScreen);

            Components.Add(levelChoose);
            Components.Add(aboutPage);
            //   Components.Add(fps);
            gamePlayScreen.Enabled = false;
            gamePlayScreen.Visible = false;
            // graphics.GraphicsDevice.RenderState.CullMode = CullMode.CullClockwiseFace;
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Services.AddService(typeof (SpriteBatch), spriteBatch);
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            oldLevelIndex = newLevelIndex;
            newLevelIndex = GameStateClass.ChosedLevel;
            if (newLevelIndex != oldLevelIndex)
            {
                gamePlayScreen.Enabled = true;
                gamePlayScreen.Visible = true;
                // System.Diagnostics.Trace.WriteLine("选择的关卡是" + GameStateClass.ChosedLevel.ToString());
                gamePlayScreen.Initialize();
            }
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            if (GameStateClass.currentGameState == GameStateClass.GameState.Paused)
            {
                if (inputState.IsLeftButtonClick())
                {
                    GameStateClass.changeState(GameStateClass.GameState.MainGame, this);
                }
            }
            base.Draw(gameTime);
            inputState.Update();
        }
    }
}