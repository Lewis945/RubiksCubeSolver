using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RubiksCube.OpenCV.TestCase.NWAugmentedReality
{
    /// <summary>
    /// https://github.com/amulware/genericgamedev-opentk-intro/tree/master/src
    /// </summary>
    public sealed class GameWindow : OpenTK.GameWindow
    {
        public GameWindow()
            // set window resolution, title, and default behaviour
            : base(300, 300, GraphicsMode.Default, "OpenTK Intro",
            GameWindowFlags.Default, DisplayDevice.Default,
            // ask for an OpenGL 3.0 forward compatible context
            3, 0, GraphicsContextFlags.ForwardCompatible)
        {
            Console.WriteLine("gl version: " + GL.GetString(StringName.Version));
        }

        protected override void OnResize(EventArgs e)
        {
            GL.Viewport(0, 0, this.Width, this.Height);
        }

        protected override void OnLoad(EventArgs e)
        {
            // this is called when the window starts running
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            // this is called every frame, put game logic here
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            GL.LoadIdentity();

            //window size is 300x300
            GL.Ortho(0, 300, 300, 0, 0.0f, 100.0f);

            GL.ClearColor(Color4.Beige);
            GL.ClearDepth(1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.EnableClientState(ArrayCap.VertexArray);
            GL.EnableClientState(ArrayCap.ColorArray);

            var triangle_vertex = new float[]
            {
                150,10,     //vertex 1
                280,250,    //vertex 2
                20,250      //vertex 3
            };
            var triangle_color = new float[]
            {
                1,0,0,      //red
                0,1,0,      //green
                0,0,1       //blue
            };
            GL.VertexPointer(2, VertexPointerType.Float, 0, triangle_vertex);
            GL.ColorPointer(3, ColorPointerType.Float, 0, triangle_color);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 3);

            GL.DisableClientState(ArrayCap.VertexArray);
            GL.DisableClientState(ArrayCap.ColorArray);

            this.SwapBuffers();
        }
    }
}
