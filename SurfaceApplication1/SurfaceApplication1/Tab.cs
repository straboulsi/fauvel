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
        public int _page;
        public TabItem _tab;
        public Image _verso, _recto;
        public Canvas _canvas, _c_v, _c_r;
        public ScatterViewItem _vSVI, _rSVI;
        public int numFingersVerso, numFingersRecto;
        public List<Point> fingerPos;
        public Point avgTouchPoint;
        public Button _delButton;
        public List<TranslationBox> _translationBoxesV, _translationBoxesR;
        public List<TextBlock> _textBlocksV, _textBlocksR;
        public Grid _vGrid, _rGrid, _vSwipeGrid, _rSwipeGrid, _vTranslationGrid, _rTranslationGrid, _vBoxesGrid, _rBoxesGrid;
        public bool _twoPage;
        public ScatterView _vSV, _rSV;
        public TextBlock _headerTB;
        public Workers _worker;

        public Tab(int page, TabItem newTab, Image newVerso, Image newRecto, Canvas canvas, Canvas c_v, Canvas c_r, Grid vGrid, Grid rGrid, Button delBtn, ScatterView vSV, ScatterView rSV, ScatterViewItem vsi, ScatterViewItem rsi, Grid vSwipeGrid, Grid rSwipeGrid, Grid vTranslationGrid, Grid rTranslationGrid, Grid vBoxesGrid, Grid rBoxesGrid, TextBlock headerText)
        {
            _page = page;
            _tab = newTab;
            _verso = newVerso;
            _recto = newRecto;
            _canvas = canvas;
            _c_v = c_v;
            _c_r = c_r;
            _vGrid = vGrid;
            _rGrid = rGrid;
            _vSVI = vsi;
            _rSVI = rsi;
            _delButton = delBtn;
            numFingersRecto = 0;
            numFingersVerso = 0;
            fingerPos = new List<Point>();
            avgTouchPoint = new Point(-1, 0);
            _vSwipeGrid = vSwipeGrid;
            _rSwipeGrid = rSwipeGrid;
            _vTranslationGrid = vTranslationGrid;
            _rTranslationGrid = rTranslationGrid;
            _vBoxesGrid = vBoxesGrid;
            _rBoxesGrid = rBoxesGrid;
            _twoPage = true;
            _vSV = vSV;
            _rSV = rSV;
            _headerTB = headerText;
            _worker = new Workers(this);
        }

    }
}
