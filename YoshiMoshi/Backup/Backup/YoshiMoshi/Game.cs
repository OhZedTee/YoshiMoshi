/*-----------------------------------Header---------------------------------------
  
  Project Name	     : YoshiMoshi
  File Name		     : Game.cs
  Author		     : Ori Talmor
  Modified Date      : Monday, January 20, 2013
  Due Date		     : Monday, January 20, 2013
  Program Description: This XNA program is a tile based platformer similar to that
                       of Mario, with some tweaks. Its a fun, interactive game
                       to pass by time. It contains 4 levels, a high score system,
                       leaderboards, a hunger system, and much much more. This
                       specific file contains all the game changers, and basically
                       runs the game.
 --------------------------------------------------------------------------------*/

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using Camera2D;
using PrimitiveDrawing;

namespace YoshiMoshi
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game : Microsoft.Xna.Framework.Game
    {

        #region Global Variables

        //Drawing:
        GraphicsDeviceManager graphics;     //The manager needed inorder to draw the game
        SpriteBatch spriteBatch;            //The sprite batch which draws out the game

        //Textures in the game
        #region Textures
        Texture2D black;      //The black tile
        Texture2D apple;      //The apple tile
        Texture2D blue;       //The blue tile
        Texture2D finish;     //The finish tile
        Texture2D grnDown;    //The green arrow down tile
        Texture2D grnUp;      //The green arrow up tile
        Texture2D lvrBlue;    //The blue lever tile
        Texture2D lvrOrange;  //The orange lever tile
        Texture2D orange;     //The orange tile
        Texture2D red;        //The red tile
        Texture2D star;       //The star tile
        Texture2D wood;       //The wood tile
        Texture2D lifeUp;    //The life up tile
        Texture2D blank;      //Blank texture used to draw primitive shapes
        Texture2D backgroundImg;                      //The background image for the game
        #endregion

        //Lists:
        List<Tile> tilesToRemove = new List<Tile>();  //List of Tiles to remove from the game when prompted
        List<Tile> level = new List<Tile>();          //List of Tiles of the entire level
        List<Tile> environmental = new List<Tile>();  //List of Environmental tiles

        //Player:
        Player player = new Player();                 //A player entity of the player class

        //Inputs:
        KeyboardState key;                                           //The current state of the keyboard
        KeyboardState prevKb;                                        //The previous state of the keyboard during the last update
        GamePadState gamePad1 = GamePad.GetState(PlayerIndex.One);   //The current state of the gamepad
        GamePadState prevGamePad = GamePad.GetState(PlayerIndex.One);//The previous state of the gamepad

        //Camera:
        Cam2D camera;        //The camera

        //Audio:
        Song backgroundMusic;       //A song to be played for the background music
        SoundEffect death;          //A sound effect to be played when the player dies
        SoundEffect levelComplete;  //A sound effect to be played when the player completes a level
        SoundEffect gameComplete;   //A sound effect to be played when the player completes the game
        SoundEffect appleEating;    //A sound effect to be played when the player eats the apple
        SoundEffect starTaking;     //A sound effect to be played when the player takes a star
        SoundEffect gameOver;       //A sound effect to be played when the player loses the game 
        SoundEffect jump;           //A sound effect to be played when the player jumps
        SoundEffect highScore;      //A sound effect to be played when the player beats a high score
        SoundEffect gravitySwap;    //A sound effect to be played when the player lands on the gravity tile
        SoundEffect lifeUpSound;    //A sound effect to be played when the player gets an extra life
        bool isPlayedOnce = false;  //To make sure that the sound effect is played only once
        bool isPaused = false;      //To make sure that the background music is paused when needed and resumed when needed

        //Output:
        SpriteFont interactionFont;         //Font for the interactions text
        SpriteFont hudFont;                 //Font for the HUD text
        SpriteFont gameOverFont;            //Font for the game over text

        //HUD variables
        const float MAX_HEALTH = 100f;      //The maximum health of the player
        float curHealth = 100;              //The current health of the player
        float healthPercentage = MAX_HEALTH;//The current percentage of the total health of the player
        const int HEALTH_BAR_SIZE = 150;    //The pixel size of the health bar
        Vector2 healthBarLoc;               //The location of the health bar
        const float MAX_HUNGER = 100f;      //The maximum hunger of the player
        float curHunger = 100;              //The current hunger of the player
        float hungerPercentage = MAX_HUNGER;//The current percentage of the total hunger of the player
        const int HUNGER_BAR_SIZE = 150;    //The pixel size of the hunger bar
        Vector2 hungerBarLoc;               //The location of the hunger bar
        int hudObjectHeight = 32;           //The height of the graphical HUD objects

        //Important info for the game
        int mapNum = 1;                     //The map number that should be loaded
        int gameScore = 0;                  //The score of the game being played
        int levelCompleteScore;             //The score the player gets depending on the time it took him to beat the level
        int starCompleteScore;              //The score the player gets depending on how many stars he has by the end of a level 

        //Important information for the HUD
        Vector2 eatingLoc;                  //The location of eating output
        int stars = 0;                      //The amount of stars the player has
        Vector2 starOutputLoc;              //The location of the interaction output
        Vector2 starsLoc;                   //The location of the stars on the HUD
        int lives = 3;                      //The number of lives the player currently has
        Vector2 livesLoc;                   //The location of the lives output
        Vector2 lifeUpOutputLoc;            //The location of the interaction output for the life up

        //Time keepers:
        int secondsPassed = 0;                   //The amount of seconds passed in the game
        double timePassed = 0;                   //The time passed in milliseconds
        int time = 0;                            //The time passed for collision detection (to maintain double jump) 
        int seconds = 0;                         //The time passed that in seconds ***This variable is manipulated many times so its not reliable for time keeping***
        int soundTime;                           //The time passed since a soundeffect was played
        int cheatTime = 0;                       //The time passed after cheat stage activated
        int levelCompleteTime = 0;               //The time it took to complete the level
        int notifTimeApple;                      //The time that the notifications for the apple stay active
        int notifTimeStar;                       //The time that the notifications for the star stay active
        int notifTimeLifeUp;                     //The time that the notifications for the life up stay active
        Vector2 timerLoc;                        //The location of the timer
        const double ONE_SECOND_IN_MILLI = 1000; //One Second (in milliseconds)
        const int ONE_SECOND = 1;                //One second
        const int FIVE_SECONDS = 5;              //Five seconds
        const int TWO_SECONDS = 2;               //Two seconds
        const int FOUR_SECONDS = 4;              //Four seconds
        const int HUNDRED_MILLI = 100;           //100 milliseconds

        //Game Changers:
        Rectangle bgBounds;                 //The boundaries of the background
        int dir = 1;                        //The direction of the player on the Y axis (upside down or not)
        bool isHitEdge = false;             //If the player hit the edge of the screen
        bool isBlueActive = true;           //If the blue lever is active
        bool hasEatingOption = false;       //If the player is able to eat
        bool hasEaten = false;              //If the player has eaten
        Vector2 ateLoc;                     //Location of the output that the player ate
        bool hasTakingOption = false;       //If the player has the option to take a star
        bool hasTaken = false;              //If the player has taken a star
        Vector2 takenLoc;                   //Location of the output that the player took a star
        bool hasLifeUpOption = false;       //If the player has the option to take the life up tile
        bool hasTakenLife = false;          //If the player has taken a life up tile
        Vector2 lifeLoc;                    //Location of the output that the player took a life up tile
        bool isMuting = false;              //If the player is muting the game
        Vector2 muteLoc;                    //The location of the mute output
        bool isMuted = false;               //If the game is muted
        bool isRestarting = false;          //If the game is restarting
        bool isGameOver = false;            //If the game is over
        bool isGameBeat = false;            //If the game is beat
        Vector2 gameOverLoc;                //The location of the game over output
        bool isLevelComplete = false;       //If the level is complete
        bool isFlipped = false;             //If the player has changed direction (gravity)
        float rotationAngle;                //The angle of the player's rotation
        Vector2 playerOrigin;               //The center of the player
        Vector2 screenPos;                  //The center of the screen
        const int TILE_SIZE = 32;           //The size of each individual tile
        Vector2 levelLoadingPos;            //The location where the level loading output takes place
        bool isScoreCalculated = false;     //Whether or not the score was already calculated

        //HighScore:
        StreamReader highScoreInput;                        //Input taken from the high score
        StreamWriter highScoreOutput;                       //Writing to the high score file
        List<HighScore> highScores = new List<HighScore>(); //An array of scores with a possible 10 indexs
        bool isPlayerBeatHighScore = false;                 //If the player beat any of the 10 high scores.

        //Text box input:
        Rectangle textBox;           //The bounds of the text box
        Texture2D textBoxColor;      //The image of the text box
        Vector2 textBoxInstruct;     //The location for the instructions for the text box
        SpriteFont textBoxFont;      //The font for the text box and the instructions
        Vector2 textBoxText;         //The location of the text in the text box
        string text;                 //The text in the text box
        bool isFinished = false;     //If the player is finished writing in the text box
        bool isRead = false;         //If the program has read the external high scores file
        bool isWritten = false;      //If the program has written to the external high scores file

        //Leader Boards:
        bool isLeaderBoardsActive = false; //If the leader board is active
        Vector2 leaderBoardsLoc;           //The location of the leader boards
        SpriteFont leaderBoardsFont;       //The font for the leader board
        Texture2D leaderBoardsImg;         //The image of the leader board
        Rectangle leaderBoardsImgBounds;   //The bounds of the leader board image
        Rectangle screen;                  //The size of the screen

        //Credits:
        bool isCreditsActive = false;      //If the credits page is active
        Vector2 creditsLoc;                //The location of the credits page
        SpriteFont creditsFont;            //The font for the credits page
        SpriteFont titleFont;              //The font for the title in the credits Page
        Texture2D creditsImg;              //The background image of the credits
        Rectangle creditsImgBounds;        //The bounds of the credits page image

        //Tips:
        Vector2 tip1Loc;                    //The location of tip 1
        Vector2 tip2Loc;                    //The location of tip 2
        Vector2 tip3Loc;                    //The location of tip 3
        Vector2 tip4Loc;                    //The location of tip 4
        Vector2 tip5Loc;                    //The location of tip 5
        Vector2 tip6Loc;                    //The location of tip 6
        Vector2 tip7Loc;                    //The location of tip 7
        Vector2 tip8Loc;                    //The location of tip 8
        Vector2 tip9Loc;                    //The location of tip 9
        SpriteFont tipFont;                 //The font for the tips

        //Cheats:
        bool isCheatActiveStage1 = false;   //Is the first stage of the cheat button press done
        bool isCheatActiveStage2 = false;   //Is the second stage of the cheat button press done
        bool isCheatActiveStage3 = false;   //Is the third stage of the cheat button press done
        bool isCheatActiveStage4 = false;   //Is the fourth stage of the cheat button press done
        bool isGodModeActive = false;       //If god mode cheat is active
        bool isLifeCheatActivated = false;  //If the player activated the life cheat
        Vector2 godModeOutputLoc;           //The location of the output for god mode


        //Frame Animation:
        double timeSinceUpdate = 0;     //The time passed since the previous update
        double lastUpdateTime = 0;      //The time of the last update
        double frameTime = 0;           //The amount of time a single frame is visible
        double fps = 60;                //The number of times the screen is redrawin in one second
        Texture2D playerFront;          //The image storing the front sprite sheet
        Texture2D playerLeft;           //The image storing the left sprite sheet
        Texture2D playerRight;          //The image storing the right sprite sheet
        Rectangle playerLeftSrc;        //A rectangle acting like a cookie cutter to grab the correct frame of the sprite sheet
        Rectangle playerRightSrc;       //A rectangle acting like a cookie cutter to grab the correct frame of the sprite sheet
        int playerLeftFrameW;           //The width of each frame of animation on the sprite sheet
        int playerLeftFrameH;           //The height of each frame of animation on the sprite sheet
        int playerRightFrameW;          //The width of each frame of animation on the sprite sheet
        int playerRightFrameH;          //The height of each frame of animation on the sprite sheet
        int playerLeftFramesWide = 4;   //How many frames across the sprite sheet is
        int playerRightFramesWide = 4;  //How many frames across the sprite sheet is
        int playerFramesHigh = 1;       //How many frames top to bottom the sprite sheet is
        int playerSidesNumFrames = 12;  //The total number of frames in the side sprite sheets
        int playerFrameNum = 0;         //The current frame number to be drawn
        int[] playerSideSequences = new int[] { 0, 0, 0, 1, 1, 1, 2, 2, 2, 3, 3, 3, };  //The sequences for the player frame animation
        bool isPlayerFront = false;     //Whether or not the player is facing the front
        bool isPlayerLeft = false;      //Whether or not the player is facing left
        bool isPlayerRight = false;     //Whether or not the player is facing right

        #endregion

        public Game()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {

            //Helps with Camera Efficiency
            graphics.PreferMultiSampling = true;

            //Size of Screen
            graphics.PreferredBackBufferHeight = 640;
            graphics.PreferredBackBufferWidth = 800;
            graphics.ApplyChanges();
            screen.Width = graphics.PreferredBackBufferWidth;
            screen.Height = graphics.PreferredBackBufferHeight;

            //textBox size
            textBox = new Rectangle(250, 200, 300, 40);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            //Tiles:
            black = Content.Load<Texture2D>(@"Tiles\black");
            player.img = Content.Load<Texture2D>(@"Tiles\player");
            apple = Content.Load<Texture2D>(@"Tiles\apple");
            blue = Content.Load<Texture2D>(@"Tiles\blue");
            finish = Content.Load<Texture2D>(@"Tiles\finish");
            grnDown = Content.Load<Texture2D>(@"Tiles\greenDown");
            grnUp = Content.Load<Texture2D>(@"Tiles\greenUp");
            lvrBlue = Content.Load<Texture2D>(@"Tiles\leverBlue");
            lvrOrange = Content.Load<Texture2D>(@"Tiles\leverOrange");
            orange = Content.Load<Texture2D>(@"Tiles\orange");
            red = Content.Load<Texture2D>(@"Tiles\red");
            star = Content.Load<Texture2D>(@"Tiles\star");
            wood = Content.Load<Texture2D>(@"Tiles\wood");
            lifeUp = Content.Load<Texture2D>(@"Tiles\life up");

            //Background:
            backgroundImg = Content.Load<Texture2D>(@"Images\background");

            //Textbox:
            textBoxColor = Content.Load<Texture2D>(@"Images\solidred");
            textBoxFont = Content.Load<SpriteFont>(@"Fonts\textBoxFont");
            textBoxInstruct = new Vector2(280, 150);
            textBoxText = new Vector2(260, 210);

            //LeaderBoards:
            leaderBoardsImg = Content.Load<Texture2D>(@"Images\leaderboards");
            leaderBoardsFont = Content.Load<SpriteFont>(@"Fonts\leaderBoardsFont");
            leaderBoardsLoc = new Vector2(20, 85);
            leaderBoardsImgBounds = new Rectangle(0, 0, screen.Width, screen.Height);

            //Credits:
            creditsImg = Content.Load<Texture2D>(@"Images\credits");
            creditsFont = Content.Load<SpriteFont>(@"Fonts\creditsFont");
            titleFont = Content.Load<SpriteFont>(@"Fonts\titleFont");
            creditsLoc = new Vector2(screen.Width / 3 + 70, 100);
            creditsImgBounds = new Rectangle(0, 0, screen.Width, screen.Height);

            //Tips:
            tipFont = Content.Load<SpriteFont>(@"Fonts\tipFont");
            tip1Loc = new Vector2(40, 704);
            tip2Loc = new Vector2(352, 736);
            tip3Loc = new Vector2(1760, 704);
            tip4Loc = new Vector2(512, 160);
            tip5Loc = new Vector2(1120, 352);
            tip6Loc = new Vector2(2272, 704);
            tip7Loc = new Vector2(576, 672);
            tip8Loc = new Vector2(448, 544);
            tip9Loc = new Vector2(1408, 352);

            ///<HUD:>
            hudFont = Content.Load<SpriteFont>(@"Fonts\hudFont");
            //Health Bar:
            healthPercentage = curHealth / MAX_HEALTH; //Recalculates the health percentage
            healthBarLoc = new Vector2(20, 20);
            //HungerBar:
            hungerPercentage = curHunger / MAX_HUNGER; //Recalculates the hunger percentage
            hungerBarLoc = new Vector2(630, 52);
            //Timer:
            timerLoc = new Vector2(370, 20);
            //Lives:
            livesLoc = new Vector2(650, 20);
            //Interaction:            
            interactionFont = Content.Load<SpriteFont>(@"Fonts\interaction");
            eatingLoc = new Vector2(270, 80);
            starOutputLoc = new Vector2(270, 120);
            lifeUpOutputLoc = new Vector2(270, 160);
            muteLoc = new Vector2(180, 20);
            godModeOutputLoc = new Vector2(180, 60);
            gameOverLoc = new Vector2(eatingLoc.X + 50, eatingLoc.Y);
            starsLoc = new Vector2(20, 56);
            ateLoc = new Vector2(450, 20);
            takenLoc = new Vector2(430, 50);
            lifeLoc = new Vector2(430, 80);
            levelLoadingPos = new Vector2(screen.Width / 2 - 125, screen.Height / 2 - 30);
            gameOverFont = Content.Load<SpriteFont>(@"Fonts\gameOver");
            ///</HUD:>

            //Keyboard Data:
            prevKb = Keyboard.GetState();

            //Texture for Primitive Shapes:
            blank = new Texture2D(GraphicsDevice, 1, 1);
            blank.SetData(new[] { Color.White });

            //Frame Animation:
            //Set up time
            lastUpdateTime = System.Environment.TickCount;
            frameTime = ONE_SECOND_IN_MILLI / fps;
            //Load the sprite sheets:
            playerFront = Content.Load<Texture2D>(@"Images\yoshi front");
            playerLeft = Content.Load<Texture2D>(@"Images\yoshi left");
            playerRight = Content.Load<Texture2D>(@"Images\yoshi right");
            //Calculate frame width and height
            playerLeftFrameW = playerLeft.Width / playerLeftFramesWide;
            playerLeftFrameH = playerLeft.Height / playerFramesHigh;
            playerRightFrameW = playerRight.Width / playerRightFramesWide;
            playerRightFrameH = playerRight.Height / playerFramesHigh;
            //Set up the first cookie cutter for each image
            //playerFrontSrc = new Rectangle(0, 0, playerFrontFrameW, playerFrontFrameH);
            playerLeftSrc = new Rectangle(0, 0, playerLeftFrameW, playerLeftFrameH);
            playerRightSrc = new Rectangle(0, 0, playerRightFrameW, playerRightFrameH);

            //Audio:
            backgroundMusic = Content.Load<Song>(@"Audio\background");
            death = Content.Load<SoundEffect>(@"Audio\death");
            levelComplete = Content.Load<SoundEffect>(@"Audio\level complete");
            gameComplete = Content.Load<SoundEffect>(@"Audio\game complete");
            appleEating = Content.Load<SoundEffect>(@"Audio\apple");
            starTaking = Content.Load<SoundEffect>(@"Audio\star");
            gameOver = Content.Load<SoundEffect>(@"Audio\game Over");
            highScore = Content.Load<SoundEffect>(@"Audio\high Score");
            jump = Content.Load<SoundEffect>(@"Audio\jump");
            gravitySwap = Content.Load<SoundEffect>(@"Audio\gravity swap");
            lifeUpSound = Content.Load<SoundEffect>(@"Audio\life up");

            //Start the background music
            MediaPlayer.Play(backgroundMusic);
            MediaPlayer.IsRepeating = true;

            //Reads in the text file and creates the map
            ReadMap("map" + mapNum + ".txt");

            //Calls the CalcRange method to calculate the ranges
            CalcRange();


            //Player rotation:
            playerOrigin.X = 0;
            playerOrigin.Y = 0;
            screenPos.X = screen.Center.X;
            screenPos.Y = screen.Center.Y;
            //Set rotation Angle to 0
            rotationAngle = 0;


            //Initialize the camera object with the viewport
            camera = new Cam2D(GraphicsDevice.Viewport);

            // Set up all of your camera options
            //Set the bounding rectangle around the whole World
            camera.SetLimits(bgBounds);
            //Set the maximum factor to zoom in by, 3.0 is the default
            camera.SetMaxZoom(4.0f);
            //Set the starting World position of the camera, The below code sets it to the exact centre of the default screen size
            camera.SetPosition(new Vector2(0, 0));
            //Set the origin of the camera, typically is the starting position of the camera
            camera.SetOrigin(camera.GetPosition());

            //centre the camera on the player
            camera.LookAt(player.bounds);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || key.IsKeyDown(Keys.Escape))
                this.Exit();

            //Time passed is the amount of time between NOW 
            //and the last time a successful update occured
            timeSinceUpdate = System.Environment.TickCount - lastUpdateTime;

            //Only update if enough time has passed between frames
            if (timeSinceUpdate >= frameTime)
            {
                //Call the UpdateAnim method to Update the Frame Animation
                UpdateAnim();

                //Reset lastUpdateTime to NOW for the next update
                lastUpdateTime = System.Environment.TickCount;
            }

            //Gets the current state of the keyboard and the controller
            key = Keyboard.GetState();
            gamePad1 = GamePad.GetState(PlayerIndex.One);

            //If the game isn't over, if the credits page isn't open, and if the leader boards page isn't open:
            if (!isGameBeat && !isGameOver && !isLeaderBoardsActive && !isCreditsActive)
            {
                //The player's X velocity equal to itself and the status of the left thumb stick on the Controller
                player.vel.X += gamePad1.ThumbSticks.Left.X;

                //If the player isn't (barely) moving:
                if (player.vel.X <= 1)
                {
                    //Set the player to the front position and turn of all other positions
                    isPlayerFront = true;
                    isPlayerLeft = false;
                    isPlayerRight = false;
                }

                //If the player is moving right on the controller's left analog
                if (gamePad1.ThumbSticks.Left.X > 0)
                {
                    //Set the player to the right position and turn of all other positions
                    isPlayerFront = false;
                    isPlayerLeft = false;
                    isPlayerRight = true;
                }

                //If the player is moving left on the controller's left analog
                else if (gamePad1.ThumbSticks.Left.X < 0)
                {
                    //Set the player to the right position and turn of all other positions
                    isPlayerFront = false;
                    isPlayerLeft = true;
                    isPlayerRight = false;
                }

                //If the player is not moving on the controller's left analog
                else
                {
                    //Set the player to the front position and turn of all other positions
                    isPlayerFront = true;
                    isPlayerLeft = false;
                    isPlayerRight = false;
                }

                //If the player hits the D key:
                if (key.IsKeyDown(Keys.D))
                {
                    //If the player does not hit the right edge:
                    if (player.bounds.X + player.bounds.Width <= bgBounds.Width)
                    {
                        //Set the boolean hitting the edge to false
                        isHitEdge = false;

                        //Set the player to the right position and turn of all other positions
                        isPlayerRight = true;
                        isPlayerLeft = false;
                        isPlayerFront = false;

                        //Increment the player's X velocity
                        player.vel.X++;
                    }

                    //Otherwise:
                    else
                    {
                        //Set the isHitEdge boolean to true
                        isHitEdge = true;
                    }
                }

                //If the player hits the A key
                if (key.IsKeyDown(Keys.A))
                {
                    //If the player is not hitting the left side:
                    if (player.bounds.X > bgBounds.X)
                    {
                        //Set the isHitEdge boolean to false
                        isHitEdge = false;

                        //Set the player to the left position and turn of all other positions
                        isPlayerLeft = true;
                        isPlayerRight = false;
                        isPlayerFront = false;

                        //Decrement the player's X velocity
                        player.vel.X--;
                    }

                    //Otherwise:
                    else
                    {
                        //Set the isHitEdge boolean to true
                        isHitEdge = true;
                    }
                }

                //If the player is able to jump:
                if (player.isJumping)
                {
                    //If the player hits the W key:
                    if (key.IsKeyDown(Keys.W) || gamePad1.IsButtonDown(Buttons.A))
                    {
                        //If the direction is greater than 0:
                        if (dir > 0)
                        {
                            //If the game is not Muted and the Leader Boards are not open and if the credits page is not open:
                            if (!isMuted && !isLeaderBoardsActive && !isCreditsActive)
                            {
                                //Play the jump sound
                                jump.Play();
                            }


                            //Set the player to the front position and turn of all other positions
                            isPlayerFront = true;
                            isPlayerLeft = false;
                            isPlayerRight = false;

                            //Decrease the player's Y velocity by a factor of 17.
                            player.vel.Y -= 17;

                            //Set the jumping boolean to false
                            player.isJumping = false;
                        }
                    }

                    //If the player hits the S key:
                    if (key.IsKeyDown(Keys.S) || gamePad1.IsButtonDown(Buttons.A))
                    {
                        //If the direction is less than 0:
                        if (dir < 0)
                        {
                            //Increase the player's Y velocity by a factor of 17
                            player.vel.Y += 17;

                            //Set the jumping boolean to false
                            player.isJumping = false;

                            //Set the player to the front position and turn of all other positions
                            isPlayerFront = true;
                            isPlayerLeft = false;
                            isPlayerRight = false;
                        }
                    }
                }

                //Kill player and check for a game over
                if ((prevKb.IsKeyUp(Keys.K) && key.IsKeyDown(Keys.K)) || (prevGamePad.IsButtonUp(Buttons.LeftShoulder) && gamePad1.IsButtonDown(Buttons.LeftShoulder)))
                {
                    //Set the restarting boolean to true
                    isRestarting = true;

                    //If the player has no lives left:
                    if (lives == 0)
                    {
                        //Set the game over boolean to true
                        isGameOver = true;
                    }
                }

                //Mutes the sound or resumes the music
                if ((prevKb.IsKeyUp(Keys.M) && key.IsKeyDown(Keys.M)) || (prevGamePad.IsButtonUp(Buttons.RightShoulder) && gamePad1.IsButtonDown(Buttons.RightShoulder)))
                {
                    //If the music isn't muted:
                    if (isMuted == false)
                    {
                        //Mute the music
                        isMuting = true;
                    }

                    //Otherwise:
                    else
                    {
                        //Plays the music and sets the muting to false
                        isMuted = false;
                        MediaPlayer.Resume();
                    }
                }

                //Injure the player and check for a death
                if (prevKb.IsKeyUp(Keys.Space) && key.IsKeyDown(Keys.Space))
                {
                    //Decrease the player's current hunger
                    curHunger -= 5;

                    //If the player's hunger is less than or equal to 0:
                    if (curHunger <= 0)
                    {
                        //Decrease the health by a factor of 10
                        curHealth -= 10;

                        //If the players current health is less than or equal to 0:
                        if (curHealth <= 0)
                        {
                            //Set the restarting boolean to true:
                            isRestarting = true;

                            //If the player has no lives left:
                            if (lives == 0)
                            {
                                //Set the game over boolean to true
                                isGameOver = true;
                            }
                        }
                    }

                    //recalculate the hunger percentage
                    hungerPercentage = curHunger / MAX_HUNGER;
                    //recalculate the health percentage
                    healthPercentage = curHealth / MAX_HEALTH;
                }

                //Update the Timer
                timePassed += gameTime.ElapsedGameTime.Milliseconds;

                //If the timer is equal to or greater than 850 milliseconds:
                if (timePassed >= 850)
                {
                    //Increment seconds
                    seconds++;
                    //Increment seconds passed
                    secondsPassed++;

                    //If the level is not complete:
                    if (!isLevelComplete)
                    {
                        //Increment the player's level complete time
                        levelCompleteTime++;
                    }

                    //Set the timer to the value of 0
                    timePassed = 0;

                    //If the music is Paused:
                    if (isPaused)
                    {
                        //Increment the sound time
                        soundTime++;
                    }

                    //If the player has eaten an apple:
                    if (hasEaten)
                    {
                        //Increment the notification time for eating
                        notifTimeApple++;
                    }

                    //If the player has taken a star:
                    if (hasTaken)
                    {
                        //Increment the notification time for taking a star
                        notifTimeStar++;
                    }

                    //If the player has taken an extra life
                    if (hasTakenLife)
                    {
                        //Increment the notification time for taking an extra life
                        notifTimeLifeUp++;
                    }

                    //If the player has activated a stage in the cheat:
                    if (isCheatActiveStage1 || isCheatActiveStage2 || isCheatActiveStage3 || isCheatActiveStage4)
                    {
                        //Increment the cheat time
                        cheatTime++;
                    }
                }

                //Update Hunger after 5 seconds
                if (seconds == FIVE_SECONDS)
                {
                    //Decrease the hunger by a factor of 5
                    curHunger -= 10;

                    //If the player's current hunger is less than or equal to 0:
                    if (curHunger <= 0)
                    {
                        //Decrease the player's health by a factor of 10
                        curHealth -= 10;

                        //If the player's current health is less than or equal to 0:
                        if (curHealth <= 0)
                        {
                            //Set the restarting boolean to true
                            isRestarting = true;
                        }
                    }

                    //If the player has no lives left:
                    if (lives == 0)
                    {
                        //Set the game over boolean to false
                        isGameOver = true;
                    }

                    //recalculate the hunger percentage
                    hungerPercentage = curHunger / MAX_HUNGER;
                    //recalculate the health percentage
                    healthPercentage = curHealth / MAX_HEALTH;

                    //Set the seconds to 0
                    seconds = 0;
                }

                //If 2 seconds have passed:
                if (seconds == FOUR_SECONDS)
                {
                    //If the music is being muted:
                    if (isMuting)
                    {
                        //Pause the music
                        MediaPlayer.Pause();
                        //Set the muting boolean to false
                        isMuting = false;
                        //Set the muted boolean to true
                        isMuted = true;
                    }
                }

                //If 4 seconds have passed:
                if (soundTime == FOUR_SECONDS)
                {
                    //If the game is not restarting:
                    if (!isRestarting)
                    {
                        //If the music is paused:
                        if (isPaused)
                        {
                            //Resume the background music
                            MediaPlayer.Resume();
                            //Set the soundTime to 0
                            soundTime = 0;
                            //Set the paused boolean to false
                            isPaused = false;
                        }
                    }
                }

                //If the level is complete:
                if (isLevelComplete)
                {
                    //Call the level complete Method using the map number
                    LevelComplete(mapNum);
                }

                //Calls the restart sub program
                Restart();
            }

            //If the player has not beat a high score:
            if (!isPlayerBeatHighScore)
            {
                //If the L key was not pressed before but is pressed now, or if the start button was not pressed before but is being pressed now:
                if ((prevKb.IsKeyUp(Keys.L) && key.IsKeyDown(Keys.L)) || (prevGamePad.IsButtonUp(Buttons.Start) && gamePad1.IsButtonDown(Buttons.Start)))
                {
                    //If the leader boards page is not open:
                    if (isLeaderBoardsActive == false && !isCreditsActive)
                    {
                        //Set the leader boards active boolean to true
                        isLeaderBoardsActive = true;
                        //Pause the background music
                        MediaPlayer.Pause();
                    }

                    //If the leader boards is open and the credits page is not:
                    else if (!isCreditsActive)
                    {
                        //Set the leader boards active boolean to false
                        isLeaderBoardsActive = false;
                        //Resume the background music
                        MediaPlayer.Resume();
                    }
                }

                //If the C key was not previously pressed, but is pressed now or if the Left DPad button was not previously pressed but is now:
                else if ((prevKb.IsKeyUp(Keys.C) && key.IsKeyDown(Keys.C)) || (prevGamePad.IsButtonUp(Buttons.DPadLeft) && gamePad1.IsButtonDown(Buttons.DPadLeft)))
                {
                    //If the credits page is not open:
                    if (isCreditsActive == false && !isLeaderBoardsActive)
                    {
                        //Set the credits active boolean to true
                        isCreditsActive = true;
                        //Pause the background music
                        MediaPlayer.Pause();
                    }

                    //If the credits page is open and the leaderboards page is not:
                    else if (!isLeaderBoardsActive)
                    {
                        //Set the credits active boolean to false
                        isCreditsActive = false;
                        //Resume the background music
                        MediaPlayer.Resume();
                    }
                }

                //If the F key was not previously pressed, but is pressed now or if the Right DPad button was not previously pressed but is now:
                else if ((prevKb.IsKeyUp(Keys.F) && key.IsKeyDown(Keys.F)) || (prevGamePad.IsButtonUp(Buttons.DPadRight) && gamePad1.IsButtonDown(Buttons.DPadRight)))
                {
                    //If the game isn't full screen:
                    if (!graphics.IsFullScreen)
                    {
                        //Set the map to full screen
                        graphics.IsFullScreen = true;
                        graphics.ApplyChanges();

                        //Set the height of the bounds of the map one tile higher so the player can see everything more clearly
                        Rectangle tempBounds = bgBounds;
                        tempBounds.Height += TILE_SIZE;

                        //Set the camera limits to the newly created temporary bounds
                        camera.SetLimits(tempBounds);
                    }

                    else
                    {
                        //Set the game to smaller screen
                        graphics.IsFullScreen = false;
                        graphics.ApplyChanges();

                        //Set the limits of the camera to the backgrounds boundary
                        camera.SetLimits(bgBounds);
                    }
                }

                //If the player presses the NumPad1 key or the X button or the cheatStage1 is active:
                else if ((key.IsKeyDown(Keys.NumPad1) || gamePad1.IsButtonDown(Buttons.X)) || isCheatActiveStage1)
                {
                    //Set the cheat stage 1 active to true
                    isCheatActiveStage1 = true;

                    //If two seconds pass since the player inputed a cheat:
                    if (cheatTime == TWO_SECONDS)
                    {
                        //Set the cheat stage 1 active to false;
                        isCheatActiveStage1 = false;

                        //Set the cheat time back to 0
                        cheatTime = 0;
                    }

                    //If the player presses the NumPad2 key or the Y button or the cheatStage2 is active:
                    if ((key.IsKeyDown(Keys.NumPad2) || gamePad1.IsButtonDown(Buttons.Y)) || isCheatActiveStage2)
                    {
                        //Set the cheat stage 2 active to true
                        isCheatActiveStage2 = true;

                        //If two seconds pass since the player inputed a cheat:
                        if (cheatTime == TWO_SECONDS)
                        {
                            //Set the cheat stage 2 active to false;
                            isCheatActiveStage2 = false;

                            //Set the cheat time back to 0
                            cheatTime = 0;
                        }

                        //If the player presses the NumPad3 key or the B button or the cheatStage3 is active:
                        if ((key.IsKeyDown(Keys.NumPad3) || gamePad1.IsButtonDown(Buttons.B)) || isCheatActiveStage3)
                        {
                            //Set the cheat stage 3 active to true
                            isCheatActiveStage3 = true;

                            //If two seconds pass since the player inputed a cheat:
                            if (cheatTime == TWO_SECONDS)
                            {
                                //Set the cheat stage 3 active to false;
                                isCheatActiveStage3 = false;

                                //Set the cheat time back to 0
                                cheatTime = 0;
                            }

                            //If the player presses the NumPad4 key or the A button or the cheatStage4 is active:
                            if ((key.IsKeyDown(Keys.NumPad4) || gamePad1.IsButtonDown(Buttons.A)) || isCheatActiveStage4)
                            {
                                //Set the cheat stage 4 active to true
                                isCheatActiveStage4 = true;

                                //If two seconds pass since the player inputed a cheat:
                                if (cheatTime == TWO_SECONDS)
                                {
                                    //Set the cheat stage 3 active to false;
                                    isCheatActiveStage4 = false;

                                    //Set the cheat time back to 0
                                    cheatTime = 0;
                                }

                                //If the map number is less than 4:
                                if (mapNum < 4)
                                {
                                    //If the game isn't muted, the leader boards aren't open, and the credits page isn't open:
                                    if (!isMuted && !isLeaderBoardsActive && !isCreditsActive)
                                    {
                                        //Play the level complete sound effect
                                        levelComplete.Play();
                                    }

                                    //Increment the map number
                                    mapNum++;

                                    //Set the is level complete boolean to true
                                    isLevelComplete = true;
                                }

                                //If the map number is 4 or greater:
                                else
                                {
                                    //Set the is game beat boolean to true
                                    isGameBeat = true;
                                }

                                //Set all the cheat active stages to false
                                isCheatActiveStage1 = false;
                                isCheatActiveStage2 = false;
                                isCheatActiveStage3 = false;
                                isCheatActiveStage4 = false;
                            }

                            //If the player presses the NumPad8 key or the D Pad Up button:
                            if ((key.IsKeyDown(Keys.NumPad8) || gamePad1.IsButtonDown(Buttons.DPadUp)))
                            {
                                //If God mode isn't active:
                                if (!isGodModeActive)
                                {
                                    //Set god mode to active:
                                    isGodModeActive = true;
                                }

                                //Otherwise, if god mode is active:
                                else
                                {
                                    //Set god mode to not active
                                    isGodModeActive = false;
                                }

                                //Set the first 3 cheat active stages to false
                                isCheatActiveStage1 = false;
                                isCheatActiveStage2 = false;
                                isCheatActiveStage3 = false;
                            }
                        }

                        //If the player presses the NumPad9 key or the D Pad Down button:
                        if ((key.IsKeyDown(Keys.NumPad9) || gamePad1.IsButtonDown(Buttons.DPadDown)))
                        {
                            //Set lives to 99999
                            lives = 99999;

                            //Set the life cheat activated to true
                            isLifeCheatActivated = true;

                            //Set the first 3 cheat active stages to false
                            isCheatActiveStage1 = false;
                            isCheatActiveStage2 = false;
                            isCheatActiveStage3 = false;
                        }

                        //If the player presses the NumPad7 key or the Left Trigger button:
                        if ((key.IsKeyDown(Keys.NumPad7) || gamePad1.IsButtonDown(Buttons.LeftTrigger)))
                        {
                            //Set hunger to 100
                            curHunger = 100;


                            //recalculate the hunger percentage
                            hungerPercentage = curHunger / MAX_HUNGER;
                            //recalculate the health percentage
                            healthPercentage = curHealth / MAX_HEALTH;

                            //Set the first 3 cheat active stages to false
                            isCheatActiveStage1 = false;
                            isCheatActiveStage2 = false;
                            isCheatActiveStage3 = false;
                        }
                    }
                }
            }

            //Call the Collision detection method with range and referencing player
            ColStatic(ref player, gameTime);

            //Checks each element in the list tilesToRemove
            foreach (Tile element in tilesToRemove)
            {
                //Removes the tile in the world using the element in the tilesToRemove list
                environmental.Remove(element);
            }

            //Call the gravity method in the player class
            player.Gravity();

            if (!isHitEdge)
            {
                //Call the updateX procedure in the player class
                player.UpdateX();
            }

            player.UpdateY();

            //Call the direction function in the player class
            player.Direction(dir);

            #region Movable Items Gravity

            //Checks every element in the the environment layer:
            foreach (Tile element in environmental)
            {
                //If the element is movable:
                if (element.movable)
                {
                    //Initialize a new temporary Tile with the values of element
                    Tile temp = element;
                    //Increase the velocity of the temporary Tile
                    temp.vel += dir;

                    //Make sure that the movable tile can't move so fast that he'll skip collsion detection
                    //If the movable tile's velocity is greater than or equal to 10:
                    if (temp.vel >= 10)
                    {
                        //Set the velocity to 10
                        temp.vel = 10;
                    }

                    //If the movable til's velocity is Less than or equal to -10:
                    else if (temp.vel <= -10)
                    {
                        //Set the velocity to -10
                        temp.vel = -10;
                    }

                    //Increase the Y value of the temporary boundary by a factor of the temporary velocity multiplied by 3
                    temp.tileBounds.Y += (int)temp.vel * 3;

                    //Initialize a boolean hit and set it to false
                    bool hit = false;

                    //For each tile in level:
                    foreach (Tile t in level)
                    {
                        //If the image of the tile is not null:
                        if (t.tileImg != null)
                        {
                            //If the bounds of the tile is not equal to the temporary bounds and if the bounds of the tile is not equal to the element's bounds:
                            if (t.tileBounds != temp.tileBounds && t.tileBounds != element.tileBounds)
                            {
                                //If the tile is not Move Through-able:
                                if (!t.moveThrough)
                                {
                                    //If the bounds of temporary Intersects with the bounds of the tile:
                                    if (temp.tileBounds.Intersects(t.tileBounds))
                                    {
                                        //Set the boolean hit to true
                                        hit = true;
                                        //Set the element's velocity to 0
                                        element.vel = 0;

                                        //If the direction is greater than 0:
                                        if (dir > 0)
                                        {
                                            //Set the Y value of the bounds of the element to the Y value of the bounds of the tile subtracted by the height of the bounds of the element
                                            element.tileBounds.Y = t.tileBounds.Y - element.tileBounds.Height;
                                        }

                                        //If the direction is less than or equal to 0:
                                        else
                                        {
                                            //Set the Y value of the bounds of the element to the Y value of the bounds of the tile plus the height of the bounds of the tile
                                            element.tileBounds.Y = t.tileBounds.Y + t.tileBounds.Height;
                                        }

                                        //Break out of the For each statement
                                        break;
                                    }
                                }
                            }
                        }
                    }

                    //If hit is equal to false:
                    if (!hit)
                    {
                        //Set the element bounds to the temporary bounds
                        element.tileBounds = temp.tileBounds;
                        //Decrease the Y value of the elements bounds by the temporary velocity
                        element.tileBounds.Y -= (int)temp.vel;
                        //Set the elements velocity to the temporary velocity
                        element.vel = temp.vel;
                    }
                }
            }

            #endregion

            //If the blue lever is active
            if (isBlueActive)
            {
                //For each tile in the level:
                foreach (Tile element in level)
                {
                    //If the tile's image is orange:
                    if (element.tileImg == orange)
                    {
                        //Set the move through ability of the tile to true
                        element.moveThrough = true;
                    }

                    //Else if the tile's image is blue:
                    else if (element.tileImg == blue)
                    {
                        //Set the move through ability of the tile to false
                        element.moveThrough = false;
                    }
                }
            }

            //Else if the blue lever is not active (The orange lever is active)
            else if (!isBlueActive)
            {
                //For each tile in the level
                foreach (Tile element in level)
                {
                    //If the tile's image is blue:
                    if (element.tileImg == blue)
                    {
                        //Set the move through ability of the tile to true
                        element.moveThrough = true;
                    }

                    //Else if the tile's image is orange:
                    else if (element.tileImg == orange)
                    {
                        //Set the move through ability of the tile to false
                        element.moveThrough = false;
                    }
                }
            }

            //If the boolean is Read is equal to false:
            if (!isRead)
            {
                //Call the High Score Reading Procedure
                HighScoreReading();
            }

            //camera.LookAt(camera.GetPosition());
            camera.LookAt(player.bounds);

            //If the game is not over:
            if (!isGameBeat && !isGameOver)
            {
                //Set the previous key to the current key
                prevKb = key;
                //Set the previous game pad to the current game pad
                prevGamePad = gamePad1;
            }

            //Update the game time
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            //If the leader board page is not open and if the credits page is not open:
            if (!isLeaderBoardsActive && !isCreditsActive)
            {
                //Clear the screen and set the colour to corn flower blue
                GraphicsDevice.Clear(Color.CornflowerBlue);

                //Begin the spriteBatch with the camera:
                spriteBatch.Begin(SpriteBlendMode.AlphaBlend,
                             SpriteSortMode.Immediate,
                             SaveStateMode.SaveState,
                             camera.GetTransformation());

                //Draw the background image:
                spriteBatch.Draw(backgroundImg, bgBounds, Color.White);

                //Draw the level:
                //For each index in level
                for (int i = 0; i < level.Count; i++)
                {
                    //If the level image at the I index is equal to orange and the blue lever is active
                    if (level[i].tileImg == orange && isBlueActive == true)
                    {
                        //Draw the tile at index I but make the colour of the tile translucent
                        spriteBatch.Draw(level[i].tileImg, level[i].tileBounds, new Color(255, 255, 255, 128));
                    }

                    //If the level image at the I index is equal to blue and the blue lever is not active (the orange lever is active)
                    else if (level[i].tileImg == blue && isBlueActive == false)
                    {
                        //Draw the tile at index I but make the colour of the tile translucent
                        spriteBatch.Draw(level[i].tileImg, level[i].tileBounds, new Color(255, 255, 255, 128));
                    }

                    //Otherwise:
                    else
                    {
                        //Call the Draw function from the tile class using level at index I to draw the tile 
                        level[i].Draw(spriteBatch);
                    }
                }

                //Tips:
                //If the player is on the first level:
                if (mapNum == 1)
                {
                    //Creates and Initializes tips:
                    string tip1 = "Use 'WASD' to move";
                    string tip2 = "Gravity Swap!";
                    string tip3 = "Eat apples to replenish hunger\nand to increase time!";
                    string tip4 = "Pick up stars to increase score\nand to decrease time";
                    string tip5 = "Levers Open new Opportunites!";
                    string tip6 = "Your time is what determines you score, So you Better Hurry!";
                    string tip7 = "Be Careful! Red blocks Hurt! A lot!";

                    //Call on the BackgroundText function to draw the background for the text/tips (for readability)
                    BackgroundText(spriteBatch, tip1Loc, 180, 22);
                    BackgroundText(spriteBatch, tip2Loc, 116, 22);
                    BackgroundText(spriteBatch, tip3Loc, 258, 44);
                    BackgroundText(spriteBatch, tip4Loc, 260, 44);
                    BackgroundText(spriteBatch, tip5Loc, 270, 22);
                    BackgroundText(spriteBatch, tip6Loc, 510, 22);
                    BackgroundText(spriteBatch, tip7Loc, 292, 22);

                    //Draw Tips:
                    spriteBatch.DrawString(tipFont, tip1, tip1Loc, Color.Black);
                    spriteBatch.DrawString(tipFont, tip2, tip2Loc, Color.Black);
                    spriteBatch.DrawString(tipFont, tip3, tip3Loc, Color.Black);
                    spriteBatch.DrawString(tipFont, tip4, tip4Loc, Color.Black);
                    spriteBatch.DrawString(tipFont, tip5, tip5Loc, Color.Black);
                    spriteBatch.DrawString(tipFont, tip6, tip6Loc, Color.Black);
                    spriteBatch.DrawString(tipFont, tip7, tip7Loc, Color.Black);
                }

                else if (mapNum == 2)
                {
                    //Creates and Initializes tips:
                    string tip8 = "If needed:";
                    string tip9 = "EXTRA LIFE!!!";

                    //Call on the BackgroundText function to draw the background for the text/tips (for readability)
                    BackgroundText(spriteBatch, tip8Loc, 90, 22);
                    BackgroundText(spriteBatch, tip9Loc, 130, 22);

                    //Draw Tip:
                    spriteBatch.DrawString(tipFont, tip8, tip8Loc, Color.Black);
                    spriteBatch.DrawString(tipFont, tip9, tip9Loc, Color.Black);
                }

                //Draw the player:
                //If the player is Flipped:
                if (isFlipped == true)
                {
                    //Set screen position to the player's position
                    screenPos.X = player.bounds.X;
                    screenPos.Y = player.bounds.Y;

                    //If the player is facing the front:
                    if (isPlayerFront)
                    {
                        //Draw the player with the front image but flip him vertically
                        spriteBatch.Draw(playerFront, screenPos, null, Color.White, rotationAngle, playerOrigin, 1.0f,
                            SpriteEffects.FlipVertically, 0);
                    }

                    //If the player is facing the right:
                    else if (isPlayerRight)
                    {
                        //Draw the player with the right image but flip him vertically
                        spriteBatch.Draw(playerRight, screenPos, playerRightSrc, Color.White, rotationAngle, playerOrigin, 1.0f,
                            SpriteEffects.FlipVertically, 0);
                    }

                    //If the player is facing the left:
                    else if (isPlayerLeft)
                    {
                        //Draw the player with the left image but flip him vertically
                        spriteBatch.Draw(playerLeft, screenPos, playerLeftSrc, Color.White, rotationAngle, playerOrigin, 1f,
                            SpriteEffects.FlipVertically, 0);
                    }
                }

                //If the player is not flipped
                else
                {
                    //If the player is facing the front:
                    if (isPlayerFront)
                    {
                        //Draw the player normally with the front image
                        spriteBatch.Draw(playerFront, player.bounds, Color.White);
                    }

                    //If the player is facing the right:
                    else if (isPlayerRight)
                    {
                        //Draw the player normally with the right image and the correct source image
                        spriteBatch.Draw(playerRight, player.bounds, playerRightSrc, Color.White);
                    }

                    //If the player is facing the left:
                    else if (isPlayerLeft)
                    {
                        //Draw the player normally with the left image and the correct source image
                        spriteBatch.Draw(playerLeft, player.bounds, playerLeftSrc, Color.White);
                    }
                }

                //Environmental Layers:
                //For each index in level
                for (int i = 0; i < environmental.Count; i++)
                {
                    //Draws every Environmental Tile
                    environmental[i].Draw(spriteBatch);

                }

                //End the spriteBatch
                spriteBatch.End();

                //Begin the spriteBatch
                spriteBatch.Begin();

                //Call the DrawHud function using the spriteBatch to draw the entire HUD (Heads Up Display)
                DrawHud(spriteBatch);

                //End the spriteBatch
                spriteBatch.End();
            }

            //Else if the leader bards page is open:
            else if (isLeaderBoardsActive && !isCreditsActive)
            {
                //Clear the page and draw the background colour as Cornflower Blue
                GraphicsDevice.Clear(Color.CornflowerBlue);

                //Begin the spriteBatch
                spriteBatch.Begin();

                //Draw the leader board
                spriteBatch.Draw(leaderBoardsImg, leaderBoardsImgBounds, Color.White);
                

                //Initialize strings s and title:
                string s;
                string title;

                //Initialize the vector2 location
                Vector2 loc;

                //Set location as the leader boards location's X value and the leader boards location Y value minus 50
                loc = new Vector2(leaderBoardsLoc.X, leaderBoardsLoc.Y - 50);

                //Set the title to "Leader Boards:"
                title = "LEADER BOARD:";

                //Draw the Title
                spriteBatch.DrawString(titleFont, title, loc, Color.Black);

                //Increase the Y value of location by 25
                loc.Y += 25;

                //For each index in the highscores list
                for (int i = 0; i < highScores.Count; i++)
                {
                    //Set the string as the place of the highscore followed by the name and value
                    s = (i + 1) + ". " + highScores[i].playerName + " — " + highScores[i].playerValue;

                    //Increase location's Y value by 50
                    loc.Y = loc.Y + 50;

                    //Draw the string onto the page
                    spriteBatch.DrawString(leaderBoardsFont, s, loc, Color.White);
                }

                //End the spriteBatch
                spriteBatch.End();
            }

            //If the credits page is open:
            else if (isCreditsActive && !isLeaderBoardsActive)
            {
                //Clear the screen and set the color to Cornflower Blue
                GraphicsDevice.Clear(Color.CornflowerBlue);

                //Begin the spriteBatch
                spriteBatch.Begin();

                //Set the boundaries of the credits page image to the screen size
                creditsImgBounds.Width = screen.Width;
                creditsImgBounds.Height = screen.Height;

                //Draw the background image
                spriteBatch.Draw(creditsImg, creditsImgBounds, Color.White);

                //Initialize the credits strings:
                string credits;    //The title
                string name;       //The name of the game
                string s;          //All the information that goes on the page

                //Set the values of the strings
                credits = "CREDITS:";
                name = "\nYoshi Moshi";

                s = "             Producer: Ori Talmor \n      Project Manager: Ori Talmor \n        Programmers: Ori Talmor \n     Game Designers: Ori Talmor \n                 A Big Thanks To:   \n\n                        Ori Almog\n    For Helping Me Solve A Few Bugs";

                //Initialize and set location as the credits page location's X value and the credits page location Y value
                Vector2 loc = new Vector2(creditsLoc.X, creditsLoc.Y);

                //The location of the background for the text
                Vector2 backScreen = new Vector2(creditsLoc.X - 80, (int)creditsLoc.Y);
                //Call the BackgroundText function to draw the background for the text (for readability)
                BackgroundText(spriteBatch, backScreen, 292, 250);

                //Draw the title for the credits page
                spriteBatch.DrawString(titleFont, credits, loc, Color.Black);

                //Increase the location's X value as itself minus 20
                loc.X = loc.X - 20;

                //Draw the name of the game on the credits page
                spriteBatch.DrawString(titleFont, name, loc, Color.Black);

                //Decrease the X value of the location by itself minus 80
                loc.X = loc.X - 80;
                //Increase the Y value of the location by itself plus 70
                loc.Y = loc.Y + 70;

                //Draw the information on the credits page
                spriteBatch.DrawString(creditsFont, s, loc, Color.Black);

                //End the spriteBatch
                spriteBatch.End();
            }

            base.Draw(gameTime);
        }

        /// <summary>
        /// Pre: name is a valid string and a valid external location of a text file in the debug folder 
        /// Post: Creates the location of each tile on the map, and sets the tile's properties, and resets the boundaries of the
        /// camera
        /// Description: The subprogram reads in a text file containing characters, each character is a tile in the map.
        /// For every character in the text file, the subprogram places it in the map accordingly and given certain properties
        /// for different tiles. It also redefins the boundaries of the camera and sets the camera to look at the player
        /// </summary>
        /// <param name="name">The name of the map to be drawn</param>
        private void ReadMap(string name)
        {
            //Clear the level of all Tiles
            level.Clear();
            environmental.Clear();

            //Create a texture toDraw and set it to null
            Texture2D toDraw = null;

            //Create and Set tile properties to false
            bool isEnvironmental = false;
            bool moveThrough = false;
            bool movable = false;
            bool interactable = false;

            dir = 1;

            //Set is restarting to false
            isRestarting = false;
            //Set is flipped to false
            isFlipped = false;
            //Set isBlueActive to true
            isBlueActive = true;

            //Create a stream reader and read in the file with the given name
            StreamReader reader = File.OpenText(name);

            //Create a temporary boundary
            Rectangle tempBounds;

            //Create and set the X and Y values to 0
            int X = 0;
            int Y = 0;

            //Set the lists as new lists
            level = new List<Tile>();
            environmental = new List<Tile>();

            //Create and initialize the length as the first line in the file
            string length = reader.ReadLine();

            //Create a string array called read
            string[] read;
            //Set read to the entire text in the file
            read = File.ReadAllLines(name);

            //Set the background boundaries as the length of both the rows and coloums of the file
            bgBounds.Width = length.Length * TILE_SIZE;
            bgBounds.Height = read.Length * TILE_SIZE;

            //For each line in the file's length in rows:
            for (int i = 0; i < read.Length; i++)
            {
                //For each character in the line at I:
                foreach (char element in read[i])
                {
                    #region Elements

                    //If the character is a 'B':
                    if (element == 'B')
                    {
                        //Set the texture toDraw to the black texture
                        toDraw = black;
                    }

                    //If the character is a '.':
                    if (element == '.')
                    {
                        //Set the texture toDraw to null
                        toDraw = null;
                    }

                    //If the character is an 'A':
                    if (element == 'A')
                    {
                        //Set the texture toDraw to the apple texture
                        toDraw = apple;
                        //Set the move through property to true
                        moveThrough = true;
                        //Set the interactable property to true
                        interactable = true;
                        //Set the environmental property to true
                        isEnvironmental = true;
                    }

                    //If the character is a 'b':
                    if (element == 'b')
                    {
                        //Set the texture toDraw to the blue texture
                        toDraw = blue;
                    }

                    //If the character is an 'F'
                    if (element == 'F')
                    {
                        //Set the texture toDraw to the finish texture
                        toDraw = finish;
                    }

                    //If the character is a 'D':
                    if (element == 'D')
                    {
                        //Set the texture toDraw to the Green Down Arrow texture
                        toDraw = grnDown;
                    }

                    //If the character is a 'U':
                    if (element == 'U')
                    {
                        //Set the texture toDraw to the Green Up Arrow texture
                        toDraw = grnUp;
                    }

                    //If the character is an 'L':
                    if (element == 'L')
                    {
                        //Set the texture toDraw to the blue lever texture
                        toDraw = lvrBlue;
                        //Set the move through property to true
                        moveThrough = true;
                        //Set the environmental property to true
                        isEnvironmental = true;
                    }

                    //If the character is an 'O':
                    if (element == 'O')
                    {
                        //Set the texture toDraw to the orange texture
                        toDraw = orange;
                    }

                    //If the character is a '0' (zero):
                    if (element == '0')
                    {
                        //Set the texture toDraw to the orange lever texture
                        toDraw = lvrOrange;
                        //Set the move through property to true
                        moveThrough = true;
                        //Set the environmental property
                        isEnvironmental = true;
                    }

                    //If the character is an 'R':
                    if (element == 'R')
                    {
                        //Set the texture toDraw to the red texture
                        toDraw = red;
                    }

                    //If the character is an 'S':
                    if (element == 'S')
                    {
                        //Set the texture toDraw to the star texture
                        toDraw = star;
                        //Set the move through property to true
                        moveThrough = true;
                        //Set the interactable property to true
                        interactable = true;
                        //Set the environmental property to true
                        isEnvironmental = true;
                    }

                    //If the character is a 'W':
                    if (element == 'W')
                    {
                        //Set the texture toDraw to the wood texture
                        toDraw = wood;
                        //Set the movable property to true
                        movable = true;
                        //Set the environmental property to true
                        isEnvironmental = true;
                    }

                    //If the character is an 'X'
                    if (element == 'X')
                    {
                        //Set the texture toDraw to the life up texture
                        toDraw = lifeUp;
                        //Set the move through property to true
                        moveThrough = true;
                        //Set the interactable property to true
                        interactable = true;
                        //Set the environmental property to true
                        isEnvironmental = true;
                    }

                    #endregion

                    //Set the temporary bounds to the X value multiplied by the tile size, and the Y value multiplied by the tile size, with a width and height of the tile size
                    tempBounds = new Rectangle(X * TILE_SIZE + 1, Y * TILE_SIZE + 1, TILE_SIZE, TILE_SIZE);

                    //If the environmental property is not set to true
                    if (!isEnvironmental)
                    {
                        //Add the tile to the level list with the texture, the temporary boundary, and the abilities
                        level.Add(new Tile(toDraw, tempBounds, moveThrough, movable, interactable));
                    }

                    //Otherwise, if the environmental property is set to true
                    else
                    {
                        //Add the tile to the environmetal list with the texture, the temporary boundary, and the properties
                        environmental.Add(new Tile(toDraw, tempBounds, moveThrough, movable, interactable));
                        //Add an extra tile to the level list with the same boundary as the other tile and the same properties
                        level.Add(new Tile(null, tempBounds, moveThrough, movable, interactable));
                    }

                    //If the character was a 'P':
                    if (element == 'P')
                    {
                        //Set the player velocity to 0;
                        player.vel.X = 0;
                        player.vel.Y = 0;
                        //Set the player bounds to the temporary bounds
                        player.bounds.X = tempBounds.X;
                        player.bounds.Y = tempBounds.Y;
                    }

                    //Set all the properties back to false;
                    moveThrough = false;
                    movable = false;
                    interactable = false;
                    isEnvironmental = false;

                    //If X is greater than or equal to the length of the column - 1
                    if (X >= (length.Length - 1))
                    {
                        //Set X as -1
                        X = -1;

                        //If Y is less than the amount of columns in the text file
                        if (Y < read.Length)
                        {
                            //Increment Y
                            Y++;
                        }

                        //If Y is greater than the amount of columns in the text file 
                        else
                        {
                            //Set Y to 0
                            Y = 0;
                        }
                    }

                    //Increment X
                    X++;
                }
            }

            //If the map number is not 1
            if (mapNum != 1)
            {
                //If the game is not full screen:
                if (!graphics.IsFullScreen)
                {
                    //Reset the bounding rectangle around the whole World incase the map is of different size
                    camera.SetLimits(bgBounds);
                    //centre the camera on the player
                    camera.LookAt(player.bounds);
                }

                //Otherwise if the game is fullscreen:
                else
                {
                    //Set the height of the bounds of the map one tile higher so the player can see everything more clearly
                    Rectangle screenBounds = bgBounds;
                    screenBounds.Height += TILE_SIZE;

                    //Set the camera limits to the newly created screen bounds
                    camera.SetLimits(screenBounds);

                    //centre the camera on the player
                    camera.LookAt(player.bounds);
                }
            }
        }
        
        /// <summary>
        /// Pre: Yoshi, is a valid player in the game, and gameTime is the snapshot of timing values
        /// Post: Checks Collision with each static tile and makes sure the player stays within certain boundaries
        /// Description: This Function checks every tile in range of the player and makes sure that the player doesn't collide 
        /// with the top, bottom, left, and right sides of each tile. It also keeps the player in line with each tile in range
        /// and makes sure that the player cannot intersect with certain tiles. This subprogram also checks if the player is
        /// intersecting with the corners of the tiles, and it makes sure that the player can't intersect with certain tiles
        /// </summary>
        /// <param name="yoshi">The player</param>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        private void ColStatic(ref Player yoshi, GameTime gameTime)
        {
            //For each tile in the level:
            foreach (Tile t in level)
            {
                //If the player is intersecting with the tile:
                if (yoshi.bounds.Intersects(t.tileBounds))
                {
                    //For each index in the tiles range:
                    foreach (int index in t.rangeIndexs)
                    {
                        //If the tile at the index's image is not null:
                        if (level[index].tileImg != null)
                        {
                            //Right
                            //If the player intersects with the left side of every tile (the player's right):
                            if ((yoshi.bounds.X + yoshi.bounds.Width <= level[index].tileBounds.X) && yoshi.bounds.X + yoshi.bounds.Width + yoshi.vel.X >= level[index].tileBounds.X)
                            {
                                if (yoshi.bounds.Y + yoshi.bounds.Height >= level[index].tileBounds.Y && yoshi.bounds.Y <= level[index].tileBounds.Y + level[index].tileBounds.Height)
                                {
                                    //If the tile at the index's move through property is set to false:
                                    if (!level[index].moveThrough)
                                    {
                                        //Call the ColEnvironment function to test for collision detection for the environment tiles
                                        ColEnvironment(level[index].tileImg);

                                        //Set the player's X velocity to 0
                                        yoshi.vel.X = 0;
                                        //Re locate the player a bit away from the tile he collided with
                                        yoshi.bounds.X = level[index].tileBounds.X - yoshi.bounds.Width - 1;
                                    }
                                    //Call the ColInteractive procedure to test for collision detection for interactive tiles
                                    ColInteractive();

                                }
                            }

                            //Left
                            //If the player intersects with the right side of every tile (the player's left):
                            else if (yoshi.bounds.X >= level[index].tileBounds.X + level[index].tileBounds.Width && yoshi.bounds.X + yoshi.vel.X <= level[index].tileBounds.X + level[index].tileBounds.Width)
                            {
                                if (yoshi.bounds.Y + yoshi.bounds.Height > level[index].tileBounds.Y && yoshi.bounds.Y < level[index].tileBounds.Y + level[index].tileBounds.Height)
                                {
                                    //If the tile at the index's move through property is set to false:
                                    if (!level[index].moveThrough)
                                    {
                                        //Call the ColEnvironment function to test for collision detection for the environment tiles
                                        ColEnvironment(level[index].tileImg);

                                        //Set the player's X velocity to 0
                                        yoshi.vel.X = 0;
                                        //Re locate the player a bit away from the tile he collided with
                                        yoshi.bounds.X = level[index].tileBounds.X + level[index].tileBounds.Width + 1;
                                    }

                                    //Call the ColInteractive procedure to test for collision detection for interactive tiles
                                    ColInteractive();
                                }
                            }

                            //Bottom
                            //If the player intersects with the top side of every tile (the player's bottom):
                            else if (yoshi.bounds.Y + yoshi.bounds.Height <= level[index].tileBounds.Y && yoshi.bounds.Y + yoshi.bounds.Height + yoshi.vel.Y * 1.5f >= level[index].tileBounds.Y)
                            {
                                if (yoshi.bounds.X <= level[index].tileBounds.X + level[index].tileBounds.Width && yoshi.bounds.X + yoshi.bounds.Width >= level[index].tileBounds.X)
                                {
                                    //If the tile at the index's move through property is set to false:
                                    if (!level[index].moveThrough)
                                    {
                                        //Call the ColEnvironment function to test for collision detection for the environment tiles
                                        ColEnvironment(level[index].tileImg);

                                        //Set the player's Y velocity to 0
                                        yoshi.vel.Y = 0;
                                        //Re locate the player a bit away from the tile he collided with
                                        yoshi.bounds.Y = level[index].tileBounds.Y - yoshi.bounds.Height - 1;
                                    }

                                    //Call the ColInteractive procedure to test for collision detection for interactive tiles
                                    ColInteractive();

                                    //If the direction is greater than 0 and the tile at the index's move through property is set to false:
                                    if (dir > 0 && !level[index].moveThrough)
                                    {
                                        //If the player isn't jumping:
                                        if (yoshi.isJumping == false)
                                        {
                                            //Add time to the current elapsed time
                                            time += gameTime.ElapsedGameTime.Milliseconds;

                                            //If 100 milliseconds have passed:
                                            if (time >= HUNDRED_MILLI)
                                            {
                                                //Set the player's jumping ability to true
                                                yoshi.isJumping = true;
                                                //Reset time to 0
                                                time = 0;
                                            }
                                        }

                                        //If the player is able to jump:
                                        else
                                        {
                                            //Set the player able to jump
                                            yoshi.isJumping = true;
                                        }
                                    }
                                }
                            }

                            //Top
                            //If the player intersects with the bottom side of every tile (the player's top):
                            else if (yoshi.bounds.Y >= level[index].tileBounds.Y + level[index].tileBounds.Height && yoshi.bounds.Y + yoshi.vel.Y * 1.5f <= level[index].tileBounds.Y + level[index].tileBounds.Height)
                            {
                                if (yoshi.bounds.X <= level[index].tileBounds.X + level[index].tileBounds.Width && yoshi.bounds.X + yoshi.bounds.Width >= level[index].tileBounds.X)
                                {
                                    //If the tile at the index's move through property is set to false:
                                    if (!level[index].moveThrough)
                                    {
                                        //Call the ColEnvironment function to test for collision detection for the environment tiles
                                        ColEnvironment(level[index].tileImg);

                                        //Set the player's Y velocity to 0
                                        yoshi.vel.Y = 0;
                                        //Re locate the player a bit away from the tile he collided with
                                        yoshi.bounds.Y = level[index].tileBounds.Y + level[index].tileBounds.Height + 1;
                                    }

                                    //Call the ColInteractive procedure to test for collision detection for interactive tiles
                                    ColInteractive();

                                    //If the direction is less than 0 and the tile at the index's move through property is set to false:
                                    if (dir < 0 && !level[index].moveThrough)
                                    {
                                        //If the player isn't jumping:
                                        if (yoshi.isJumping == false)
                                        {
                                            //Add time to the current elapsed time
                                            time += gameTime.ElapsedGameTime.Milliseconds;

                                            //If 100 milliseconds have passed:
                                            if (time >= HUNDRED_MILLI)
                                            {
                                                //Set the player's jumping ability to true
                                                yoshi.isJumping = true;
                                                //Reset time to 0
                                                time = 0;
                                            }
                                        }

                                        //If the player is able to jump:
                                        else
                                        {
                                            //Set the player able to jump
                                            yoshi.isJumping = true;
                                        }
                                    }
                                }
                            }

                            #region Corners

                            //If the tile at the index's move through ability is set to false:
                            if (!level[index].moveThrough)
                            {
                                //If the player intersects with the tile at the index:
                                if (yoshi.bounds.Intersects(level[index].tileBounds))
                                {
                                    //Entering left corner
                                    //If the player intersects with the left corner of every tile:
                                    if (yoshi.bounds.X <= level[index].tileBounds.X && yoshi.bounds.X + yoshi.bounds.Width >= level[index].tileBounds.X)
                                    {
                                        //Top
                                        //If the player intersects with the top side of the left corner of every tile:
                                        if (yoshi.bounds.Y <= level[index].tileBounds.Y && yoshi.bounds.Y + yoshi.bounds.Height >= level[index].tileBounds.Y)
                                        {
                                            //Decrement the player's X boundary
                                            yoshi.bounds.X--;
                                            //Set the player's X velocity to 0
                                            yoshi.vel.X = 0;
                                        }

                                        //Bottom
                                        //If the player intersects with the bottom side of the left corner of every tile:
                                        else if (yoshi.bounds.Y <= level[index].tileBounds.Y + level[index].tileBounds.Height && yoshi.bounds.Y + yoshi.bounds.Height >= level[index].tileBounds.Y)
                                        {
                                            //Decrement the player's X boundary
                                            yoshi.bounds.X--;
                                            //Set the player's X velocity to 0
                                            yoshi.vel.X = 0;
                                        }
                                    }

                                    //Entering right corner
                                    //If the player intersects with the right corner of every tile:
                                    else if (yoshi.bounds.X <= level[index].tileBounds.X + level[index].tileBounds.Width && yoshi.bounds.X + yoshi.bounds.Width >= level[index].tileBounds.X)
                                    {
                                        //Top
                                        //If the player intersects with the top side of the right corner of every tile:
                                        if (yoshi.bounds.Y <= level[index].tileBounds.Y && yoshi.bounds.Y + yoshi.bounds.Height >= level[index].tileBounds.Y)
                                        {
                                            //Increment the player's X boundary
                                            yoshi.bounds.X++;
                                            //Set the player's X velocity to 0
                                            yoshi.vel.X = 0;
                                        }

                                        //Bottom
                                        //If the player intersects with the bottom side of the right corner of every tile:
                                        else if (yoshi.bounds.Y <= level[index].tileBounds.Y + level[index].tileBounds.Height && yoshi.bounds.Y + yoshi.bounds.Height >= level[index].tileBounds.Y)
                                        {
                                            //Increment the player's X boundary
                                            yoshi.bounds.X++;
                                            //Set the player's X velocity to 0
                                            yoshi.vel.X = 0;
                                        }
                                    }
                                }
                            }

                            #endregion
                        }
                    }
                }

                //Call the ColMovable procedure to check collsion detection for movable tiles
                ColMovable();
            }
        }

        /// <summary>
        /// Pre: Image is a valid 2D texture in the game which the player is currently colliding with
        /// Post: Provides a reaction for certain tiles that the player collides with in the environment
        /// Description: Checks if the image of the tile that the player is currently colliding with matches that of certain
        /// environmental tiles 
        /// </summary>
        /// <param name="image">The image the player is currently colliding with</param>
        private void ColEnvironment(Texture2D image)
        {
            //If the image that the player collided with is the green up arrow texture:
            if (image == grnUp)
            {
                //If the direction is greater than 0:
                if (dir > 0)
                {
                    //If the game isn't muted:
                    if (!isMuted)
                    {
                        //Play the gravity swap sound effect
                        gravitySwap.Play();
                    }

                    //Set is flipped to true
                    isFlipped = true;

                    //Set the direction to -1
                    dir = -1;
                }
            }

            //If the texture that the player collided with is the green down arrow texture:
            else if (image == grnDown)
            {
                //If the direction is less than 0:
                if (dir < 0)
                {
                    //If the game isn't muted:
                    if (!isMuted)
                    {
                        //Play the gravity swap sound effect
                        gravitySwap.Play();
                    }

                    //Set is flipped to false
                    isFlipped = false;

                    //Set the direction to 1
                    dir = 1;
                }
            }

            //If the texture that the player collided with is the finish texture:
            else if (image == finish)
            {
                //If the map number is less than 4:
                if (mapNum < 4)
                {
                    //If the game isn't muted, the leader boards aren't open, and the credits page isn't open:
                    if (!isMuted && !isLeaderBoardsActive && !isCreditsActive)
                    {
                        //Play the level complete sound effect
                        levelComplete.Play();
                    }

                    //Increment the map number
                    mapNum++;
                    //Set the is level complete boolean to true
                    isLevelComplete = true;
                }

                //If the map number is 4 or greater:
                else
                {
                    //Set the is game beat boolean to true
                    isGameBeat = true;
                }
            }

            //If god mode isn't active:
            if (!isGodModeActive)
            {
                //If the texture that the player collided with is the red texture:
                if (image == red)
                {
                    //Set the is restarting boolean to true
                    isRestarting = true;
                }
            }
        }

        /// <summary>
        /// Pre: None
        /// Post: Checks if the player is currently colliding with any interactable tiles (such as an apple or a star)and gives
        /// the player an option to eat or take these interactable tiles, with advantages and consequences of course. In some 
        /// cases if the tile is a lever it lets the player interact with it, and unlock new possiblities
        /// Description: Checks every tile in the environmental list, and if the player intersects with the tile and the image is
        /// an interactable one, then it gives the player an option to take or eat it. If the player does than it removes the
        /// tile from the game, and rewards the player with certain advantages or consequences. Or if the tile is a lever it
        /// opens up new possibilities in the game
        /// </summary>
        private void ColInteractive()
        {
            //For every index in the environmental list:
            for (int i = 0; i < environmental.Count; i++)
            {
                //If the player is intersecting with the tile at the index of the environmental list:
                if (player.bounds.Intersects(environmental[i].tileBounds))
                {
                    //If the tile in environmental list's texture is an apple and its interact property is set to true:
                    if (environmental[i].tileImg == apple && environmental[i].interact == true)
                    {
                        //Set the eating option to true
                        hasEatingOption = true;

                        //If the player hits the E key or the X button:
                        if ((prevKb.IsKeyUp(Keys.E) && key.IsKeyDown(Keys.E)) || (prevGamePad.IsButtonUp(Buttons.X) && gamePad1.IsButtonDown(Buttons.X)))
                        {
                            //If the game isn't muted, the leader boards page isn't open, and neither is the credits page
                            if (!isMuted && !isLeaderBoardsActive && !isCreditsActive)
                            {
                                //Play the apple eating sound effect
                                appleEating.Play();
                            }

                            //Reset the time, player health, the ability to eat again option variable (hasEatingOption), and the 
                            //has eaten option, back to their default values. Also setting the interact ability for that tile
                            //to false and increasing the time passed by 5 seconds.
                            curHunger = 100;
                            timePassed = 0;
                            seconds = 0;
                            hasEatingOption = false;
                            hasEaten = true;
                            environmental[i].interact = false;
                            secondsPassed += 5;

                            //recalculate the hunger percentage
                            hungerPercentage = curHunger / MAX_HUNGER;

                            //Adds this tile in the to remove list
                            tilesToRemove.Add(environmental[i]);
                        }

                        //Break out of this loop
                        break;
                    }

                    //If the tile in environmental list's texture is a star and its interact property is set to true:
                    if (environmental[i].tileImg == star && environmental[i].interact == true)
                    {
                        //Set the taking option to true
                        hasTakingOption = true;

                        //If the player hits the E key or the X button:
                        if ((prevKb.IsKeyUp(Keys.E) && key.IsKeyDown(Keys.E)) || (prevGamePad.IsButtonUp(Buttons.X) && gamePad1.IsButtonDown(Buttons.X)))
                        {
                            //If the game isn't muted, the leader boards page isn't open, and neither is the credits page
                            if (!isMuted && !isLeaderBoardsActive && !isCreditsActive)
                            {
                                //Play the star taking sound effect
                                starTaking.Play();

                            }

                            //Reset the ability to take again option variable (hasTakingOption), and the 
                            //has taken option, back to their default values. Also setting the interact ability for that tile
                            //to false and decreasing the level complete time by 5 seconds.
                            hasTakingOption = false;
                            hasTaken = true;
                            stars++;
                            environmental[i].interact = false;
                            levelCompleteTime -= 5;

                            //Adds this tile in the to remove list
                            tilesToRemove.Add(environmental[i]);
                        }

                        //Break out of this loop
                        break;
                    }

                    //If the tile in environmental list's texture is a life up and its interact property is set to true:
                    if (environmental[i].tileImg == lifeUp && environmental[i].interact == true)
                    {
                        //Set the life up option to true
                        hasLifeUpOption = true;

                        //If the player hits the E key or the X button:
                        if ((prevKb.IsKeyUp(Keys.E) && key.IsKeyDown(Keys.E)) || (prevGamePad.IsButtonUp(Buttons.X) && gamePad1.IsButtonDown(Buttons.X)))
                        {
                            //If the game isn't muted, the leader boards page isn't open, and neither is the credits page
                            if (!isMuted && !isLeaderBoardsActive && !isCreditsActive)
                            {
                                //Play the star taking sound effect
                                lifeUpSound.Play();

                            }

                            //Reset the ability to take a life up again option variable (hasLifeUpOption) back to its default value,
                            //and set the has taken life option to true. Also setting the interact ability for that tile to false
                            //and increasing the amount of lives the player has.
                            hasLifeUpOption = false;
                            hasTakenLife = true;
                            lives++;
                            environmental[i].interact = false;

                            //Adds this tile in the to remove list
                            tilesToRemove.Add(environmental[i]);
                        }

                        //Break out of this loop
                        break;
                    }



                    //If the tile in environmental list's texture is a blue lever:
                    if (environmental[i].tileImg == lvrBlue)
                    {
                        //Set is blue active to true
                        isBlueActive = true;
                    }

                    //If the tile in environmental list's texture is an orange lever:
                    else if (environmental[i].tileImg == lvrOrange)
                    {
                        //Set is blue active to false;
                        isBlueActive = false;
                    }
                }

                //Otherwise:
                else
                {
                    //Set the has eating, taking, and life up options to false
                    hasEatingOption = false;
                    hasTakingOption = false;
                    hasLifeUpOption = false;
                }
            }
        }

        /// <summary>
        /// Pre: none
        /// Post: Checks to see if the player is colliding with movable tiles, if so, it makes sure the player is outside the
        /// boundaries of the tile and if the player can move the tile, it lets him do so
        /// Description: Checks every tile in the environmental list, and if the tile is movable, and the player intersects with
        /// the tile, than it moves the player to the outer side of the tile (so the player doesn't go inside the tile/intersect
        /// with the tile) then if there is room to move the tile, it lets the player move the tile based on the player's velocity
        /// </summary>
        private void ColMovable()
        {
            //For each tile in the environmental list:
            foreach (Tile t in environmental)
            {
                if (t.movable)
                {
                    //Right
                    //If the player intersects with the right side of any movable tile (the player's left):                    
                    if ((player.bounds.X + player.bounds.Width <= t.tileBounds.X) && player.bounds.X + player.bounds.Width + player.vel.X >= t.tileBounds.X)
                    {
                        if (player.bounds.Y + player.bounds.Height >= t.tileBounds.Y && player.bounds.Y <= t.tileBounds.Y + t.tileBounds.Height)
                        {
                            //Re locate the player a bit away from the tile he collided with
                            player.bounds.X = t.tileBounds.X - player.bounds.Width - 1;

                            #region Move Right

                            //Create a temporary rectangle
                            Rectangle temp;
                            //Set the temporary rectangle to the current colliding tile's bounds
                            temp = t.tileBounds;
                            //Set the X value of the temporary rectangle to the temporary X plus the player's X velocity
                            temp.X = (int)(temp.X + player.vel.X);

                            //For each tile in level:
                            foreach (Tile element in level)
                            {
                                //If the tile's image is null and the temporary rectangle intersects with the tile:
                                if (element.tileImg == null && temp.Intersects(element.tileBounds))
                                {
                                    //For each index in the tile's range:
                                    foreach (int index in element.rangeIndexs)
                                    {
                                        //If the tile at the index in level's image is not null and that the move through property of the tile is set to false:
                                        if (level[index].tileImg != null && !level[index].moveThrough)
                                        {
                                            //If the temporary rectangle doesn't intersect with the tile at the index in level:
                                            if (!temp.Intersects(level[index].tileBounds))
                                            {
                                                //Set the current colliding tile's X bounds to the temporary X bounds
                                                t.tileBounds.X = temp.X;
                                            }

                                            //Otherwise:
                                            else
                                            {
                                                //Set the current colliding tile's X bounds to the temporary X bounds minus the player's X velocity
                                                t.tileBounds.X = (int)(temp.X - player.vel.X);
                                                //Set the player's X velocity to 0
                                                player.vel.X = 0;

                                                //Break out of the loop
                                                break;
                                            }
                                        }

                                        //Otherwise, if the image is null:
                                        else
                                        {
                                            //Set the current colliding tile's X bounds to the temporary X bounds
                                            t.tileBounds.X = temp.X;
                                        }
                                    }
                                }
                            }

                            #endregion
                        }
                    }

                    //Left
                    //If the player intersects with the left side of any movable tile (the player's right):
                    else if (player.bounds.X >= t.tileBounds.X + t.tileBounds.Width && player.bounds.X + player.vel.X <= t.tileBounds.X + t.tileBounds.Width)
                    {
                        if (player.bounds.Y + player.bounds.Height >= t.tileBounds.Y && player.bounds.Y <= t.tileBounds.Y + t.tileBounds.Height)
                        {
                            //Re locate the player a bit away from the tile he collided with
                            player.bounds.X = t.tileBounds.X + t.tileBounds.Width + 1;

                            #region Move Left

                            //Create a temporary rectangle
                            Rectangle temp;
                            //Set the temporary rectangle to the current colliding tile's bounds
                            temp = t.tileBounds;
                            //Set the X value of the temporary rectangle to the temporary X plus the player's X velocity
                            temp.X = (int)(temp.X + player.vel.X);

                            //For each tile in level:
                            foreach (Tile element in level)
                            {
                                //If the tile's image is null and the temporary rectangle intersects with the tile:
                                if (element.tileImg == null && temp.Intersects(element.tileBounds))
                                {
                                    //For each index in the tile's range:
                                    foreach (int index in element.rangeIndexs)
                                    {
                                        //If the tile at the index in level's image is not null and that the move through property of the tile is set to false:
                                        if (level[index].tileImg != null && !level[index].moveThrough)
                                        {
                                            //If the temporary rectangle doesn't intersect with the tile at the index in level:
                                            if (!temp.Intersects(level[index].tileBounds))
                                            {
                                                //Set the current colliding tile's X bounds to the temporary X bounds
                                                t.tileBounds.X = temp.X;
                                            }

                                            //Otherwise:
                                            else
                                            {
                                                //Set the current colliding tile's X bounds to the temporary X bounds minus the player's X velocity plus 1
                                                t.tileBounds.X = (int)(temp.X - player.vel.X + 1);
                                                //Set the player's X velocity to 0
                                                player.vel.X = 0;

                                                //Break out of the loop
                                                break;
                                            }
                                        }

                                        //Otherwise, if the image is null:
                                        else
                                        {
                                            //Set the current colliding tile's X bounds to the temporary X bounds
                                            t.tileBounds.X = temp.X;
                                        }
                                    }
                                }
                            }

                            #endregion
                        }
                    }

                    //Top
                    //If the player intersects with the top side of any movable tile (the player's bottom):
                    else if (player.bounds.Y + player.bounds.Height <= t.tileBounds.Y && player.bounds.Y + player.bounds.Height + player.vel.Y * 1.5f >= t.tileBounds.Y)
                    {
                        if (player.bounds.X <= t.tileBounds.X + t.tileBounds.Width && player.bounds.X + player.bounds.Width >= t.tileBounds.X)
                        {
                            //Set the player's Y velocity to 0
                            player.vel.Y = 0;
                            //Re locate the player a bit away from the tile he collided with
                            player.bounds.Y = t.tileBounds.Y - player.bounds.Height - 1;

                            //If the direction is greater than 0:
                            if (dir > 0)
                            {
                                //Set the player able to jump
                                player.isJumping = true;
                            }
                        }
                    }

                    //Bottom
                    //If the player intersects with the bottom side of any movable tile (the player's top):
                    else if (player.bounds.Y >= t.tileBounds.Y + t.tileBounds.Height && player.bounds.Y + player.vel.Y * 1.5f <= t.tileBounds.Y + t.tileBounds.Height)
                    {
                        if (player.bounds.X <= t.tileBounds.X + t.tileBounds.Width && player.bounds.X + player.bounds.Width >= t.tileBounds.X)
                        {
                            //Set the player's Y velocity to 0
                            player.vel.Y = 0;
                            //Re locate the player a bit away from the tile he collided with
                            player.bounds.Y = t.tileBounds.Y + t.tileBounds.Height + 1;

                            //If the direction is less than 0:
                            if (dir < 0)
                            {
                                //Set the player able to jump
                                player.isJumping = true;
                            }
                        }
                    }

                    #region Corners

                    //If the player intersects with the tile at the index:
                    if (player.bounds.Intersects(t.tileBounds))
                    {
                        //Entering left corner
                        //If the player intersects with the left corner of any movable tile:
                        if (player.bounds.X <= t.tileBounds.X && player.bounds.X + player.bounds.Width >= t.tileBounds.X)
                        {
                            //Top
                            //If the player intersects with the top side of the left corner of any movable tile:
                            if (player.bounds.Y <= t.tileBounds.Y && player.bounds.Y + player.bounds.Height >= t.tileBounds.Y)
                            {
                                //Decrement the player's X boundary
                                player.bounds.X--;
                                //Set the player's X velocity to 0
                                player.vel.X = 0;
                            }

                            //Bottom
                            //If the player intersects with the bottom side of the left corner of any movable tile:
                            else if (player.bounds.Y <= t.tileBounds.Y + t.tileBounds.Height && player.bounds.Y + player.bounds.Height >= t.tileBounds.Y)
                            {
                                //Decrement the player's X boundary
                                player.bounds.X--;
                                //Set the player's X velocity to 0
                                player.vel.X = 0;
                            }
                        }

                        //Entering right corner
                        //If the player intersects with the right corner of every tile:
                        else if (player.bounds.X <= t.tileBounds.X + t.tileBounds.Width && player.bounds.X + player.bounds.Width >= t.tileBounds.X)
                        {
                            //Top
                            //If the player intersects with the top side of the right corner of any movable tile:
                            if (player.bounds.Y <= t.tileBounds.Y && player.bounds.Y + player.bounds.Height >= t.tileBounds.Y)
                            {
                                //Increment the player's X boundary
                                player.bounds.X++;
                                //Set the player's X velocity to 0
                                player.vel.X = 0;
                            }

                            //Bottom
                            //If the player intersects with the bottom side of the right corner of any movable tile:
                            else if (player.bounds.Y <= t.tileBounds.Y + t.tileBounds.Height && player.bounds.Y + player.bounds.Height >= t.tileBounds.Y)
                            {
                                //Increment the player's X boundary
                                player.bounds.X++;
                                //Set the player's X velocity to 0
                                player.vel.X = 0;
                            }
                        }
                    }

                    #endregion
                }
            }
        }

        /// <summary>
        /// Pre: a valid point as the player's location and a valid point as the tile's location
        /// Post: Plugs in the distance formula to check the distance between the two valid points
        /// Description: Returns the sum of:
        /// The X coordinate of one point subtracted by the other and then squared the result.
        /// And the:
        /// The Y coordinate of one point subtracted by the other and then squared the result.
        /// </summary>
        /// <param name="playerLoc">The center of the player</param>
        /// <param name="tileLoc">The center of the tile</param>
        /// <returns>The distance between two given points</returns>
        private double CheckRange(Point playerLoc, Point tileLoc)
        {
            //Return the range using the Distance Formula:
            //D = Sqrt((x2 - x1)^2 + (y2 - y1)^2).
            return Math.Sqrt(Math.Pow((tileLoc.X - playerLoc.X), 2) + Math.Pow((tileLoc.Y - playerLoc.Y), 2));
        }

        /// <summary>
        /// Pre: A valid spriteBatch used to draw textures
        /// Post: Draws out the HUD display and any needed output on the HUD
        /// Description: Draws out the amount of health, hunger, and lives the player has. Also, it draws out the time passed
        /// since the start of the game. Any cheats that are activated, and any other output that the player needs
        /// </summary>
        /// <param name="sb">The spriteBatch used to draw textures</param>
        private void DrawHud(SpriteBatch sb)
        {            
            //Draw Health (Top Left)
            Drawing.DrawFillRectangle(sb, blank, Color.Red, new Rectangle((int)healthBarLoc.X, (int)healthBarLoc.Y, (int)(HEALTH_BAR_SIZE * healthPercentage), hudObjectHeight), 0.5f);
            Drawing.DrawRectangle(sb, blank, 3.0f, Color.Black, new Rectangle((int)healthBarLoc.X, (int)healthBarLoc.Y, (int)(HEALTH_BAR_SIZE), hudObjectHeight));

            //Draw Hunger (Mid Right)
            Drawing.DrawFillRectangle(sb, blank, Color.Green, new Rectangle((int)hungerBarLoc.X, (int)hungerBarLoc.Y, (int)(HUNGER_BAR_SIZE * hungerPercentage), hudObjectHeight), 0.5f);
            Drawing.DrawRectangle(sb, blank, 3.0f, Color.Black, new Rectangle((int)hungerBarLoc.X, (int)hungerBarLoc.Y, (int)(HUNGER_BAR_SIZE), hudObjectHeight));

            //Draw Timer (Mid Screen)
            sb.DrawString(hudFont, secondsPassed.ToString(), timerLoc, Color.DarkGoldenrod);

            //If the map is restarting or the level is complete:
            if (isRestarting || isLevelComplete)
            {
                //If the game is not over:
                if (!isGameOver)
                {
                    //Draw output to player saying that the level is loading
                    spriteBatch.DrawString(interactionFont, "Level Loading... Please Wait.", levelLoadingPos, Color.White);
                }
            }

            if (lives <= 3)
            {
                //Draw Lives (Top Right)
                for (int i = 0; i < lives; i++)
                {
                    Drawing.DrawFillRectangle(sb, player.img, Color.White, new Rectangle((int)livesLoc.X + (i * 50), (int)livesLoc.Y, hudObjectHeight, hudObjectHeight), 1.0f);
                }
            }

            else
            {
                spriteBatch.DrawString(interactionFont, "Lives: " + lives, livesLoc, Color.White); 
            }

            //Draw Stars (Mid Left)
            for (int i = 0; i < stars; i++)
            {
                Drawing.DrawFillRectangle(sb, star, Color.White, new Rectangle((int)starsLoc.X + (i * 50), (int)starsLoc.Y, hudObjectHeight, hudObjectHeight), 1.0f);
            }

            //Draw Interaction Opportunities:

            //If the player has an Eating Option:
            if (hasEatingOption)
            {
                //Draw Eating Oportunnity
                sb.DrawString(interactionFont, "Press <E> to Eat the Apple", eatingLoc, Color.White);
            }

            //If the player has already eaten:
            if (hasEaten)
            {
                //Draw consequence for eating
                sb.DrawString(interactionFont, "+5 for Eating.", ateLoc, Color.Red);

                //If the notification time for the apple is equal to 5 seconds:
                if (notifTimeApple == FIVE_SECONDS)
                {
                    //Set hasEaten to false
                    hasEaten = false;
                    //Set notifTimeApple to 0
                    notifTimeApple = 0;
                }
            }

            //If the player has a taking option:
            if (hasTakingOption)
            {
                //Draw Taking Opportunity
                sb.DrawString(interactionFont, "Press <E> to Take the Star", starOutputLoc, Color.White);
            }

            //If the player has already taken:
            if (hasTaken)
            {
                //Draw the notifaction that the player took a star
                sb.DrawString(interactionFont, "You got a Star!", takenLoc, Color.Green);

                //If the notification time for the star is equal to 5 seconds:
                if (notifTimeStar == FIVE_SECONDS)
                {

                    //Set hasTaken to false
                    hasTaken = false;
                    //Set notifTimeStar to 0
                    notifTimeStar = 0;
                }
            }

            //If the player has a taking life up option:
            if (hasLifeUpOption)
            {
                //Draw Taking Opportunity
                sb.DrawString(interactionFont, "Press <E> to Gain an extra life!", lifeUpOutputLoc , Color.White);
            }

            //If the player has already taken a life:
            if (hasTakenLife)
            {
                //Draw the notifaction that the player took a life up
                sb.DrawString(interactionFont, "You got an extra life!", lifeLoc, Color.Green);

                //If the notification time for the life up tile is equal to 5 seconds:
                if (notifTimeLifeUp == FIVE_SECONDS)
                {

                    //Set hasTakenLife to false
                    hasTakenLife = false;
                    //Set notifTimeLife to 0
                    notifTimeLifeUp = 0;
                }
            }


            //Output to User:

            //If the music is in process for being muted, but isn't already muted:
            if (isMuting && !isMuted)
            {
                //Draw Output:
                sb.DrawString(interactionFont, "Muting the music, \n please wait...", muteLoc, Color.White);
            }
            
            //If cheat activated
            if (isGodModeActive)
            {
                //Draw Output:
                sb.DrawString(interactionFont, "GOD MODE ACTIVATED", godModeOutputLoc, Color.White);
            }

            //If the game is over:
            if (isGameOver)
            {
                //Call the GameOver procedure to reset certain variables, to calculate a score, and to check for a high score
                GameOver();

                //Create message to output:
                string message = String.Format("GAME OVER!!! \nYour score is: {0}", gameScore);
                //Draw out message/output
                sb.DrawString(gameOverFont, message, gameOverLoc, Color.Red);

                //If the player didn't beat a high score:
                if (!isPlayerBeatHighScore)
                {
                    //Call Play Audio method to check if any audio needs to be played
                    PlayAudio();
                }
            }

            //If the player beat the game:
            if (isGameBeat)
            {

                LevelComplete(mapNum);
                //Call the GameOver procedure to reset certain variables, to calculate a score, and to check for a high score
                GameOver();

                //Create message to output:
                string message = String.Format("GAME WON!!! \nYour score is: {0}", gameScore);
                //Draw out message/output
                sb.DrawString(gameOverFont, message, gameOverLoc, Color.Green);

                //If the player didn't beat a high score:
                if (!isPlayerBeatHighScore)
                {
                    //Call Play Audio method to check if any audio needs to be played
                    PlayAudio();
                }
            }

            //If the game is over or the game is beat:
            if (isGameOver || isGameBeat)
            {
                //Call the CheckGameScore procedure to check if the player beat a high score:
                CheckGameScore();

                //If the player isn't finished typing in his name:
                if (!isFinished)
                {
                    //If the player beat the high score:
                    if (isPlayerBeatHighScore)
                    {
                        //Call the PlayAudio method to check if any audio needs to be played
                        PlayAudio();
                        //Call the KeyBoard procedure to track the player's inserted text
                        KeyBoard();

                        //Draw output to user letting him know that he beat a high score and that he must enter his name
                        sb.DrawString(textBoxFont, "You Beat a High Score!!! \nPlease Enter in your Name:", textBoxInstruct, Color.Black);
                        //Draw the textbox where he will enter in his text
                        sb.Draw(textBoxColor, textBox, Color.White);

                        //If he has entered in text:
                        if (text != null)
                        {
                            //Draw the text that the user enters
                            sb.DrawString(textBoxFont, text, textBoxText, Color.White);
                        }
                    }
                }

                //Otherwise, if the player is finished typing in his name:
                else
                {
                    //If the program hasn't written out his name and score:
                    if (!isWritten)
                    {
                        //Call the subprogram that will write his name and score into the file based on the other scores
                        HighScoreWriting();
                    }
                }
            }
        }

        /// <summary>
        /// Pre: none
        /// Post: Restarts the game and reloads the map. Sets all needed variables back to their default values, and checks to see
        /// if the game is over
        /// Description: Removes one of the player's lives and resets the map, checks if the game is over, and plays any audio
        /// needed
        /// </summary>
        private void Restart()
        {
            //If the game is restarting:
            if (isRestarting)
            {
                //Decrement the amount of lives the player has
                lives--;

                //Set the health back to its original value
                curHealth = MAX_HEALTH;

                //recalculate the hunger percentage
                hungerPercentage = curHunger / MAX_HUNGER;
                //recalculate the health percentage
                healthPercentage = curHealth / MAX_HEALTH;

                //Set seconds to 0
                seconds = 0;

                //If the player has no lives left:
                if (lives == 0)
                {
                    //Set isGameOver to true
                    isGameOver = true;
                }

                //If the game is not over:
                if (!isGameOver)
                {
                    //Call the PlayAudio subprogram to check if any sounds need to be played
                    PlayAudio();

                    //Call the ReadMap function with the map location in order to load the next map
                    ReadMap("map" + mapNum + ".txt");

                    ////Call the CalcRange procedure to Calculate the Range for each tile
                    CalcRange();

                    //Set the direction and stars back to their original values
                    dir = 1;
                    stars = 0;
                }
            }
        }

        /// <summary>
        /// Pre: A valid number that represents the map number needed to be drawn
        /// Post: Draws out the next level
        /// Description: Adds to the player's level complete score and star score for every star the player takes, Redraws the map at
        /// the next level, and Calculates the new range.  
        /// </summary>
        /// <param name="num">The map number</param>
        private void LevelComplete(int num)
        {
            //If the player gets a certain level complete time, add a certain amount of points to their score:

            //If the player didn't beat the 4th level:
            if (!isGameBeat)
            {
                //If the player completes the level in less than 25 seconds:
                if (levelCompleteTime <= 25)
                {
                    //Add 500 points to the level complete score
                    levelCompleteScore += 500;
                }

                //If the player completes the level in less than 30 seconds:
                else if (levelCompleteTime <= 30)
                {
                    //Add 100 points to the level complete score
                    levelCompleteScore += 100;
                }

                //If the player completes the level in more than 31 seconds:
                else if (levelCompleteTime >= 31)
                {
                    //Add no points to the level complete score
                    levelCompleteScore += 0;
                }
            }

            //Otherwise, If the player just beat the fourth level:
            else
            {
                //If the player completes the level in less than 25 seconds:
                if (levelCompleteTime <= 40)
                {
                    //Add 500 points to the level complete score
                    levelCompleteScore += 500;
                }

                //If the player completes the level in less than 30 seconds:
                else if (levelCompleteTime <= 55)
                {
                    //Add 100 points to the level complete score
                    levelCompleteScore += 100;
                }

                //If the player completes the level in more than 31 seconds:
                else if (levelCompleteTime >= 56)
                {
                    //Add no points to the level complete score
                    levelCompleteScore += 0;
                }
            }

            //Add 300 points for every star the player gets to starCompleteScore
            starCompleteScore += stars * 300;

            //Set levelCompleteTime to 0
            levelCompleteTime = 0;


            if (!isGameBeat)
            {
                //Call the ReadMap subprogram to redraw the next map
                ReadMap("map" + num + ".txt");

                //Call the CalcRange procedure to Calculate the Range for each tile
                CalcRange();

                //Set stars to 0
                stars = 0;
            }

            //Set isLevelComplete to false
            isLevelComplete = false;
        }

        /// <summary>
        /// Pre: none
        /// Post: Adds indexs for each tile that represent the range for all nearby tiles to that specific tile
        /// Description: Foreach tile in the map, it checks the range between that tile, and all other tiles in the map. If the
        /// range is less than 70 pixels, than it adds the index for the tile in level into the list for the element in the map
        /// </summary>
        private void CalcRange()
        {
            if (!isRestarting)
            {
                //For each Tile in level:
                foreach (Tile element in level)
                {
                    //Clear the tile's list that contains the indexs
                    element.rangeIndexs.Clear();

                    //For every index in level:
                    for (int i = 0; i < level.Count; i++)
                    {
                        //If the tile's Center is not equal to the level at the index's center:
                        if (element.tileBounds.Center != level[i].tileBounds.Center)
                        {
                            //If The distance between the tile's center and the level at the index's center is less than 70 pixels:
                            //Call CheckRange Function to check the distance between two points
                            if (CheckRange(element.tileBounds.Center, level[i].tileBounds.Center) <= 70)
                            {
                                //Add the index to the tiles list of indexs
                                element.rangeIndexs.Add(i);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Pre: none
        /// Post: Plays any sound effects needed
        /// Description: Checks to see if certain requirments have been met, and that the game isn't muted, and plays different
        /// sound effects accordingly
        /// </summary>
        private void PlayAudio()
        {
            //If the game isn't muted, the leader boards not open, and the credits page not open:
            if (!isMuted && !isLeaderBoardsActive && !isCreditsActive)
            {
                //If The player beat the high score and hasn't been played once:
                if (isPlayerBeatHighScore && !isPlayedOnce)
                {
                    //Stop the background music/song
                    MediaPlayer.Stop();

                    //Set the MasterVolume for the sound effect to 1 (Max - 1)(Min - 0)
                    SoundEffect.MasterVolume = 1.0f;
                    //Play the high score sound effect
                    highScore.Play();
                    //Set isPlayedOnce to true
                    isPlayedOnce = true;
                }

                //If the game is over:
                if (isGameOver)
                {
                    //If isPlayedOnce is set to false:
                    if (!isPlayedOnce)
                    {
                        //Stop the background song
                        MediaPlayer.Stop();
                        //Play the game over sound effect
                        gameOver.Play();
                        //Set isPlayedOnce to true
                        isPlayedOnce = true;
                    }
                }

                //If the game is beat:
                if (isGameBeat)
                {
                    //If the sound effect hasn't been played once:
                    if (!isPlayedOnce)
                    {
                        //Stop the Background Music
                        MediaPlayer.Stop();

                        //Set the SoundEffect Master Volume to 0.3 (Max - 1) (Min - 0)
                        SoundEffect.MasterVolume = 0.3f;
                        //Play the game complete sound effect
                        gameComplete.Play();
                        //Set isPlayedOnce to true
                        isPlayedOnce = true;
                    }
                }

                //If the game isRestarting:
                if (isRestarting)
                {
                    //If the game isn't over:
                    if (!isGameOver)
                    {
                        //Pause the Background song
                        MediaPlayer.Pause();
                        //Set isPaused to true
                        isPaused = true;
                        //Play the death sound effect
                        death.Play();
                        //Set isRestarting to false
                        isRestarting = false;
                    }
                }
            }
        }

        /// <summary>
        /// Pre: none
        /// Post: End the game, displays the player's score, and calls subprograms to check if the player beat a high score
        /// Description: Sets all needed variables back to their original values, calculates the score, Calls the subprograms
        /// needed to check if the player has beat a high score
        /// </summary>
        private void GameOver()
        {
            //Set all interactive opportunities to false:
            hasEatingOption = false;
            hasTakingOption = false;

            //Set the player to the front position and turn of all other positions
            isPlayerFront = true;
            isPlayerLeft = false;
            isPlayerRight = false;

            //If the score hasn't been calculated yet:
            if (!isScoreCalculated)
            {
                //Set gameScore to the levelCompleteScore plus the star completeScore minus the secondsPassed times 10
                gameScore = (levelCompleteScore + starCompleteScore) - (secondsPassed * 10);

                //If the life cheat wasn't activated:
                if (!isLifeCheatActivated)
                {
                    //Create and initialize a variable liveFactor that removes points for every life lost
                    int liveFactor = 3 - lives;

                    //Set gameScore to the player's score (gameScore) minus the liveFactor multiplied by 200
                    gameScore = gameScore - (liveFactor * 200);
                }

                //Otherwise if the life cheat was activated:
                else
                {
                    int startingLifeFactor = 3;

                    //Remove 600 points (the starting value of the player's lives multiplied by 200 for each life) from the player's
                    //score
                    gameScore = gameScore - (startingLifeFactor * 200);
                }

                //Set the isScoreCalculated boolean to true/ set the score to calculated
                isScoreCalculated = true;
            }

            //If it hasn't been read/If isRead is equal to false:
            if (!isRead)
            {
                //Call the HighScoreReading method to read in the high scores from an exterior file
                HighScoreReading();
            }

            //Call the subprogram CheckGameScore to check to see if the player's score is higher than any of the top ten high scores
            CheckGameScore();
        }

        /// <summary>
        /// Pre: none
        /// Post: Adds all the values and names of the high scores into a list
        /// Description: Reads in the external high scores text file and adds all the names and their values into the list, and
        /// then sorts the list by descending order based on the values of the highscores
        /// </summary>
        private void HighScoreReading()
        {
            //Set the high score input to the high scores text file
            highScoreInput = File.OpenText("highscores.txt");

            //Create and initialize a high score list
            List<HighScore> list = new List<HighScore>();

            //Create and initialize arrays for the names and values
            string[] names = new string[10];
            int[] values = new int[10];

            //Create two indexs and set them to 0
            int index = 0;
            int index2 = 0;
            //Create and Set value to 0
            int value = 0;

            //create a string array called read
            string[] read;

            //Read in all the lines in the high scores text file
            read = File.ReadAllLines("highscores.txt");


            //For each line in the file:
            foreach (string line in read)
            {
                //Create an array called parts and set it to split the dash's in the current line in the file
                string[] parts = line.Split('-');

                //For every index in parts starting at 0:
                for (int i = 0; i + 1 < parts.Length; i++)
                {
                    //Set the names at index to the index in parts
                    names[index] = parts[i];

                    //Increment the names index
                    index++;
                }

                //For every index in parts starting at 1:
                for (int i = 1; i < parts.Length; i++)
                {
                    //Set the values at the second index to the index in parts
                    values[index2] = Convert.ToInt32(parts[i]);

                    //Increment the values index
                    index2++;
                }
            }

            //For each line in read:
            foreach (string s in read)
            {
                //Add to the high scores list the name and value
                list.Add(new HighScore(names[value], values[value]));

                //Increment value
                value++;
            }

            //Sort the list by score in a descending order
            highScores = list.OrderByDescending(o => o.playerValue).ToList<HighScore>();

            //Close the text file
            highScoreInput.Close();

            //Set is read to true
            isRead = true;
        }

        /// <summary>
        /// Pre: none
        /// Post: Checks to see if the player beat a high score
        /// Description: Checks to see if the player beat a high score, if so than it sets the player beat high score boolean to
        /// true
        /// </summary>
        private void CheckGameScore()
        {
            //If the high score count is greater than or equal to 10:
            if (highScores.Count >= 10)
            {
                //For every index in the high scores list:
                for (int i = 0; i < highScores.Count; i++)
                {
                    //If the player's score is greater than the current high score in the list:
                    if (gameScore > highScores[i].playerValue)
                    {
                        //Set is player beat high score to true
                        isPlayerBeatHighScore = true;

                        //Break out of the loop
                        break;
                    }
                }
            }

            //If there are no scores:
            else if (highScores.Count == 0)
            {
                //Set is player beat high score to true
                isPlayerBeatHighScore = true;
            }

            //If there are less than 10 high scores:
            else
            {
                //For every index in the high scores list:
                for (int i = 0; i < highScores.Count; i++)
                {
                    //If the player's score is greater than the current high score in the list:
                    if (gameScore > highScores[i].playerValue)
                    {
                        //Set is player beat high score to true
                        isPlayerBeatHighScore = true;

                        //Break out of the loop
                        break;

                    }
                }
            }

        }

        /// <summary>
        /// Pre: none
        /// Post: Adds the player's high score into the high score list, and then writes the entire high scores list onto the 
        /// external high scores file
        /// Description: Checks to see if the player has beat any of the high scores, and if he has and there are currently 10
        /// scores on the list, than it deletes the last score, and adds the player's score where needed. If there are less than
        /// 10 scores on the list, than it just adds the player's high score where needed. Then it writes the entire high scores
        /// list onto the external high scores file. Once it's done it closes the hgh score file
        /// </summary>
        private void HighScoreWriting()
        {
            //If there are more than or equal to 10 high scores:
            if (highScores.Count >= 10)
            {
                //For every index in the high score list:
                for (int i = 0; i < highScores.Count; i++)
                {
                    //If the player's score is greater than the high score at the current index:
                    if (gameScore > highScores[i].playerValue)
                    {
                        //Remove the last highscore (10)
                        highScores.RemoveAt(highScores.Count - 1);
                        //Insert the player's high score and name at the index that the player beat
                        highScores.Insert(i, new HighScore(text, gameScore));

                        //Break out of the loop
                        break;
                    }
                }
            }

            //If there are no high scores:
            else if (highScores.Count == 0)
            {
                //Insert the player's high score and name at the index that the player beat
                highScores.Insert(0, new HighScore(text, gameScore));
            }

            //Otherwise, If there are less than 10 high scores:
            else
            {
                //For every index in the high score list:
                for (int i = 0; i < highScores.Count; i++)
                {
                    //If the player's score is greater than the high score at the current index:
                    if (gameScore > highScores[i].playerValue)
                    {
                        //Insert the player's high score and name at the index that the player beat
                        highScores.Insert(i, new HighScore(text, gameScore));

                        //Break out of the loop
                        break;

                    }
                }
            }

            //Set the output to the high scores file
            highScoreOutput = File.CreateText("highscores.txt");

            //Create and initialize the output text array with 10 open indexs
            string[] outText = new string[10];
            //Set the index to 0
            int index = 0;

            //For every element in the high scores list:
            for (int i = 0; i < highScores.Count; i++)
            {
                    //Set the output text in the current index to the current high score elements name and value seperated by a dash
                    outText[index] = highScores[i].playerName + "-" + highScores[i].playerValue;

                    //Increment the index
                    index++;
            }

            //For every index in output text:
            for (int i = 0; i < outText.Length; i++)
            {
                //Write out to the file the output text at the current index
                highScoreOutput.WriteLine(outText[i]);
            }

            //Close the file
            highScoreOutput.Close();

            //Set is written to true
            isWritten = true;
        }

        /// <summary>
        /// Pre: none
        /// Post: Adds the player's keyboard strokes onto the text variable to be displayed when the player writes his name after
        /// he beats a high score
        /// Description: Checks to see that the player presses a new key, and doesn't hold the key down for an extended period of
        /// time. Then checks to see if the key is a space or a delete to which it does what it needs to do accordingly. It also
        /// checks if the key is an enter key, if so, it sets the finished boolean to true, to let the program know that the
        /// player has finished entering his name. Otherwise, if its any other key in the alphabet, it adds the key into the text
        /// variable
        /// </summary>
        private void KeyBoard()
        {
            //Create a current keyboard state
            KeyboardState curKey;

            //Get the current state of the keyboard and set it as the current keyboard
            curKey = Keyboard.GetState();

            //Create an array of pressed keys
            Keys[] pressedKeys;
            //Set pressed keys to all the currently pressed keys
            pressedKeys = curKey.GetPressedKeys();

            //Create and initialize an array of all the acceptable characters
            Keys[] acceptableKeys = new Keys[] {Keys.Q ,  Keys.W ,  Keys.E ,  Keys.R ,  Keys.T ,  Keys.Y ,
                                                Keys.U ,  Keys.I ,  Keys.O ,  Keys.P ,  Keys.A ,  Keys.S ,
                                                Keys.D ,  Keys.F ,  Keys.G ,  Keys.H ,  Keys.J ,  Keys.K ,
                                                Keys.L ,  Keys.Z ,  Keys.X ,  Keys.C ,  Keys.V ,  Keys.B ,
                                                Keys.N ,  Keys.M ,  Keys.D0,  Keys.D1,  Keys.D2,  Keys.D3, 
                                                Keys.D4,  Keys.D5,  Keys.D6,  Keys.D7,  Keys.D8,  Keys.D9,
                                                Keys.OemPeriod,  Keys.NumPad0, Keys.NumPad1, Keys.NumPad2, 
                                                Keys.NumPad3,    Keys.NumPad4, Keys.NumPad5, Keys.NumPad6, 
                                                Keys.NumPad7,    Keys.NumPad8, Keys.NumPad9};

            //For each key in the pressed keys array:
            foreach (Keys key in pressedKeys)
            {
                //If the previous key that was pressed is not the curent key:
                if (prevKb.IsKeyUp(key))
                {
                    //If the backspace or delete key is pressed:
                    if (key == Keys.Back || key == Keys.Delete)
                    {
                        //If the text is not null:
                        if (text != null)
                        {
                            //If the text length is greater than 0:
                            if (text.Length > 0)
                            {
                                //Set text, as text minus 1 (remove one character)
                                text = text.Remove(text.Length - 1, 1);
                            }
                        }
                    }

                    //If the enter key is pressed:
                    else if (key == Keys.Enter)
                    {
                        //If the text is not null:
                        if (text != null)
                        {
                            //Set is finished to true
                            isFinished = true;
                        }

                        //Otherwise, if the text is null:
                        else
                        {
                            //Set the text to an empty string
                            text = "";
                        }
                    }

                    //If the text is not null:
                    if (text != null)
                    {
                        //If the text's length is less than 18:
                        if (text.Length < 18)
                        {
                            //If the space key is pressed:
                            if (key == Keys.Space)
                            {
                                //Insert a space key
                                text = text.Insert(text.Length, " ");
                            }

                            //For every index in the alphabet:
                            for (int i = 0; i < acceptableKeys.Length; i++)
                            {
                                //If any key in the alphabet is pressed:
                                if (key == acceptableKeys[i])
                                {
                                    //If its a number:
                                    #region Numbers

                                    //If its the number 0:
                                    if (key == Keys.D0 || key == Keys.NumPad0)
                                    {
                                        //Add the key to the text variable
                                        text += "0";
                                    }

                                    //If its the number 1:
                                    else if (key == Keys.D1 || key == Keys.NumPad1)
                                    {
                                        //Add the key to the text variable
                                        text += "1";
                                    }

                                    //If its the number 2:
                                    else if (key == Keys.D2 || key == Keys.NumPad2)
                                    {
                                        //Add the key to the text variable
                                        text += "2";
                                    }

                                    //If its the number 3:
                                    else if (key == Keys.D3 || key == Keys.NumPad3)
                                    {
                                        //Add the key to the text variable
                                        text += "3";
                                    }

                                    //If its the number 4:
                                    else if (key == Keys.D4 || key == Keys.NumPad4)
                                    {
                                        //Add the key to the text variable
                                        text += "4";
                                    }

                                    //If its the number 5:
                                    else if (key == Keys.D5 || key == Keys.NumPad5)
                                    {
                                        //Add the key to the text variable
                                        text += "5";
                                    }

                                    //If its the number 6:
                                    else if (key == Keys.D6 || key == Keys.NumPad6)
                                    {
                                        //Add the key to the text variable
                                        text += "6";
                                    }

                                    //If its the number 7:
                                    else if (key == Keys.D7 || key == Keys.NumPad7)
                                    {
                                        //Add the key to the text variable
                                        text += "7";
                                    }

                                    //If its the number 8:
                                    else if (key == Keys.D8 || key == Keys.NumPad8)
                                    {
                                        //Add the key to the text variable
                                        text += "8";
                                    }

                                    //If its the number 9:
                                    else if (key == Keys.D9 || key == Keys.NumPad9)
                                    {
                                        //Add the key to the text variable
                                        text += "9";
                                    }

                                    #endregion

                                    //If its a period:
                                    else if (key == Keys.OemPeriod)
                                    {
                                        //Add the key to the text variable
                                        text += ".";
                                    }

                                    //If its an alphabetical letter:
                                    else
                                    {
                                        //Add the key to the text variable
                                        text += key.ToString();
                                    }
                                }
                            }
                        }
                    }

                    //Otherwise, if the text is null:
                    else
                    {
                        //If the space key is pressed:
                        if (key == Keys.Space)
                        {
                            //Insert a space key
                            text = text.Insert(text.Length, " ");
                        }

                        //For every index in the alphabet:
                        for (int i = 0; i < acceptableKeys.Length; i++)
                        {
                            //If any key in the alphabet is pressed:
                            if (key == acceptableKeys[i])
                            {
                                //If its a number:
                                #region Numbers

                                //If its the number 0:
                                if (key == Keys.D0 || key == Keys.NumPad0)
                                {
                                    //Add the key to the text variable
                                    text += "0";
                                }

                                //If its the number 1:
                                else if (key == Keys.D1 || key == Keys.NumPad1)
                                {
                                    //Add the key to the text variable
                                    text += "1";
                                }

                                //If its the number 2:
                                else if (key == Keys.D2 || key == Keys.NumPad2)
                                {
                                    //Add the key to the text variable
                                    text += "2";
                                }

                                //If its the number 3:
                                else if (key == Keys.D3 || key == Keys.NumPad3)
                                {
                                    //Add the key to the text variable
                                    text += "3";
                                }

                                //If its the number 4:
                                else if (key == Keys.D4 || key == Keys.NumPad4)
                                {
                                    //Add the key to the text variable
                                    text += "4";
                                }

                                //If its the number 5:
                                else if (key == Keys.D5 || key == Keys.NumPad5)
                                {
                                    //Add the key to the text variable
                                    text += "5";
                                }

                                //If its the number 6:
                                else if (key == Keys.D6 || key == Keys.NumPad6)
                                {
                                    //Add the key to the text variable
                                    text += "6";
                                }

                                //If its the number 7:
                                else if (key == Keys.D7 || key == Keys.NumPad7)
                                {
                                    //Add the key to the text variable
                                    text += "7";
                                }

                                //If its the number 8:
                                else if (key == Keys.D8 || key == Keys.NumPad8)
                                {
                                    //Add the key to the text variable
                                    text += "8";
                                }

                                //If its the number 9:
                                else if (key == Keys.D9 || key == Keys.NumPad9)
                                {
                                    //Add the key to the text variable
                                    text += "9";
                                }

                                #endregion

                                //If its a period:
                                else if (key == Keys.OemPeriod)
                                {
                                    //Add the key to the text variable
                                    text += ".";
                                }

                                //If its an alphabetical letter:
                                else
                                {
                                    //Add the key to the text variable
                                    text += key.ToString();
                                }
                            }
                        }
                    }
                }
            }

            //Set the previous key to the current key
            prevKb = curKey;
        }

        /// <summary>
        /// Pre: none
        /// Post: Animates the player
        /// Description: Calculates the next source rectangle based on which frame the player is currently on
        /// </summary>
        private void UpdateAnim()
        {
            //If the player is facing the left:
            if (isPlayerLeft)
            {
                //Set the player left source's X value to the remainder of the current frame needing to be drawn divided by the
                //amount of frames in the image multiplied by the left frame width
                playerLeftSrc.X = (playerSideSequences[playerFrameNum] % playerLeftFramesWide) * playerLeftFrameW;

                //Update the Frame number and make sure it resets if it passes the last frame
                playerFrameNum = (playerFrameNum + 1) % playerSidesNumFrames;

            }

            //If the player is facing the right:
            else if (isPlayerRight)
            {
                //Set the player right source's X value to the remainder of the current frame needing to be drawn divided by the
                //amount of frames in the image multiplied by the right frame width
                playerRightSrc.X = (playerSideSequences[playerFrameNum] % playerRightFramesWide) * playerRightFrameW;

                //Update the Frame number and make sure it resets if it passes the last frame
                playerFrameNum = (playerFrameNum + 1) % playerSidesNumFrames;

            }
        }

        /// <summary>
        /// Pre: A valid spriteBatch used to draw textures, a valid location for the background of the text, and a valid size
        /// (width and height) for the background of the text
        /// Post: Draws a background behind not clearly visible text
        /// Description: Draws a 75% opaque white background behind text that is not clearly visible using Primitive Drawing
        /// Resources
        /// </summary>
        /// <param name="sb">The spriteBatch used to draw textures</param>
        /// <param name="loc">The location of the background for the text</param>
        /// <param name="width">The width of the background for the text</param>
        /// <param name="height">The height of the background for the text</param>
        private void BackgroundText(SpriteBatch sb, Vector2 loc, int width, int height)
        {
            //Create and initialize a rectangle as given location's X and Y, and the given size's width and height 
            Rectangle bounds = new Rectangle((int)loc.X, (int)loc.Y, width, height);

            //Draw a blank white background at the given location at 75 percent opacity
            Drawing.DrawFillRectangle(sb, blank, Color.White, bounds, 0.75f);
        }
    }
}