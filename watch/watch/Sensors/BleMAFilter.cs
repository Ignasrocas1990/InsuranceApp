using System;
using System.Collections;
using System.Linq;
using System.Numerics;
using Android.Util;

namespace watch
{
    public class BleMaFilter
    {
        private const String TAG = "mono-stdout";
        private float[] accArr;
        private float[] gyroArr;
        private const int NumberOfSamples = 3;

        public BleMaFilter()
        {
            ClearAcc();
            ClearGyro();
        }
        
        public void AddAcc( Vector3 aVector3)
        {
            accArr[0] += aVector3.X;
            accArr[1] += aVector3.Y;
            accArr[2] += aVector3.Z;
        }

        public void AddGyro(Vector3 gVector3)
        {
            gyroArr[0] += gVector3.X;
            gyroArr[1] += gVector3.Y;
            gyroArr[2] += gVector3.Z;
        }

        public String GetAcc()
        {
            try
            {
                var totalNumber = accArr.Select(x => x / NumberOfSamples);
                return "A" + string.Join(",", totalNumber);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Log.Verbose(TAG, $"bleMAFilter error GETACC(){e}");
                throw;
            }
            

        }

        public String GetGyro()
        {
            try
            {
                var totalNumber = gyroArr.Select(x => x / NumberOfSamples);
                return "G" + string.Join(",", totalNumber);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Log.Verbose(TAG, $"bleMAFilter error GETGyro(){e}");

                throw;
            }
            
        }

        public void ClearGyro()
        {
            gyroArr = new float[]{0,0,0};
        }
        public void ClearAcc()
        {
            accArr = new float[]{0,0,0};
        }
        
    }
}