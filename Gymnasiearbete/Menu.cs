using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gymnasiearbete
{
    class Menu
    {
        public enum State
        {
            Invalid = 0,
            MainMenu,
            InfoScreen,
            Simulation,
            PausedSimulation
        }

        private State state;
        public State CurrentState => state;

        public Menu(State state)
        {
            this.state = state;
        }
        public static Menu Default => new Menu(State.MainMenu);


        private UIElementButtonGroup btn_mainmenu;
        public UIElementButtonGroup ButtonGroup_MainMenu
        {
            get { return btn_mainmenu; }
            set { btn_mainmenu = value; }
        }

        private UIElementButtonGroup btn_info;
        public UIElementButtonGroup ButtonGroup_InfoScreen
        {
            get { return btn_info; }
            set { btn_info = value; }
        }

        private UIElementButtonGroup btn_sim;
        public UIElementButtonGroup ButtonGroup_Simulation
        {
            get { return btn_sim; }
            set { btn_sim = value; }
        }

        private UIElementButtonGroup btn_paused;
        public UIElementButtonGroup ButtonGroup_PausedSimulation
        {
            get { return btn_paused; }
            set { btn_paused = value; }
        }

        bool firstUpdateHasBeenCalled = false;
        public void FirstUpdate()
        {
            if (!firstUpdateHasBeenCalled)
            {
                ButtonGroup_MainMenu?.Resize();
                ButtonGroup_InfoScreen?.Resize();
                ButtonGroup_Simulation?.Resize();
                ButtonGroup_PausedSimulation?.Resize();
            }
            else firstUpdateHasBeenCalled = true;
        }

        public void ChangeState(State state)
        {
            ButtonGroup_MainMenu?.ResetIndex();
            ButtonGroup_InfoScreen?.ResetIndex();
            ButtonGroup_Simulation?.ResetIndex();
            ButtonGroup_PausedSimulation?.ResetIndex();

            this.state = state;
        }
    }
}
