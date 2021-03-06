using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace SlaamMono
{
    /// <summary>
    /// Manages the feed you see at the bottom of menus within the game.
    /// </summary>
    static class FeedManager
    {
        #region Variables

        public const float BarMovement = (15f / 10f);
        public const float TextMovement = (3f / 20f);

        public static bool FeedsActive = false;

        private static Rectangle FeedRect = new Rectangle(0, 975, 0, 54);

        private static float TextX = 1350f;

        private static string FeedText;

        #endregion

        #region Constructor

        public static void InitializeFeeds(string str)
        {
            FeedsActive = true;
            FeedText = str;
            TextX = 1350;
            FeedRect.Width = 0;
        }

        #endregion

        #region Update

        public static void Update()
        {
            if (FeedsActive)
            {
                if (FeedRect.Width >= 1280)
                {
                    TextX -= (FPSManager.MovementFactor * TextMovement);
                    if (TextX <= FeedText.Length * -8)
                        TextX = 1350;
                }
                else
                {
                    FeedRect.Width += (int)(FPSManager.MovementFactor * BarMovement);
                }
            }
        }

        #endregion

        #region Draw

        public static void Draw(SpriteBatch batch)
        {
#if !ZUNE
            if (FeedsActive)
            {
                batch.Draw(Resources.Feedbar.Texture, FeedRect, Color.White);
                Resources.DrawString(FeedText, new Vector2(TextX, FeedRect.Y + 32), Resources.SegoeUIx14pt, FontAlignment.Left, Color.White,true);
            }
#endif
        }

        #endregion
    }
}
