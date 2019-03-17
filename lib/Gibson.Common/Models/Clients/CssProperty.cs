using System;
using System.Drawing;
using System.Globalization;

namespace Gibson.Common.Models
{
    public class CSSParameter
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public int R () => ToRgbaHex(Value).Item1;
        public int G () => ToRgbaHex(Value).Item2;
        public int B () => ToRgbaHex(Value).Item3;

        private static Tuple<int, int, int> ToRgbaHex(string hex)
        {
            int argb = Int32.Parse(hex.Replace("#", ""), NumberStyles.HexNumber);
            Color clr = Color.FromArgb(argb);
            return new Tuple<int, int, int>(clr.R, clr.G, clr.B);
        }
    }
}