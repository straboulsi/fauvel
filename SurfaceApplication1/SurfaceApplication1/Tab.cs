using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Surface.Presentation;
using Microsoft.Surface;
using Microsoft.Surface.Presentation.Controls;

namespace SurfaceApplication1
{
    class Tab
    {
        public int pageNumber;
        public TabItem _tab;
        public Image _verso, _recto;
        public Canvas _canvas, _cs;
        public ScatterViewItem _vSVI, _rSVI;
        public int numFingersVerso, numFingersRecto;
        public List<Point> fingerPos;
        public Point avgTouchPoint;
        public Button _delButton;

        public Tab(int page, TabItem newTab, Image newVerso, Image newRecto, Canvas canvas, ScatterViewItem vScatterViewItem, ScatterViewItem rScatterViewItem, Canvas cs, Button delBtn)
        {
            pageNumber = page;
            _tab = newTab;
            _verso = newVerso;
            _recto = newRecto;
            _canvas = canvas;
            _vSVI = vScatterViewItem;
            _rSVI = rScatterViewItem;
            _cs = cs;
            _delButton = delBtn;
            numFingersRecto = 0;
            numFingersVerso = 0;
            fingerPos = new List<Point>();
            avgTouchPoint = new Point(-1, 0);
        }

    }
}
