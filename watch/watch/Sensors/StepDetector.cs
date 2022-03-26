/*
    Copyright 2020,Ignas Rocas

    Licensed under the Apache License, Version 2.0 (the "License");
    you may not use this file except in compliance with the License.
    You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

    Unless required by applicable law or agreed to in writing, software
    distributed under the License is distributed on an "AS IS" BASIS,
    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
    See the License for the specific language governing permissions and
    limitations under the License.
    
              Name : Ignas Rocas
    Student Number : C00135830
           Purpose : 4th year project
          Based on : http://www.gadgetsaint.com/android/create-pedometer-step-counter-android/
 */

using Java.Lang;

namespace watch.Sensors
{
    public class StepDetector
    {
        private const int AccelRingSize = 50;
        private const int VelRingSize = 10;

        // change this threshold according to your sensitivity preferences
        private static  float STEP_THRESHOLD = 1.5f;//50

        private const int StepDelayMs = 150;

        private int accelRingCounter = 0;
        private readonly float[] accelRingX = new float[AccelRingSize];
        private readonly float[] accelRingY = new float[AccelRingSize];
        private readonly float[] accelRingZ = new float[AccelRingSize];
        private int velRingCounter = 0;
        private readonly float[] velRing = new float[VelRingSize];
        private long lastStepTimeNs = 0;
        private float oldVelocityEstimate = 0;


        public int UpdateAccel(long timeMSec, float x, float y, float z)
        {
            var currentAccel = new[] {x,y,z};

            // First step is to update our guess of where the global z vector is.
            accelRingCounter++;
            var pos = accelRingCounter % AccelRingSize;
            accelRingX[pos] = currentAccel[0];
            accelRingY[pos] = currentAccel[1];
            accelRingZ[pos] = currentAccel[2];
            
            var min = Math.Min(accelRingCounter, AccelRingSize);
            var worldZ = new[]
            {
                SensorFilter.Sum(accelRingX) / min, 
                SensorFilter.Sum(accelRingY) / min, 
                SensorFilter.Sum(accelRingZ) / min
            };

            var normalizationFactor = SensorFilter.Norm(worldZ);

            worldZ[0] /= normalizationFactor;
            worldZ[1] /= normalizationFactor;
            worldZ[2] /= normalizationFactor;

            var currentZ = SensorFilter.Dot(worldZ, currentAccel) - normalizationFactor;
            velRingCounter++;
            velRing[velRingCounter % VelRingSize] = currentZ;

            var velocityEstimate = SensorFilter.Sum(velRing);
           // Log.Verbose(TAG,$"{velocityEstimate} > {STEP_THRESHOLD} and {oldVelocityEstimate}<= {STEP_THRESHOLD} " +
    //                        $"and {timeMSec - lastStepTimeNs} > {STEP_DELAY_MS}");
            if (velocityEstimate > STEP_THRESHOLD && oldVelocityEstimate <= STEP_THRESHOLD && (timeMSec - lastStepTimeNs > StepDelayMs))
            {
                lastStepTimeNs = timeMSec;
                return 1;
            }
            oldVelocityEstimate = velocityEstimate;
            return 0;
        }
    }
    
    
}