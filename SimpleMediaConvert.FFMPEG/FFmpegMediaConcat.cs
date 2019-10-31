// <copyright file="FFmpegMediaConcat.cs" company="Mark Linton">
// Copyright (c) Mark Linton. All rights reserved.
// Licensed under the GNU GENERAL PUBLIC LICENSE Version 3 license.
// See LICENSE file in the solution root for full license information.
// </copyright>

namespace SimpleMediaConvert.FFMPEG
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using Caliburn.Micro;
    using SimpleMediaConvert.Common;
    using SimpleMediaConvert.Common.EventModels;

    /// <summary>
    /// Concrete implementation of IMediaConcat that uses FFmpeg.
    /// </summary>
    public class FFmpegMediaConcat : IMediaConcat
    {
        private readonly string filename = "list.txt";

        private readonly IEventAggregator events;

        /// <summary>
        /// Initializes a new instance of the <see cref="FFmpegMediaConcat"/> class.
        /// </summary>
        /// <param name="events">Provides access to event publishing.</param>
        public FFmpegMediaConcat(IEventAggregator events)
        {
            this.events = events;
        }

        /// <summary>
        /// Join 2 or more parts into a single media file without converting the media format in any way.
        /// </summary>
        /// <param name="request">The join to perform.</param>
        /// <param name="progress">The IProgress callback.</param>
        public void Join(JoinRequest request, IProgress<double> progress)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }

            if (request.Parts == null || request.Parts.Count == 0)
            {
                throw new ArgumentException("parts is null or empty.");
            }

            if (string.IsNullOrWhiteSpace(request.DestinationPath))
            {
                throw new ArgumentException("outputName not specified.");
            }

            this.JoinImpl(request.Parts, request.DestinationPath);
        }

        /// <summary>
        /// Perform multiple Join requests.
        /// </summary>
        /// <param name="requests">The List of joins to perform.</param>
        /// <param name="progress">The IProgress callback.</param>
        public void JoinMultiple(List<JoinRequest> requests, IProgress<double> progress)
        {
            if (requests == null || requests.Count == 0)
            {
                throw new ArgumentException("requests is null or empty");
            }

            foreach (var request in requests)
            {
                this.Join(request, progress);
            }
        }

        private void JoinImpl(List<string> parts, string outputPath)
        {
            this.CreateSourceListFile(parts);

            var fullPath = Path.GetFullPath(this.filename);

            using (var joinerProcess = new Process
            {
                StartInfo =
                {
                    FileName = "ffmpeg.exe",
                    Arguments = $"-f concat -safe 0 -i \"{fullPath}\" -c copy \"{outputPath}\"",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                },
            })
            {
                joinerProcess.OutputDataReceived += this.JoinerProcess_OutputDataReceived;
                joinerProcess.ErrorDataReceived += this.JoinerProcess_ErrorDataReceived;

                joinerProcess.Start();
                joinerProcess.BeginOutputReadLine();
                joinerProcess.BeginErrorReadLine();

                joinerProcess.WaitForExit();

                this.events.PublishOnUIThread(new TaskCompletedEvent());
            }
        }

        private void JoinerProcess_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            var logEvent = new LogEvent
            {
                Entry = new LogEntry
                {
                    Time = DateTime.Now,
                    Text = e.Data,
                },
            };

            this.events.PublishOnUIThread(logEvent);
        }

        private void JoinerProcess_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            var logEvent = new LogEvent
            {
                Entry = new LogEntry
                {
                    Time = DateTime.Now,
                    Text = e.Data,
                },
            };

            this.events.PublishOnUIThread(logEvent);
        }

        private void CreateSourceListFile(List<string> parts)
        {
            for (int i = 0; i < parts.Count; i++)
            {
                parts[i] = $"file '{parts[i]}'";
            }

            File.WriteAllLines(this.filename, parts);
        }
    }
}
