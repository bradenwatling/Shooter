using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Shooter
{
    class LineBatch : MatrixTransformer
    {
        GraphicsDevice graphics;
        Color color;
        BasicEffect basicEffect;

        /// <summary>
        /// Create a new LineBatch object
        /// </summary>
        /// <param name="graphics"></param>
        public LineBatch(GraphicsDevice graphics, BasicEffect basicEffect = null)
        {
            this.graphics = graphics;
            this.color = Color.Black;
            this.transformMatrix = Matrix.Identity;

            this.basicEffect = basicEffect;
            if(basicEffect == null)
                this.basicEffect = new BasicEffect(graphics);
            this.basicEffect.VertexColorEnabled = true;
            this.basicEffect.Projection = Matrix.CreateOrthographicOffCenter(0, graphics.Viewport.Width, graphics.Viewport.Height, 0, 0, 1);
        }

        /// <summary>
        /// Returns the size of the viewport
        /// </summary>
        /// <returns></returns>
        public Rectangle getView()
        {
            return graphics.Viewport.Bounds;
        }

        /// <summary>
        /// Sets the current Color to draw in
        /// </summary>
        /// <param name="color"></param>
        public void setColor(Color color)
        {
            this.color = color;
        }
        
        /// <summary>
        /// Draws a circle from a position and a radius
        /// </summary>
        /// <param name="position">Vector2 containing the position of the circle</param>
        /// <param name="radius">The radius of the circle</param>
        public void drawCircle(Vector2 position, float radius)
        {
            const int DEGREE_INCREMENT = 5;
            const int DEGREES_IN_CIRCLE = 360;

            List<VertexPositionColor> vertices = new List<VertexPositionColor>();

            for (int i = 0; i <= DEGREES_IN_CIRCLE; i += DEGREE_INCREMENT)
            {
                Vector3 pos = new Vector3(radius * (float)Math.Cos(MathHelper.ToRadians(i)), radius * (float)Math.Sin(MathHelper.ToRadians(i)), 0);
                vertices.Add(new VertexPositionColor(Vector3.Transform(pos, Matrix.CreateTranslation(new Vector3(position, 0)) * transformMatrix), color));
            }

            basicEffect.CurrentTechnique.Passes[0].Apply();
            graphics.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineStrip, vertices.ToArray(), 0, vertices.Count - 1, VertexPositionColor.VertexDeclaration);
        }

        /// <summary>
        /// Draws a line based on a Curve object
        /// </summary>
        /// <param name="curve">The Curve object (can only have 1 value for each Position value)</param>
        public void drawCurve(Curve curve)
        {
            Vector2 first = new Vector2(curve.Keys[0].Position, curve.Keys[0].Value);
            Vector2 second = new Vector2(curve.Keys[curve.Keys.Count - 1].Position, curve.Keys[curve.Keys.Count - 1].Value);

            int numVertices = (int)Vector2.Distance(first, second);

            List<VertexPositionColor> vertices = new List<VertexPositionColor>();

            for (int i = 0; i < numVertices; ++i)
            {
                Vector3 pos = new Vector3(curve.Evaluate(i), i, 0);
                vertices.Add(new VertexPositionColor(Vector3.Transform(pos, transformMatrix), color));
            }

            basicEffect.CurrentTechnique.Passes[0].Apply();
            graphics.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineStrip, vertices.ToArray(), 0, vertices.Count - 1, VertexPositionColor.VertexDeclaration);
        }

        /// <summary>
        /// Draws the outline of a Rectangle object
        /// </summary>
        /// <param name="rect"></param>
        public void drawRectangle(Rectangle rect)
        {
            List<VertexPositionColor> vertices = new List<VertexPositionColor>();

            Vector3 topLeft = new Vector3(rect.Left, rect.Top, 0);
            Vector3 topRight = new Vector3(rect.Right, rect.Top, 0);
            Vector3 bottomRight = new Vector3(rect.Right, rect.Bottom, 0);
            Vector3 bottomLeft = new Vector3(rect.Left, rect.Bottom, 0);

            vertices.Add(new VertexPositionColor(Vector3.Transform(topLeft, transformMatrix), color));
            vertices.Add(new VertexPositionColor(Vector3.Transform(topRight, transformMatrix), color));
            vertices.Add(new VertexPositionColor(Vector3.Transform(bottomRight, transformMatrix), color));
            vertices.Add(new VertexPositionColor(Vector3.Transform(bottomLeft, transformMatrix), color));
            vertices.Add(new VertexPositionColor(Vector3.Transform(topLeft, transformMatrix), color));

            basicEffect.CurrentTechnique.Passes[0].Apply();
            graphics.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineStrip, vertices.ToArray(), 0, vertices.Count - 1, VertexPositionColor.VertexDeclaration);
        }

        /// <summary>
        /// Draws a filled Rectangle object
        /// </summary>
        /// <param name="rect"></param>
        public void fillRectangle(Rectangle rect)
        {
            List<VertexPositionColor> vertices = new List<VertexPositionColor>();

            Vector3 topLeft = new Vector3(rect.Left, rect.Top, 0);
            Vector3 topRight = new Vector3(rect.Right, rect.Top, 0);
            Vector3 bottomRight = new Vector3(rect.Right, rect.Bottom, 0);
            Vector3 bottomLeft = new Vector3(rect.Left, rect.Bottom, 0);

            vertices.Add(new VertexPositionColor(Vector3.Transform(bottomLeft, transformMatrix), color));
            vertices.Add(new VertexPositionColor(Vector3.Transform(topLeft, transformMatrix), color));
            vertices.Add(new VertexPositionColor(Vector3.Transform(bottomRight, transformMatrix), color));
            vertices.Add(new VertexPositionColor(Vector3.Transform(topRight, transformMatrix), color));

            basicEffect.CurrentTechnique.Passes[0].Apply();
            graphics.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.TriangleStrip, vertices.ToArray(), 0, vertices.Count - 2, VertexPositionColor.VertexDeclaration);
        }

        /// <summary>
        /// Draws a line between two points
        /// </summary>
        /// <param name="one">The first point</param>
        /// <param name="two">The second point</param>
        public void drawLine(Vector2 one, Vector2 two)
        {
            List<VertexPositionColor> vertices = new List<VertexPositionColor>();

            vertices.Add(new VertexPositionColor(Vector3.Transform(new Vector3(one.X, one.Y, 0), transformMatrix), color));
            vertices.Add(new VertexPositionColor(Vector3.Transform(new Vector3(two.X, two.Y, 0), transformMatrix), color));

            basicEffect.CurrentTechnique.Passes[0].Apply();
            graphics.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineStrip, vertices.ToArray(), 0, vertices.Count - 1, VertexPositionColor.VertexDeclaration);
        }
    }
}
