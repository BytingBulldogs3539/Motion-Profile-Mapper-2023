package motion.profile.mapper;

import java.util.ArrayList;
import java.util.Arrays;
import java.util.List;

import org.apache.commons.math3.linear.Array2DRowRealMatrix;
import org.apache.commons.math3.linear.ArrayRealVector;
import org.apache.commons.math3.linear.LUDecomposition;
import org.apache.commons.math3.linear.RealMatrix;
import org.apache.commons.math3.linear.RealVector;

import edu.wpi.first.math.geometry.Translation2d;

public class CubicSpline {

    public static class CubicSpline1D {

        private List<Double> a, b, c, d;
        private double[] x, y;
        private int nx;

        public CubicSpline1D(double[] x, double[] y) {
            this.x = x;
            this.y = y;
            this.nx = x.length;

            double[] h = diff(x);
            for (double value : h) {
                if (value < 0) {
                    throw new IllegalArgumentException("x coordinates must be sorted in ascending order");
                }
            }

            this.a = new ArrayList<>();
            for (double value : y) {
                this.a.add(value);
            }

            RealMatrix A = calcA(h);
            RealVector B = calcB(h, this.a);

            this.c = new ArrayList<>();
            RealVector cVector = new LUDecomposition(A).getSolver().solve(B);
            for (int i = 0; i < cVector.getDimension(); i++) {
                this.c.add(cVector.getEntry(i));
            }

            this.b = new ArrayList<>();
            this.d = new ArrayList<>();
            for (int i = 0; i < nx - 1; i++) {
                double d = (this.c.get(i + 1) - this.c.get(i)) / (3.0 * h[i]);
                double b = (1.0 / h[i]) * (this.a.get(i + 1) - this.a.get(i))
                        - h[i] / 3.0 * (2.0 * this.c.get(i) + this.c.get(i + 1));
                this.d.add(d);
                this.b.add(b);
            }
        }

        public Double calcPosition(double x) {
            if (x < this.x[0] || x > this.x[nx - 1]) {
                return null;
            }

            int i = searchIndex(x);
            double dx = x - this.x[i];
            return this.a.get(i) + this.b.get(i) * dx + this.c.get(i) * dx * dx + this.d.get(i) * dx * dx * dx;
        }

        public Double calcFirstDerivative(double x) {
            if (x < this.x[0] || x > this.x[nx - 1]) {
                return null;
            }

            int i = searchIndex(x);
            double dx = x - this.x[i];
            return this.b.get(i) + 2.0 * this.c.get(i) * dx + 3.0 * this.d.get(i) * dx * dx;
        }

        public Double calcSecondDerivative(double x) {
            if (x < this.x[0] || x > this.x[nx - 1]) {
                return null;
            }

            int i = searchIndex(x);
            double dx = x - this.x[i];
            return 2.0 * this.c.get(i) + 6.0 * this.d.get(i) * dx;
        }

        private int searchIndex(double x) {
            int index = Arrays.binarySearch(this.x, x);
            return (index < 0) ? -index - 2 : index;
        }

        private double[] diff(double[] array) {
            double[] diff = new double[array.length - 1];
            for (int i = 0; i < array.length - 1; i++) {
                diff[i] = array[i + 1] - array[i];
            }
            return diff;
        }

        private RealMatrix calcA(double[] h) {
            double[][] matrix = new double[nx][nx];
            matrix[0][0] = 1.0;

            for (int i = 0; i < nx - 1; i++) {
                if (i != (nx - 2)) {
                    matrix[i + 1][i + 1] = 2.0 * (h[i] + h[i + 1]);
                }
                matrix[i + 1][i] = h[i];
                matrix[i][i + 1] = h[i];
            }

            matrix[0][1] = 0.0;
            matrix[nx - 1][nx - 2] = 0.0;
            matrix[nx - 1][nx - 1] = 1.0;

            return new Array2DRowRealMatrix(matrix);
        }

        private RealVector calcB(double[] h, List<Double> a) {
            double[] b = new double[nx];
            for (int i = 0; i < nx - 2; i++) {
                b[i + 1] = 3.0 * (a.get(i + 2) - a.get(i + 1)) / h[i + 1]
                        - 3.0 * (a.get(i + 1) - a.get(i)) / h[i];
            }
            return new ArrayRealVector(b);
        }
    }

