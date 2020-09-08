using System;
using Android.Content;
using Android.Views;
using Android.Widget;

namespace Space_Attack.Resources
{
    public class Asteroid
    {

        RelativeLayout Asteroid_Layout;
        TextView Asteroid_Health_TextView;

        float Gravity;
        int Health;
        int Points;

        float X, Y;

        // Overloading The Asteroid Function.

        public Asteroid(int Screen_Width, int Screen_Height, int Cureent_Level, Context Context)
        {

            Random Random_Generator = new Random(); // Used For Generating Random Numbers.

            // Generating The Size Of The Asteroid.
            int Temp_Width = Random_Generator.Next((int)(Screen_Width * 0.15), (int)(Screen_Width * 0.40)); // The Width Of The Asteroid Can Be Between 10% Of The Screen Length To 60%.
            int Temp_Height = (int)(Temp_Width * 1.72727273); // Adjusting Based On The Size Of The Image.


            Y = -Temp_Height; // Setting The Y Value Above The Screen Border.
            X = -Random_Generator.Next(0, Screen_Width - Temp_Width); // Setting The X Vlue Inside The Screen Border.

            Health = (int)(Temp_Width * 0.8); // The Bigger The Size Of The Asteroid The Bigger Health It Has.
            Gravity = (float)((Screen_Width * 0.45) / Temp_Width); // The Bigger The Size Of The Asteroid The Slower It Is.

            Health = (int)(Health * Cureent_Level * 0.75); // Changing The Health Of The Asteroid Based On The Current Level.

            Points = Health * 2;

            Setup_Asteroid_Layout(X, Y, Temp_Width, Temp_Height, Health, Context);
        }

        public Asteroid(int X, int Y, int Width, int Height, int Health, int Gravity, Context Context)
        {

            this.X = X;
            this.Y = Y;

            this.Health = Health;
            this.Gravity = Gravity;

            Points = Health * 2;


            Setup_Asteroid_Layout(X, Y, Width, Height, Health, Context);
        }

        private void Setup_Asteroid_Layout(float X, float Y, int Width, int Height, int Health, Context Context)
        {

            /*
                This Function Creates An Asteroid Based On The Given Variables.

                Asteroid Is Made Of 3 Elements.
                Layout - Holds The Image And The Health Display Number.
                ImageView - Holds An Image Of An Asteroid. ( Visualise For The User )
                TextView - Holds The Number Of HP (Health) Left To The Asteroid.
            */

            // --------------------- Creating The Asteroid Views ---------------------

            Asteroid_Layout = new RelativeLayout(Context);
            Asteroid_Layout.LayoutParameters = new RelativeLayout.LayoutParams(RelativeLayout.LayoutParams.WrapContent, RelativeLayout.LayoutParams.WrapContent);

            // Setting The Location Of The ImageView.
            Asteroid_Layout.SetX(X); // Setting The X Coordinates.
            Asteroid_Layout.SetY(Y); // Setting The Y Coordinates.

            // --- Create Asteroid ImageView ---

            ImageView ImageView = new ImageView(Context); // Creating A New ImageView. ( Allocating Memory )

            ImageView.LayoutParameters = new RelativeLayout.LayoutParams(Width, Height); // Setting The Width And The Height.
            ImageView.SetImageResource(Resource.Drawable.asteroid); // Setting The Image Of The ImageView.

            // --- Create Astroid TextView ---

            Asteroid_Health_TextView = new TextView(Context); // Creating A New TextView. ( Allocating Memory )

            Asteroid_Health_TextView.Text = Health.ToString(); // Setting The Text.
            Asteroid_Health_TextView.SetTextColor(Android.Graphics.Color.White); // Setting The Text Color.
            Asteroid_Health_TextView.SetTextSize(Android.Util.ComplexUnitType.Dip, Width / 10); // Setting The Font. ( By Dip )

            Asteroid_Health_TextView.Gravity = Android.Views.GravityFlags.Center; // Setting The Gravity.
            Asteroid_Health_TextView.LayoutParameters = new RelativeLayout.LayoutParams(Width, Height); // Setting The Width And The Height.

            // -------- Finishing Adding The Elements To The Asteroid Layout --------

            // Adding The Elements To The Asteroid Layout.
            Asteroid_Layout.AddView(ImageView); // Adding The ImageView To The Astroid Layout.
            Asteroid_Layout.AddView(Asteroid_Health_TextView); // Adding The TextView To The Astroid Layout.
        }

        // --- Getters & Setters ---

        // Getters

        public RelativeLayout Get_Layout()
        {

            // Returns The Layout Of The Asteroid.
            return Asteroid_Layout;
        }

        public float GetY()
        {

            // Returns The Y Value.
            return Y;
        }

        public float GetX()
        {

            // Returns The X Value.
            return X;
        }

        public int GetPoints()
        {

            // Returns The Points.
            return Points;

        }

        public int GetHealth()
        {

            // Returns The Health Value.
            return Health;
        }

        // Setters

        public void SetY(int Y)
        {

            // This Function Sets The Y Coordinate Of The Layout And Updates The Y Value Of The Object.
            Asteroid_Layout.SetY(Y);

            // Updating The Y Value.
            this.Y = Y;
        }

        public void SetX(int X)
        {

            // This Function Sets The X Coordinate Of The Layout And Updates The X Value Of The Object.
            Asteroid_Layout.SetX(X);

            // Updating The X Value.
            this.X = X;
        }

        // Game Mechanics

        public void Update_Gravity()
        {

            /*
                This Function Moves The Asteroid Based On The Gravity Value. 
            */

            // Moving The Asteroid Based On The Gravity.
            Asteroid_Layout.SetY(Asteroid_Layout.GetY() + Gravity);

            // Updates the Y Coordinate.
            Y += Gravity;
        }

        public int Update_Health(int Bullet_Dmg)
        {

            /*
                This Function Updates The Value Of The Health Based On The Bullet_Dmg Passed.
            */

            Health -= Bullet_Dmg; // Updating The Health Value.

            Asteroid_Health_TextView.Text = Health.ToString(); // Updating The Health TextView.

            return Health;
        }

        public bool Collision_Detection(View Layout, int Screen)
        {

            /*
                This Function Detect Collision Between The Asteroid Layout, And Another View.
            */

            float Asteroid_Top = this.Y;
            float Asteroid_Bottom = this.Y + Asteroid_Layout.Height;
            float Asteroid_Right = Screen + this.X;
            float Asteroid_Left = Screen + this.X - Asteroid_Layout.Width;


            float Layout_Top = Layout.GetY();
            float Layout_Bottom = Layout.GetY() + Layout.Height;
            float Layout_Right = Layout.GetX() + Layout.Width;
            float Layout_Left = Layout.GetX();

            //Log.Debug("Game - Logging", Asteroid_Top + ">" + Layout_Bottom + "||" + Asteroid_Bottom + "<" + Layout_Top + "||" + Asteroid_Right + "<" + Layout_Left + "||" + Asteroid_Left + ">" + Layout_Right);
            // Checking If The Asteroid Layout Is Colliding With The Bullet Layout.
            if (Asteroid_Top > Layout_Bottom || Asteroid_Bottom < Layout_Top || Asteroid_Right < Layout_Left || Asteroid_Left > Layout_Right)
                return false;

            return true;
        }
    }
}