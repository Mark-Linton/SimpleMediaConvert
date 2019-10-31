namespace SimpleMediaConvert.Common
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Represents a physical media file.
    /// </summary>
    public class MediaItem
    {
        /// <summary>
        /// Gets or sets the Name, excluding the path.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the Path.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Gets or sets the Extension.
        /// </summary>
        public string Extension { get; set; }
    }
}
