using UnityEngine;

namespace EasyEditor
{
    /// <summary>
    /// Attribute for ProgressBarDrawer.
    /// </summary>
    public class ProgressBarAttribute : PropertyAttribute
    {
        /// <summary>
        /// The max value is the value that fill the entire bar.
        /// </summary>
        public float max = 1f;

        public ProgressBarAttribute()
        {
        }

        public ProgressBarAttribute(float max)
        {
            this.max = max;
        }
    }
}