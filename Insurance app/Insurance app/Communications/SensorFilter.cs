using Java.Lang;

namespace Insurance_app.Communications
{
    public class SensorFilter
    {
        private SensorFilter()
        {
        }

        public static float sum(float[] array)
        {
            float retval = 0;
            for (int i = 0; i < array.Length; i++)
            {
                retval += array[i];
            }

            return retval;
        }

        public static float[] Cross(float[] arrayA, float[] arrayB)
        {
            float[] retArray = new float[3];
            retArray[0] = arrayA[1] * arrayB[2] - arrayA[2] * arrayB[1];
            retArray[1] = arrayA[2] * arrayB[0] - arrayA[0] * arrayB[2];
            retArray[2] = arrayA[0] * arrayB[1] - arrayA[1] * arrayB[0];
            return retArray;
        }

        public static float Norm(float[] array)
        {
            float retval = 0;
            for (int i = 0; i < array.Length; i++)
            {
                retval += array[i] * array[i];
            }

            return (float) Math.Sqrt(retval);
        }


        public static float dot(float[] a, float[] b)
        {
            float retval = a[0] * b[0] + a[1] * b[1] + a[2] * b[2];
            return retval;
        }

        public static float[] Normalize(float[] a)
        {
            float[] retval = new float[a.Length];
            float norm = Norm(a);
            for (int i = 0; i < a.Length; i++)
            {
                retval[i] = a[i] / norm;
            }

            return retval;
        }
    }
}