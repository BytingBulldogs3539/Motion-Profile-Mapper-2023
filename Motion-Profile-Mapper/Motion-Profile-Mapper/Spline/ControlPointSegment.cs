﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotionProfile.Spline {
    public class ControlPointSegment {

        public List<SplinePoint> points = new List<SplinePoint>();
        public ControlPoint startPoint;
        public ControlPoint endPoint;
    }
}
