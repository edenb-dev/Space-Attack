using System;
using System.Collections.Generic;
using System.Linq;


using Android.App;
using Android.Content;
using Android.OS;

using Android.Views;
using Android.Widget;
using Android.Util;
using Space_Attack.Resources;
using Android.Views.Animations;
using Android.Media;

namespace Space_Attack
{
    [Activity(Label = "PlayActivity")]
    public class PlayActivity : Activity
    {
        // ----------------- Basic Variables ------------------

        Random Random_Generator = new Random(); // Used For Generating Random Numbers.

        int Screen_Width;   // Holds The Width Of The Screen.
        int Screen_Height; // Holds The Height Of The Screen.

        // ------------------ Game Variables ------------------

        // ----- Searching Variables -----

        List<Asteroid> Asteroids = new List<Asteroid>(); // Contains The Asteroids Objects.

        List<ImageView> Bullets = new List<ImageView>(); // Contains The Layout Of The Bullets.

        Player Player; // Holds The Player Object.

        // --- New Astroids - Limiters ---

        int Maximum_Asteroids_Amount = 6;   // Holds The Maximum Amount Of Asteroids, That Could Appear At Once On The Screen.

        // Holds The Range The Timer Could Have.
        const int New_Asteroid_Every_Min = 50;
        const int New_Asteroid_Every_Max = 300;

        int New_Asteroid_Timer = 0;     // Holds The Amount The Update Function Need To Be Ran, Before A New Astroid Can Be Created.
        int New_Asteroid_Counter = 0;   // Holds The Amount The Update Function Ran.

        // ---- New Bullte - Limiters ----

        int New_Bullte_Counter = 0; // Holds The Amount The Update Function Ran.

        // --------- Game Memory ---------

        ISharedPreferences Memory; // Holds The Game Memory.
        int Current_Level; // Holds The Current Level Of The Player. (Loaded From Memory)
        int User_Score = 0; // Holds The User Score.
        int Score_Till_Next_Level = 2500;

        // ------- Level Animation -------

        TextView Level_Title_TextView;
        TextView Level_SubTitle_TextView;

        List<string> Optional_SubTitles;

        int Need_To_Wait = 600; // Holds The Number Of Milisec * 50ms That Need To Pass Till The Level Animation Finishes.

        // ---- GUI - Score & Health -----

        TextView Health_TextView;
        TextView Score_TextView;

        // For music
        MediaPlayer mp;

        // -------- Update Timer ---------

        System.Timers.Timer Timer; // Holds The Global Timer.

        // -------------------------------

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.play_layout);

            // Setting The Background Of The Game Layout.
            FindViewById<RelativeLayout>(Resource.Id.GameLayout).SetBackgroundResource(Resource.Drawable.space);

            // Setting The Width And the Height Of The Screen.
            var Display = Resources.DisplayMetrics; // Getting The Display Metrics.

            Screen_Width = Display.WidthPixels; // Setting The Screen Width.
            Screen_Height = Display.HeightPixels; // Setting The Screen Height.

            // Loading The Game Memory.

            Memory = this.GetSharedPreferences("Game_Data", FileCreationMode.Private);

            Current_Level = Memory.GetInt("Current_Level", 1); // Loading The Current User.

            // Loading The Play Object And Displaying To The Screen.

            Create_Player();

            // Running The Clock.

            Timer = new System.Timers.Timer();
            Timer.Interval = 5; // Runs 50ms
            Timer.Elapsed += RunTimer;
            Timer.Enabled = true;

            // Loading The Level TextViews

            Create_Level_TextViews();

            // Animating The Level TextViews.

            Load_Level_Animation();

            Create_Score_And_Health_Bar();

