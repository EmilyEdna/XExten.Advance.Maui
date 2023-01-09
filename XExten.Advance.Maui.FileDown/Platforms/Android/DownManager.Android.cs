using Android.App;
using Android.Content;
using Application = Android.App.Application;
using Android.Database;
using Android.OS;
using System.Collections.Specialized;

namespace XExten.Advance.Maui.FileDown.Platforms.Android
{
    public class DownManager : IDownManager, IDownFileManager
    {
        private Handler _downloadWatcherHandler;
        private Java.Lang.Runnable _downloadWatcherHandlerRunnable;

        private readonly DownloadManager _downloadManager;

        private readonly IList<IDownloadFile> _queue;

        public IEnumerable<IDownloadFile> Queue
        {
            get
            {
                lock (_queue)
                {
                    return _queue.ToList();
                }
            }
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public Func<IDownloadFile, string> PathNameForDownloadedFile { get; set; }

        public DownloadVisibility NotificationVisibility;

        public bool IsVisibleInDownloadsUi { get; set; } = true; // true is the default behavior from Android DownloadManagerApi

        public DownManager()
        {
            _queue = new List<IDownloadFile>();

            _downloadManager = (DownloadManager)Application.Context.GetSystemService(Context.DownloadService);

            // Add all items to the Queue that are pending, paused or running
            LoopOnDownloads(new Action<ICursor>(cursor => ReinitializeFile(cursor)));

            // Check sequentially if parameters for any of the registered downloads changed
            StartDownloadWatcher();
        }

        public IDownloadFile CreateDownloadFile(string url)
        {
            return CreateDownloadFile(url, new Dictionary<string, string>());
        }

        public IDownloadFile CreateDownloadFile(string url, IDictionary<string, string> headers)
        {
            return new DownloadFile(url, headers);
        }

        public void Start(IDownloadFile i, bool mobileNetworkAllowed = true)
        {
            var file = (DownloadFile)i;

            string destinationPathName = null;
            if (PathNameForDownloadedFile != null)
            {
                destinationPathName = PathNameForDownloadedFile(file);
            }

            file.StartDownload(_downloadManager, destinationPathName, mobileNetworkAllowed, NotificationVisibility, IsVisibleInDownloadsUi);
            AddFile(file);
        }

        public void Abort(IDownloadFile i)
        {
            var file = (DownloadFile)i;

            file.Status = StatusEnum.CANCELED;
            _downloadManager.Remove(file.Id);
            RemoveFile(file);
        }

        public void AbortAll()
        {
            foreach (var file in Queue)
            {
                Abort(file);
            }
        }

        void LoopOnDownloads(Action<ICursor> runnable)
        {
            // Reinitialize downloads that were started before the app was terminated or suspended
            var query = new DownloadManager.Query();
            query.SetFilterByStatus(
                DownloadStatus.Paused |
                DownloadStatus.Pending |
                DownloadStatus.Running
            );

            try
            {
                using (var cursor = _downloadManager.InvokeQuery(query))
                {
                    while (cursor != null && cursor.MoveToNext())
                    {
                        runnable.Invoke(cursor);
                    }
                    cursor?.Close();
                }
            }
            catch (Exception)
            {

            }
        }

        void ReinitializeFile(ICursor cursor)
        {
            var downloadFile = new DownloadFile(cursor);

            AddFile(downloadFile);
            UpdateFileProperties(cursor, downloadFile);
        }

        void StartDownloadWatcher()
        {
            // Create an instance for a runnable-handler
            _downloadWatcherHandler = new Handler();

            // Create a runnable, restarting itself to update every file in the queue
            _downloadWatcherHandlerRunnable = new Java.Lang.Runnable(() => {
                var downloads = Queue.Cast<DownloadFile>().ToList();

                foreach (var file in downloads)
                {
                    var query = new DownloadManager.Query();
                    query.SetFilterById(file.Id);

                    try
                    {
                        using (var cursor = _downloadManager.InvokeQuery(query))
                        {
                            if (cursor != null && cursor.MoveToNext())
                            {
                                UpdateFileProperties(cursor, file);
                            }
                            else
                            {
                                // This file is not listed in the native download manager anymore. Let's mark it as canceled.
                                Abort(file);
                            }
                            cursor?.Close();
                        }
                    }
                    catch (Exception)
                    {

                    }
                }

                _downloadWatcherHandler.PostDelayed(_downloadWatcherHandlerRunnable, 1000);
            });

            // Start this playing handler immediately
            _downloadWatcherHandler.PostDelayed(_downloadWatcherHandlerRunnable, 0);
        }

        /**
         * Update the properties for a file by it's cursor.
         * This method should be called in an interval and on reinitialization.
         */
        public void UpdateFileProperties(ICursor cursor, DownloadFile downloadFile)
        {
            downloadFile.TotalBytesWritten = cursor.GetFloat(cursor.GetColumnIndex(DownloadManager.ColumnBytesDownloadedSoFar));
            downloadFile.TotalBytesExpected = cursor.GetFloat(cursor.GetColumnIndex(DownloadManager.ColumnTotalSizeBytes));

            switch ((DownloadStatus)cursor.GetInt(cursor.GetColumnIndex(DownloadManager.ColumnStatus)))
            {
                case DownloadStatus.Successful:
                    downloadFile.DestinationPathName = cursor.GetString(cursor.GetColumnIndex("local_uri"));
                    downloadFile.StatusDetails = default(string);
                    downloadFile.Status = StatusEnum.COMPLETED;
                    RemoveFile(downloadFile);
                    break;

                case DownloadStatus.Failed:
                    var reasonFailed = cursor.GetInt(cursor.GetColumnIndex(DownloadManager.ColumnReason));
                    if (reasonFailed < 600)
                    {
                        downloadFile.StatusDetails = "Error.HttpCode: " + reasonFailed;
                    }
                    else
                    {
                        switch ((DownloadError)reasonFailed)
                        {
                            case DownloadError.CannotResume:
                                downloadFile.StatusDetails = "Error.CannotResume";
                                break;
                            case DownloadError.DeviceNotFound:
                                downloadFile.StatusDetails = "Error.DeviceNotFound";
                                break;
                            case DownloadError.FileAlreadyExists:
                                downloadFile.StatusDetails = "Error.FileAlreadyExists";
                                break;
                            case DownloadError.FileError:
                                downloadFile.StatusDetails = "Error.FileError";
                                break;
                            case DownloadError.HttpDataError:
                                downloadFile.StatusDetails = "Error.HttpDataError";
                                break;
                            case DownloadError.InsufficientSpace:
                                downloadFile.StatusDetails = "Error.InsufficientSpace";
                                break;
                            case DownloadError.TooManyRedirects:
                                downloadFile.StatusDetails = "Error.TooManyRedirects";
                                break;
                            case DownloadError.UnhandledHttpCode:
                                downloadFile.StatusDetails = "Error.UnhandledHttpCode";
                                break;
                            case DownloadError.Unknown:
                                downloadFile.StatusDetails = "Error.Unknown";
                                break;
                            default:
                                downloadFile.StatusDetails = "Error.Unregistered: " + reasonFailed;
                                break;
                        }
                    }
                    downloadFile.Status = StatusEnum.FAILED;
                    RemoveFile(downloadFile);
                    break;

                case DownloadStatus.Paused:
                    var reasonPaused = cursor.GetInt(cursor.GetColumnIndex(DownloadManager.ColumnReason));
                    switch ((DownloadPausedReason)reasonPaused)
                    {
                        case DownloadPausedReason.QueuedForWifi:
                            downloadFile.StatusDetails = "Paused.QueuedForWifi";
                            break;
                        case DownloadPausedReason.WaitingToRetry:
                            downloadFile.StatusDetails = "Paused.WaitingToRetry";
                            break;
                        case DownloadPausedReason.WaitingForNetwork:
                            downloadFile.StatusDetails = "Paused.WaitingForNetwork";
                            break;
                        case DownloadPausedReason.Unknown:
                            downloadFile.StatusDetails = "Paused.Unknown";
                            break;
                        default:
                            downloadFile.StatusDetails = "Paused.Unregistered: " + reasonPaused;
                            break;
                    }
                    downloadFile.Status = StatusEnum.PAUSED;
                    break;

                case DownloadStatus.Pending:
                    downloadFile.StatusDetails = default(string);
                    downloadFile.Status = StatusEnum.PENDING;
                    break;

                case DownloadStatus.Running:
                    downloadFile.StatusDetails = default(string);
                    downloadFile.Status = StatusEnum.RUNNING;
                    break;
            }
        }

        protected internal void AddFile(IDownloadFile file)
        {
            lock (_queue)
            {
                _queue.Add(file);
            }

            CollectionChanged?.Invoke(Queue, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, file));
        }

        protected internal void RemoveFile(IDownloadFile file)
        {
            lock (_queue)
            {
                _queue.Remove(file);
            }

            CollectionChanged?.Invoke(Queue, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, file));
        }
    }
}
