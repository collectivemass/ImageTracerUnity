using System;

namespace CollectiveMass.ImageTracerUnity.OptionTypes
{
    [Serializable]
    public class SvgRendering
    {
        public double scale = 1f;
        public double simplifyTolerance = 0f;
        public int roundCoords = 1;

        // LinearControlPointRadius
        public double linearControlPointRadius = 0f;

        // QuadraticControlPointRadius
        public double quadraticControlPointRadius = 0f;
        public bool viewbox = false;
    }
}
