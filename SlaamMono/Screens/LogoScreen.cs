using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace SlaamMono
{
    public class LogoScreen : IScreen
    {
        #region Variables

        private Timer displaytime = new Timer(new TimeSpan(0, 0, 4));

        private Transition LogoColor = new Transition(null, new Vector2(0), new Vector2(255), TimeSpan.FromSeconds(1));
        private Boolean hasShown = false;

        #endregion

        #region Constructor

        public LogoScreen()
        {

        }

        public void Initialize()
        {

        }

        #endregion

        #region Update

        public void Update()
        {
            if (!hasShown)
            {
                LogoColor.Update(FPSManager.MovementFactorTimeSpan);
                if (LogoColor.IsFinished())
                {
                    hasShown = true;
                    displaytime.Reset();
                }
            }
            else
            {
                displaytime.Update(FPSManager.MovementFactorTimeSpan);
                LogoColor.Update(FPSManager.MovementFactorTimeSpan);
                if (displaytime.Active)
                {
                    LogoColor.Reverse(null);
                }
                if (LogoColor.Goal.X == 0 && LogoColor.IsFinished())
                {
                    // if (ProfileManager.FirstTime)
                    //  ScreenHelper.ChangeScreen(new FirstTimeScreen());
                    // else
                    ScreenHelper.ChangeScreen(MenuScreen.Instance);
                }
            }
        }

        #endregion

        #region Draw

        public void Draw(SpriteBatch batch)
        {
            byte alpha = (byte)LogoColor.Position.X;
            batch.Draw(Resources.ZibithLogoBG.Texture, new Rectangle(0, 0, GameGlobals.DRAWING_GAME_WIDTH, GameGlobals.DRAWING_GAME_HEIGHT), Color.White);
            batch.Draw(Resources.ZibithLogo.Texture, new Vector2(GameGlobals.DRAWING_GAME_WIDTH / 2 - (Resources.ZibithLogo.Width / 2), GameGlobals.DRAWING_GAME_HEIGHT / 2 - (Resources.ZibithLogo.Height / 2)), new Color((byte)255, (byte)255, (byte)255, (byte)alpha));
        }

        #endregion

        #region Dispose

        public void Dispose()
        {
            Resources.ZibithLogo.Dispose();
            Resources.ZibithLogoBG.Dispose();
        }

        #endregion
    }
}
