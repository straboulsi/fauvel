﻿using System;
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
        public Canvas _canvas;
        public ScatterViewItem _vSVI, _rSVI;
        public int numFingersVerso, numFingersRecto;
        public List<Point> fingerPos;
        public Point avgTouchPoint;
        public Button _delButton;
        public List<TranslationBox> _translationBoxesV, _translationBoxesR;
        public List<TextBlock> _textBlocksV, _textBlocksR;
        public Grid _vGrid, _rGrid, _vSwipeGrid, _rSwipeGrid, _vTranslationGrid, _rTranslationGrid;

        public Tab(int page, TabItem newTab, Image newVerso, Image newRecto, Canvas canvas, Grid vGrid, Grid rGrid, Button delBtn, ScatterViewItem vsi, ScatterViewItem rsi, Grid vSwipeGrid, Grid rSwipeGrid, Grid vTranslationGrid, Grid rTranslationGrid)
        {
            _page = page;
            _tab = newTab;
            _verso = newVerso;
            _recto = newRecto;
            _canvas = canvas;
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
        }

    }
}
