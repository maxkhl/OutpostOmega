using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OutpostOmega.Data.converter
{
    /// <summary>
    /// Contains options that are used to modify the behaviour of the converter-class
    /// </summary>
    public struct Converter_Options
    {
        /// <summary>
        /// Defines if the objects properties are already being handled by this converter
        /// </summary>
        public bool HandlesProperties { get; set; }

        /// <summary>
        /// Defines if the Check-Methods should pass subtypes aswell
        /// </summary>
        public bool ConvertSubtypes { get; set; }

    }
}
