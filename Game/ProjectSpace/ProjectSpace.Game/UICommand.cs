using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutpostOmega.Game
{
    /// <summary>
    /// Contains commands that get sent to the UI to execute certain functions on the frontend
    /// </summary>
    public enum UICommand
    {
        Open,
        Close,
        Highlight,
        HighlightStop
    }
}
