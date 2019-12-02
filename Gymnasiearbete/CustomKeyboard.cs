using Microsoft.Xna.Framework.Input;

namespace Gymnasiearbete
{
    class CustomInputHandler
    {
        static bool hasBegun = false;

        #region Error Region
        protected static void CheckBegin()
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
        protected static void CheckEnd()
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
        protected static void CheckOther()
        {

        }
        #endregion
    class CustomKeyboard : CustomInputHandler
    {
        static KeyboardState oldState;
        static KeyboardState currentState;

        public static void Begin()
        {
            Begin(Keyboard.GetState());

            // Ellen genius stoopid idé
            /* om knappen är nedtryckt i x sekunder är det ett knapptryck
             * annars om längre är den "håller inne"
             */
        }
        static public void Begin(KeyboardState keyState)
        {
            CheckBegin();
            currentState = keyState;
        }

        static public void End()
        {
            CheckEnd();
            oldState = currentState;
        }

        static public bool IsKeyClicked(Keys key)
        {
            CheckOther();
            return (currentState.IsKeyDown(key) && oldState.IsKeyUp(key));
        }

        static public bool IsKeyHeld(Keys key)
        {
            CheckOther();
            return (currentState.IsKeyDown(key) && oldState.IsKeyDown(key));
        }
    }
}
