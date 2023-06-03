using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VelocityMap.VelocityGenerate
{
    public class MaxVelocityConstraint : TrajectoryConstraint
    {
    private  double maxVelocity;

    public MaxVelocityConstraint(double maxVelocity)
    {
        this.maxVelocity = maxVelocity;
    }

    public override double getMaxVelocity(PState state)
    {
        return maxVelocity;
    }
}
}
