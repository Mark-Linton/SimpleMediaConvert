// <copyright file="IMediaConcat.cs" company="Mark Linton">
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
    /// Interface used to define media concatenation functionality.
    /// </summary>
    public interface IMediaConcat
    {
        /// <summary>
        /// Join 2 or more parts into a single media file without converting the media format in any way.
        /// </summary>
        /// <param name="request">The join to perform.</param>
        /// <param name="progress">The IProgress callback.</param>
        void Join(JoinRequest request, IProgress<double> progress);

        /// <summary>
        /// Perform multiple Join requests.
        /// </summary>
        /// <param name="requests">The List of joins to perform.</param>
        /// <param name="progress">The IProgress callback.</param>
        void JoinMultiple(List<JoinRequest> requests, IProgress<double> progress);
    }
}
