using UnityEngine;

namespace CollectiveMass.ImageTracerUnity
{
    internal class ColorReference
    {
        public Color32 Color { get; }

        public ColorReference(Color32 color)
        {
            Color = color;
        }

        public ColorReference(byte alpha, byte red, byte green, byte blue)
            : this(new Color32(red, green, blue, alpha))
        {
        }

        public byte A => Color.a;
        public byte R => Color.r;
        public byte G => Color.g;
        public byte B => Color.b;

        private ColorReference() { }
        public static ColorReference Empty { get; } = new ColorReference();
    }
}
