/*--------------------------------Header------------------------------------------
  
  Project Name	     : YoshiMoshi
  File Name		     : Tile.cs
  Author		     : Ori Talmor
  Modified Date      : Monday, January 20, 2013
  Due Date		     : Monday, January 20, 2013
  Program Description: This file is the program's backbone. It contains every tile
                       within the game discluding the player. It maintains all
                       their properties, their image, their location, and even
                       draws them onto the screen at a specific location.
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
    public class Tile
    {
        //Global Variables:

        //Tile properties:
        public Texture2D tileImg;                        //The image of the tile
        public Rectangle tileBounds;                     //The bounds of the tile
        public bool moveThrough;                         //The tile's move through property
        public bool movable;                             //The tile's movable property
        public bool interact;                            //The tile's interact property
        public float vel;                                //The velocity of the tile
        public List<int> rangeIndexs = new List<int>();  //A list of range indexs for the tile

        /// <summary>
        /// Pre: A valid:
        ///                 image for the tile,
        ///                 boundary for the tile, 
        ///                 properties for the tile including the move through, movable, and interactable properties
        /// Post: Initializes a new instance of the tile class
        /// Description: Sets the tile's image, boundary, and properties
        /// </summary>
        /// <param name="img">The tile's image</param>
        /// <param name="bounds">The tile's boundary</param>
        /// <param name="moveThrough">The tile's move through property</param>
        /// <param name="move">The tile's moveable property</param>
        /// <param name="interactable">The tile's interactable property</param>
        public Tile(Texture2D img, Rectangle bounds, bool moveThrough, bool move, bool interactable)
        {
            tileImg = img;
            tileBounds = bounds;
            this.moveThrough = moveThrough;
            movable = move;
            interact = interactable;
        }

        /// <summary>
        /// Pre: A valid spriteBatch used to draw textures
        /// Post: Draw the tile's onto the screen
        /// Description: If the image isn't null, it draws the tile using its image, boundary, and the color white
        /// </summary>
        /// <param name="sb">The spriteBatch used to draw textures</param>
        public void Draw(SpriteBatch sb)
        {
            if (tileImg != null)
            {
                sb.Draw(tileImg, tileBounds, Color.White);
            }
        }

    }
}
    
