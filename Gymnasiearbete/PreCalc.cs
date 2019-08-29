using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Gymnasiearbete
{
    /// <summary>
    /// PreCalc contains pre-calculated stuff for optimization purposes
    /// </summary>
    class PreCalc
    {
        /// <summary>
        /// Returns Vector2[] containing each corner in a 16 sided regular polygon with sidelength 1 in clockwise order
        /// </summary>
        public static Vector2[] Point16UnitCircle
        {
            get
            {
                return new Vector2[]
                {
                    new Vector2(  0f   , -1    ),
                    new Vector2(  0.38f, -0.92f),
                    new Vector2(  0.71f, -0.71f),
                    new Vector2(  0.92f, -0.38f),
                    new Vector2(  1    ,  0    ),
                    new Vector2(  0.92f,  0.38f),
                    new Vector2(  0.71f,  0.71f),
                    new Vector2(  0.38f,  0.92f),
                    new Vector2(  0    ,  1    ),
                    new Vector2( -0.38f,  0.92f),
                    new Vector2( -0.71f,  0.71f),
                    new Vector2( -0.92f,  0.38f),
                    new Vector2( -1    ,  0    ),
                    new Vector2( -0.92f, -0.38f),
                    new Vector2( -0.71f, -0.71f),
                    new Vector2( -0.38f, -0.92f)
                };
            }
        }

        /// <summary>
        /// Returns Vector2[] containing each corner in a 8 sided regular polygon with sidelength 1 in clockwise order
        /// </summary>
        public static Vector2[] Point8UnitCircle
        {
            get
            {
                return new Vector2[]
                {
                    new Vector2(  0    , -1    ),
                    new Vector2(  0.71f, -0.71f),
                    new Vector2(  1    ,  0    ),
                    new Vector2(  0.71f,  0.71f),
                    new Vector2(  0    ,  1    ),
                    new Vector2( -0.71f,  0.71f),
                    new Vector2( -1    ,  0    ),
                    new Vector2( -0.71f, -0.71f)
                };
            }
        }
    }
}
