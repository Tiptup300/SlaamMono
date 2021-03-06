using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using SlaamMono.Input;

namespace SlaamMono
{
    class StatsScreen : IScreen
    {
        #region Variables

        public MatchScoreCollection ScoreCollection;

        private IntRange CurrentPage = new IntRange(0, 0, 2);
        private IntRange CurrentChar;

        private StatsBoard PlayerStats;
        private StatsBoard Kills;
        private StatsBoard PvP;

#if !ZUNE
        private Rectangle StatsRect = new Rectangle(175, 290, 930, 700);
        public const int MAX_HIGHSCORES = 29;
#else
        private Rectangle StatsRect = new Rectangle(20, 110, GameGlobals.DRAWING_GAME_WIDTH - 40, GameGlobals.DRAWING_GAME_HEIGHT);

        public const int MAX_HIGHSCORES = 5;
#endif
        private Color StatsCol = new Color(0, 0, 0, 125);


        #endregion

        #region Constructor

        public StatsScreen(MatchScoreCollection scorecollection)
        {
            ScoreCollection = scorecollection;
        }

        public void Initialize()
        {
            BackgroundManager.ChangeBG(BackgroundManager.BackgroundType.Menu);
            if (ScoreCollection.ParentGameScreen.ThisGameType == GameType.Classic)
                PlayerStats = new NormalStatsBoard(ScoreCollection, StatsRect, StatsCol);
            else if (ScoreCollection.ParentGameScreen.ThisGameType == GameType.Spree || ScoreCollection.ParentGameScreen.ThisGameType == GameType.TimedSpree)
                PlayerStats = new SpreeStatsBoard(ScoreCollection, StatsRect, StatsCol);
            else if (ScoreCollection.ParentGameScreen.ThisGameType == GameType.Survival)
                PlayerStats = new SurvivalStatsBoard(ScoreCollection, StatsRect, StatsCol,MAX_HIGHSCORES);

            PlayerStats.CalculateStats();
            PlayerStats.ConstructGraph(0);

            if (ScoreCollection.ParentGameScreen.ThisGameType != GameType.Survival)
            {

                Kills = new KillsStatsBoard(ScoreCollection, StatsRect, StatsCol);
                Kills.CalculateStats();
                Kills.ConstructGraph(0);

                PvP = new PvPStatsBoard(ScoreCollection, StatsRect, StatsCol);
                PvP.CalculateStats();
                PvP.ConstructGraph(0);

                string first = "First: ",
                       second = "Second: ",
                       third = "Third: ";

                for (int x = 0; x < PlayerStats.MainBoard.Items.Count; x++)
                {
                    if (PlayerStats.MainBoard.Items[x].Details[1] == "First")
                        first += PlayerStats.MainBoard.Items[x].Details[0] + ", ";

                    if (PlayerStats.MainBoard.Items[x].Details[1] == "Second")
                        second += PlayerStats.MainBoard.Items[x].Details[0] + ", ";

                    if (PlayerStats.MainBoard.Items[x].Details[1] == "Third")
                        third += PlayerStats.MainBoard.Items[x].Details[0] + ", ";

                }

                if (second == "Second: ")
                    second = "";
                else
                    second = second.Substring(0, second.Length - 2);

                if (third == "Third: ")
                    third = "";
                else
                    third = third.Substring(0, third.Length - 2);

                FeedManager.InitializeFeeds(first.Substring(0, first.Length - 2) + " " + second + " " + third);

                CurrentChar = new IntRange(0, 0, PvP.MainBoard.Items.Count - 1);

            }
        }

        #endregion

        #region Update

        public void Update()
        {
            BackgroundManager.SetRotation(1f);

            if (ScoreCollection.ParentGameScreen.ThisGameType != GameType.Survival)
            {

                if (InputComponent.Players[0].PressedLeft)
                {
                    CurrentPage.Sub(1);
                }

                if (InputComponent.Players[0].PressedRight)
                {
                    CurrentPage.Add(1);
                }

                if (CurrentPage.Value == 2)
                {
                    if (InputComponent.Players[0].PressedUp)
                    {
                        CurrentChar.Sub(1);
                        PvP.ConstructGraph(CurrentChar.Value);
                    }
                    else if (InputComponent.Players[0].PressedDown)
                    {
                        CurrentChar.Add(1);
                        PvP.ConstructGraph(CurrentChar.Value);
                    }
                }

            }

            if (InputComponent.Players[0].PressedAction)
            {
                ScreenHelper.ChangeScreen(MenuScreen.Instance);
            }
        }

        #endregion

        #region Draw

        public void Draw(SpriteBatch batch)
        {
            Vector2 Statsboard = new Vector2((GameGlobals.DRAWING_GAME_WIDTH / 2) - Resources.StatsBoard.Width / 2, (GameGlobals.DRAWING_GAME_HEIGHT / 2) - Resources.StatsBoard.Height / 2);
            //MainBG.Draw(batch);
            for (int x = 0; x < 3; x++)
            {
                batch.Draw(Resources.StatsButtons[x], Statsboard, (x == CurrentPage.Value ? Color.LightSkyBlue : (ScoreCollection.ParentGameScreen.ThisGameType == GameType.Survival) ? Color.DarkGray : Color.White));
            }
            batch.Draw(Resources.StatsBoard.Texture, Statsboard, Color.White);
            //DrawingButton.Draw(batch);
            if (CurrentPage.Value == 0)
                PlayerStats.MainBoard.Draw(batch);
            else if (CurrentPage.Value == 1)
                Kills.MainBoard.Draw(batch);
            else
                PvP.MainBoard.Draw(batch);

#if !ZUNE
            Resources.DrawString(ScoreCollection.ParentGameScreen.ThisGameType != GameType.Survival ? "Player Stats" : "Survival High Scores", new Vector2(131+Statsboard.X, 255), Resources.SegoeUIx14pt, FontAlignment.Center, Color.White, true);
            Resources.DrawString("Kills", new Vector2(352+Statsboard.X, 255), Resources.SegoeUIx14pt, FontAlignment.Center, Color.White, true);
            Resources.DrawString("Player Vs. Player", new Vector2(573+Statsboard.X, 255), Resources.SegoeUIx14pt, FontAlignment.Center, Color.White, true);
#endif
            }

        #endregion

        #region Dispose

        public void Dispose()
        {

        }

        #endregion
    }
}
