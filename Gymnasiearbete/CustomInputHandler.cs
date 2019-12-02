using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace Gymnasiearbete
{
    class CustomInputHandler
    {
        bool hasBegun = false;

        #region Error Region
        protected void CheckBegin()
        {
            // Check if Begin() hasn't already been called.
            // Otherwise, throw Error.
            if (!hasBegun)
            {
                hasBegun = true;
            }
            else
            {
                throw new System.InvalidOperationException("Begin cannot be called again until End has been successfully called.");
            }
        }
        protected void CheckEnd()
        {
            // Check if Begin() has been called successfully.
            // If not, throw Error.
            if (hasBegun)
            {
                hasBegun = false;
            }
            else
            {
                throw new System.InvalidOperationException("Begin must be called successfully before End can be called.");
            }
        }
        protected void CheckOther()
        {
            if (!hasBegun)
            {
                throw new System.InvalidOperationException("Begin must be called successfully before any method can be called.");
            }
        }
        #endregion
    }

    public enum CustomMouseButtons
    {
        MOUSE_1 = 1,
        MOUSE_2,
        MOUSE_3,
        MOUSE_4,
        MOUSE_5,
        // Aliases
        LeftButton = MOUSE_1,
        MiddleButton = MOUSE_3,
        RightButton = MOUSE_2,
        XButton1 = MOUSE_4,
        XButton2 = MOUSE_5,
    }

    class CustomMouse : CustomInputHandler
    {
        MouseState oldState;
        MouseState currentState;

        public void Begin()
        {
            Begin(Mouse.GetState());
        }
        public void Begin(MouseState mouseState)
        {
            CheckBegin();
            currentState = mouseState;
        }
        public void End()
        {
            CheckEnd();
            oldState = currentState;
        }

        private bool IsButtonPressed(ButtonState state)
        {
            return state == ButtonState.Pressed;
        }
        private bool IsButtonReleased(ButtonState state)
        {
            return state == ButtonState.Released;
        }

        public bool IsButtonClicked(CustomMouseButtons button)
        {
            CheckOther();

            switch (button)
            {
                case CustomMouseButtons.MOUSE_1:
                    return
                        IsButtonPressed(currentState.LeftButton) &&
                        IsButtonReleased(oldState.LeftButton);
                case CustomMouseButtons.MOUSE_2:
                    return
                        IsButtonPressed(currentState.RightButton) &&
                        IsButtonReleased(oldState.RightButton);
                case CustomMouseButtons.MOUSE_3:
                    return
                        IsButtonPressed(currentState.MiddleButton) &&
                        IsButtonReleased(oldState.MiddleButton);
                case CustomMouseButtons.MOUSE_4:
                    return
                        IsButtonPressed(currentState.XButton1) &&
                        IsButtonReleased(oldState.XButton1);
                case CustomMouseButtons.MOUSE_5:
                    return
                        IsButtonPressed(currentState.XButton2) &&
                        IsButtonReleased(oldState.XButton2);
                default: return false;
            }
        }
        public bool IsButtonHeld(CustomMouseButtons button)
        {
            CheckOther();

            switch (button)
            {
                case CustomMouseButtons.MOUSE_1:
                    return
                        IsButtonPressed(currentState.LeftButton) &&
                        IsButtonPressed(oldState.LeftButton);
                case CustomMouseButtons.MOUSE_2:
                    return
                        IsButtonPressed(currentState.RightButton) &&
                        IsButtonPressed(oldState.RightButton);
                case CustomMouseButtons.MOUSE_3:
                    return
                        IsButtonPressed(currentState.MiddleButton) &&
                        IsButtonPressed(oldState.MiddleButton);
                case CustomMouseButtons.MOUSE_4:
                    return
                        IsButtonPressed(currentState.XButton1) &&
                        IsButtonPressed(oldState.XButton1);
                case CustomMouseButtons.MOUSE_5:
                    return
                        IsButtonPressed(currentState.XButton2) &&
                        IsButtonPressed(oldState.XButton2);
                default: return false;
            }
        }

        public int ScrollWheelDifference
        {
            get
            {
                return oldState.ScrollWheelValue - currentState.ScrollWheelValue;
            }
        }

        public Point Location
        {
            get { return new Point(currentState.X, currentState.Y); }
        }
    }

    class CustomKeyboard : CustomInputHandler
    {
        KeyboardState oldState;
        KeyboardState currentState;

        public void Begin()
        {
            Begin(Keyboard.GetState());

            // Ellen genius stoopid idé
            /* om knappen är nedtryckt i x sekunder är det ett knapptryck
             * annars om längre är den "håller inne"
             */
        }
        public void Begin(KeyboardState keyState)
        {
            CheckBegin();
            currentState = keyState;
        }
        public void End()
        {
            CheckEnd();
            oldState = currentState;
        }

        public bool IsKeyClicked(Keys key)
        {
            CheckOther();
            return (currentState.IsKeyDown(key) && oldState.IsKeyUp(key));
        }
        public bool IsKeyHeld(Keys key)
        {
            CheckOther();
            return (currentState.IsKeyDown(key) && oldState.IsKeyDown(key));
        }
        public bool IsKeyDown(Keys key)
        {
            CheckOther();
            return currentState.IsKeyDown(key);
        }
        public bool IsKeyUp(Keys key)
        {
            CheckOther();
            return currentState.IsKeyUp(key);
        }
    }
}