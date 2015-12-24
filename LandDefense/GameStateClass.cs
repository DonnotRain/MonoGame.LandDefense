using Microsoft.Xna.Framework;

namespace LandDefense
{
    public static class GameStateClass
    {
        public enum GameState
        {
            Menu,
            LevelChoose,
            MainGame,
            About,
            Paused
        }

        public static int ChosedLevel = -1;
        public static GameState currentGameState;
        public static int LayerDepth = -1;
        public static int AllPowerNum = 0;
        public static int AllLifeNum = 20;

        public static void changeState(GameState gs, Game game)
        {
            currentGameState = gs;
            if (gs == GameState.Menu)
            {
                game.Window.Title = "主选单";
            }
            else if (gs == GameState.LevelChoose)
            {
                game.Window.Title = "选择要进行游戏的关卡";
            }
            else if (gs == GameState.MainGame)
            {
                game.Window.Title = "游戏进行中。。。";
            }
            else if (gs == GameState.About)
            {
                game.Window.Title = "关于";
            }
        }
    }
}