using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Shooter
{
    class EnemySpawn
    {
        public const int SPIDER_MAX_DELAY = 5000; //Milliseconds
        LineBatch lineBatch;
        Level level;
        Player player;
        Random random;
        Timer timer;
        public List<Spider> spiders;
        public List<Zombie> zombies;

        public EnemySpawn(LineBatch lineBatch, Level level, Player player)
        {
            this.lineBatch = lineBatch;
            this.level = level;
            this.player = player;
            this.random = new Random();
            this.timer = new Timer(this.random.Next(SPIDER_MAX_DELAY));
            spiders = new List<Spider>();
            zombies = new List<Zombie>();
        }

        public void update(GameTime gameTime)
        {
            if (timer.update(gameTime))
            {
                timer.setPeriod(random.Next(SPIDER_MAX_DELAY));

                Color color = new Color(0.0f, (float)random.NextDouble() * 0.75f + 0.25f, (float)random.NextDouble() * 0.75f + 0.25f);
                float speed = 1.5f + (float)random.NextDouble() * (Player.PLAYER_SPEED - 2), scale = 1 + (float)random.NextDouble() * 5;
                bool rightSide = random.Next(2) != 1; //Generate a random boolean
                bool zombie = random.Next(2) != 1;

                if (zombie)
                    zombies.Add(new Zombie(lineBatch, level, player, color, speed, rightSide));
                else
                    spiders.Add(new Spider(lineBatch, level, player, color, speed, scale, rightSide));
            }

            foreach (Zombie zombie in zombies)
                zombie.update();
            foreach (Spider spider in spiders)
                spider.update();
        }

        public void draw()
        {
            foreach (Zombie zombie in zombies)
                zombie.draw();
            foreach (Spider spider in spiders)
                spider.draw();
        }
    }

    class Zombie : Stickman
    {
        LineBatch lineBatch;
        Player player;
        Color color;
        public float speed;
        int moveValue;

        bool faceRight;

        public Zombie(LineBatch lineBatch, Level level, Player player, Color color, float speed, bool rightSide)
        {
            this.lineBatch = lineBatch;
            this.level = level;
            this.player = player;
            this.color = color;
            this.speed = speed;
            this.position = Vector2.UnitY * lineBatch.getView().Top + Vector2.UnitX * (rightSide ? -level.offset.X + lineBatch.getView().Right : -level.offset.X);
            this.faceRight = !rightSide;

            this.rightHand = new Vector2(28, -56);

            this.moveValue = 0;
        }

        public void update()
        {
            if (Math.Abs(Vector2.Distance(position + level.offset, player.position)) > Game.DAMAGE_DISTANCE)
            {
                faceRight = position.X + level.offset.X < player.position.X;

                Line line = level.verticalCollision(position + level.offset);

                if (line != null && line.horizontal)
                    position.Y = line.vertexOne.Y - 1;

                if (faceRight && line != null && line.angle < 0)
                    position += Vector2.UnitY * (line.angle * Player.PLAYER_SPEED);
                else if (!faceRight && line != null && line.angle > 0)
                    position += Vector2.UnitY * (-line.angle * Player.PLAYER_SPEED);

                if (level.horizontalCollision(position + level.offset + Vector2.UnitX * leftHand) == null && level.horizontalCollision(position + level.offset + Vector2.UnitX * rightHand) == null)
                {
                    position += faceRight ? Vector2.UnitX * speed : Vector2.UnitX * -speed;
                    ++moveValue;
                }
                else if (line != null)
                    yVelocity = -MAX_Y_VELOCITY;
            }
            else
                moveValue = 0; //Stop the zombie from walking

            doGravity();
        }

        public void draw()
        {
            Vector2 tempPosition = position + level.offset;
            lineBatch.setMatrix(0, tempPosition);
            lineBatch.setColor(color);

            lineBatch.drawCircle((head - Vector2.UnitY * HEAD_RADIUS), HEAD_RADIUS);
            lineBatch.drawLine(head, waist);

            lineBatch.drawLine(shoulder, faceRight ? rightHand : new Vector2(-rightHand.X, rightHand.Y));
            lineBatch.drawLine(shoulder, faceRight ? leftHand : new Vector2(-leftHand.X, leftHand.Y));

            float angle = (float)Math.Sin(moveValue / 5) / 5;

            lineBatch.setMatrix(angle, tempPosition + waist, waist);
            lineBatch.drawLine(waist, rightKnee);
            lineBatch.drawLine(rightKnee, rightFoot);

            lineBatch.setMatrix(-angle, tempPosition + waist, waist);
            lineBatch.drawLine(waist, leftKnee);
            lineBatch.drawLine(leftKnee, leftFoot);
        }
    }

    class Spider
    {
        public Vector2 position = Vector2.Zero;
        public Vector2 body = new Vector2(0, -16);
        public Vector2 rightKnee = new Vector2(-2, -16);
        public Vector2 leftKnee = new Vector2(4, -16);
        public Vector2 rightFoot = new Vector2(-4, 0);
        public Vector2 leftFoot = new Vector2(4, 0);

        public const float APPROX_WIDTH = 16;

        LineBatch lineBatch;
        Level level;
        Player player;
        Color color;
        public float speed, scale;
        int moveValue;
        float yVelocity;

        public Spider(LineBatch lineBatch, Level level, Player player, Color color, float speed, float scale, bool rightSide)
        {
            this.lineBatch = lineBatch;
            this.level = level;
            this.player = player;
            this.color = color;
            this.speed = speed;
            this.scale = scale;
            this.position = Vector2.UnitY * lineBatch.getView().Top + Vector2.UnitX * (rightSide ? -level.offset.X +  lineBatch.getView().Right : -level.offset.X);

            this.moveValue = 0;
        }

        public void update()
        {
            bool moveRight = false;
            if (Math.Abs(Vector2.Distance(position + level.offset, player.position)) > Game.DAMAGE_DISTANCE)
                moveRight = position.X + level.offset.X < player.position.X;

            Line line = level.verticalCollision(position + level.offset);

            if (moveRight && line != null && line.angle < 0)
                position += Vector2.UnitY * (line.angle * speed);
            else if (!moveRight && line != null && line.angle > 0)
                position += Vector2.UnitY * (-line.angle * speed);

            if (level.horizontalCollision(position + level.offset) == null && level.horizontalCollision(position + level.offset) == null)
                position += moveRight ? Vector2.UnitX * speed : -Vector2.UnitX * speed;
            else if(line != null)
                yVelocity = -Stickman.MAX_Y_VELOCITY;
            ++moveValue;

            doGravity();
        }

        public void draw()
        {
            float legAngle = (float)Math.Sin(moveValue);

            Vector2 newBody = body * scale;

            MatrixTransformer matrixTransformer = new MatrixTransformer(legAngle, Vector2.Zero, -body);
            Vector2 newRightKnee = matrixTransformer.applyMatrix(rightKnee * scale);
            Vector2 newRightFoot = matrixTransformer.applyMatrix(rightFoot * scale);

            matrixTransformer.setMatrix(-legAngle, Vector2.Zero, -body);
            Vector2 newLeftKnee = matrixTransformer.applyMatrix(leftKnee * scale);
            Vector2 newLeftFoot = matrixTransformer.applyMatrix(leftFoot * scale);

            lineBatch.setMatrix(0, position + level.offset);
            lineBatch.setColor(color);

            lineBatch.drawLine(body, newRightKnee);
            lineBatch.drawLine(newRightKnee, newRightFoot);
            lineBatch.drawLine(body, newLeftKnee);
            lineBatch.drawLine(newLeftKnee, newLeftFoot);
        }

        public void doGravity()
        {
            position += Vector2.UnitY * yVelocity;

            Line line = level.verticalCollision(position + level.offset + Vector2.UnitY * yVelocity);
            Line rightLine = level.verticalCollision(position + level.offset + Vector2.UnitX * -APPROX_WIDTH + Vector2.UnitY * yVelocity);
            Line leftLine = level.verticalCollision(position + level.offset + Vector2.UnitX * APPROX_WIDTH + Vector2.UnitY * yVelocity);

            if (line == null && (rightLine == null || rightLine.middle) && (leftLine == null || leftLine.middle))
                yVelocity += yVelocity < Stickman.MAX_Y_VELOCITY ? Stickman.Y_VELOCITY_INTERVAL : 0;
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
}
