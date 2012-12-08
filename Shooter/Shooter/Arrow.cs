using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Shooter
{
    class Arrow
    {
        public static Color ARROW_COLOR = Color.Yellow;
        public const int ARROW_LENGTH = 50, MAX_SPEED = 25, DEGREE_RANGE = 20; //The degrees before the angle is automatically set to 90

        LineBatch lineBatch;
        public Vector2 position;
        public float angle;
        public int speed;

        public Arrow(LineBatch lineBatch, Vector2 position, float angle)
        {
            this.lineBatch = lineBatch;
            this.position = position;
            this.angle = angle;
            this.speed = 0;
        }

        public void update(GameTime gameTime)
        {
            lineBatch.setMatrix(angle, position);
            position.X += speed * (float)Math.Cos(angle);
            position.Y += speed * (float)Math.Sin(angle);

            float degrees = MathHelper.ToDegrees(angle);

            if ((degrees > 90 - DEGREE_RANGE && degrees < 90 + DEGREE_RANGE) || (degrees < -270 + DEGREE_RANGE))
            {
                angle = MathHelper.ToRadians(90);
                speed += speed < MAX_SPEED ? 1 : 0;
            }
            else
                angle += degrees <= 90 && degrees >= -90 ? MathHelper.ToRadians(1) * 50 / speed : -MathHelper.ToRadians(1) * 50 / speed;
        }

        public void draw()
        {
            lineBatch.setMatrix(angle, position, new Vector2(ARROW_LENGTH, 0));
            lineBatch.setColor(ARROW_COLOR);

            lineBatch.drawLine(Vector2.Zero, Vector2.UnitX * ARROW_LENGTH);
            lineBatch.drawLine(Vector2.UnitX * ARROW_LENGTH, new Vector2(ARROW_LENGTH * 9 / 10, -3));
            lineBatch.drawLine(Vector2.UnitX * ARROW_LENGTH, new Vector2(ARROW_LENGTH * 9 / 10, 3));
        }
    }
}
