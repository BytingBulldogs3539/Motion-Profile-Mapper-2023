using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VelocityMap.VelocityGenerate
{
    public class Pose2d
    {
        private Translation2d m_translation;
        private Rotation2d m_rotation;

        /** Constructs a pose at the origin facing toward the positive X axis. */
        public Pose2d()
        {
            m_translation = new Translation2d();
            m_rotation = new Rotation2d();
        }

        /**
         * Constructs a pose with the specified translation and rotation.
         *
         * @param translation The translational component of the pose.
         * @param rotation The rotational component of the pose.
         */
        public Pose2d(Translation2d translation, Rotation2d rotation)
        {
            m_translation = translation;
            m_rotation = rotation;
        }

        /**
         * Constructs a pose with x and y translations instead of a separate Translation2d.
         *
         * @param x The x component of the translational component of the pose.
         * @param y The y component of the translational component of the pose.
         * @param rotation The rotational component of the pose.
         */
        public Pose2d(double x, double y, Rotation2d rotation)
        {
            m_translation = new Translation2d(x, y);
            m_rotation = rotation;
        }
        public Translation2d getTranslation()
        {
            return m_translation;
        }

        /**
         * Returns the X component of the pose's translation.
         *
         * @return The x component of the pose's translation.
         */
        public double getX()
        {
            return m_translation.getX();
        }

        /**
         * Returns the Y component of the pose's translation.
         *
         * @return The y component of the pose's translation.
         */
        public double getY()
        {
            return m_translation.getY();
        }

        /**
         * Returns the rotational component of the transformation.
         *
         * @return The rotational component of the pose.
         */
        public Rotation2d getRotation()
        {
            return m_rotation;
        }

        /**
         * Multiplies the current pose by a scalar.
         *
         * @param scalar The scalar.
         * @return The new scaled Pose2d.
         */
        public Pose2d times(double scalar)
        {
            return new Pose2d(m_translation.times(scalar), m_rotation.times(scalar));
        }

        /**
         * Divides the current pose by a scalar.
         *
         * @param scalar The scalar.
         * @return The new scaled Pose2d.
         */
        public Pose2d div(double scalar)
        {
            return times(1.0 / scalar);
        }


        public String toString()
        {
            return String.Format("Pose2d({0}, {0})", m_translation, m_rotation);
        }

        /**
         * Checks equality between this Pose2d and another object.
         *
         * @param obj The other object.
         * @return Whether the two objects are equal or not.
         */

        public bool equals(Object obj)
        {
            if (obj.GetType() == typeof(Pose2d))
            {
                return ((Pose2d)obj).m_translation.equals(m_translation)
                    && ((Pose2d)obj).m_rotation.equals(m_rotation);
            }
            return false;
        }

        public int hashCode()
        {
            int hash = 17;
            hash = hash * 31 + m_translation.GetHashCode();
            hash = hash * 31 + m_rotation.GetHashCode();
            return hash;
        }
    }
}
