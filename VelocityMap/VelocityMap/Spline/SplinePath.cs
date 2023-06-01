using MotionProfile;
using MotionProfile.Spline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotionProfile.Spline
{
    class SplinePath
    {
        private static ParametricSpline spline;
        public static List<ControlPointSegment> GenSpline(List<ControlPoint> points, List<VelocityPoint> velocityPoints = null)
        {
            List<ControlPointSegment> splineSegments = new List<ControlPointSegment>();
            splineSegments.Clear();

            List<double> xs = new List<double>();
            List<double> ys = new List<double>();

            foreach (ControlPoint point in points)
            {
                xs.Add(point.X);
                ys.Add(point.Y);
            }

            List<CubicSplinePoint> outxs;
            List<CubicSplinePoint> outys;

            spline = new ParametricSpline(xs.ToArray(), ys.ToArray(), 100, out outxs, out outys);

            int requiredNumPoints = (int)(spline.length * 10); //Put a point every 10cm

            spline = new ParametricSpline(xs.ToArray(), ys.ToArray(), requiredNumPoints, out outxs, out outys);

            List<double> CPDistances = new List<double>();

            double d = 0.0;

            CPDistances.Add(d);

            for (int i = 0; i < outxs.Last().ControlPointNum + 1; i++)
            {
                for (int x = 1; x < outxs.Count; x++)
                {
                    if (outxs[x].ControlPointNum == i)
                    {
                        d += GetDistance(outxs[x-1].Y, outys[x-1].Y, outxs[x].Y, outys[x].Y);
                    }
                }
                CPDistances.Add(d);
            }

            xs.Clear();
            ys.Clear();

            foreach (CubicSplinePoint point in outxs)
            {
                xs.Add(point.Y);
            }

            foreach (CubicSplinePoint point in outys)
            {
                ys.Add(point.Y);
            }

            spline = new ParametricSpline(xs.ToArray(), ys.ToArray(), requiredNumPoints, out outxs, out outys);
            if (velocityPoints==null)
            {
                for (int i = 0; i < outxs.Last().ControlPointNum + 1; i++)
                {
                    ControlPointSegment seg = new ControlPointSegment();

                    for (int x = 0; x < outxs.Count; x++)
                    {
                        if (outxs[x].ControlPointNum == i)
                        {
                            seg.points.Add(new SplinePoint(outxs[x].Y, outys[x].Y, i));
                        }
                    }
                    splineSegments.Add(seg);
                }
            }
            else
            {
                splineSegments.Clear();
                
                SplinePoint spoint = null;

                ControlPointSegment[] segments = new ControlPointSegment[CPDistances.Count - 1];

                for (int i = 0; i < velocityPoints.Count; i++)
                {
                    spoint = spline.Eval(Math.Abs((double)velocityPoints[i].Pos));


                    double distance = Math.Abs((double)velocityPoints[i].Pos);

                    for (int dis = 0; dis < CPDistances.Count - 1; dis++)
                    {
                        if(segments[dis] == null)
                        {
                            segments[dis] = new ControlPointSegment();
                        }
                        if(distance >= CPDistances[dis] && distance <= CPDistances[dis + 1])
                        {
                            spoint.ControlPointNum = dis;
                            segments[dis].startPoint = points[dis];
                            segments[dis].endPoint = points[dis + 1];
                            segments[dis].points.Add(spoint);
                        }
                    }
                }
                splineSegments = segments.ToList();
            }
            return splineSegments;

        }
        private static double GetDistance(double x1, double y1, double x2, double y2)
        {
            return Math.Sqrt(Math.Pow((x2 - x1), 2.0) + Math.Pow((y2 - y1), 2.0));
        }
        public static double getLength()
        {
            if (spline == null)
            {
                throw new NoSplineGenerated();
            }
            return spline.length;
        }

    }
    public class NoSplineGenerated: Exception
    {
        public NoSplineGenerated()
            : base("NoSplineGenerated: Spline must be generated before distance is found.")
        {
        }
    }
}

