// <copyright file="MediaConcatViewModel.cs" company="Mark Linton">
// Copyright (c) Mark Linton. All rights reserved.
// Licensed under the GNU GENERAL PUBLIC LICENSE Version 3 license.
// See LICENSE file in the solution root for full license information.
// </copyright>

namespace SimpleMediaConvert.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Threading.Tasks;
    using Caliburn.Micro;
    using SimpleMediaConvert.Common;
    using SimpleMediaConvert.Common.EventModels;
    using SimpleMediaConvert.Helpers;

    /// <summary>
    /// View used to concatenate (join) multiple media files into a single file.
    /// </summary>
    public class MediaConcatViewModel : Screen, IHandle<TaskCompletedEvent>, IHandle<LogEvent>
    {
        private readonly IMediaConcat joiner;
        private readonly IWindowManager windowManager;
        private readonly MediaHelper mediaHelper = new MediaHelper();

        private string sourceFolder = string.Empty;
        private string destinationFolder = string.Empty;

        private MediaItem selectedSource;
        private MediaItem selectedPart;
        private BindingList<LogEntry> log = new BindingList<LogEntry>();

        private BindingList<MediaItem> sources = new BindingList<MediaItem>();
        private BindingList<MediaItem> selectedParts = new BindingList<MediaItem>();

        /// <summary>
        /// Initializes a new instance of the <see cref="MediaConcatViewModel"/> class.
        /// </summary>
        /// <param name="windowManager">Provides access to the window manager.</param>
        /// <param name="mediaConcat">The IMediaConcat instance.</param>
        public MediaConcatViewModel(IEventAggregator events, IWindowManager windowManager, IMediaConcat mediaConcat)
        {
            this.windowManager = windowManager;
            this.joiner = mediaConcat;
            events.Subscribe(this);
        }

        /// <summary>
        /// Event handler for TaskCompletedEvent.
        /// </summary>
        /// <param name="message">The received event.</param>
        public void Handle(TaskCompletedEvent message)
        {
            this.Log.Add(new LogEntry
            {
                Time = DateTime.Now,
                Text = $"Done", // : {request.DestinationPath}",
            });

            this.NotifyOfPropertyChange(() => this.Log);
            this.SelectedParts.Clear();
            this.SelectedPart = null;
        }

        public void Handle(LogEvent message)
        {
            this.Log.Add(message.Entry);
            this.NotifyOfPropertyChange(() => this.Log);
        }

        /// <summary>
        /// Gets or sets the Sources. Files available for joining.
        /// </summary>
        public BindingList<MediaItem> Sources
        {
            get
            {
                return this.sources;
            }

            set
            {
                // Not called when items are added to BindingList.
                if (this.sources != value)
                {
                    this.sources = value;
                    this.NotifyOfPropertyChange(() => this.Sources);
                }
            }
        }

        /// <summary>
        /// Gets or sets the SelectedParts. Files selected for joining.
        /// </summary>
        public BindingList<MediaItem> SelectedParts
        {
            get
            {
                return this.selectedParts;
            }

            set
            {
                // Not called when items are added to BindingList.
                if (this.selectedParts != value)
                {
                    this.selectedParts = value;
                    this.NotifyOfPropertyChange(() => this.SelectedParts);
                }
            }
        }

        /// <summary>
        /// Gets or sets the SelectedSource. The file to be moved to SelectedParts.
        /// </summary>
        public MediaItem SelectedSource
        {
            get
            {
                return this.selectedSource;
            }

            set
            {
                if (this.selectedSource != value)
                {
                    this.selectedSource = value;
                    this.NotifyOfPropertyChange(() => this.SelectedSource);
                    this.NotifyOfPropertyChange(() => this.CanAddItem);
                }
            }
        }

        /// <summary>
        /// Gets or sets the SelectedPart. The file to be removed from SelectedParts.
        /// </summary>
        public MediaItem SelectedPart
        {
            get
            {
                return this.selectedPart;
            }

            set
            {
                if (this.selectedPart != value)
                {
                    this.selectedPart = value;
                    this.NotifyOfPropertyChange(() => this.SelectedPart);
                    this.NotifyOfPropertyChange(() => this.CanRemoveItem);
                }
            }
        }

        /// <summary>
        /// Gets or sets the SourceFolder. The location to look for valid media files.
        /// </summary>
        public string SourceFolder
        {
            get
            {
                return this.sourceFolder;
            }

            set
            {
                if (this.sourceFolder != value)
                {
                    this.sourceFolder = value;
                    this.Sources = new BindingList<MediaItem>(this.mediaHelper.GetMediaInSourceDirectory(this.sourceFolder));
                    this.NotifyOfPropertyChange(() => this.SourceFolder);
                    this.NotifyOfPropertyChange(() => this.Sources);
                }
            }
        }

        /// <summary>
        /// Gets or sets the Destination. The filesystem location the joined files will be written to.
        /// </summary>
        public string DestinationFolder
        {
            get
            {
                return this.destinationFolder;
            }

            set
            {
                if (this.destinationFolder != value)
                {
                    this.destinationFolder = value;
                    this.NotifyOfPropertyChange(() => this.DestinationFolder);
                    this.NotifyOfPropertyChange(() => this.CanJoin);
                }
            }
        }

        /// <summary>
        /// Gets the Log. Application feedback for the user.
        /// </summary>
        public BindingList<LogEntry> Log
        {
            get
            {
                return this.log;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the AddItem button should be enabled or not.
        /// </summary>
        public bool CanAddItem
        {
            get
            {
                return this.SelectedSource != null && !this.SelectedParts.Contains(this.SelectedSource);
            }
        }

        /// <summary>
        /// Gets a value indicating whether the RemoveItem button should be enabled or not.
        /// </summary>
        public bool CanRemoveItem
        {
            get
            {
                return this.SelectedPart != null;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the Join button should be enabled or not.
        /// </summary>
        public bool CanJoin
        {
            get
            {
                return this.SelectedParts?.Count >= 2
                    && this.mediaHelper.FolderExists(this.DestinationFolder);
            }
        }

        /// <summary>
        /// Called when the AddItem button is pressed.
        /// </summary>
        public void AddItem()
        {
            // Need to sort in case the parts aren't added in the correct order.
            this.SelectedParts.Add(this.SelectedSource);
            this.NotifyOfPropertyChange(() => this.CanAddItem);
            this.NotifyOfPropertyChange(() => this.CanRemoveItem);
            this.NotifyOfPropertyChange(() => this.CanJoin);
        }

        /// <summary>
        /// Called when the RemoveItem button is pressed.
        /// </summary>
        public void RemoveItem()
        {
            this.SelectedParts.Remove(this.SelectedPart);
            this.NotifyOfPropertyChange(() => this.CanRemoveItem);
            this.NotifyOfPropertyChange(() => this.CanAddItem);
            this.NotifyOfPropertyChange(() => this.CanJoin);
        }

        /// <summary>
        /// Called when the SourcePicker button is pressed.
        /// </summary>
        public void SourcePicker()
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    this.SourceFolder = dialog.SelectedPath;
                }
            }
        }

        /// <summary>
        /// Called when the DestinationPicker button is pressed.
        /// </summary>
        public void DestinationPicker()
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    this.DestinationFolder = dialog.SelectedPath;
                }
            }
        }

        /// <summary>
        /// Called when the Join button is pressed.
        /// </summary>
        public async void Join()
        {
            List<string> parts = new List<string>();

            foreach (var source in this.SelectedParts)
            {
                parts.Add(Path.Combine(source.Path, source.Name));
            }

            var request = new JoinRequest
            {
                Parts = parts,
                DestinationPath = this.mediaHelper.GetDestinationPath(this.destinationFolder, this.SelectedParts[0]),
            };

            var progress = new Progress<double>();
            progress.ProgressChanged += this.Progress_ProgressChanged;

            this.Log.Add(new LogEntry
            {
                Time = DateTime.Now,
                Text = $"Starting: {request.DestinationPath}",
            });
            this.NotifyOfPropertyChange(() => this.Log);

            await Task.Run(() => this.joiner.Join(request, progress)).ConfigureAwait(false);
        }

        private void Progress_ProgressChanged(object sender, double e)
        {
            throw new NotImplementedException();
        }
    }
}
