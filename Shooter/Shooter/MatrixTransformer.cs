using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Shooter
{
    class MatrixTransformer
    {
        public Matrix transformMatrix;

        /// <summary>
        /// Create an empty MatrixTransformer
        /// </summary>
        public MatrixTransformer()
        {
        }

        /// <summary>
        /// Create a MatrixTransformer with initial values
        /// </summary>
        /// <param name="rotation"></param>
        /// <param name="translation"></param>
        /// <param name="origin"></param>
        public MatrixTransformer(float rotation, Vector2 translation, Vector2 origin = new Vector2())
        {
            setMatrix(rotation, translation, origin);
        }

        /// <summary>
        /// Sets the matrix used for transformations
        /// </summary>
        /// <param name="rotation"></param>
        /// <param name="translation"></param>
        /// <param name="origin">Defaults to (0, 0)</param>
        public void setMatrix(float rotation, Vector2 translation, Vector2 origin = new Vector2()) //Origin defaults to 0, 0
        {
            transformMatrix = Matrix.CreateTranslation(new Vector3(-origin, 0)) * Matrix.CreateRotationZ(rotation) * Matrix.CreateTranslation(new Vector3(translation, 0));
        }

        /// <summary>
        /// Applies the current matrix to a Vector2
        /// </summary>
        /// <param name="vector">The Vector2 to transform</param>
        /// <returns>The transformed Vector2</returns>
        public Vector2 applyMatrix(Vector2 vector)
        {
            return Vector2.Transform(vector, transformMatrix);
        }

        /// <summary>
        /// Applies the current matrix to a Vector3
        /// </summary>
        /// <param name="vector">The Vector3 to transform</param>
        /// <returns>The transformed Vector3</returns>
        public Vector3 applyMatrix(Vector3 vector)
        {
            return Vector3.Transform(vector, transformMatrix);
        }
    }
}
