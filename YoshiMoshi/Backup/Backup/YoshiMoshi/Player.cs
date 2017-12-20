/*-----------------------------------Header---------------------------------------
  
  Project Name	     : YoshiMoshi
  File Name		     : Player.cs
  Author		     : Ori Talmor
  Modified Date      : Monday, January 20, 2013
  Due Date		     : Monday, January 20, 2013
  Program Description: This file contains the player class which maintains
                       everything the player might need, from his location, to his
                       velocity. This class also updates the player's gravity and
                       modifies the player's values if needed
 --------------------------------------------------------------------------------*/

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace YoshiMoshi
{
    class Player
    {
        //Global Variables:

        //Player properties:
        public Texture2D img;                                   //The image of the player
        public Rectangle bounds = new Rectangle(0, 0, 32, 32);  //The boundary of the player
        public Vector2 vel;                                     //The player's velocity
        public bool isJumping = false;                          //The player's jumping property; is the player able to jump?
        private const float HORIZ_SPEED = 0.90f;                //The horizontal speed of gravity
        private const float VERT_SPEED = 0.98f;                 //The vertical speed of gravity

        /// <summary>
        /// Pre: none
        /// Post: Update the player's X location
        /// Description: Add the player's X velocity to the player's X boundary
        /// </summary>
        public void UpdateX()
        {
            bounds.X += (int)vel.X;
        }

        /// <summary>
        /// Pre: none
        /// Post: Update the player's Y location
        /// Description: Add the player's Y velocity to the player's Y boundary
        /// </summary>
        public void UpdateY()
        {
            bounds.Y += (int)vel.Y;
        }

        /// <summary>
        /// Pre: a valid gravity direction in the game (Either 1 or -1)
        /// Post: Make sure that the player is affected by directional gravity
        /// Description: Add the direction to the the player's Y velocity 
        /// </summary>
        /// <param name="direction">The gravity direction in the game (Either 1 or -1)</param>
        public void Direction(int direction)
        {
            vel.Y += direction;
        }

        /// <summary>
        /// Pre: none
        /// Post: Make sure that the player cannot move faster than a certain speed (The horizontal speed)
        ///       Make sure that when the player jumps, he always ends up falling back (gravity)       
        /// Description: Multiply the Player's X velocity by the horizontal speed of gravity.
        ///              Multiply the Player's Y velocity by the vertical speed of gravity
        /// </summary>
        public void Gravity()
        {
            vel.X *= HORIZ_SPEED;
            vel.Y *= VERT_SPEED;
        }
    }
}
