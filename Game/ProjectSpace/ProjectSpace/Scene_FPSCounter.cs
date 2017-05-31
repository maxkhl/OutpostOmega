using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutpostOmega
{
    /// <summary>
    /// This part handles the FPS counter that should be visible everywhere when enabled
    /// </summary>
    partial class Scene
    {
        private Gwen.Control.TextBox tb_fpsCounter;

        private void InitFPS()
        {
            tb_fpsCounter = new Gwen.Control.TextBox(this.Canvas);
            tb_fpsCounter.Font = new Gwen.Font(this.renderer, "Arial", 8);
            tb_fpsCounter.X = 0;
            tb_fpsCounter.Y = 0;
        }

        private void UpdateFPS(float ElapsedTime)
        {
            var RPS = (int)Math.Round((double)(1000 / ElapsedTime));
            tb_fpsCounter.Text = RPS.ToString();
        }
    }
}
