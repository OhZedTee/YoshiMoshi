/*--------------------------------------Header------------------------------------
  
  Project Name	     : YoshiMoshi
  File Name		     : HighScore.cs
  Author		     : Ori Talmor
  Modified Date      : Monday, January 20, 2013
  Due Date		     : Monday, January 20, 2013
  Program Description: This file contains the high score class which maintains an integer
                       value and a string value for all high scores
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
    class HighScore
    {
        //Global Varialbes:

        //Create the variables needed for the list of high scores
        public string playerName;  //The player's name
        public int playerValue;    //The player's score

        /// <summary>
        /// Pre: A valid name for the player and a valid for the player
        /// Post: Initializes a new instance of the high score class
        /// Description: Sets the player's name as the name and the player value as the value
        /// </summary>
        /// <param name="name">The name of the player</param>
        /// <param name="value">The value of the player's score</param>
        public HighScore(string name, int value)
        {
            playerName = name;
            playerValue = value;
        }
    }
}
