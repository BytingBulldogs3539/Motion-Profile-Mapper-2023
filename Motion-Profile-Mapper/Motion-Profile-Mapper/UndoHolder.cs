using MotionProfile.SegmentedProfile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotionProfileMapper
{
    public class UndoHolder
    {
        public int selectedProfileIndex = -1;
        public int selectedPathIndex = -1;
        public List<Profile> profiles = new List<Profile>();
        public bool usePOI = true;
        public string reason;
    }
}
