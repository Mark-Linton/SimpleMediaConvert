// <copyright file="ShellViewModel.cs" company="Mark Linton">
// Copyright (c) Mark Linton. All rights reserved.
// Licensed under the GNU GENERAL PUBLIC LICENSE Version 3 license.
// See LICENSE file in the solution root for full license information.
// </copyright>

namespace SimpleMediaConvert.ViewModels
{
    using Caliburn.Micro;

    /// <summary>
    /// The main container used to display all other pages.
    /// </summary>
    public class ShellViewModel : Conductor<object>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ShellViewModel"/> class.
        /// </summary>
        public ShellViewModel()
        {
            this.ActivateItem(IoC.Get<MediaConcatViewModel>());
        }
    }
}
