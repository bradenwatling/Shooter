using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Shooter
{
    class Bow
    {
        public static Color BOW_COLOR = Color.Orange;
        public const float BOW_WIDTH = 15, BOW_HEIGHT = 100;
        public const int SHOOT_DELAY = 500;

        public static Vector2 bowOrigin = new Vector2(BOW_WIDTH, BOW_HEIGHT / 2), bowRotationOrigin = new Vector2(-BOW_WIDTH * 2, BOW_HEIGHT / 2);

        LineBatch lineBatch;
        Curve curve;
        Timer timer;
        public List<Arrow> arrows;
        Arrow tempArrow;

        public Vector2 position;

        public float angle;

        int arrowPower;
        bool mousePressed;

        public Bow(LineBatch lineBatch, Vector2 position)
        {
            this.lineBatch = lineBatch;

            this.curve = new Curve();
            this.curve.Keys.Add(new CurveKey(0, 0));
            this.curve.Keys.Add(new CurveKey(BOW_HEIGHT / 2, BOW_WIDTH));
            this.curve.Keys.Add(new CurveKey(BOW_HEIGHT, 0));

            this.timer = new Timer(SHOOT_DELAY);

            this.arrows = new List<Arrow>();

            this.position = position;

            this.arrowPower = 0;
            this.mousePressed = false;
        }

        public void update(GameTime gameTime, Vector2 position)
        {
            this.position = position;

            MouseState mouseState = Mouse.GetState();
            angle = (float)Math.Atan2(mouseState.Y - position.Y, mouseState.X - (position.X + lineBatch.getView().Left));

            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                MatrixTransformer matrixTransformer = new MatrixTransformer(angle, position, bowRotationOrigin);
                //Calculate where the origin of the bow is using the matrix
                //Set the back of the arrow to the middle of the string of the bow and subtract the arrow power
                Vector2 arrowPosition = matrixTransformer.applyMatrix(bowOrigin + Vector2.UnitX * (Arrow.ARROW_LENGTH - BOW_WIDTH - arrowPower));

                if (!mousePressed)
                {
                    if (timer.update(gameTime)) //Nested so that it wont change tempArrow when we press the mouse but we havent delayed
                    {
                        tempArrow = new Arrow(lineBatch, arrowPosition, angle);
                        mousePressed = true;
                    }
                }
                else
                {
                    tempArrow.position = arrowPosition;
                    tempArrow.angle = angle;
                    arrowPower += tempArrow.speed < Arrow.MAX_SPEED ? 1 : 0;
                    tempArrow.speed = arrowPower;
                }
            }
            else if (mouseState.LeftButton == ButtonState.Released)
            {
                mousePressed = false;
                if (tempArrow != null)
                {
                    tempArrow.speed = tempArrow.speed == 0 ? 1 : tempArrow.speed;
                    arrows.Add(tempArrow);
                    tempArrow = null;
                    arrowPower = 0;
                }
            }

            List<Arrow> removeArrows = new List<Arrow>();

            foreach (Arrow arrow in arrows)
            {
                arrow.update(gameTime);
                if (arrow.position.Y >= lineBatch.getView().Bottom) //If it went below the screen
                    removeArrows.Add(arrow);
            }
            foreach (Arrow arrow in removeArrows)
                arrows.Remove(arrow);
        }

        public void draw()
        {
            lineBatch.setMatrix(angle, position, bowRotationOrigin);
            lineBatch.setColor(BOW_COLOR);

            lineBatch.drawCurve(curve);

            Vector2 middle = tempArrow != null ? Vector2.UnitY * BOW_HEIGHT / 2 - Vector2.UnitX * tempArrow.speed : Vector2.UnitY * BOW_HEIGHT / 2;
            lineBatch.drawLine(Vector2.Zero, middle);
            lineBatch.drawLine(middle, Vector2.UnitY * BOW_HEIGHT);

            if(tempArrow != null)
                tempArrow.draw();
            foreach (Arrow arrow in arrows)
                arrow.draw();
        }
    }
}