    public static class CubicSpline2D {
        private CubicSpline1D sx, sy;

        double[] controlPointDistances;

        public CubicSpline2D(double[] x, double[] y) {
            if (x.length != y.length) {
                throw new IllegalArgumentException("Input arrays must be the same length");
            }
            double[] roughControlPointDistances = new double[x.length];
            controlPointDistances = new double[x.length];

            for (int i = 1; i < x.length; i++) {
                double dx = x[i] - x[i - 1];
                double dy = y[i] - y[i - 1];
                double distance = Math.sqrt(dx * dx + dy * dy);
                roughControlPointDistances[i] = roughControlPointDistances[i - 1] + distance;
            }

            this.sx = new CubicSpline1D(roughControlPointDistances, x);
            this.sy = new CubicSpline1D(roughControlPointDistances, y);
            controlPointDistances = new double[x.length];

            for (int i = 1; i < x.length; i++) {
                double roughDistanceBetweenControlPoints = roughControlPointDistances[i]
                        - roughControlPointDistances[i - 1];
                double start = roughControlPointDistances[i - 1];
                double end = roughControlPointDistances[i];
                if (end == roughControlPointDistances[roughControlPointDistances.length - 1]) {
                    end -= 0.0000000001; // Hacky fix for not being able to get the derivative at the last point
                }
                int steps = (int) (roughDistanceBetweenControlPoints * 10); // Use 10 time the number of meters as steps
                                                                            // for integration. This gives us a fairly
                                                                            // good estimate of the length of the curve
                                                                            // usually within a couple of mm.
                if (steps < 10) {
                    steps = 10;
                }
                controlPointDistances[i] = controlPointDistances[i - 1] + integrateLength(start, end, steps);

            }

            System.out.println(controlPointDistances[controlPointDistances.length - 1]);

            this.sx = new CubicSpline1D(controlPointDistances, x);
            this.sy = new CubicSpline1D(controlPointDistances, y);
        }

        private double integrateLength(double a, double b, int n) {
            int counter=0;
            if (n % 2 != 0)
                n++; // Simpson's rule requires an even number of intervals
            double h = (b - a) / n;
            double sum = 0.0;

            for (int i = 0; i <= n; i++) {
                double t = a + i * h;
                double dx = sx.calcFirstDerivative(t);
                double dy = sy.calcFirstDerivative(t);
                double ds = Math.sqrt(dx * dx + dy * dy);

                if (i == 0 || i == n) {
                    sum += ds;
                } else if (i % 2 == 0) {
                    sum += 2 * ds;
                } else {
                    sum += 4 * ds;
                }
                counter++;
            }
            System.out.println(counter);

            return sum * h / 3.0;
        }

        public Translation2d calcPosition(double s) {
            Double x = this.sx.calcPosition(s);
            Double y = this.sy.calcPosition(s);
            return new Translation2d(x, y);
        }

        public Double calcCurvature(double s) {
            Double dx = sx.calcFirstDerivative(s);
            Double ddx = sx.calcSecondDerivative(s);
            Double dy = sy.calcFirstDerivative(s);
            Double ddy = sy.calcSecondDerivative(s);
            if (dx == null || ddx == null || dy == null || ddy == null) {
                return null;
            }
            return (ddy * dx - ddx * dy) / Math.pow(dx * dx + dy * dy, 1.5);
        }

        public double getLength() {
            return controlPointDistances[controlPointDistances.length - 1];
        }

        public Double calcYaw(double s) {
            Double dx = this.sx.calcFirstDerivative(s);
            Double dy = this.sy.calcFirstDerivative(s);
            if (dx == null || dy == null) {
                return null;
            }
            return Math.atan2(dy, dx);
        }

        private double[] diff(double[] array) {
            double[] diff = new double[array.length - 1];
            for (int i = 0; i < array.length - 1; i++) {
                diff[i] = array[i + 1] - array[i];
            }
            return diff;
        }
    }
}
