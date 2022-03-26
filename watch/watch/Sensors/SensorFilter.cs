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

using System.Collections.Generic;
using System.Linq;
using Java.Lang;

namespace watch.Sensors
{
    /// <summary>
    /// used to filter measurements
    /// </summary>
    public static class SensorFilter
    {
        public static float Sum(IEnumerable<float> array)=> array.Sum();

        /// <summary>
        /// Normalization performed
        /// </summary>
        public static float Norm(IEnumerable<float> array) =>
         (float) Math.Sqrt(array.Sum(t => t * t));
        
        /// <summary>
        /// Dot product performed
        /// </summary>
        public static float Dot(float[] a, float[] b) =>
            a[0] * b[0] + a[1] * b[1] + a[2] * b[2];
    }
}