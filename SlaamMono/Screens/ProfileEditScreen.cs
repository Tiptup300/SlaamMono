using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using SlaamMono.Input;

namespace SlaamMono
{
    class ProfileEditScreen : IScreen
    {
        #region Variables

        public static ProfileEditScreen Instance = new ProfileEditScreen();
        private const float RotationSpeed = MathHelper.Pi / 3000f;
        private Graph MainMenu = new Graph(new Rectangle(100, 200, GameGlobals.DRAWING_GAME_WIDTH - 100, 624), 2,new Color(0,0,0,150));
        private Graph SubMenu = new Graph(new Rectangle(100, 200, GameGlobals.DRAWING_GAME_WIDTH - 100, 624), 2, new Color(0, 0, 0, 150));
        private IntRange CurrentMenu = new IntRange(0, 0, 1);
        private IntRange CurrentMenuChoice = new IntRange(0, 0, 0);
        private int EditingProfile;
        private bool WaitingForQwerty = false;
        public bool SetupNewProfile = false;
        #endregion

        #region Constructor

        public ProfileEditScreen()
        {
            
        }

        public void Initialize()
        {
            BackgroundManager.ChangeBG(BackgroundManager.BackgroundType.Menu);

            SetupMainMenu();
            ResetSubMenu();


            if (SetupNewProfile)
            {
                SetupNewProfile = false;
                CurrentMenu.Value = 0;
                WaitingForQwerty = true;
                Qwerty.DisplayBoard("");
            }
        }

        #endregion

        #region Update

        public void Update()
        {
            BackgroundManager.SetRotation(1f);

            if (CurrentMenu.Value == 0)
            {
                if (WaitingForQwerty)
                {
                    if (Qwerty.EditingString.Trim() != "")
                    {
                        ProfileManager.AddNewProfile(new PlayerProfile(Qwerty.EditingString,false));
                        
                    } 
                    WaitingForQwerty = false;
                    Qwerty.EditingString = "";
                    SetupMainMenu();
                }
                else
                {
                    if (InputComponent.Players[0].PressedUp)
                    {
                        CurrentMenuChoice.Sub(1);
                        MainMenu.SetHighlight(CurrentMenuChoice.Value);
                    }
                    else if (InputComponent.Players[0].PressedDown)
                    {
                        CurrentMenuChoice.Add(1);
                        MainMenu.SetHighlight(CurrentMenuChoice.Value);
                    }
                    if (InputComponent.Players[0].PressedAction)
                    {
                        if (MainMenu.Items[CurrentMenuChoice.Value].Details[1] == "new")
                        {
                            Qwerty.DisplayBoard("");
                            WaitingForQwerty = true;
                        }
                        else
                        {
                            EditingProfile = int.Parse(MainMenu.Items[CurrentMenuChoice.Value].Details[1]);
                            CurrentMenu.Value = 1;
                            CurrentMenuChoice = new IntRange(0, 0, 2);
                            SubMenu.SetHighlight(0);
                        }
                    }
                    if (InputComponent.Players[0].PressedAction2)
                        ScreenHelper.ChangeScreen(MenuScreen.Instance);
                }
            }
            else if (CurrentMenu.Value == 1)
            {
                if (WaitingForQwerty)
                {
                    if (Qwerty.EditingString.Trim() != "")
                    {
                        ProfileManager.PlayableProfiles[EditingProfile].Name = Qwerty.EditingString;
                        ProfileManager.SaveProfiles();
                    }
                    WaitingForQwerty = false;
                    Qwerty.EditingString = "";

                    CurrentMenu.Value = 0;
                    SetupMainMenu();
                }
                else
                {
                    if (InputComponent.Players[0].PressedUp)
                    {
                        CurrentMenuChoice.Sub(1);
                        SubMenu.SetHighlight(CurrentMenuChoice.Value);
                    }
                    else if (InputComponent.Players[0].PressedDown)
                    {
                        CurrentMenuChoice.Add(1);
                        SubMenu.SetHighlight(CurrentMenuChoice.Value);
                    }
                    if (InputComponent.Players[0].PressedAction)
                    {
                        if (SubMenu.Items[CurrentMenuChoice.Value].Details[1] == "del")
                        {
                            ProfileManager.RemovePlayer(ProfileManager.PlayableProfiles.GetRealIndex(EditingProfile));
                            EditingProfile = -1;
                            CurrentMenu.Value = 0;
                            SetupMainMenu();
                        }
                        else if (SubMenu.Items[CurrentMenuChoice.Value].Details[1] == "ren")
                        {
                            Qwerty.DisplayBoard(ProfileManager.PlayableProfiles[EditingProfile].Name);
                            WaitingForQwerty = true;
                        }
                        else if (SubMenu.Items[CurrentMenuChoice.Value].Details[1] == "clr")
                        {
                            ProfileManager.PlayableProfiles[EditingProfile].TotalDeaths = 0;
                            ProfileManager.PlayableProfiles[EditingProfile].TotalGames = 0;
                            ProfileManager.PlayableProfiles[EditingProfile].TotalKills = 0;
                            ProfileManager.PlayableProfiles[EditingProfile].TotalPowerups = 0;
                            ProfileManager.PlayableProfiles[EditingProfile].BestGame = TimeSpan.Zero;
                            ProfileManager.SaveProfiles();
                            EditingProfile = -1;
                            CurrentMenu.Value = 0;
                            SetupMainMenu();
                        }
                    }
                    if (InputComponent.Players[0].PressedAction2)
                    {
                        CurrentMenu.Value = 0;
                        SetupMainMenu();
                    }
                }
            }
        }

        #endregion

        #region Draw

        public void Draw(SpriteBatch batch)
        {

            if (CurrentMenu.Value == 0)
                MainMenu.Draw(batch);
            else
                SubMenu.Draw(batch);
        }

        #endregion

        #region Dispose

        public void Dispose()
        {

        }

        #endregion

        private void SetupMainMenu()
        {
            MainMenu.Items.Columns.Clear();
            MainMenu.Items.Columns.Add("PROFILES");
            MainMenu.Items.Clear();
            for (int x = 1; x < ProfileManager.PlayableProfiles.Count; x++)
                MainMenu.Items.Add(true, new GraphItem(ProfileManager.PlayableProfiles[x].Name, x.ToString()));
            MainMenu.Items.Add(true, new GraphItem("Create New Profile...", "new"));
            MainMenu.SetHighlight(0);
            CurrentMenuChoice = new IntRange(0, 0, MainMenu.Items.Count - 1);
        }

        private void ResetSubMenu()
        {
            SubMenu.Items.Clear();
            SubMenu.Items.Columns.Clear();
            SubMenu.Items.Columns.Add("OPTIONS");
            SubMenu.Items.Add(true, new GraphItem("Rename","ren"),new GraphItem("Delete","del"),new GraphItem("Clear Stats","clr"));
            SubMenu.CalculateBlocks();
        }
    }
}