            StartMusic();

        }

        void StartMusic()
        {

            // Checking If The User Disabled The Game Music.
            ISharedPreferences Settings = this.GetSharedPreferences("settings", FileCreationMode.Private);

            if (Settings.GetInt("music", 1) == 1)
            { // Music On.
                mp = MediaPlayer.Create(this, Resource.Raw.bgmusic);
                mp.Start();
            }
        }

        void Update()
        {

            /*
                This Function Is Used To Run The Game Mechanics.
            */

            // -------------- New Asteroids ---------------

            if (Need_To_Wait == 0) // Stoping The Game If Between Levels.
                Game_Mechanics_New_Asteroids();

            // ------------- Asteroid Gravity -------------

            Game_Mechanics_Asteroid_Gravity();

            // --------------- New Bulltes ----------------

            if (Need_To_Wait == 0) // Stoping The Game If Between Levels.
                Game_Mechanics_Shoot_Bullets();

            // -------------- Update Bulltes --------------

            Game_Mechanics_Update_Bullets();

            // --- Player/Asteroid Collision Detection ---

            /*
                This Section Manages The Player & Asteroid Collision Detection.
            */

            // -- Asteroid - Bullet Collision --

            if (Need_To_Wait == 0) // Stoping The Game If Between Levels.
                Game_Mechanics_Asteroid_Colision();

            // -- Player - Asteroid Collision --

            if (Need_To_Wait == 0) // Stoping The Game If Between Levels.
                Game_Mechanics_Player_Colision();

            // ----------------- Untitled ----------------

            if (Need_To_Wait != 0)
                Need_To_Wait--;
        }

        // Animations.

        void Load_Level_Animation()
        {

            /*
                This Function Loads The Level Animaion From Memory, Changes The Text Of The Level_SubTitle_TextView And Plays The Animations.
            */

            if (Need_To_Wait == 0) // Updating The Waiting Time.
                Need_To_Wait = 1000;

            // Updating The Level_Title_TextView.
            Level_Title_TextView.Text = "Level " + Current_Level.ToString();

            // Changing The Text Of The Level_SubTitle_TextView In To A Random SubTitle From The List.
            Level_SubTitle_TextView.Text = Optional_SubTitles[Random_Generator.Next(0, Optional_SubTitles.Count() - 1)];

            // Loading The Animation Script From The Resource.
            var Animations = AnimationUtils.LoadAnimation(this, Resource.Animation.level_animation);

            // Animating The TextViews.
            Level_Title_TextView.StartAnimation(Animations);
            Level_SubTitle_TextView.StartAnimation(Animations);
        }

        // Game Mec Functions.

        void Game_Mechanics_Asteroid_Gravity()
        {

            /*
                This Section Manages The Asteroids Movements Towards The Bottom Of The Screen,
                And Their Removal From The Screen, And Memory.
            */

            // Looping Thru The Asteroids List.
            for (int Index = 0; Index < Asteroids.Count(); Index++)
            {
                Asteroid Asteroid = Asteroids[Index]; // Creating A Reference To The Asteroid.

                if (Asteroid.GetY() < Screen_Height)
                { // If The Asteroid Didn't Hit The Bottom Of The Screen.

                    // Moving The Asteroid Relative To The Gravity.
                    Asteroid.Update_Gravity();

                }
                else
                { // If The Astroid Did Hit The Bottom Of The Screen.

                    // Removing The Asteroid From The List And The Game_Layout.
                    Remove_Asteroid(Asteroid);
                }
            }

        }

        void Game_Mechanics_New_Asteroids()
        {

            /*
                This Section Manages The Astroids Creation. ( Based On The Limits Specified )

                Limits :
                    * Maximum_Astroids_Amount - Controls The Maximum Amount Of Asteroids.
                    * New_Asteroid_Timer - Controls The Amount Of Time Needs To Be Passed Before A New Asteroid Could Be Created.

                Helper :
                    * New_Astroid_Counter - Counts The Amount Of Times The Function 'Update()' Ran.
            */

            // Checking If There Are Enough Asteroids On The Screen, And If The Enough Time Passed Before Last Creation.
            if ((Asteroids.Count() < Maximum_Asteroids_Amount) && (New_Asteroid_Counter > New_Asteroid_Timer))
            {

                // Creating A New Astroid.
                Create_Asteroid(Current_Level);

                // Reseting The Counter.
                New_Asteroid_Counter = 0;

                // After Every Asteroid Creation A New Timer Is Generated. ( Based On The Min And Max )
                New_Asteroid_Timer = Random_Generator.Next(New_Asteroid_Every_Min, New_Asteroid_Every_Max);
            }

            New_Asteroid_Counter++; // Incriminating The Counter.
        }

        void Game_Mechanics_Shoot_Bullets()
        {

            /*
                This Section Manages The Bullets Creation. ( Based On The Limits Specified )

                Limits :
                        * Player -> Shooting_Speed - Controls The Speed At Which The Bulltes Are Created.
                  
                Helper :
                    * New_Bullte_Counter - Counts The Amount Of Times The Function 'Update()' Ran.
            */

            if (New_Bullte_Counter == Player.GetShooting_Speed())
            {

                // Creating A New Bullet.
                Create_Bullet();

                // Reseting The Counter
                New_Bullte_Counter = 0;

            }

            New_Bullte_Counter++; // Incriminating The Counter.

        }

        void Game_Mechanics_Update_Bullets()
        {

            /*
                This Section Manages The Bullets Movements Towards The Top Of The Screen,
                And Their Removal From The Screen, And Memory.
            */

            // Looping Thru The Bullets List.
            for (int Index = 0; Index < Bullets.Count(); Index++)
            {
                ImageView Bullet = Bullets[Index]; // Creating A Reference To The Bullet.

                if (Bullet.GetY() + Bullet.Height > 0)
                { // If The Bullet Didn't Hit The Top Of The Screen.

                    // Moving The Bullet Relative To The Gravity.
                    Bullet.SetY(Bullet.GetY() - 5);

                }
                else
                { // If The Bullet Did Hit The Top Of The Screen.

                    // Removing The Bullet From The List And The Game_Layout.
                    Remove_Bullet(Bullet);

                    Log.Debug("Game - Logging", "Amount Of Bulltes " + Bullets.Count.ToString());
                }
            }
        }

        void Game_Mechanics_Asteroid_Colision()
        {

            /*
                This Section Manages The Bullets Colision With The Asteroids.
            */

            // Looping Thru The Bullets List.
            for (int Bullet_Index = 0; Bullet_Index < Bullets.Count(); Bullet_Index++)
            {
                ImageView Bullet = Bullets[Bullet_Index]; // Creating A Reference To The Bullet.

                // Looping Thru The Asteroids List.
                for (int Asteroid_Index = 0; Asteroid_Index < Asteroids.Count(); Asteroid_Index++)
                {
                    Asteroid Asteroid = Asteroids[Asteroid_Index]; // Creating A Reference To The Asteroid.

                    if (Asteroid.Collision_Detection(Bullet, Screen_Width))
                    { // If The Bullet Hit The Asteroid.

                        // Updating The Health Of The Asteroid, And Checking If It's Health Is Depleted Below  0.
                        if (Asteroid.Update_Health(Player.GetBullet_Strength()) <= 0)
                        {

                            // Updating The Score Of The User.
                            User_Score += Asteroid.GetPoints();

                            // Removing The Asteroid.
                            Remove_Asteroid(Asteroid);

                            if (User_Score >= Score_Till_Next_Level * 1.5)
                            {

                                Score_Till_Next_Level = (int)(Score_Till_Next_Level * 2.5);//Updating The Score Needed To Pass To The Next Level.
                                Current_Level++; // Updating The User Level.

                                // Loading The New Level Animaiton.
                                Load_Level_Animation();
                            }

                            // Updating The Score TextView.
                            Score_TextView.Text = (Score_Till_Next_Level * 1.5).ToString() + " / " + User_Score.ToString();
                        }

                        // Removing The Bullte.
                        Remove_Bullet(Bullet);
                    }
                }
            }
        }

        void Game_Mechanics_Player_Colision()
        {

            /*
                This Section Manages The Player Colision With The Asteroids.
            */

            // Looping Thru The Asteroids List.
            for (int Asteroid_Index = 0; Asteroid_Index < Asteroids.Count(); Asteroid_Index++)
            {
                Asteroid Asteroid = Asteroids[Asteroid_Index]; // Creating A Reference To The Asteroid.

                if (Player.Collision_Detection(Asteroid.Get_Layout()))
                { // If The Asteroid Hit The Player.

                    // Vibrating The Phone When User Hit Asteroid.
                    Vibrator Vibrator = (Vibrator)Application.Context.GetSystemService(VibratorService);
                    Vibrator.Vibrate(100);

                    // Updating The Health Of The Player, And Checking If It's Health Is Depleted Below  0.
                    if (Player.Update_Health(Asteroid.GetHealth()) <= 0)
                    {

                        // Saving The Game And Exiting To Main Screen.
                        Save_And_Exit();
                    }

                    Health_TextView.Text = Player.GetPlayer_Health().ToString(); // Update The Health TextView.

                    Remove_Asteroid(Asteroid);
                }
            }


        }

        void Save_And_Exit()
        {

            // Saving The User Score In To Memory.

            ISharedPreferencesEditor Memory_Editor = Memory.Edit();
            Memory_Editor.PutInt("User_Score", Memory.GetInt("User_Score", 0) + User_Score);
            Memory_Editor.Commit();

            // Stoping The Timer That Call The Function Update.
            Timer.Stop();

            // Redirecting The User To Main Screen.
            Finish();
        }

        // User Touch Function.

        public override bool OnTouchEvent(MotionEvent User_Touch)
        {

            // --- Moving The Player Based On The User Touch ---

            if (Player != null)
            { // Checking If The Player Is Alive.

                // Making Sure The Player Fully Renders On The Screen.
                if ((User_Touch.GetX() - (Player.GetPlayer_Size() / 2)) <= 0)
                    Player.SetX(0);
                else if ((User_Touch.GetX() + (Player.GetPlayer_Size() / 2)) >= Screen_Width)
                    Player.SetX(Screen_Width - Player.GetPlayer_Size());
                else
                    Player.SetX(User_Touch.GetX() - (Player.GetPlayer_Size() / 2));
            }

            return true;
        }

        // Timer Function.

        private void RunTimer(object sender, System.Timers.ElapsedEventArgs e) { RunOnUiThread(() => { Update(); }); }

        // Game Create Functions.

        void Create_Asteroid(int X, int Y, int Width, int Height, int Health, int Gravity)
        {

            /*
                This Function Creates A New Asteroid Object, Adds It To The Asteroid List And Adds It To The Game_Layout.
            */

            // Creating The Asteroid Object.
            Asteroid Asteroid = new Asteroid(X, Y, Width, Height, Health, Gravity, this);
            Asteroids.Add(Asteroid); // Adding The Asteroid Object To The Asteroid List.

            // Adding The Asteroid To The Game_Layout. ( Visual Representation )
            RelativeLayout GameLayout = FindViewById<RelativeLayout>(Resource.Id.GameLayout); // Creating A Reference To The Game_Layout.
            GameLayout.AddView(Asteroid.Get_Layout());
        }

        void Create_Asteroid(int Current_Level)
        {

            /*
                This Function Creates A New Asteroid Object, Adds It To The Asteroid List And Adds It To The Game_Layout.
            */

            // Creating The Asteroid Object.
            Asteroid Asteroid = new Asteroid(Screen_Width, Screen_Height, Current_Level, this);
            Asteroids.Add(Asteroid); // Adding The Asteroid Object To The Asteroid List.

            // Adding The Asteroid To The Game_Layout. ( Visual Representation )
            RelativeLayout GameLayout = FindViewById<RelativeLayout>(Resource.Id.GameLayout); // Creating A Reference To The Game_Layout.
            GameLayout.AddView(Asteroid.Get_Layout());
        }

        void Remove_Asteroid(Asteroid Asteroid)
        {

            /*
                This Function Removes An Asteroid From The Asteroid List And The Game_Layout.
            */

            // Removing The Asteroid From The List.
            Asteroids.Remove(Asteroid);

            // Removing The Asteroid From The Layout. ( Visual Representation )
            RelativeLayout GameLayout = FindViewById<RelativeLayout>(Resource.Id.GameLayout); // Creating A Reference To The Game Layout.
            GameLayout.RemoveView(Asteroid.Get_Layout());
        }

        void Create_Player()
        {

            /*
                This Function Creates, And Loads The Saved Player Data.
            */

            // Creating The Player Object, And Loading The Saved Player Data.
            Player = new Player(Memory.GetInt("Player_Health", 100), Memory.GetInt("Player_Defence", 2), Memory.GetInt("Bullet_Strength", 25), Memory.GetInt("Shooting_Speed", 20), Screen_Width, Screen_Height, this);

            // Adding The Player To The Game_Layout. ( Visual Representation )
            RelativeLayout GameLayout = FindViewById<RelativeLayout>(Resource.Id.GameLayout); // Creating A Reference To The Game_Layout.
            GameLayout.AddView(Player.Get_Layout());
        }

        void Create_Bullet()
        {

            /*
                This Function Creates A New Bullet Layout, Adds It To The Bullets List And Adds It To The Game_Layout.
            */

            // Creating The Bullet Layout.
            ImageView Bullet = Create_Costume_ImageView((Player.GetX() - Screen_Width) + (Player.Get_Layout().Width / 2) + ((int)(Screen_Width * 0.015)), Player.GetY(), (int)(Screen_Width * 0.03), (int)(Screen_Width * 0.03), Resource.Drawable.lizer);
            Bullets.Add(Bullet); // Creating, And Adding The Bullet Layout To The Bullets List.

            // Adding The Bullet To The Game_Layout. ( Visual Representation )
            RelativeLayout GameLayout = FindViewById<RelativeLayout>(Resource.Id.GameLayout); // Creating A Reference To The Game_Layout.
            GameLayout.AddView(Bullet);
        }

        void Remove_Bullet(ImageView Bullet)
        {

            /*
                This Function Removes An Bullet From The Bullets List And The Game_Layout.
            */

            // Removing The Bullet From The List.
            Bullets.Remove(Bullet);

            // Removing The Bullet From The Layout. ( Visual Representation )
            RelativeLayout GameLayout = FindViewById<RelativeLayout>(Resource.Id.GameLayout); // Creating A Reference To The Game Layout.
            GameLayout.RemoveView(Bullet);
        }

        void Create_Level_TextViews()
        {

            /*
                This Function Creates New Level TextViews And Adds Them To The Game Layout.
            */

            // Loading The Texts That Can Appear In The Level_SubTitle_TextView.
            Optional_SubTitles = new List<string>() {   "Come Here A Lot ?",
                                                        "What Did The Fox Say ?",
                                                        "Haven't I Been Here Before ?",
                                                        "Does This Game Even Have An Ending ?",
                                                        "Please Come Again ;)",
                                                        "It's Hard Sometimes, But Never Give Up !",
                                                        "3 L33t3r 8ack Word5",
                                                        "¡ǝW ǝʌɐS ǝsɐǝlԀ",
                                                        "Yfb Zoo Gsv Rgvnh Zmw Sv Droo Ivovzhv Nv!",
                                                        "It's Upside Down Silly ;)" };

            // Loading The Level TextViews

            Level_Title_TextView = Create_Costume_TextView(-(Screen_Width / 2) + (int)(Screen_Height * 0.1), -(int)(Screen_Height * 0.2), (int)(Screen_Height * 0.2), (int)(Screen_Height * 0.2), "", (int)(Screen_Height * 0.2) / 10);
            Level_SubTitle_TextView = Create_Costume_TextView(-(Screen_Width / 2) + (int)(Screen_Height * 0.1), -(int)(Screen_Height * 0.155), (int)(Screen_Height * 0.2), (int)(Screen_Height * 0.2), "", (int)(Screen_Height * 0.2) / 30);

            // Adding The Level TextViews To The Game Layout.
            RelativeLayout Game_Layout = FindViewById<RelativeLayout>(Resource.Id.GameLayout); // Creating A Reference To The Game_Layout.
            Game_Layout.AddView(Level_Title_TextView);
            Game_Layout.AddView(Level_SubTitle_TextView);
        }

        void Create_Score_And_Health_Bar()
        {

            ImageView Score_Bar_Background = Create_Costume_ImageView(0, -(int)(Screen_Width * 0.139), Screen_Width, 0, Resource.Drawable.score_background);

            Health_TextView = Create_Costume_TextView(-(int)(Screen_Width * 0.08), 0, (int)(Screen_Width * 0.22), (int)(Screen_Height * 0.047), Player.GetPlayer_Health().ToString(), (int)(Screen_Height * 0.01));
            Score_TextView = Create_Costume_TextView(-(int)(Screen_Width * 0.35), 0, (int)(Screen_Width * 0.4), (int)(Screen_Height * 0.047), Score_Till_Next_Level * 1.5 + " / " + 0, (int)(Screen_Height * 0.01)); ;

            // Adding The Bar Elements To The Game Layout.
            RelativeLayout Game_Layout = FindViewById<RelativeLayout>(Resource.Id.Main_Layout); // Creating A Reference To The Game_Layout.
            Game_Layout.AddView(Score_Bar_Background);
            Game_Layout.AddView(Health_TextView);
            Game_Layout.AddView(Score_TextView);
        }



        // Helpers

        ImageView Create_Costume_ImageView(float X, float Y, int Width, int Height, int Image_Resource)
        {

            ImageView ImageView = new ImageView(this); // Creating A New ImageView. ( Allocating Memory )

            // Setting The Location Of The ImageView.
            ImageView.SetX(X); // Setting The X Coordinates.
            ImageView.SetY(Y); // Setting The Y Coordinates.

            if (Height == 0)
                Height = WindowManagerLayoutParams.WrapContent;
            if (Width == 0)
                Width = WindowManagerLayoutParams.WrapContent;
            ImageView.LayoutParameters = new RelativeLayout.LayoutParams(Width, Height); // Setting The Width And The Height.
            ImageView.SetImageResource(Image_Resource); // Setting The Image Of The ImageView.

            return ImageView;
        }

        TextView Create_Costume_TextView(float X, float Y, int Width, int Height, string Text, int Font_Size)
        {

            TextView TextView = new TextView(this); // Creating A New ImageView. ( Allocating Memory )

            // Setting The Location Of The TextView.
            TextView.SetX(X); // Setting The X Coordinates.
            TextView.SetY(Y); // Setting The Y Coordinates.

            // Width And Height Settings.
            TextView.LayoutParameters = new RelativeLayout.LayoutParams(Width, Height); // Setting The Width And The Height.

            // Text Settings.
            TextView.Text = Text; // Setting The Text.
            TextView.SetTextColor(Android.Graphics.Color.White); // Setting The Text Color.
            TextView.SetTextSize(Android.Util.ComplexUnitType.Dip, Font_Size); // Setting The Font. ( By Dip )

            // Text Gravity Settings.
            TextView.Gravity = Android.Views.GravityFlags.Center; // Setting The Gravity. 

            return TextView;
        }

        // System Calls

        public override bool OnKeyDown(Keycode keyCode, KeyEvent KeyEvent)
        {
            if (KeyEvent.KeyCode == Keycode.Back)
            {
                // Saving The Game And Exiting To Main Screen.
                Save_And_Exit();

                base.OnBackPressed();
            }
            return base.OnKeyDown(keyCode, KeyEvent);
        }

        protected override void OnPause()
        {
            // Checking If The User Disabled The Game Music.
            ISharedPreferences Settings = this.GetSharedPreferences("settings", FileCreationMode.Private);

            if (Settings.GetInt("music", 1) == 1)
                mp.Stop();

            // Stoping The Game Timer.
            Timer.Stop();

            base.OnPause();
        }

        protected override void OnResume()
        {

            // Checking If The User Disabled The Game Music.
            ISharedPreferences Settings = this.GetSharedPreferences("settings", FileCreationMode.Private);

            if (Settings.GetInt("music", 1) == 1 && !mp.IsPlaying)
            { // Music On.
                mp = MediaPlayer.Create(this, Resource.Raw.bgmusic);
                mp.Start();
            }

            // Starting The Game Timer.
            Timer.Start();
            base.OnResume();
        }
    }
}