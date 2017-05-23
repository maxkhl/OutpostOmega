using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace OutpostOmega
{
    /// <summary>
    /// MainMenu cursor functions
    /// </summary>
    partial class MainGame
    {
        /// <summary>
        /// Structure for storing mouse information
        /// </summary>
        public struct MouseDataStructure
        {
            public Vector2 LastPosition;
        }

        /// <summary>
        /// Stored mouse information
        /// </summary>
        public MouseDataStructure MouseData;

        /// <summary>
        /// Locks and hides the cursor
        /// </summary>
        public void LockCursor()
        {
            if (SceneManager.Active == null)
                return;

            this.MouseData.LastPosition = new Vector2(OpenTK.Input.Mouse.GetState().X, OpenTK.Input.Mouse.GetState().Y);

            this.CursorVisible = false;

            ResetCursor(null, null);

            this.UpdateFrame -= ResetCursor;
            this.UpdateFrame += ResetCursor;
        }

        /// <summary>
        /// Unlocks the cursor and makes it visible
        /// </summary>
        public void UnlockCursor()
        {
            if (SceneManager.Active == null)
                return;

            this.CursorVisible = true;

            this.UpdateFrame -= ResetCursor;
        }

        private void ResetCursor(object sender, FrameEventArgs e)
        {
            OpenTK.Input.Mouse.SetPosition(this.Bounds.Left + this.Bounds.Width / 2, this.Bounds.Top + this.Bounds.Height / 2);
            this.MouseData.LastPosition = new Vector2(OpenTK.Input.Mouse.GetState().X, OpenTK.Input.Mouse.GetState().Y);
        }

        protected override void OnFocusedChanged(EventArgs e)
        {
            if (!this.CursorVisible && !this.Focused)
                UnlockCursor();

            if (!this.CursorVisible && this.Focused)
            {
                ResetCursor(this, null);
            }
            base.OnFocusedChanged(e);
        }
    }
}
