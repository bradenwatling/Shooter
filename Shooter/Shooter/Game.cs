using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Shooter
{
    public class Game : Microsoft.Xna.Framework.Game
    {
        public const float DAMAGE_DISTANCE = 32;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        LineBatch lineBatch;

        Level level1;
        Level currentLevel;
        Player player;
        EnemySpawn enemySpawn;

        SpriteFont arialFont;

        int enemiesKilled;

        public Game()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            this.IsMouseVisible = true;
            this.Window.Title = "";

            this.graphics.PreferredBackBufferHeight = 768;
            this.graphics.PreferredBackBufferWidth = 1024;
            this.graphics.ApplyChanges();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(graphics.GraphicsDevice);
            lineBatch = new LineBatch(graphics.GraphicsDevice);

            arialFont = Content.Load<SpriteFont>("Arial");

            level1 = new Level(lineBatch, "Level1");

            currentLevel = level1;

            player = new Player(lineBatch, currentLevel, Player.PLAYER_START);
            enemySpawn = new EnemySpawn(lineBatch, currentLevel, player);
        }

        protected override void UnloadContent()
        {
        }

        void changeLevel(Level level)
        {
            currentLevel = level;
            player = new Player(lineBatch, currentLevel, Player.PLAYER_START);
            enemySpawn = new EnemySpawn(lineBatch, currentLevel, player);
        }

        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);
            KeyboardState keyboardState = Keyboard.GetState();
            if (gamePadState.Buttons.Back == ButtonState.Pressed || keyboardState.IsKeyDown(Keys.Escape))
                this.Exit();
            if (keyboardState.IsKeyDown(Keys.Enter))
                graphics.ToggleFullScreen();

            player.update(gameTime);
            enemySpawn.update(gameTime);

            foreach (Zombie zombie in enemySpawn.zombies)
                if (Math.Abs(Vector2.Distance(zombie.position + currentLevel.offset, player.position)) <= DAMAGE_DISTANCE) //Player is near a zombie
                    player.health -= Player.ZOMBIE_DAMAGE;

            foreach (Spider spider in enemySpawn.spiders)
                if (Math.Abs(Vector2.Distance(spider.position + currentLevel.offset, player.position)) <= DAMAGE_DISTANCE) //Player is near a spider
                    player.health -= spider.scale / 5;

            if (player.health <= 0) //End game
                Exit();

            List<Arrow> removeArrows = new List<Arrow>();
            List<Zombie> removeZombies = new List<Zombie>();
            List<Spider> removeSpiders = new List<Spider>();

            bool doZombie = true, doSpider = true;

            foreach (Arrow arrow in player.bow.arrows)
            {
                if (doZombie)
                {
                    foreach (Zombie zombie in enemySpawn.zombies)
                    {
                        if (Math.Abs(Vector2.Distance(zombie.position + currentLevel.offset + Stickman.head - Vector2.UnitY * Stickman.HEAD_RADIUS, arrow.position)) <= Stickman.HEAD_RADIUS) //Zombie was shot in the head
                        {
                            removeArrows.Add(arrow);
                            if (arrow.speed >= Arrow.MAX_SPEED)
                            {
                                ++enemiesKilled;
                                removeZombies.Add(zombie); //Kill the zombie
                                doZombie = false;
                                break;
                            }
                        }
                    }
                }

                if (doSpider)
                {
                    foreach (Spider spider in enemySpawn.spiders)
                    {
                        if (Math.Abs(Vector2.Distance(spider.position + currentLevel.offset + spider.body, arrow.position)) <= DAMAGE_DISTANCE) //Shot near spider
                        {
                            removeArrows.Add(arrow);
                            spider.scale -= arrow.speed / 10;
                            if (spider.scale < 1)
                            {
                                ++enemiesKilled;
                                removeSpiders.Add(spider); //Kill the spider
                                doSpider = false;
                                break;
                            }
                        }
                    }
                }
            }

            foreach (Arrow arrow in removeArrows)
                player.bow.arrows.Remove(arrow);
            foreach (Zombie zombie in removeZombies)
                enemySpawn.zombies.Remove(zombie);
            foreach (Spider spider in removeSpiders)
                enemySpawn.spiders.Remove(spider);
            
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();
            spriteBatch.DrawString(arialFont, enemiesKilled.ToString(), Vector2.Zero, Color.White);
            spriteBatch.End();

            lineBatch.setColor(Color.White);
            currentLevel.draw();
            player.draw();
            enemySpawn.draw();

            base.Draw(gameTime);
        }
    }
}
