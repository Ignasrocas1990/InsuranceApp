using System;
using System.Linq;
using System.Numerics;
using Android.Util;

namespace watch.Sensors
{
    public class BleMaFilter
    {
        private const String TAG = "mono-stdout";
        private float[] accArr;
        private const int NumberOfSamples = 3;

        public BleMaFilter() => ClearFilter();

        public void AddAcc( Vector3 aVector3)
        {
            accArr[0] += aVector3.X;
            accArr[1] += aVector3.Y;
            accArr[2] += aVector3.Z;
        }

        public String GetAcc()
        {
            try
            {
                var totalNumber = accArr.Select(x => x / NumberOfSamples);
                return ","+string.Join(",", totalNumber);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Log.Verbose(TAG, $"bleMAFilter error GETACC(){e}");
                return " ";
            }
        }
        public void ClearFilter()
        {
            accArr = new float[]{0,0,0};
        }
        
    }
}