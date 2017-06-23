using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutpostOmega.Game.Tools
{
    /// <summary>
    /// Contains helper functions for the keys enum
    /// </summary>
    public static class KeysHelper
    {
        #region ConvertIn
        /// <summary>
        /// Convert from opentk keyboard key to oo key
        /// </summary>
        /// <param name="otkKey">Opentk keyboard key</param>
        /// <returns>OO key</returns>
        public static Keys ConvertIn(OpenTK.Input.Key otkKey)
        {
            return ConvertIn((byte)otkKey);
        }

        /// <summary>
        /// Convert from opentk mouse key to oo key
        /// </summary>
        /// <param name="otkKey">Opentk mouse key</param>
        /// <returns>OO key</returns>
        public static Keys ConvertIn(OpenTK.Input.MouseButton otkKey)
        {
            return ConvertIn((byte)(((byte)otkKey) + 200));
        }

        /// <summary>
        /// Convert from string-key to oo key
        /// </summary>
        /// <param name="KeyNumber">KeyNumber</param>
        /// <returns>OO key</returns>
        public static Keys ConvertIn(byte KeyNumber)
        {
            return (Keys)KeyNumber;
        }
        #endregion

        #region ConvertOut

        /// <summary>
        /// Convert from oo key to opentk key mouse/keyboard
        /// </summary>
        /// <param name="ooKey">Opentk keyboard key</param>
        /// <returns>Typle of enumtype and result (needs to be casted to the type)</returns>
        public static Tuple<Type, object> ConvertOut(Keys ooKey)
        {
            return ConvertOut((byte)ooKey);
        }

        /// <summary>
        /// Convert from string-key to oo key
        /// </summary>
        /// <param name="KeyNumber">KeyNumber</param>
        /// <returns>OO key</returns>
        public static Tuple<Type, object> ConvertOut(byte KeyNumber)
        {
            var isMouse = GetInputDevice((Keys)KeyNumber) == InputDevice.Mouse;
            return new Tuple<Type, object>(
                isMouse ? typeof(OpenTK.Input.MouseButton) : typeof(OpenTK.Input.Key),
                isMouse ? (object)(OpenTK.Input.MouseButton)(KeyNumber - 200) : (object)(OpenTK.Input.Key)KeyNumber);
        }
        #endregion

        /// <summary>
        /// Returns the input device of the given key
        /// </summary>
        /// <param name="Key">OO Key</param>
        /// <returns>According input device</returns>
        public static InputDevice GetInputDevice(Keys Key)
        {
            return (byte)Key >= 200 ? InputDevice.Mouse : InputDevice.Keyboard;
        }
    }

    /// <summary>
    /// Combines keyboard and mousekeys. Use KeysHelper to translate from one to the other
    /// </summary>
    public enum Keys : byte
    {
        //
        // Zusammenfassung:
        //     The left mouse button.
        LeftMouseButton = 200,
        //
        // Zusammenfassung:
        //     The middle mouse button.
        MiddleMouseButton = 201,
        //
        // Zusammenfassung:
        //     The right mouse button.
        RightMouseButton = 202,
        //
        // Zusammenfassung:
        //     The first extra mouse button.
        MouseButton1 = 203,
        //
        // Zusammenfassung:
        //     The second extra mouse button.
        MouseButton2 = 204,
        //
        // Zusammenfassung:
        //     The third extra mouse button.
        MouseButton3 = 205,
        //
        // Zusammenfassung:
        //     The fourth extra mouse button.
        MouseButton4 = 206,
        //
        // Zusammenfassung:
        //     The fifth extra mouse button.
        MouseButton5 = 207,
        //
        // Zusammenfassung:
        //     The sixth extra mouse button.
        MouseButton6 = 208,
        //
        // Zusammenfassung:
        //     The seventh extra mouse button.
        MouseButton7 = 209,
        //
        // Zusammenfassung:
        //     The eigth extra mouse button.
        MouseButton8 = 210,
        //
        // Zusammenfassung:
        //     The ninth extra mouse button.
        MouseButton9 = 211,
        //
        // Zusammenfassung:
        //     Indicates the last available mouse button.
        LastMouseButton = 212,



        //
        // Zusammenfassung:
        //     A key outside the known keys.
        Unknown = 0,
        //
        // Zusammenfassung:
        //     The left shift key.
        ShiftLeft = 1,
        //
        // Zusammenfassung:
        //     The left shift key (equivalent to ShiftLeft).
        LShift = 1,
        //
        // Zusammenfassung:
        //     The right shift key.
        ShiftRight = 2,
        //
        // Zusammenfassung:
        //     The right shift key (equivalent to ShiftRight).
        RShift = 2,
        //
        // Zusammenfassung:
        //     The left control key.
        ControlLeft = 3,
        //
        // Zusammenfassung:
        //     The left control key (equivalent to ControlLeft).
        LControl = 3,
        //
        // Zusammenfassung:
        //     The right control key.
        ControlRight = 4,
        //
        // Zusammenfassung:
        //     The right control key (equivalent to ControlRight).
        RControl = 4,
        //
        // Zusammenfassung:
        //     The left alt key.
        AltLeft = 5,
        //
        // Zusammenfassung:
        //     The left alt key (equivalent to AltLeft.
        LAlt = 5,
        //
        // Zusammenfassung:
        //     The right alt key.
        AltRight = 6,
        //
        // Zusammenfassung:
        //     The right alt key (equivalent to AltRight).
        RAlt = 6,
        //
        // Zusammenfassung:
        //     The left win key.
        WinLeft = 7,
        //
        // Zusammenfassung:
        //     The left win key (equivalent to WinLeft).
        LWin = 7,
        //
        // Zusammenfassung:
        //     The right win key.
        WinRight = 8,
        //
        // Zusammenfassung:
        //     The right win key (equivalent to WinRight).
        RWin = 8,
        //
        // Zusammenfassung:
        //     The menu key.
        Menu = 9,
        //
        // Zusammenfassung:
        //     The F1 key.
        F1 = 10,
        //
        // Zusammenfassung:
        //     The F2 key.
        F2 = 11,
        //
        // Zusammenfassung:
        //     The F3 key.
        F3 = 12,
        //
        // Zusammenfassung:
        //     The F4 key.
        F4 = 13,
        //
        // Zusammenfassung:
        //     The F5 key.
        F5 = 14,
        //
        // Zusammenfassung:
        //     The F6 key.
        F6 = 15,
        //
        // Zusammenfassung:
        //     The F7 key.
        F7 = 16,
        //
        // Zusammenfassung:
        //     The F8 key.
        F8 = 17,
        //
        // Zusammenfassung:
        //     The F9 key.
        F9 = 18,
        //
        // Zusammenfassung:
        //     The F10 key.
        F10 = 19,
        //
        // Zusammenfassung:
        //     The F11 key.
        F11 = 20,
        //
        // Zusammenfassung:
        //     The F12 key.
        F12 = 21,
        //
        // Zusammenfassung:
        //     The F13 key.
        F13 = 22,
        //
        // Zusammenfassung:
        //     The F14 key.
        F14 = 23,
        //
        // Zusammenfassung:
        //     The F15 key.
        F15 = 24,
        //
        // Zusammenfassung:
        //     The F16 key.
        F16 = 25,
        //
        // Zusammenfassung:
        //     The F17 key.
        F17 = 26,
        //
        // Zusammenfassung:
        //     The F18 key.
        F18 = 27,
        //
        // Zusammenfassung:
        //     The F19 key.
        F19 = 28,
        //
        // Zusammenfassung:
        //     The F20 key.
        F20 = 29,
        //
        // Zusammenfassung:
        //     The F21 key.
        F21 = 30,
        //
        // Zusammenfassung:
        //     The F22 key.
        F22 = 31,
        //
        // Zusammenfassung:
        //     The F23 key.
        F23 = 32,
        //
        // Zusammenfassung:
        //     The F24 key.
        F24 = 33,
        //
        // Zusammenfassung:
        //     The F25 key.
        F25 = 34,
        //
        // Zusammenfassung:
        //     The F26 key.
        F26 = 35,
        //
        // Zusammenfassung:
        //     The F27 key.
        F27 = 36,
        //
        // Zusammenfassung:
        //     The F28 key.
        F28 = 37,
        //
        // Zusammenfassung:
        //     The F29 key.
        F29 = 38,
        //
        // Zusammenfassung:
        //     The F30 key.
        F30 = 39,
        //
        // Zusammenfassung:
        //     The F31 key.
        F31 = 40,
        //
        // Zusammenfassung:
        //     The F32 key.
        F32 = 41,
        //
        // Zusammenfassung:
        //     The F33 key.
        F33 = 42,
        //
        // Zusammenfassung:
        //     The F34 key.
        F34 = 43,
        //
        // Zusammenfassung:
        //     The F35 key.
        F35 = 44,
        //
        // Zusammenfassung:
        //     The up arrow key.
        Up = 45,
        //
        // Zusammenfassung:
        //     The down arrow key.
        Down = 46,
        //
        // Zusammenfassung:
        //     The left arrow key.
        Left = 47,
        //
        // Zusammenfassung:
        //     The right arrow key.
        Right = 48,
        //
        // Zusammenfassung:
        //     The enter key.
        Enter = 49,
        //
        // Zusammenfassung:
        //     The escape key.
        Escape = 50,
        //
        // Zusammenfassung:
        //     The space key.
        Space = 51,
        //
        // Zusammenfassung:
        //     The tab key.
        Tab = 52,
        //
        // Zusammenfassung:
        //     The backspace key.
        BackSpace = 53,
        //
        // Zusammenfassung:
        //     The backspace key (equivalent to BackSpace).
        Back = 53,
        //
        // Zusammenfassung:
        //     The insert key.
        Insert = 54,
        //
        // Zusammenfassung:
        //     The delete key.
        Delete = 55,
        //
        // Zusammenfassung:
        //     The page up key.
        PageUp = 56,
        //
        // Zusammenfassung:
        //     The page down key.
        PageDown = 57,
        //
        // Zusammenfassung:
        //     The home key.
        Home = 58,
        //
        // Zusammenfassung:
        //     The end key.
        End = 59,
        //
        // Zusammenfassung:
        //     The caps lock key.
        CapsLock = 60,
        //
        // Zusammenfassung:
        //     The scroll lock key.
        ScrollLock = 61,
        //
        // Zusammenfassung:
        //     The print screen key.
        PrintScreen = 62,
        //
        // Zusammenfassung:
        //     The pause key.
        Pause = 63,
        //
        // Zusammenfassung:
        //     The num lock key.
        NumLock = 64,
        //
        // Zusammenfassung:
        //     The clear key (Keypad5 with NumLock disabled, on typical keyboards).
        Clear = 65,
        //
        // Zusammenfassung:
        //     The sleep key.
        Sleep = 66,
        //
        // Zusammenfassung:
        //     The keypad 0 key.
        Keypad0 = 67,
        //
        // Zusammenfassung:
        //     The keypad 1 key.
        Keypad1 = 68,
        //
        // Zusammenfassung:
        //     The keypad 2 key.
        Keypad2 = 69,
        //
        // Zusammenfassung:
        //     The keypad 3 key.
        Keypad3 = 70,
        //
        // Zusammenfassung:
        //     The keypad 4 key.
        Keypad4 = 71,
        //
        // Zusammenfassung:
        //     The keypad 5 key.
        Keypad5 = 72,
        //
        // Zusammenfassung:
        //     The keypad 6 key.
        Keypad6 = 73,
        //
        // Zusammenfassung:
        //     The keypad 7 key.
        Keypad7 = 74,
        //
        // Zusammenfassung:
        //     The keypad 8 key.
        Keypad8 = 75,
        //
        // Zusammenfassung:
        //     The keypad 9 key.
        Keypad9 = 76,
        //
        // Zusammenfassung:
        //     The keypad divide key.
        KeypadDivide = 77,
        //
        // Zusammenfassung:
        //     The keypad multiply key.
        KeypadMultiply = 78,
        //
        // Zusammenfassung:
        //     The keypad subtract key.
        KeypadSubtract = 79,
        //
        // Zusammenfassung:
        //     The keypad minus key (equivalent to KeypadSubtract).
        KeypadMinus = 79,
        //
        // Zusammenfassung:
        //     The keypad add key.
        KeypadAdd = 80,
        //
        // Zusammenfassung:
        //     The keypad plus key (equivalent to KeypadAdd).
        KeypadPlus = 80,
        //
        // Zusammenfassung:
        //     The keypad decimal key.
        KeypadDecimal = 81,
        //
        // Zusammenfassung:
        //     The keypad period key (equivalent to KeypadDecimal).
        KeypadPeriod = 81,
        //
        // Zusammenfassung:
        //     The keypad enter key.
        KeypadEnter = 82,
        //
        // Zusammenfassung:
        //     The A key.
        A = 83,
        //
        // Zusammenfassung:
        //     The B key.
        B = 84,
        //
        // Zusammenfassung:
        //     The C key.
        C = 85,
        //
        // Zusammenfassung:
        //     The D key.
        D = 86,
        //
        // Zusammenfassung:
        //     The E key.
        E = 87,
        //
        // Zusammenfassung:
        //     The F key.
        F = 88,
        //
        // Zusammenfassung:
        //     The G key.
        G = 89,
        //
        // Zusammenfassung:
        //     The H key.
        H = 90,
        //
        // Zusammenfassung:
        //     The I key.
        I = 91,
        //
        // Zusammenfassung:
        //     The J key.
        J = 92,
        //
        // Zusammenfassung:
        //     The K key.
        K = 93,
        //
        // Zusammenfassung:
        //     The L key.
        L = 94,
        //
        // Zusammenfassung:
        //     The M key.
        M = 95,
        //
        // Zusammenfassung:
        //     The N key.
        N = 96,
        //
        // Zusammenfassung:
        //     The O key.
        O = 97,
        //
        // Zusammenfassung:
        //     The P key.
        P = 98,
        //
        // Zusammenfassung:
        //     The Q key.
        Q = 99,
        //
        // Zusammenfassung:
        //     The R key.
        R = 100,
        //
        // Zusammenfassung:
        //     The S key.
        S = 101,
        //
        // Zusammenfassung:
        //     The T key.
        T = 102,
        //
        // Zusammenfassung:
        //     The U key.
        U = 103,
        //
        // Zusammenfassung:
        //     The V key.
        V = 104,
        //
        // Zusammenfassung:
        //     The W key.
        W = 105,
        //
        // Zusammenfassung:
        //     The X key.
        X = 106,
        //
        // Zusammenfassung:
        //     The Y key.
        Y = 107,
        //
        // Zusammenfassung:
        //     The Z key.
        Z = 108,
        //
        // Zusammenfassung:
        //     The number 0 key.
        Number0 = 109,
        //
        // Zusammenfassung:
        //     The number 1 key.
        Number1 = 110,
        //
        // Zusammenfassung:
        //     The number 2 key.
        Number2 = 111,
        //
        // Zusammenfassung:
        //     The number 3 key.
        Number3 = 112,
        //
        // Zusammenfassung:
        //     The number 4 key.
        Number4 = 113,
        //
        // Zusammenfassung:
        //     The number 5 key.
        Number5 = 114,
        //
        // Zusammenfassung:
        //     The number 6 key.
        Number6 = 115,
        //
        // Zusammenfassung:
        //     The number 7 key.
        Number7 = 116,
        //
        // Zusammenfassung:
        //     The number 8 key.
        Number8 = 117,
        //
        // Zusammenfassung:
        //     The number 9 key.
        Number9 = 118,
        //
        // Zusammenfassung:
        //     The tilde key.
        Tilde = 119,
        //
        // Zusammenfassung:
        //     The grave key (equivaent to Tilde).
        Grave = 119,
        //
        // Zusammenfassung:
        //     The minus key.
        Minus = 120,
        //
        // Zusammenfassung:
        //     The plus key.
        Plus = 121,
        //
        // Zusammenfassung:
        //     The left bracket key.
        BracketLeft = 122,
        //
        // Zusammenfassung:
        //     The left bracket key (equivalent to BracketLeft).
        LBracket = 122,
        //
        // Zusammenfassung:
        //     The right bracket key.
        BracketRight = 123,
        //
        // Zusammenfassung:
        //     The right bracket key (equivalent to BracketRight).
        RBracket = 123,
        //
        // Zusammenfassung:
        //     The semicolon key.
        Semicolon = 124,
        //
        // Zusammenfassung:
        //     The quote key.
        Quote = 125,
        //
        // Zusammenfassung:
        //     The comma key.
        Comma = 126,
        //
        // Zusammenfassung:
        //     The period key.
        Period = 127,
        //
        // Zusammenfassung:
        //     The slash key.
        Slash = 128,
        //
        // Zusammenfassung:
        //     The backslash key.
        BackSlash = 129,
        //
        // Zusammenfassung:
        //     The secondary backslash key.
        NonUSBackSlash = 130,
        //
        // Zusammenfassung:
        //     Indicates the last available keyboard key.
        LastKey = 131
    }
}
