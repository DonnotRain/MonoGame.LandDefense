#region Using Statements

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

#endregion

namespace LandDefense
{
    public class InputState
    {
        public KeyboardState CurrentKeyboardStates;
        public MouseState CurrentMouseState;
        public KeyboardState LastKeyboardStates;
        public MouseState LastMouseState;

        public InputState()
        {
            CurrentKeyboardStates = new KeyboardState();
            LastKeyboardStates = new KeyboardState();

            CurrentMouseState = new MouseState();
            LastMouseState = new MouseState();
        }

        public void Update()
        {
            LastKeyboardStates = CurrentKeyboardStates;
            CurrentKeyboardStates = Keyboard.GetState();

            LastMouseState = CurrentMouseState;
            CurrentMouseState = Mouse.GetState();
        }

        public bool IsMouseOn(Rectangle rec)
        {
            return (CurrentMouseState.X < rec.X + rec.Width && CurrentMouseState.X > rec.X &&
                    CurrentMouseState.Y > rec.Y && CurrentMouseState.Y < rec.Y + rec.Height);
        }

        public bool IsNewKeyPress(Keys key)
        {
            return (CurrentKeyboardStates.IsKeyDown(key) &&
                    LastKeyboardStates.IsKeyUp(key));
        }

        public bool IsLeftButtonClick()
        {
            return (CurrentMouseState.LeftButton == ButtonState.Pressed &&
                    LastMouseState.LeftButton == ButtonState.Released);
        }

        public bool IsMenuCancel()
        {
            return IsNewKeyPress(Keys.Escape);
        }

        public bool IsMenuUp()
        {
            return IsNewKeyPress(Keys.Up);
        }

        public bool IsMenuDown()
        {
            return IsNewKeyPress(Keys.Down);
        }

        public bool IsPauseGame()
        {
            return IsNewKeyPress(Keys.Escape);
        }
    }
}