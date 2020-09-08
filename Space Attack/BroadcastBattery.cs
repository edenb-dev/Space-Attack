using Android.App;
using Android.Content;
using Android.Widget;

namespace Space_Attack
{
    [BroadcastReceiver(Enabled = true)]
    [IntentFilter(new[] { Intent.ActionBatteryChanged })]
    public class BroadcastBattery : BroadcastReceiver
    {
        TextView tv;
        public BroadcastBattery()
        {
        }
        public BroadcastBattery(TextView tv)
        {
            this.tv = tv;
        }
        public override void OnReceive(Context context, Intent intent)
        {
            int battery = intent.GetIntExtra("level", 0);
            if (battery < 50)
            {
                tv.Text = "אולי כדאי להטעין? יש לך" + battery + "%";
            }
        }
    }
}