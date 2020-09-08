using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;

namespace Space_Attack
{
    [Activity(Label = "SettingsActivity")]
    public class SettingsActivity : Activity
    {
        ISharedPreferences sp;

        Switch swM;
        Switch swS;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.settings_layout);

            sp = this.GetSharedPreferences("settings", Android.Content.FileCreationMode.Private);



            swM = FindViewById<Switch>(Resource.Id.sw_music);
            swS = FindViewById<Switch>(Resource.Id.sw_sound);

            swM.CheckedChange += SwM_CheckedChange;

        }

        private void SwM_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {

            ISharedPreferencesEditor editor = sp.Edit();

            if (e.IsChecked)
                editor.PutInt("music", 1);
            else
                editor.PutInt("music", 0);

            editor.Commit();
        }

        private void SwS_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {

            ISharedPreferencesEditor editor = sp.Edit();

            if (e.IsChecked)
                editor.PutInt("sound", 1);
            else
                editor.PutInt("sound", 0);

            editor.Commit();
        }
    }
}