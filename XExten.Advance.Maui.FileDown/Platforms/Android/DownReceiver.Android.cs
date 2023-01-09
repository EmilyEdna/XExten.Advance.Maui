using Android.App;
using Android.Content;
using Xamarin.Google.Crypto.Tink.Proto;
using XExten.Advance.Maui.FileDown.Platforms.Android;

namespace XExten.Advance.Maui.FileDown
{
    [BroadcastReceiver(Enabled = true, Exported = true), IntentFilter(new[] { DownloadManager.ActionDownloadComplete })]
    public class DownReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            var reference = intent.GetLongExtra(DownloadManager.ExtraDownloadId, -1);

            var downloadFile = IDownFileManager.Current.Queue.Cast<DownloadFile>().FirstOrDefault(f => f.Id == reference);
            if (downloadFile == null) return;

            var query = new DownloadManager.Query();
            query.SetFilterById(downloadFile.Id);

            try
            {
                using (var cursor = ((DownloadManager)context.GetSystemService(Context.DownloadService)).InvokeQuery(query))
                {
                    while (cursor != null && cursor.MoveToNext())
                    {
                        ((DownManager)IDownFileManager.Current).UpdateFileProperties(cursor, downloadFile);
                    }
                    cursor?.Close();
                }
            }
            catch (Exception)
            {

            }
        }
    }
}