using Android.Content;
using Android.Views;
using Android.Widget;

namespace Space_Attack.Resources
{
    class Player
    {

        RelativeLayout Player_Layout;

        int Player_Defence;
        int Player_Health;

        int Bullet_Strength;
        int Shooting_Speed;

        float X, Y;

        int Player_Size;

        public Player(int Player_Health, int Player_Defence, int Bullet_Strength, int Shooting_Speed, int Screen_Width, int Screen_Height, Context Context)
        {

            // Setting The Size Of The Player.
            int Player_Width = (int)(Screen_Width * 0.15);
            int Player_Height = (int)(Player_Width * 1.33333333); // Adjusting Based On The Size Of The Image.

            Player_Size = Player_Width;

            this.Player_Health = Player_Health;
            this.Player_Defence = Player_Defence;

            this.Bullet_Strength = Bullet_Strength;
            this.Shooting_Speed = Shooting_Speed;

            this.X = (int)((Screen_Width / 2) - (int)(Player_Width / 2)); // Center Of Screen.
            this.Y = (int)((Screen_Height * 0.86) - Player_Width);

            Setup_Player_Layout(-X, Y, Player_Width, Player_Height, Player_Health, Context);
        }

        private void Setup_Player_Layout(float X, float Y, int Width, int Height, int Health, Context Context)
        {

            /*
                This Function Creates An Player Based On The Given Variables.
                
                Player Is Made Of 2 Elements.
                Layout - Holds The Image And The Health Display Number.
                ImageView - Holds An Image Of An Player. ( Visualise For The User )
            */

            // --------------------- Creating The Asteroid Views ---------------------

            Player_Layout = new RelativeLayout(Context);
            Player_Layout.LayoutParameters = new RelativeLayout.LayoutParams(RelativeLayout.LayoutParams.WrapContent, RelativeLayout.LayoutParams.WrapContent);

            // Setting The Location Of The ImageView.
            Player_Layout.SetX(X); // Setting The X Coordinates.
            Player_Layout.SetY(Y); // Setting The Y Coordinates.

            // --- Create Player ImageView ---

            ImageView ImageView = new ImageView(Context); // Creating A New ImageView. ( Allocating Memory )

            ImageView.LayoutParameters = new RelativeLayout.LayoutParams(Width, Height); // Setting The Width And The Height.
            ImageView.SetImageResource(Resource.Drawable.spaceship); // Setting The Image Of The ImageView.

            // -------- Finishing Adding The Elements To The Player Layout --------

            // Adding The Elements To The Player Layout.
            Player_Layout.AddView(ImageView); // Adding The ImageView To The Player Layout.
        }

        // --- Getters & Setters ---

        // Getters

        public RelativeLayout Get_Layout()
        {

            // Returns The Layout Of The Player.
            return Player_Layout;
        }

        public int GetPlayer_Defence()
        {

            // Returns The Player_Defence Value.
            return Player_Defence;
        }

        public int GetPlayer_Health()
        {

            // Returns The Player_Health Value.
            return Player_Health;
        }

        public int GetBullet_Strength()
        {

            // Returns The Bullet_Strength Value.
            return Bullet_Strength;
        }

        public int GetShooting_Speed()
        {

            // Returns The Shooting_Speed Value.
            return Shooting_Speed;
        }

        public int GetPlayer_Size()
        {

            // Returns The Player_Size Value.
            return Player_Size;
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

        // Setters

        public void SetY(int Y)
        {

            // This Function Sets The Y Coordinate Of The Layout And Updates The Y Value Of The Object.
            Player_Layout.SetY(Y);

            // Updating The Y Value.
            this.Y = Y;
        }

        public void SetX(float X)
        {

            // This Function Sets The X Coordinate Of The Layout And Updates The X Value Of The Object.
            Player_Layout.SetX(X);

            // Updating The X Value.
            this.X = X;
        }

        // Game Mechanics

        public int Update_Health(int Asteroid_Dmg)
        {

            /*
                This Function Updates The Health Of The Player, Based On The Dmg Of The Asteroid And The Player Defective Power. 
            */

            Player_Health -= Asteroid_Dmg / Player_Defence; // Updating The Health Value.

            return Player_Health;
        }

        public bool Collision_Detection(View Layout)
        {

            /*
                This Function Detect Collision Between The Player Layout, And Another View.
            */

            float Player_Top = this.Y;
            float Player_Bottom = this.Y + Player_Layout.Height;
            float Player_Right = this.X + Player_Layout.Width;
            float Player_Left = this.X;


            float Layout_Top = Layout.GetY();
            float Layout_Bottom = Layout.GetY() + Layout.Height;
            float Layout_Right = Layout.GetX() + Layout.Width;
            float Layout_Left = Layout.GetX();

            if (Player_Top > Layout_Bottom || Player_Bottom < Layout_Top || Player_Right < Layout_Left || Player_Left > Layout_Right)
                return false;

            return true;
        }
    }
}