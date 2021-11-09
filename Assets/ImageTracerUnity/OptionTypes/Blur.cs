﻿using System;

namespace ImageTracerUnity.OptionTypes
{
    [Serializable]
    public class Blur
    {
        public int BlurRadius { get; set; } = 0;
        public double BlurDelta { get; set; } = 20f;
    }
}
