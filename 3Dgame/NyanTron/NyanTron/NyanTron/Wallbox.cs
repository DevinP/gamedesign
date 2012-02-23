using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace NyanTron
{
    public class Wallbox
    {
        public static int BOXHEIGHT = 1000;    //The height (y-axis measurement) from the bottom to the top of the box
        public static int BOXWIDTH = 1000;     //The width (x-axis measurement) from side to side of the box
        public static int BOXDEPTH = 1000;     //The depth (z-axis measurement) from from to the back of the box

       // Quad backWall;
        //Quad rightWall;

        Quad[] walls;

        BasicEffect quadEffect;

        VertexDeclaration vertexDeclaration;

        GraphicsDevice graphicsDevice;

        private const int BACKWALL = 0;
        private const int RIGHTWALL = 1;
        private const int LEFTWALL = 2;
        private const int TOPWALL = 3;
        private const int BOTWALL = 4;
        private const int FRONTWALL = 5;

        public Wallbox(GraphicsDevice device, Matrix viewMat, Matrix projectionMat, Texture2D wallTexture)
        {
            graphicsDevice = device;

            walls = new Quad[6];

            walls[BACKWALL] = new Quad(new Vector3(0, 0, -BOXDEPTH / 2), Vector3.Backward, Vector3.Up, BOXWIDTH, BOXHEIGHT);
            walls[RIGHTWALL] = new Quad(new Vector3(BOXWIDTH / 2, 0, 0), -Vector3.UnitX, Vector3.Up, BOXWIDTH, BOXHEIGHT);
            walls[LEFTWALL] = new Quad(new Vector3(-BOXWIDTH / 2, 0, 0), Vector3.UnitX, Vector3.Up, BOXWIDTH, BOXHEIGHT);
            walls[TOPWALL] = new Quad(new Vector3(0, BOXHEIGHT / 2, 0), -Vector3.UnitY, Vector3.Backward, BOXWIDTH, BOXHEIGHT);
            walls[BOTWALL] = new Quad(new Vector3(0, -BOXHEIGHT / 2, 0), Vector3.UnitY, Vector3.Backward, BOXWIDTH, BOXHEIGHT);
            walls[FRONTWALL] = new Quad(new Vector3(0, 0, BOXDEPTH / 2), -Vector3.Backward, Vector3.Up, BOXWIDTH, BOXHEIGHT);


            quadEffect = new BasicEffect(device);
            quadEffect.EnableDefaultLighting();
            quadEffect.TextureEnabled = true;
            quadEffect.Texture = wallTexture;

            vertexDeclaration = new VertexDeclaration(new VertexElement[]
                 {
                    new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
                    new VertexElement(12, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0),
                    new VertexElement(24, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0)
                 }
            );
        }

        public void Draw(Matrix viewMatrix, Matrix projectionMatrix)
        {
            quadEffect.World = Matrix.Identity;
            quadEffect.View = viewMatrix;
            quadEffect.Projection = projectionMatrix;

            foreach (Quad quad in walls)
            {
                foreach (EffectPass pass in quadEffect.CurrentTechnique.Passes)
                {
                    pass.Apply();

                    graphicsDevice.DrawUserIndexedPrimitives
                        <VertexPositionNormalTexture>(PrimitiveType.TriangleList, quad.Vertices, 0, 4, quad.Indexes, 0, 2);
                    
                }
            }
        }
    }

    public class Quad
    {
        public VertexPositionNormalTexture[] Vertices;
        public short[] Indexes;
        Vector3 Origin; //The point in the very center of the quad
        Vector3 Normal;
        Vector3 Up;

        Vector3 Left, UpperLeft, UpperRight, LowerLeft, LowerRight;


        public Quad(Vector3 origin, Vector3 normal, Vector3 up,
        float width, float height)
        {
            Vertices = new VertexPositionNormalTexture[4];
            Indexes = new short[6];
            Origin = origin;
            Normal = normal;
            Up = up;

            // Calculate the quad corners
            Left = Vector3.Cross(normal, Up);
            Vector3 uppercenter = (Up * height / 2) + origin;
            UpperLeft = uppercenter + (Left * width / 2);
            UpperRight = uppercenter - (Left * width / 2);
            LowerLeft = UpperLeft - (Up * height);
            LowerRight = UpperRight - (Up * height);

            FillVertices();
        }

        private void FillVertices()
        {
            // Fill in texture coordinates to display full texture
            // on quad
            Vector2 textureUpperLeft = new Vector2(0.0f, 0.0f);
            Vector2 textureUpperRight = new Vector2(1.0f, 0.0f);
            Vector2 textureLowerLeft = new Vector2(0.0f, 1.0f);
            Vector2 textureLowerRight = new Vector2(1.0f, 1.0f);

            // Provide a normal for each vertex
            for (int i = 0; i < Vertices.Length; i++)
            {
                Vertices[i].Normal = Normal;
            }

            // Set the position and texture coordinate for each
            // vertex
            Vertices[0].Position = LowerLeft;
            Vertices[0].TextureCoordinate = textureLowerLeft;
            Vertices[1].Position = UpperLeft;
            Vertices[1].TextureCoordinate = textureUpperLeft;
            Vertices[2].Position = LowerRight;
            Vertices[2].TextureCoordinate = textureLowerRight;
            Vertices[3].Position = UpperRight;
            Vertices[3].TextureCoordinate = textureUpperRight;

            // Set the index buffer for each vertex, using
            // clockwise winding
            Indexes[0] = 0;
            Indexes[1] = 1;
            Indexes[2] = 2;
            Indexes[3] = 2;
            Indexes[4] = 1;
            Indexes[5] = 3;

        }
    }
}
