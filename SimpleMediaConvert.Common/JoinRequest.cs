// <copyright file="JoinRequest.cs" company="Mark Linton">
// Copyright (c) Mark Linton. All rights reserved.
// Licensed under the GNU GENERAL PUBLIC LICENSE Version 3 license.
// See LICENSE file in the solution root for full license information.
// </copyright>

namespace SimpleMediaConvert.Common
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Represents the files being joined by the IMediaConcat implementation.
    /// </summary>
    public class JoinRequest
    {
        /// <summary>
        /// Gets or sets the Parts. The media files to be joined.
        /// </summary>
        public List<string> Parts { get; set; }

        /// <summary>
        /// Gets or sets the DestinationPath. The location including filename to
        /// save the resulting joined file.
        /// </summary>
        public string DestinationPath { get; set; }
    }
}
