using System;

namespace CollectiveMass.ImageTracerUnity.OptionTypes
{
    [Serializable]
    public class ColorQuantization
    {
        public double colorSampling = 1f;
        public int numberOfColors = 16;
        public double minColorRatio = 0.02f;
        public int colorQuantCycles = 3;
    }
}
