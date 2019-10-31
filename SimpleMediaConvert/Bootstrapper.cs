// <copyright file="Bootstrapper.cs" company="Mark Linton">
// Copyright (c) Mark Linton. All rights reserved.
// Licensed under the GNU GENERAL PUBLIC LICENSE Version 3 license.
// See LICENSE file in the solution root for full license information.
// </copyright>

namespace SimpleMediaConvert
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using Caliburn.Micro;
    using SimpleMediaConvert.Common;
    using SimpleMediaConvert.FFMPEG;
    using SimpleMediaConvert.ViewModels;

    public class Bootstrapper : BootstrapperBase
    {
        private SimpleContainer container = new SimpleContainer();

        public Bootstrapper()
        {
            this.Initialize();
        }

        protected override void Configure()
        {
            this.container.Instance(this.container);

            this.container.PerRequest<IMediaConcat, FFmpegMediaConcat>();

            this.container
                .Singleton<IWindowManager, WindowManager>()
                .Singleton<IEventAggregator, EventAggregator>();

            this.GetType().Assembly.GetTypes()
                .Where(type => type.IsClass)
                .Where(type => type.Name.EndsWith("ViewModel"))
                .ToList()
                .ForEach(viewModelType => this.container.RegisterPerRequest(
                    viewModelType, viewModelType.ToString(), viewModelType));
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            this.DisplayRootViewFor<ShellViewModel>();
        }

        protected override object GetInstance(Type service, string key)
        {
            return this.container.GetInstance(service, key);
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return this.container.GetAllInstances(service);
        }

        protected override void BuildUp(object instance)
        {
            this.container.BuildUp(instance);
        }
    }
}
