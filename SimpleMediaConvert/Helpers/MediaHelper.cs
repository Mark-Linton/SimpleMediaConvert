namespace SimpleMediaConvert.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using SimpleMediaConvert.Common;

    /// <summary>
    /// Various filesystem/media file helper methods.
    /// </summary>
    public class MediaHelper
    {
        private readonly string[] videoExtensions = { ".mp4", ".mkv", ".avi", ".wmv" };
        private readonly string[] partsSuffix = { "part 1", "part 2", "part 3", "part 4", "part 5", "part 6", "part 7", "part 8", "part 9" };

        /// <summary>
        /// Returns all files in a directory that are video files and named according to the Emby multi-part naming convension.
        /// </summary>
        /// <param name="sourceFolder">The directory to parse for media files.</param>
        /// <returns>A List of MediaItems. Will return an empty List if no valid media found. Will not return null.</returns>
        public List<MediaItem> GetMediaInSourceDirectory(string sourceFolder)
        {
            var items = new List<MediaItem>();

            if (!this.FolderExists(sourceFolder))
            {
                return items;
            }

            var files = Directory.GetFiles(sourceFolder)
                .Where(file => this.IsValidMediaFile(file))
                .Select(file => { return new FileInfo(file); })
                .ToList();

            if (files == null || files.Count == 0)
            {
                return items;
            }

            foreach (var file in files)
            {
                var mediaItem = new MediaItem
                {
                    Name = file.Name,
                    Path = file.DirectoryName,
                    Extension = file.Extension,
                };

                items.Add(mediaItem);
            }

            return items;
        }

        /// <summary>
        /// Determines whether or not a supplied path exists and is a folder.
        /// </summary>
        /// <param name="path">The path to check whether it exists.</param>
        /// <returns>True if the path exists and is a directory, False otherwise.</returns>
        public bool FolderExists(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return false;
            }

            return Directory.Exists(path);
        }

        /// <summary>
        /// Build a string containing a cleaned up filename & path for the supplied values.
        /// </summary>
        /// <param name="destinationFolder">The folder portion of the path.</param>
        /// <param name="item">The filename to be cleaned up.</param>
        /// <returns>A path containing the supplied filename/path with "part" stripped out.</returns>
        public string GetDestinationPath(string destinationFolder, MediaItem item)
        {
            var name = item.Name.Substring(0, item.Name.LastIndexOf("part", StringComparison.OrdinalIgnoreCase) - 1);
            return Path.Combine(destinationFolder, name + item.Extension);
        }

        private bool IsValidMediaFile(string file)
        {
            var info = new FileInfo(file);
            return this.IsVideoFile(info) && this.IsMultiPartFile(info);
        }

        private bool IsVideoFile(FileInfo info)
        {
            return this.videoExtensions.Contains(info.Extension);
        }

        private bool IsMultiPartFile(FileInfo info)
        {
            foreach (var suffix in this.partsSuffix)
            {
                if (info.Name.Contains(suffix))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
