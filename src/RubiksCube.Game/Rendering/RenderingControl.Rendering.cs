using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace RubiksCube.Game.Rendering
{
    public abstract partial class RenderingControl<T>
    {
        #region Fields

        private List<double> _frameTimes;
        private IEnumerable<T>[] _buffer;
        private Thread _updateThread, _renderThread;
        private AutoResetEvent[] _updateHandle;
        private AutoResetEvent[] _renderHandle;
        private int _currentBufferIndex;

        protected double zoom;

        #endregion

        #region Properties

        public double Zoom
        {
            get
            {
                int min = Math.Min(Screen.Height, Screen.Width);
                return zoom / (3 * (min / (double)400));
            }
            set
            {
                int min = Math.Min(Screen.Height, Screen.Width);
                zoom = 3 * (min / (double)400) * value;
            }
        }

        public Rectangle Screen { get; private set; }

        public double MaxFps { get; set; }
        public double Fps { get; protected set; }

        public bool IsRunning { get; protected set; }

        public DrawingMode DrawingMode { get; set; }

        public int RotationSpeed { get; set; }

        public double[] Rotation { get; protected set; }

        #endregion

        #region Private Methods

        private void InitRenderer()
        {
            SetDrawingArea(ClientRectangle);

            _frameTimes = new List<double>();
            IsRunning = false;
            MaxFps = 30;

            _updateHandle = new AutoResetEvent[2];
            for (int i = 0; i < _updateHandle.Length; i++)
            {
                _updateHandle[i] = new AutoResetEvent(false);
            }

            _renderHandle = new AutoResetEvent[2];
            for (int i = 0; i < _renderHandle.Length; i++)
            {
                _renderHandle[i] = new AutoResetEvent(true);
            }

            _buffer = new IEnumerable<T>[2];
            for (int i = 0; i < _buffer.Length; i++)
            {
                _buffer[i] = new List<T>();
            }
        }

        private void RenderLoop()
        {
            var _sw = new Stopwatch();

            int bufferIndex = 0x0;

            while (IsRunning)
            {
                _sw.Restart();
                Render(bufferIndex);
                bufferIndex ^= 0x1;

                double minTime = 1000.0 / MaxFps;
                while (_sw.Elapsed.TotalMilliseconds < minTime) { } // keep max fps

                _sw.Stop();

                _frameTimes.Add(_sw.Elapsed.TotalMilliseconds);
                int counter = 0;
                int index = _frameTimes.Count - 1;
                double ms = 0;
                while (index >= 0 && ms + _frameTimes[index] <= 1000)
                {
                    ms += _frameTimes[index];
                    counter++;
                    index--;
                }
                if (index > 0)
                {
                    _frameTimes.RemoveRange(0, index);
                }
                Fps = counter + ((1000 - ms) / _frameTimes[0]);
            }
        }

        private void UpdateLoop()
        {
            int bufferIndex = 0x0;
            _currentBufferIndex = 0x1;

            while (IsRunning)
            {
                Update(bufferIndex);
                _currentBufferIndex = bufferIndex;
                bufferIndex ^= 0x1;
            }
        }

        private void Render(int bufferIndex)
        {
            _updateHandle[bufferIndex].WaitOne();
            Invalidate();
            _renderHandle[bufferIndex].Set();
        }

        private void Update(int bufferIndex)
        {
            _renderHandle[bufferIndex].WaitOne();

            Update((f) => { _buffer[bufferIndex] = f; });

            _updateHandle[bufferIndex].Set();
        }

        #endregion

        #region Abstract

        public abstract void Update(Action<IEnumerable<T>> set);

        public abstract void Render(Graphics g, IEnumerable<T> frame);

        #endregion

        #region Override

        protected override void OnSizeChanged(EventArgs e)
        {
            SetDrawingArea(ClientRectangle);
            Invalidate();
            base.OnSizeChanged(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var frame = _buffer[_currentBufferIndex];
            if (frame != null)
            {
                Render(e.Graphics, frame);
            }
        }

        #endregion

        #region Methods

        public void SetDrawingArea(Rectangle screen)
        {
            Screen = new Rectangle(screen.X, screen.Y, screen.Width, screen.Height - 50);
            Zoom = 1;
            if (screen.Width > screen.Height)
            {
                screen.X = (screen.Width - screen.Height) / 2;
            }
            else if (screen.Height > screen.Width)
            {
                screen.Y = (screen.Height - screen.Width) / 2;
            }
        }

        public void StartRender()
        {
            if (!IsRunning)
            {
                InitRenderer();

                IsRunning = true;
                _updateThread = new Thread(UpdateLoop);
                _updateThread.Start();
                _renderThread = new Thread(RenderLoop);
                _renderThread.Start();
            }
        }

        public void StopRender()
        {
            if (IsRunning)
            {
                IsRunning = false;
                _updateThread.Join();
                _renderThread.Join();
                Fps = 0;
                _frameTimes.Clear();
            }
        }

        public void AbortRender()
        {
            if (IsRunning)
            {
                IsRunning = false;
                Fps = 0;
                _frameTimes.Clear();
                _updateThread.Abort();
                _renderThread.Abort();
            }
        }

        #endregion
    }
}
