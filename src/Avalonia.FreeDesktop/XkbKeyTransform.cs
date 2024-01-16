using Avalonia.Input;

namespace Avalonia.FreeDesktop
{
    internal static class XkbKeyTransform
    {
        public static Key ConvertKey(XkbKey key) => key switch
        {
            XkbKey.Cancel => Key.Cancel,
            XkbKey.BackSpace => Key.Back,
            XkbKey.Tab => Key.Tab,
            XkbKey.Linefeed => Key.LineFeed,
            XkbKey.Clear => Key.Clear,
            XkbKey.Return => Key.Return,
            XkbKey.KP_Enter => Key.Return,
            XkbKey.Pause => Key.Pause,
            XkbKey.Caps_Lock => Key.CapsLock,
            // XkbKey.? => Key.HangulMode,
            // XkbKey.? => Key.JunjaMode,
            // XkbKey.? => Key.FinalMode,
            // XkbKey.? => Key.KanjiMode,
            XkbKey.Escape => Key.Escape,
            // XkbKey.? => Key.ImeConvert,
            // XkbKey.? => Key.ImeNonConvert,
            // XkbKey.? => Key.ImeAccept,
            // XkbKey.? => Key.ImeModeChange,
            XkbKey.space => Key.Space,
            XkbKey.Prior => Key.Prior,
            XkbKey.KP_Prior => Key.Prior,
            XkbKey.Page_Down => Key.PageDown,
            XkbKey.KP_Page_Down => Key.PageDown,
            XkbKey.End => Key.End,
            XkbKey.KP_End => Key.End,
            XkbKey.Home => Key.Home,
            XkbKey.KP_Home => Key.Home,
            XkbKey.Left => Key.Left,
            XkbKey.KP_Left => Key.Left,
            XkbKey.Up => Key.Up,
            XkbKey.KP_Up => Key.Up,
            XkbKey.Right => Key.Right,
            XkbKey.KP_Right => Key.Right,
            XkbKey.Down => Key.Down,
            XkbKey.KP_Down => Key.Down,
            XkbKey.Select => Key.Select,
            XkbKey.Print => Key.Print,
            XkbKey.Execute => Key.Execute,
            // XkbKey.? => Key.Snapshot,
            XkbKey.Insert => Key.Insert,
            XkbKey.KP_Insert => Key.Insert,
            XkbKey.Delete => Key.Delete,
            XkbKey.KP_Delete => Key.Delete,
            XkbKey.Help => Key.Help,
            XkbKey.A => Key.A,
            XkbKey.B => Key.B,
            XkbKey.C => Key.C,
            XkbKey.D => Key.D,
            XkbKey.E => Key.E,
            XkbKey.F => Key.F,
            XkbKey.G => Key.G,
            XkbKey.H => Key.H,
            XkbKey.I => Key.I,
            XkbKey.J => Key.J,
            XkbKey.K => Key.K,
            XkbKey.L => Key.L,
            XkbKey.M => Key.M,
            XkbKey.N => Key.N,
            XkbKey.O => Key.O,
            XkbKey.P => Key.P,
            XkbKey.Q => Key.Q,
            XkbKey.R => Key.R,
            XkbKey.S => Key.S,
            XkbKey.T => Key.T,
            XkbKey.U => Key.U,
            XkbKey.V => Key.V,
            XkbKey.W => Key.W,
            XkbKey.X => Key.X,
            XkbKey.Y => Key.Y,
            XkbKey.Z => Key.Z,
            XkbKey.a => Key.A,
            XkbKey.b => Key.B,
            XkbKey.c => Key.C,
            XkbKey.d => Key.D,
            XkbKey.e => Key.E,
            XkbKey.f => Key.F,
            XkbKey.g => Key.G,
            XkbKey.h => Key.H,
            XkbKey.i => Key.I,
            XkbKey.j => Key.J,
            XkbKey.k => Key.K,
            XkbKey.l => Key.L,
            XkbKey.m => Key.M,
            XkbKey.n => Key.N,
            XkbKey.o => Key.O,
            XkbKey.p => Key.P,
            XkbKey.q => Key.Q,
            XkbKey.r => Key.R,
            XkbKey.s => Key.S,
            XkbKey.t => Key.T,
            XkbKey.u => Key.U,
            XkbKey.v => Key.V,
            XkbKey.w => Key.W,
            XkbKey.x => Key.X,
            XkbKey.y => Key.Y,
            XkbKey.z => Key.Z,
            XkbKey.Super_L => Key.LWin ,
            XkbKey.Super_R => Key.RWin ,
            XkbKey.Meta_L => Key.LWin,
            XkbKey.Meta_R => Key.RWin,
            XkbKey.Menu => Key.Apps,
            // XkbKey.? => Key.Sleep,
            XkbKey.KP_0 => Key.NumPad0,
            XkbKey.KP_1 => Key.NumPad1,
            XkbKey.KP_2 => Key.NumPad2,
            XkbKey.KP_3 => Key.NumPad3,
            XkbKey.KP_4 => Key.NumPad4,
            XkbKey.KP_5 => Key.NumPad5,
            XkbKey.KP_6 => Key.NumPad6,
            XkbKey.KP_7 => Key.NumPad7,
            XkbKey.KP_8 => Key.NumPad8,
            XkbKey.KP_9 => Key.NumPad9,
            XkbKey.multiply => Key.Multiply,
            XkbKey.KP_Multiply => Key.Multiply,
            XkbKey.KP_Add => Key.Add,
            // XkbKey.? => Key.Separator,
            XkbKey.KP_Subtract => Key.Subtract,
            XkbKey.KP_Decimal => Key.Decimal,
            XkbKey.KP_Divide => Key.Divide,
            XkbKey.F1 => Key.F1,
            XkbKey.F2 => Key.F2,
            XkbKey.F3 => Key.F3,
            XkbKey.F4 => Key.F4,
            XkbKey.F5 => Key.F5,
            XkbKey.F6 => Key.F6,
            XkbKey.F7 => Key.F7,
            XkbKey.F8 => Key.F8,
            XkbKey.F9 => Key.F9,
            XkbKey.F10 => Key.F10,
            XkbKey.F11 => Key.F11,
            XkbKey.F12 => Key.F12,
            XkbKey.L3 => Key.F13,
            XkbKey.F14 => Key.F14,
            XkbKey.L5 => Key.F15,
            XkbKey.F16 => Key.F16,
            XkbKey.F17 => Key.F17,
            XkbKey.L8 => Key.F18,
            XkbKey.L9 => Key.F19,
            XkbKey.L10 => Key.F20,
            XkbKey.R1 => Key.F21,
            XkbKey.R2 => Key.F22,
            XkbKey.F23 => Key.F23,
            XkbKey.R4 => Key.F24,
            XkbKey.Num_Lock => Key.NumLock,
            XkbKey.Scroll_Lock => Key.Scroll,
            XkbKey.Shift_L => Key.LeftShift,
            XkbKey.Shift_R => Key.RightShift,
            XkbKey.Control_L => Key.LeftCtrl,
            XkbKey.Control_R => Key.RightCtrl,
            XkbKey.Alt_L => Key.LeftAlt,
            XkbKey.Alt_R => Key.RightAlt,
            // XkbKey.? => Key.BrowserBack,
            // XkbKey.? => Key.BrowserForward,
            // XkbKey.? => Key.BrowserRefresh,
            // XkbKey.? => Key.BrowserStop,
            // XkbKey.? => Key.BrowserSearch,
            // XkbKey.? => Key.BrowserFavorites,
            // XkbKey.? => Key.BrowserHome,
            // XkbKey.? => Key.VolumeMute,
            // XkbKey.? => Key.VolumeDown,
            // XkbKey.? => Key.VolumeUp,
            // XkbKey.? => Key.MediaNextTrack,
            // XkbKey.? => Key.MediaPreviousTrack,
            // XkbKey.? => Key.MediaStop,
            // XkbKey.? => Key.MediaPlayPause,
            // XkbKey.? => Key.LaunchMail,
            // XkbKey.? => Key.SelectMedia,
            // XkbKey.? => Key.LaunchApplication1,
            // XkbKey.? => Key.LaunchApplication2,
            XkbKey.minus => Key.OemMinus,
            XkbKey.underscore => Key.OemMinus,
            XkbKey.plus => Key.OemPlus,
            XkbKey.equal => Key.OemPlus,
            XkbKey.bracketleft => Key.OemOpenBrackets,
            XkbKey.braceleft => Key.OemOpenBrackets,
            XkbKey.bracketright => Key.OemCloseBrackets,
            XkbKey.braceright => Key.OemCloseBrackets,
            XkbKey.backslash => Key.OemPipe,
            XkbKey.bar => Key.OemPipe,
            XkbKey.semicolon => Key.OemSemicolon,
            XkbKey.colon => Key.OemSemicolon,
            XkbKey.apostrophe => Key.OemQuotes,
            XkbKey.quotedbl => Key.OemQuotes,
            XkbKey.comma => Key.OemComma,
            XkbKey.less => Key.OemComma,
            XkbKey.period => Key.OemPeriod,
            XkbKey.greater => Key.OemPeriod,
            XkbKey.slash => Key.Oem2,
            XkbKey.question => Key.Oem2,
            XkbKey.grave => Key.OemTilde,
            XkbKey.asciitilde => Key.OemTilde,
            XkbKey.XK_1 => Key.D1,
            XkbKey.exclam => Key.D1,
            XkbKey.XK_2 => Key.D2,
            XkbKey.at => Key.D2,
            XkbKey.XK_3 => Key.D3,
            XkbKey.numbersign => Key.D3,
            XkbKey.XK_4 => Key.D4,
            XkbKey.dollar => Key.D4,
            XkbKey.XK_5 => Key.D5,
            XkbKey.percent => Key.D5,
            XkbKey.XK_6 => Key.D6,
            XkbKey.asciicircum => Key.D6,
            XkbKey.XK_7 => Key.D7,
            XkbKey.ampersand => Key.D7,
            XkbKey.XK_8 => Key.D8,
            XkbKey.asterisk => Key.D8,
            XkbKey.XK_9 => Key.D9,
            XkbKey.parenleft => Key.D9,
            XkbKey.XK_0 => Key.D0,
            XkbKey.parenright => Key.D0,
            // XkbKey.? => Key.AbntC1,
            // XkbKey.? => Key.AbntC2,
            // XkbKey.? => Key.Oem8,
            // XkbKey.? => Key.Oem102,
            // XkbKey.? => Key.ImeProcessed,
            // XkbKey.? => Key.System,
            // XkbKey.? => Key.OemAttn,
            // XkbKey.? => Key.OemFinish,
            // XkbKey.? => Key.DbeHiragana,
            // XkbKey.? => Key.OemAuto,
            // XkbKey.? => Key.DbeDbcsChar,
            // XkbKey.? => Key.OemBackTab,
            // XkbKey.? => Key.Attn,
            // XkbKey.? => Key.DbeEnterWordRegisterMode,
            // XkbKey.? => Key.DbeEnterImeConfigureMode,
            // XkbKey.? => Key.EraseEof,
            // XkbKey.? => Key.Play,
            // XkbKey.? => Key.Zoom,
            // XkbKey.? => Key.NoName,
            // XkbKey.? => Key.DbeEnterDialogConversionMode,
            // XkbKey.? => Key.OemClear,
            // XkbKey.? => Key.DeadCharProcessed,
            _ => Key.None
        };
    }
}
