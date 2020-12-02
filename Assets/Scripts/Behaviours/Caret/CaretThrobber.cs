using UnityEngine;

namespace Caret
{
    /// <summary>
    /// Shared behaviour between the 
    /// </summary>
    public class CaretThrobber : MonoBehaviour
    {
        #region Fields Exposed To Unity Editor
        /// <summary>
        /// Speed of the rotation effect in roughly "degree seconds per frame" units
        /// </summary>
        public float RotationSpeed = 30f;
        /// <summary>
        /// Frequence of the pulse effect in roughly "per frames" units: w
        /// <see cref="ThrobberScalePulse(int)"/> 
        /// </summary>
        public float PulseWidth = 20f;
        /// <summary>
        /// Amplitdue of the pulse effect as an amplitude in unity units: A
        /// <see cref="ThrobberScalePulse(int)"/> 
        /// </summary>
        public float PulseAmplitude = 0.2f;
        /// <summary>
        /// The median of the pulses: m
        /// <see cref="ThrobberScalePulse(int)"/> 
        /// </summary>
        public float PulseMedian = 0.7f;
        #endregion

        #region Internal State Variables.
        private Vector3 scale;
        #endregion

        #region Unity Lifecycle Methods
        void Awake()
        {
            scale = transform.localScale;
        }
        protected void Update()
        {
            transform.Rotate(new Vector3(0, Time.deltaTime * RotationSpeed, 0));
            transform.localScale = ThrobberScalePulse(Time.frameCount);
        }
        #endregion


        #region Logic Functions
        /// <summary>
        /// The amount of scale at time <paramref name="t"/>: m + A * sin(t / w)
        /// </summary>
        /// <param name="t">The time in frames since start.</param>
        /// <returns>a scale vector for the size of the caret </returns>
        protected Vector3 ThrobberScalePulse(int t)
        {
            return (scale * PulseMedian) + (scale * PulseAmplitude * Mathf.Sin(t / PulseWidth));
        } 
        #endregion
    }
}