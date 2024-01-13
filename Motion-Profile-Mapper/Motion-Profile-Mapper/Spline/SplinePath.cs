using MotionProfile;
using MotionProfile.Spline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotionProfile.Spline
{
    public class SplinePath
    {
        ParametricSpline spline;
        List<double> CPDistances = new List<double>();
        List<ControlPoint> points = null;
        double length = 0;

        public void GenSpline(List<ControlPoint> points)
        {
            this.points = points;

            List<double> xs = new List<double>();
            List<double> ys = new List<double>();

            foreach (ControlPoint point in points)
            {
                xs.Add(point.X);
                ys.Add(point.Y);
            }

            double totalDist = 0;
            double[] ctlDists = new double[xs.Count];
            for (int i = 1; i < xs.Count; i++)
            {
                double dx = xs[i] - xs[i - 1];
                double dy = ys[i] - ys[i - 1];
                double dist = (double)Math.Sqrt(dx * dx + dy * dy);
                totalDist += dist;
                ctlDists[i] = totalDist;
            }

            List<double> dists = new List<double>();
            double stepSize = .1;
            int index = 0;
            List<double> ctlPointIndexs = new List<double>();

            for (double dist = 0.0; dist < totalDist; dist += stepSize)
            {
                dists.Add(dist);
                index++;
                foreach(double ctldist in ctlDists)
                {
                    if (ctldist == dist)
                    {
                        ctlPointIndexs.Add(index-1);
                    }
                    if (ctldist>dist && ctldist<dist+stepSize)
                    {
                        dists.Add(ctldist);
                        ctlPointIndexs.Add(index);
                        index++;
                    }
                }
            }


            spline = new ParametricSpline(xs.ToArray(), ys.ToArray());
            List<SplinePoint> splinePoints = spline.Eval(dists.ToArray());

            CPDistances.Clear();

            double l = 0.0;
            if(ctlPointIndexs.Contains(0))
            {
                CPDistances.Add(l);
            }
            for (int i = 1; i < splinePoints.Count; i++)
            {
                l += GetDistance(splinePoints[i - 1], splinePoints[i]);
                if (ctlPointIndexs.Contains(i))
                {
                    CPDistances.Add(l);
                }
            }

            List<double> x1 = splinePoints.Select(o => o.X).ToList();
            List<double> y1 = splinePoints.Select(o => o.Y).ToList();

            length = CPDistances.Last();

            spline = new ParametricSpline(x1.ToArray(), y1.ToArray());
        }

        public SplinePoint calculate(double distance)
        {

            SplinePoint spoint = spline.Eval(distance);

            return spoint;
        }

        private double GetDistance(SplinePoint p1, SplinePoint p2)
        {
            return GetDistance(p1.X, p1.Y, p2.X, p2.Y);
        }

        public List<double> getControlPointDistances()
        {
            return CPDistances;
        }
        private double GetDistance(double x1, double y1, double x2, double y2)
        {
            return Math.Sqrt(Math.Pow((x2 - x1), 2.0) + Math.Pow((y2 - y1), 2.0));
        }
        public double getLength()
        {
            return length;
        }

    }
}

