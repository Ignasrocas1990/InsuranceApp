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
        private float[] _accArr;
        private float[] _gyroArr;
        private const int NumberOfSamples = 3;

        public BleMaFilter()
        {
            ClearAcc();
            ClearGyro();
        }
        
        public void AddAcc( Vector3 aVector3)
        {
            _accArr[0] += aVector3.X;
            _accArr[1] += aVector3.Y;
            _accArr[2] += aVector3.Z;
        }

        public void AddGyro(Vector3 gVector3)
        {
            _gyroArr[0] += gVector3.X;
            _gyroArr[1] += gVector3.Y;
            _gyroArr[2] += gVector3.Z;
        }

        public String GetAcc()
        {
            try
            {
                var totalNumber = _accArr.Select(x => x / NumberOfSamples);
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
                var totalNumber = _gyroArr.Select(x => x / NumberOfSamples);
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
            _gyroArr = new float[]{0,0,0};
        }
        public void ClearAcc()
        {
            _accArr = new float[]{0,0,0};
        }
        
    }
}