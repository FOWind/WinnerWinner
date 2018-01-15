using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScreenShot
{
    public class ClipRect
    {
        private double _startX;
        private double _startY;
        private double _width;
        private double _height;

        public double StartX { get => _startX; set => _startX = value; }
        public double StartY { get => _startY; set => _startY = value; }
        public double Width { get => _width; set => _width = value; }
        public double Height { get => _height; set => _height = value; }
    }
}
