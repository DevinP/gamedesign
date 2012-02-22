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

namespace NyanTron
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        GraphicsDevice device;
        Effect effect;

        VertexBuffer vertexBuffer;

        Texture2D wallTexture;
        static int BOXHEIGHT = 2500;    //The height (y-axis measurement) from the bottom to the top of the box
        static int BOXWIDTH = 2500;     //The width (x-axis measurement) from side to side of the box
        static int BOXDEPTH = 2500;     //The depth (z-axis measurement) from from to the back of the box

        Matrix viewMatrix;
        Matrix projectionMatrix;


        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
            graphics.IsFullScreen = false;
            graphics.ApplyChanges();
            Window.Title = "Nyan Tron";

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            device = graphics.GraphicsDevice;
            // TODO: use this.Content to load your game content here
            effect = Content.Load<Effect>("effects");

            wallTexture = Content.Load<Texture2D>("wallTex");

            SetUpVertices();

        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            KeyboardState keyState = Keyboard.GetState();

            // Allows the game to exit
            if (keyState.IsKeyDown(Keys.Escape))
                this.Exit();

            // TODO: Add your update logic here
            UpdateCamera();

            base.Update(gameTime);
        }

        private void UpdateCamera()
        {
            Vector3 campos = new Vector3(-50, 0, 0);
            Vector3 camup = new Vector3(0, 1, 0);

            viewMatrix = Matrix.CreateLookAt(campos, new Vector3(BOXWIDTH / 2, 0, 0), camup);
            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, device.Viewport.AspectRatio, 0.2f, 500.0f);
        }


        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            DrawWallbox();

            base.Draw(gameTime);
        }

        private void DrawWallbox()
        {
            effect.CurrentTechnique = effect.Techniques["Textured"];
            effect.Parameters["xWorld"].SetValue(Matrix.Identity);
            effect.Parameters["xView"].SetValue(viewMatrix);
            effect.Parameters["xProjection"].SetValue(projectionMatrix);
            effect.Parameters["xTexture"].SetValue(wallTexture);

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                device.SetVertexBuffer(vertexBuffer);
                device.DrawPrimitives(PrimitiveType.TriangleList, 0, vertexBuffer.VertexCount / 3);
            }
        }

        /* SetUpVertices
         * 
         * Sets up the vertices of the triangles used to define the wallbox
         * */
        private void SetUpVertices()
        {
            List<VertexPositionNormalTexture> verticesList = new List<VertexPositionNormalTexture>();

            verticesList.Add(new VertexPositionNormalTexture(new Vector3(BOXWIDTH/2, BOXHEIGHT/2, 0), new Vector3(-1, 0, 0), new Vector2(0, 0)));
            verticesList.Add(new VertexPositionNormalTexture(new Vector3(BOXWIDTH/2, BOXHEIGHT/2, -BOXDEPTH/2), new Vector3(-1, 0, 0), new Vector2(0,0)));
            verticesList.Add(new VertexPositionNormalTexture(new Vector3(BOXWIDTH / 2, -BOXHEIGHT / 2, -BOXDEPTH / 2), new Vector3(-1, 0, 0), new Vector2(0, 0)));

            vertexBuffer = new VertexBuffer(device, VertexPositionNormalTexture.VertexDeclaration, verticesList.Count, BufferUsage.WriteOnly);

            vertexBuffer.SetData<VertexPositionNormalTexture>(verticesList.ToArray());
        }
    }
}
