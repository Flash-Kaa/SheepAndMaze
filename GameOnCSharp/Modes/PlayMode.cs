﻿using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Content;

namespace GameOnCSharp
{
    public class PlayMode : IGameMode
    {
        public static bool HaveStartedExecutingCommands { get; set; }
        public static float BlockSize { get; private set; }

        private bool _doFirstAfterPress;
        private SpriteBatch _spriteBatch;
        private List<Lazy<IGameObject>> _components;

        // Для TextBox
        private SpriteFont _font;
        private Texture2D _buttonSpriteForTextbox;

        private const double ShareSizeInWidth = 0.3;
        private const int Indent = 30;

        public PlayMode(SpriteBatch spriteBatch)
        {
            _doFirstAfterPress = true;
            _spriteBatch = spriteBatch;
            HaveStartedExecutingCommands = false;
            BlockSize = (int)(Game1.Graphics.PreferredBackBufferHeight / 20);
        }

        public void LoadContent(ContentManager content)
        {
            UpdateLocationAndSize();

            _font = content.Load<SpriteFont>(@"Fonts/VlaShu");
            _buttonSpriteForTextbox = content.Load<Texture2D>(@"Sprites/MyPixelButton");

            _components.AsParallel().ForAll(x => x.Value.LoadContent(content));

        }

        public void Update(GameTime gameTime)
        {

            _components.ForEach(x => x.Value.Update(gameTime));

            if (HaveStartedExecutingCommands && _doFirstAfterPress)
            {
                Commands.SetCommands((_components[0].Value as TextBox).Text);
                _doFirstAfterPress = false;
            }

            if (!HaveStartedExecutingCommands)
                _doFirstAfterPress = true;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Game1.Graphics.GraphicsDevice.Clear(Color.ForestGreen);
            _components.ForEach(x => x.Value.Draw(_spriteBatch));
        }

        public void UpdateLocationAndSize()
        {
            var textboxCollider = new Rectangle(
                location: new Point(
                    (int)(Game1.Graphics.PreferredBackBufferWidth * (1 - ShareSizeInWidth)),
                    (int)Indent),
                size: new Point(
                    (int)(Game1.Graphics.PreferredBackBufferWidth * ShareSizeInWidth - 2 * Indent),
                    (int)(Game1.Graphics.PreferredBackBufferHeight - Indent * 2)));

            var buttonForTextboxCollider = new Rectangle(
                location: new Point(
                    (int)textboxCollider.X,
                    (int)(Game1.Graphics.PreferredBackBufferHeight * 8 / 10)),
                size: new Point(
                    (int)textboxCollider.Width,
                    (int)(Game1.Graphics.PreferredBackBufferHeight * 1.5 / 10)));

            _components = new List<Lazy<IGameObject>>
            {
                new Lazy<IGameObject>(() => new TextBox(_font, textboxCollider)),
                new Lazy<IGameObject>(() => new Maze()),
                new Lazy<IGameObject>(() => new Button(_buttonSpriteForTextbox, 
                    buttonForTextboxCollider,
                    i => HaveStartedExecutingCommands = i))
            };
        }
    }
}