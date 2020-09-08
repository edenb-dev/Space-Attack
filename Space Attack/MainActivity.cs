using Android.App;
using Android.OS;
using Android.Widget;
using Android.Content;

namespace Space_Attack
{
    [BroadcastReceiver(Enabled = true)]
    [IntentFilter(new[] { Intent.ActionBatteryChanged })]
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : Android.Support.V7.App.AppCompatActivity
    {//AppCompatActivity

        ImageButton btnInstructions;
        ImageButton btnSettings;
        ImageButton btnPlay;

        Dialog dins;

        TextView tv;

        BroadcastBattery broadCastBattery;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);
            btnInstructions = FindViewById<ImageButton>(Resource.Id.btn_instructions);
            btnPlay = FindViewById<ImageButton>(Resource.Id.btn_play);
            btnSettings = FindViewById<ImageButton>(Resource.Id.btn_settings);

            btnInstructions.Click += BtnInstructions_Click;
            btnPlay.Click += BtnPlay_Click;
            btnSettings.Click += BtnSettings_Click;



            tv = FindViewById<TextView>(Resource.Id.tv);
            broadCastBattery = new BroadcastBattery(tv);

            // Updating The Score TextView.
            Update_Game_Score_TextView();
        }


        void Update_Game_Score_TextView()
        {

            /*
                This Function Loads The User's Game Score From Memory And Updates The Score TextView With It. 
            */

            ISharedPreferences Memory = this.GetSharedPreferences("Game_Data", FileCreationMode.Private); // Holds The Game Memory.
            TextView Score_TextView = FindViewById<TextView>(Resource.Id.Score_TextView);
            if (Memory.GetInt("User_Score", 0).ToString() != "0")
            {
                Score_TextView.Text = "סך הכל צברת " + Memory.GetInt("User_Score", 0).ToString() + " נקודות !";
            }
        }


        private void BtnSettings_Click(object sender, System.EventArgs e)
        {

            Intent intent = new Intent(this, typeof(SettingsActivity));
            StartActivity(intent);
        }

        private void BtnPlay_Click(object sender, System.EventArgs e)
        {
            Intent intent = new Intent(this, typeof(PlayActivity));
            StartActivity(intent);
        }

        private void BtnInstructions_Click(object sender, System.EventArgs e)
        {
            dins = new Dialog(this);
            dins.SetContentView(Resource.Layout.instructions_layout);
            dins.SetTitle("instructions");
            dins.SetCancelable(true);
            dins.Show();
        }

        protected override void OnResume()
        {
            // Updating The Score TextView.
            Update_Game_Score_TextView();
            base.OnResume();
            RegisterReceiver(broadCastBattery, new IntentFilter(Intent.ActionBatteryChanged));
        }

        protected override void OnPause()
        {
            UnregisterReceiver(broadCastBattery);
            base.OnPause();
        }
    }
}


