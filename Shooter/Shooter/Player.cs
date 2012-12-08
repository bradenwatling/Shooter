using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Shooter
{
    class Stickman
    {
        public static Vector2 PLAYER_START = new Vector2(100);
        public const int HEAD_RADIUS = 16;
        public const float MAX_Y_VELOCITY = 12, Y_VELOCITY_INTERVAL = 0.5f;

        public Level level;

        public Vector2 position = Vector2.Zero;

        public static Vector2 head = new Vector2(0, -64);
        public static Vector2 waist = new Vector2(0, -32);
        public static Vector2 neck = head - Vector2.UnitY * head.Y / 2;
        public static Vector2 shoulder = (head - waist) * 5 / 3;
        public Vector2 rightHand = new Vector2(-32, -64);
        public Vector2 leftHand = new Vector2(32, -64);

        public Vector2 rightKnee = new Vector2(-4, -8);
        public Vector2 leftKnee = new Vector2(4, -8);
        public Vector2 rightFoot = new Vector2(-4, 0);
        public Vector2 leftFoot = new Vector2(4, 0);

        public float yVelocity;

        public void doGravity()
        {
            position += Vector2.UnitY * yVelocity;
            
            Line line = level.verticalCollision(position + level.offset + Vector2.UnitY * yVelocity);
            Line rightLine = level.verticalCollision(position + level.offset + Vector2.UnitX * rightHand + Vector2.UnitY * yVelocity);
            Line leftLine = level.verticalCollision(position + level.offset + Vector2.UnitX * leftHand + Vector2.UnitY * yVelocity);

            if (line == null && (rightLine == null || rightLine.middle) && (leftLine == null || leftLine.middle))
                yVelocity += yVelocity < MAX_Y_VELOCITY ? Y_VELOCITY_INTERVAL : 0;
            else if ((rightLine != null && rightLine.horizontal) || (leftLine != null && leftLine.horizontal))
            {
                yVelocity = 0;

                if (line != null && line.horizontal)
                    position.Y = line.vertexOne.Y - 1;
                else if (rightLine != null && rightLine.horizontal)
                    position.Y = rightLine.vertexOne.Y - 1;
                else if (leftLine != null && leftLine.horizontal)
                    position.Y = leftLine.vertexOne.Y - 1;
            }
            else
                yVelocity = 0;
        }
    }

    class Player : Stickman
    {
        public static Color PLAYER_COLOR = Color.Red;
        public const int MAX_HEALTH = 300, HEALTH_BAR_WIDTH = 300, HEALTH_BAR_HEIGHT = 20, ZOMBIE_DAMAGE = 2, JUMP_DELAY = 1000;
        public const float PLAYER_SPEED = 5;

        LineBatch lineBatch;
        public Bow bow;
        int moveValue;

        public float health;

        Timer jumpDelay;

        public Player(LineBatch lineBatch, Level level, Vector2 position)
        {
            this.lineBatch = lineBatch;
            this.level = level;
            this.position = position;

            this.bow = new Bow(this.lineBatch, position + leftHand);

            this.yVelocity = 0;
            this.moveValue = 0;

            this.health = MAX_HEALTH;

            this.jumpDelay = new Timer(JUMP_DELAY);
        }

        public void walk(Vector2 direction)
        {
            position += direction * PLAYER_SPEED;

            if (yVelocity == 0)
                ++moveValue;
            else
                moveValue = 0;
        }

        public void update(GameTime gameTime)
        {
            Rectangle screen = lineBatch.getView();
            KeyboardState keyboardState = Keyboard.GetState();

            Vector2 moveLeft = position + Vector2.UnitX * rightHand, moveRight = position + Vector2.UnitX * leftHand;
            Vector2 direction = Vector2.Zero;

            Line line = level.verticalCollision(position);

            if (keyboardState.IsKeyDown(Keys.A) && level.horizontalCollision(moveLeft) == null && moveLeft.X > screen.Left)
            {
                if (line != null && line.angle > 0) //Angle is positive when the line is going down
                    direction = Vector2.UnitY * (-line.angle * PLAYER_SPEED / 2);

                if (level.offset.X < 0 && moveLeft.X <= lineBatch.getView().Right * 1 / 3)
                {
                    level.move(-Vector2.UnitX * PLAYER_SPEED);
                    walk(Vector2.Zero);
                }
                else
                    direction += -Vector2.UnitX;

                walk(direction);
            }
            else if (keyboardState.IsKeyDown(Keys.D) && level.horizontalCollision(moveRight) == null && moveRight.X < screen.Right)
            {
                if (line != null && line.angle < 0) //Angle is negative when the line is going up
                    direction = Vector2.UnitY * (line.angle * PLAYER_SPEED / 2);

                if (-level.offset.X + lineBatch.getView().Width < level.size.X && moveRight.X >= lineBatch.getView().Right * 2 / 3)
                {
                    level.move(Vector2.UnitX * PLAYER_SPEED);
                    walk(Vector2.Zero);
                }
                else
                    direction += Vector2.UnitX;
                walk(direction);
            }
            else// if (!(keyboardState.IsKeyDown(Keys.A) || keyboardState.IsKeyDown(Keys.D)))
                moveValue = 0;

            if (keyboardState.IsKeyDown(Keys.Space) && yVelocity == 0 && jumpDelay.update(gameTime))
                yVelocity = -MAX_Y_VELOCITY;

            position -= level.offset;
            doGravity();
            position += level.offset;

            bow.update(gameTime, position + shoulder);
        }

        public void draw()
        {
            Vector2 healthBar = new Vector2(lineBatch.getView().Center.X, 20) - Vector2.UnitX * HEALTH_BAR_WIDTH / 2;
            lineBatch.setMatrix(0, healthBar);

            float percentHealth = health / MAX_HEALTH;
            int healthWidth = (int)(percentHealth * HEALTH_BAR_WIDTH);
            
            lineBatch.setColor(Color.Green);
            lineBatch.fillRectangle(new Rectangle(0, 0, healthWidth, HEALTH_BAR_HEIGHT));

            lineBatch.setColor(Color.Red);
            lineBatch.fillRectangle(new Rectangle(healthWidth, 0, HEALTH_BAR_WIDTH - healthWidth, HEALTH_BAR_HEIGHT));

            lineBatch.setColor(Color.Orange);
            lineBatch.drawRectangle(new Rectangle(0, 0, HEALTH_BAR_WIDTH, HEALTH_BAR_HEIGHT));
            lineBatch.drawLine(Vector2.UnitX * healthWidth, new Vector2(healthWidth, HEALTH_BAR_HEIGHT));

            lineBatch.setMatrix(0, position);
            lineBatch.setColor(PLAYER_COLOR);

            lineBatch.drawCircle(head - Vector2.UnitY * HEAD_RADIUS, HEAD_RADIUS);
            lineBatch.drawLine(head, waist);

            float angle = (float)Math.Sin(moveValue / 5) / 5;

            lineBatch.setMatrix(angle, position + waist, waist);
            lineBatch.drawLine(waist, rightKnee);
            lineBatch.drawLine(rightKnee, rightFoot);

            lineBatch.setMatrix(-angle, position + waist, waist);
            lineBatch.drawLine(waist, leftKnee);
            lineBatch.drawLine(leftKnee, leftFoot);

            lineBatch.setMatrix(0, Vector2.Zero);
            MatrixTransformer matrixTransformer = new MatrixTransformer(bow.angle, bow.position, Bow.bowRotationOrigin);
            lineBatch.drawLine(position + shoulder, matrixTransformer.applyMatrix(Bow.bowOrigin));

            bow.draw();
        }
    }
}
