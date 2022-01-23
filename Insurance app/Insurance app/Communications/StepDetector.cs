using System;
using Math = Java.Lang.Math;

namespace Insurance_app.Communications
{
    public class StepDetector
    {
        private static int ACCEL_RING_SIZE = 50;
        private static  int VEL_RING_SIZE = 10;
        private int n = 0;

        // change this threshold according to your sensitivity preferences
        private static  float STEP_THRESHOLD = 0.3f;// 50f

        private static int STEP_DELAY_MS = 200;//000000;//250000000

        private int accelRingCounter = 0;
        private float[] accelRingX = new float[ACCEL_RING_SIZE];
        private float[] accelRingY = new float[ACCEL_RING_SIZE];
        private float[] accelRingZ = new float[ACCEL_RING_SIZE];
        private int velRingCounter = 0;
        private float[] velRing = new float[VEL_RING_SIZE];
        private long lastStepTimeNs = 0;
        private float oldVelocityEstimate = 0;


        public int updateAccel(long timeMSec, float x, float y, float z)
        {
            float[] currentAccel = new float[3];
            currentAccel[0] = x;
            currentAccel[1] = y;
            currentAccel[2] = z;

            // First step is to update our guess of where the global z vector is.
            accelRingCounter++;
            accelRingX[accelRingCounter % ACCEL_RING_SIZE] = currentAccel[0];
            accelRingY[accelRingCounter % ACCEL_RING_SIZE] = currentAccel[1];
            accelRingZ[accelRingCounter % ACCEL_RING_SIZE] = currentAccel[2];

            float[] worldZ = new float[3];
            worldZ[0] = SensorFilter.sum(accelRingX) / Math.Min(accelRingCounter, ACCEL_RING_SIZE);
            worldZ[1] = SensorFilter.sum(accelRingY) / Math.Min(accelRingCounter, ACCEL_RING_SIZE);
            worldZ[2] = SensorFilter.sum(accelRingZ) / Math.Min(accelRingCounter, ACCEL_RING_SIZE);

            float normalization_factor = SensorFilter.Norm(worldZ);

            worldZ[0] = worldZ[0] / normalization_factor;
            worldZ[1] = worldZ[1] / normalization_factor;
            worldZ[2] = worldZ[2] / normalization_factor;

            float currentZ = SensorFilter.dot(worldZ, currentAccel) - normalization_factor;
            velRingCounter++;
            velRing[velRingCounter % VEL_RING_SIZE] = currentZ;

            float velocityEstimate = SensorFilter.sum(velRing);
            if (velocityEstimate > STEP_THRESHOLD && oldVelocityEstimate <= STEP_THRESHOLD && (timeMSec - lastStepTimeNs > STEP_DELAY_MS))
            {
                Console.WriteLine("step counted");
                lastStepTimeNs = timeMSec;
                return 1;
            }
            oldVelocityEstimate = velocityEstimate;
            return 0;
        }
    }
}