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

namespace Globus
{
    /// <summary>
    /// Game comment.
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        private GraphicsDeviceManager graphics;
        private GraphicsDevice device;
        private BasicEffect effect;

        //WVP-matrisene:
        private Matrix world;
        private Matrix projection;
        private Matrix view;

        //Kameraposisjon:
        private Vector3 cameraPosition = new Vector3(13.0f, 2f, -26.0f);
        private Vector3 cameraTarget = Vector3.Zero;
        private Vector3 cameraUpVector = new Vector3(0.0f, 1.0f, 0.0f);

        SpriteBatch spriteBatch;
        SpriteFont spriteFont;

        // Kalkulasjonsfaktorer
        float c = (float)Math.PI / 180.0f; //Opererer med radianer. 
        float phir = 0.0f;
        float phir20 = 0.0f;
        float thetar = 0.0f;
        float x = 0.0f, y = 0.0f, z = 0.0f;
        int i = 0;

        //Finn antall vertekser: 
        VertexPositionColor[] verticesSphere = new VertexPositionColor[1406];
        VertexPositionColor[] xAxis = new VertexPositionColor[2];
        VertexPositionColor[] yAxis = new VertexPositionColor[2];
        VertexPositionColor[] zAxis = new VertexPositionColor[2];
        //VertexPositionColor[] verticesTop = new VertexPositionColor[78];
        //VertexPositionColor[] verticesBottom = new VertexPositionColor[78];

        // Skaleringsfaktorer
        Matrix earthScale;

        // Rotasjonsfaktorer
        float satRotY;
        float earthRotY;
        Matrix earthRotatY;
        float moonRotY;
        float orbRotY;

        // Translasjonsfaktorer
        Matrix earthTrans;

