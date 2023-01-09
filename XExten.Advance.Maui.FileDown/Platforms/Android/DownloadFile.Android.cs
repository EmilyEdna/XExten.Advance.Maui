using Android.App;
using Android.Database;
using System.ComponentModel;
using Uri = Android.Net.Uri;

namespace XExten.Advance.Maui.FileDown.Platforms.Android
{
    public class DownloadFile: IDownloadFile
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public long Id;

        public string Url { get; set; }

        private string _destinationPathName;

        public string DestinationPathName
        {
            get { return _destinationPathName; }
            set
            {
                if (Equals(_destinationPathName, value)) return;
                _destinationPathName = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DestinationPathName)));
            }
        }

        public IDictionary<string, string> Headers { get; }

        private StatusEnum _status;

        public StatusEnum Status
        {
            get
            {
                return _status;
            }
            set
            {
                if (Equals(_status, value)) return;
                _status = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Status)));
            }
        }

        private string _statusDetails;

        public string StatusDetails
        {
            get
            {
                return _statusDetails;
            }
            set
            {
                if (Equals(_statusDetails, value)) return;
                _statusDetails = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(StatusDetails)));
            }
        }

        private float _totalBytesExpected;

        public float TotalBytesExpected
        {
            get
            {
                return _totalBytesExpected;
            }
            set
            {
                if (Equals(_totalBytesExpected, value)) return;
                _totalBytesExpected = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TotalBytesExpected)));
            }
        }

        private float _totalBytesWritten;

        public float TotalBytesWritten
        {
            get
            {
                return _totalBytesWritten;
            }
            set
            {
                if (Equals(_totalBytesWritten, value)) return;
                _totalBytesWritten = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TotalBytesWritten)));
            }
        }

        /**
         * Initializing a new object to add it to the download-queue
         */
        public DownloadFile(string url, IDictionary<string, string> headers)
        {
            Url = url;
            Headers = headers;

            Status = StatusEnum.INITIALIZED;
        }

        /**
         * Reinitializing an object after the app restarted
         */
        public DownloadFile(ICursor cursor)
        {
            Id = cursor.GetLong(cursor.GetColumnIndex(DownloadManager.ColumnId));
            Url = cursor.GetString(cursor.GetColumnIndex(DownloadManager.ColumnUri));

            switch ((DownloadStatus)cursor.GetInt(cursor.GetColumnIndex(DownloadManager.ColumnStatus)))
            {
                case DownloadStatus.Failed:
                    Status = StatusEnum.FAILED;
                    break;

                case DownloadStatus.Paused:
                    Status = StatusEnum.PAUSED;
                    break;

                case DownloadStatus.Pending:
                    Status = StatusEnum.PENDING;
                    break;

                case DownloadStatus.Running:
                    Status = StatusEnum.RUNNING;
                    break;

                case DownloadStatus.Successful:
                    Status = StatusEnum.COMPLETED;
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void StartDownload(DownloadManager downloadManager, string destinationPathName,
            bool allowedOverMetered, DownloadVisibility notificationVisibility, bool isVisibleInDownloadsUi)
        {
            using (var downloadUrl = Uri.Parse(Url))
            using (var request = new DownloadManager.Request(downloadUrl))
            {
                if (Headers != null)
                {
                    foreach (var header in Headers)
                    {
                        request.AddRequestHeader(header.Key, header.Value);
                    }
                }

                if (destinationPathName != null)
                {
                    var file = new Java.IO.File(destinationPathName);
                    request.SetDestinationUri(Uri.FromFile(file));

                    if (file.Exists())
                    {
                        file.Delete();
                    }
                }

                request.SetVisibleInDownloadsUi(isVisibleInDownloadsUi);
                request.SetAllowedOverMetered(allowedOverMetered);

                request.SetNotificationVisibility(notificationVisibility);

                Id = downloadManager.Enqueue(request);
            }
        }
    }
}
