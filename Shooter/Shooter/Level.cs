using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Shooter
{
    class Level
    {
        LineBatch lineBatch;
        public List<Line> lines;
        public Vector2 size;
        public Vector2 offset;
        Color color;

        public Level(LineBatch lineBatch, string fileName)
        {
            this.lineBatch = lineBatch;

            Rectangle screen = this.lineBatch.getView();

            this.lines = new List<Line>();

            try
            {
                using (StreamReader streamReader = new StreamReader(fileName + ".txt"))
                {
                    string levelString = "";
                    int lastX = -1, lastY = -1;
                    while ((levelString = streamReader.ReadLine()) != null)
                    {
                        if (levelString.Trim() == ">>")
                        {
                            lastX = -1;
                            lastY = -1;
                            continue;
                        }
                        string[] orderedPairs = levelString.Split('~');

                        string[] values = orderedPairs[0].Trim().Split(',');

                        int x = Convert.ToInt32(values[0]), y = Convert.ToInt32(values[1]);

                        if (lastX != -1 && lastY != -1)
                            this.lines.Add(new Line(new Vector2(lastX, screen.Bottom - lastY), new Vector2(x, screen.Bottom - y)));
                        lastX = x;
                        lastY = y;
                    }
                }
            }
            catch { }

            foreach (Line line in lines)
            {
                size.X = line.vertexOne.X > size.X ? line.vertexOne.X : size.X;
                size.X = line.vertexTwo.X > size.X ? line.vertexTwo.X : size.X;

                size.Y = line.vertexOne.Y > size.Y ? line.vertexOne.Y : size.Y;
                size.Y = line.vertexTwo.Y > size.Y ? line.vertexTwo.Y : size.Y;
            }

            this.offset = Vector2.Zero;
            this.color = Color.Pink;
        }

        public void move(Vector2 value)
        {
            offset -= value;
            for (int i = 0; i < lines.Count; ++i)
            {
                lines[i].vertexOne -= value;
                lines[i].vertexTwo -= value;
            }
        }

        public void draw()
        {
            lineBatch.setMatrix(0, Vector2.Zero);
            lineBatch.setColor(color);
            foreach (Line line in lines)
                lineBatch.drawLine(line.vertexOne, line.vertexTwo);
        }

        public Line verticalCollision(Vector2 position)
        {
            Line ret = null;
            foreach (Line line in lines)
            {
                if (line.collision(position, false))
                {
                    ret = line;
                    break;
                }
            }

            return ret;
        }

        public Line horizontalCollision(Vector2 position)
        {
            Line ret = null;
            foreach (Line line in lines)
            {
                if (line.collision(position, true))
                {
                    ret = line;
                    break;
                }
            }

            return ret;
        }
    }

    class Line
    {
        public const float VERTICAL_COLLISION_SENSITIVITY = Stickman.MAX_Y_VELOCITY, HORIZONTAL_COLLISION_SENSITIVITY = Player.PLAYER_SPEED, ANGLED_COLLISION_SENSITIVITY = 1.0f;
        public Vector2 vertexOne, vertexTwo;
        public float distance, angle;
        public bool vertical, horizontal, middle;

        public Line(Vector2 vertexOne, Vector2 vertexTwo)
        {
            this.vertexOne = vertexOne;
            this.vertexTwo = vertexTwo;
            this.distance = Vector2.Distance(this.vertexOne, this.vertexTwo);
            this.angle = (float)Math.Atan2(vertexTwo.Y - vertexOne.Y, vertexTwo.X - vertexOne.X);

            float degreesAngle = MathHelper.ToDegrees(angle);
            this.vertical = vertexOne.X == vertexTwo.X;
            this.horizontal = vertexOne.Y == vertexTwo.Y;
            this.middle = !horizontal && degreesAngle <= 35 && degreesAngle >= -35;
        }

        public bool collision(Vector2 position, bool movingHorizontally)
        {
            float positionAngle = (float)Math.Atan2(position.Y - vertexOne.Y, position.X - vertexOne.X);
            float angleDifference = angle - positionAngle;
            float perpendicularDistance = angleDifference * Math.Abs(Vector2.Distance(vertexOne, position));

            bool betweenYVertices = !(position.Y < vertexOne.Y && position.Y <= vertexTwo.Y) && !(position.Y > vertexOne.Y && position.Y > vertexTwo.Y);
            bool betweenXVertices = !(position.X < vertexOne.X && position.X <= vertexTwo.X) && !(position.X > vertexOne.X && position.X > vertexTwo.X);

            bool ret = false;

            if (vertical && movingHorizontally)
                ret = betweenYVertices && Math.Abs(perpendicularDistance) <= HORIZONTAL_COLLISION_SENSITIVITY;
            else if (horizontal && !movingHorizontally)
                ret = betweenXVertices && Math.Abs(perpendicularDistance) <= VERTICAL_COLLISION_SENSITIVITY;
            else if (middle && !movingHorizontally)
                ret = betweenXVertices && perpendicularDistance <= ANGLED_COLLISION_SENSITIVITY;

            return ret;
        }
    }
}