        // Matrisestack
        private Stack<Matrix> universe = new Stack<Matrix>(); 


        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);

            this.IsMouseVisible = true;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {

            base.Initialize();
            InitDevice();
            InitCamera();
            InitVertices();

            Content.RootDirectory = "Content";
            spriteBatch = new SpriteBatch(graphics.GraphicsDevice);
            spriteFont = Content.Load<SpriteFont>(@"Fonts\Arial");
        }

        /// <summary>
        /// Diverse initilaliseringer. 
        /// Henter ut device-objektet.
        /// </summary>
        private void InitDevice()
        {
            device = graphics.GraphicsDevice;

            //Setter størrelse på framebuffer:
            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 600;
            graphics.IsFullScreen = false;
            graphics.ApplyChanges();

            Window.Title = "Måne og sol, skyer og vind og blomster og barn";

            //Initialiserer Effect-objektet:
            effect = new BasicEffect(graphics.GraphicsDevice);

            effect.VertexColorEnabled = true;

        }

        /// <summary>
        /// Stiller inn kameraet.
        /// </summary>
        private void InitCamera()
        {
            //Projeksjon:
            float aspectRatio = (float)graphics.GraphicsDevice.Viewport.Width / (float)graphics.GraphicsDevice.Viewport.Height;

            //Oppretter view-matrisa:
            Matrix.CreateLookAt(ref cameraPosition, ref cameraTarget, ref cameraUpVector, out view);

            //Oppretter projeksjonsmatrisa:
            Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspectRatio, 0.01f, 1000.0f, out projection);

            //Gir matrisene til shader:
            effect.Projection = projection;
            effect.View = view;
        }

        protected void InitVertices()
        {
            //Varierer fi: 
            for (float phi = -90.0f; phi <= 90.0f; phi += 10)
            {
                phir = c * phi;   //phi radianer
                phir20 = c * (phi + 10);  //(phi+10) radianer

                //Varierer teta: 
                for (float theta = -180.0f; theta <= 180.0f; theta += 10)
                {
                    thetar = c * theta;
                    //Her skal x,y og z beregnes for pkt.1-3-5-7...:
                    x = (float)(Math.Sin(thetar) * Math.Cos(phir));
                    y = (float)(Math.Cos(thetar) * Math.Cos(phir));
                    z = (float)(Math.Sin(phir));
                    verticesSphere[i].Position = new Vector3(x, y, z);
                    verticesSphere[i].Color = Color.Red;
                    //if ((i == 2) || (i == 1404))
                    //{
                    //    //verticesSphere[i].Position = new Vector3(0, 0, 0);
                    //    verticesSphere[i].Color = Color.Blue;
                    //}
                    i++;

                    //Her skal x,y og z beregnes for pkt.2-4-6-8  
                    x = (float)(Math.Sin(thetar) * Math.Cos(phir20));
                    y = (float)(Math.Cos(thetar) * Math.Cos(phir20));
                    z = (float)(Math.Sin(phir20));
                    verticesSphere[i].Position = new Vector3(x, y, z);
                    verticesSphere[i].Color = Color.White;
                    i++;
                }
            }


            // Set axis lines
            xAxis[0] = new VertexPositionColor(new Vector3(-100.0f, 0f, 0f), Color.Yellow);
            xAxis[1] = new VertexPositionColor(new Vector3(100.0f, 0f, 0f), Color.Yellow);
            yAxis[0] = new VertexPositionColor(new Vector3(0f, -100.0f, 0f), Color.Green);
            yAxis[1] = new VertexPositionColor(new Vector3(0f, 100.0f, 0f), Color.Green);
            zAxis[0] = new VertexPositionColor(new Vector3(0f, 0f, -100.0f), Color.Pink);
            zAxis[1] = new VertexPositionColor(new Vector3(0f, 0f, 100.0f), Color.Pink);


            //// Get the top and bottom pieces
            //Array.Copy(verticesSphere, 0, verticesTop, 0, 76);
            //Array.Copy(verticesSphere, 1328, verticesBottom, 0, 76);

            //// Color the top and bottom pieces black
            //int j = 0;
            //while (j < verticesTop.Length)
            //{
            //    verticesTop[j].Color = Color.Black;
            //    verticesBottom[j].Color = Color.Black;
            //    j++;
            //}

            //i++;
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
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
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            base.Update(gameTime);
        }

        /// <summary>
        /// Draw overlaytext
        /// </summary>
        /// <param name="text">Text to be drawn</param>
        /// <param name="x">position x</param>
        /// <param name="y">position y</param>
        private void DrawOverlayText(string text, int x, int y)
        {
            spriteBatch.Begin();

            // Draw the string twice to create a drop shadow, first colored black
            // and offset one pixel to the bottom right, then again in white at the
            // intended position. This makes text easier to read over the background.
            spriteBatch.DrawString(spriteFont, text, new Vector2(x, y), Color.Black);
            spriteBatch.DrawString(spriteFont, text, new Vector2(x - 1, y - 1), Color.White);

            spriteBatch.End();
        }

        /// <summary>
        /// Draw the axis lines.
        /// </summary>
        private void DrawAxis()
        {
            //Starter tegning - må bruke effect-objektet:
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                device.DrawUserPrimitives(PrimitiveType.LineList, xAxis, 0, 1);
                device.DrawUserPrimitives(PrimitiveType.LineList, yAxis, 0, 1);
                device.DrawUserPrimitives(PrimitiveType.LineList, zAxis, 0, 1);
            }
        }

        /// <summary>
        /// Draw our beautiful planet Earth (Tellus)
        /// </summary>
        private void DrawEarth()
        {
            Matrix matIdentify = Matrix.Identity;

            // Oppgave 1b: Size of earth is 6367.5 kilometers
            Matrix.CreateScale(0.63675f, 0.63675f, 0.63675f, out earthScale);

            // Oppgave 1b: Alternative scaling
            //Matrix mt = new Matrix();
            //mt.M11 = 0.63675f; mt.M12 = 0.0f; mt.M13 = 0.0f; mt.M14 = 0.0f;
            //mt.M21 = 0.0f; mt.M22 = 0.63675f; mt.M23 = 0.0f; mt.M24 = 0.0f;
            //mt.M31 = 0.0f; mt.M32 = 0.0f; mt.M33 = 0.63675f; mt.M34 = 0.0f;
            //mt.M41 = 0.0f; mt.M42 = 0.0f; mt.M43 = 0.0f; mt.M44 = 1.0f;
            //earthScale = mt;

            // Rotasjon om egen akse
            earthRotatY = Matrix.CreateRotationY(earthRotY);
            earthRotY += (float)TargetElapsedTime.Milliseconds / -1000f;
            earthRotY = earthRotY % (float)(2 * Math.PI);

            // oppg 1c) Forflytning fra origo 
            earthTrans = Matrix.CreateTranslation(10f, 0f, -20f);

            world = matIdentify * earthScale * earthRotatY * earthTrans;

            universe.Push(matIdentify * earthScale * earthTrans);

            // Draw earth
            effect.World = world;
            this.DrawGlobus();
        }

        /// <summary>
        /// Draw the main satellite ouside Tellus.
        /// </summary>
        private void DrawMoon(int count)
        {
            Matrix earth = universe.Peek();

            Matrix matIdentify, matRotatY, matScale, orbRotatY, matTrans;
            matIdentify = Matrix.Identity;

            // Make the moon orbit the earth
            orbRotatY = Matrix.CreateRotationY(orbRotY);
            orbRotY += (float)TargetElapsedTime.Milliseconds / 800f;
            orbRotY = orbRotY % (float)(2 * Math.PI);

            // Rotasjon om egen akse
            matRotatY = Matrix.CreateRotationY(moonRotY);
            moonRotY += (float)TargetElapsedTime.Milliseconds / -200f;
            moonRotY = moonRotY % (float)(2 * Math.PI);

            Matrix.CreateScale(0.2728f, 0.2728f, 0.2728f, out matScale);

            for (int i = 0; i < count; i++)
            {
                // Distance moon to earth
                matTrans = Matrix.CreateTranslation(1.2964f*(i+1), 0.2964f * (i+1), 0.94964f);

                // Scale the moon, then rotate it around itself, then move it into orbit, then rotate the orbit, then move it all to the position of the earth
                world = matIdentify * matScale * matRotatY * matTrans * orbRotatY * earth;

                universe.Push(matIdentify * matScale * matRotatY * matTrans * orbRotatY * earth);

                effect.World = world;
                this.DrawGlobus();
            }
        }

        /// <summary>
        /// Draw a satellite around the moon
        /// </summary>
        private void DrawSatellite()
        {
            Matrix moon = universe.Peek();

            Matrix matIdentify, satRotatY, matScale, orbRotatY, matTrans;
            matIdentify = Matrix.Identity;

            // Make the moon orbit the earth
            orbRotatY = Matrix.CreateRotationY(orbRotY);
            orbRotY += (float)TargetElapsedTime.Milliseconds / -300f;
            orbRotY = orbRotY % (float)(2 * Math.PI);

            // Rotasjon om egen akse
            satRotatY = Matrix.CreateRotationY(satRotY);
            satRotY += (float)TargetElapsedTime.Milliseconds / 200f;
            satRotY = moonRotY % (float)(2 * Math.PI);

            Matrix.CreateScale(0.2f, 0.2f, 0.2f, out matScale);

            // Distance moon to earth
            matTrans = Matrix.CreateTranslation(1.2964f, 0.2964f, 0.94964f);

            // Scale the moon, then rotate it around itself, then move it into orbit, then rotate the orbit, then move it all to the position of the earth
            world = matIdentify * matScale * satRotatY * matTrans * orbRotatY * moon;

            effect.World = world;
            this.DrawGlobus();
        }

        /// <summary>
        /// Draw a globus.
        /// </summary>
        private void DrawGlobus()
        {
            //Starter tegning - må bruke effect-objektet:
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                // Angir primitivtype, aktuelle vertekser, en offsetverdi og antall 
                // primitiver (her 1 siden verteksene beskriver en trekant):
                device.DrawUserPrimitives(PrimitiveType.TriangleStrip, verticesSphere, 0, 1329, VertexPositionColor.VertexDeclaration);
                //device.DrawUserPrimitives(PrimitiveType.TriangleStrip, verticesTop, 0, 74, VertexPositionColor.VertexDeclaration);
                //device.DrawUserPrimitives(PrimitiveType.TriangleStrip, verticesBottom, 0, 74, VertexPositionColor.VertexDeclaration);
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            RasterizerState rasterizerState1 = new RasterizerState();
            rasterizerState1.CullMode = CullMode.None;
            rasterizerState1.FillMode = FillMode.WireFrame;
            device.RasterizerState = rasterizerState1;

            device.Clear(Color.DeepSkyBlue);

            //Setter world=I:
            world = Matrix.Identity;

            // Setter world-matrisa på effect-objektet (verteks-shaderen):
            effect.World = world;

            // Oppgave 1a)
            DrawAxis();

            // Oppgave 1b
            //DrawOverlayText(earthScale.ToString().Replace("}", "\n").Replace("{", ""), 0, 0);


            DrawEarth();
            DrawMoon(2);
            DrawSatellite();

            // Oppgave 1c) Se DrawEarth og definisjonen at earthTrans

            //DrawOverlayText(world.ToString().Replace("}", "\n").Replace("{", ""), 0, 0);

            base.Draw(gameTime);
        }
    }
}
