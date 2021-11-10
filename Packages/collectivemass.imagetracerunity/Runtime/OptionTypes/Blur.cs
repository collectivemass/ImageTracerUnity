using System;

namespace CollectiveMass.ImageTracerUnity.OptionTypes
{
    [Serializable]
    public class Blur
    {
        public int BlurRadius { get; set; } = 0;
        public float BlurDelta { get; set; } = 2f;
    }
}
