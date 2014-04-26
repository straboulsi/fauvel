using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Surface.Presentation;
using Microsoft.Surface;
using Microsoft.Surface.Presentation.Controls;

namespace DigitalFauvel
{
    /**
     * This class defines a new main tab that displays an opening of Fauvel.
     * Primary Coder: Brendan Chou
     * */
    public class MainTab
    {
        public bool _twoPage;
        public Button _delButton;
        public Grid _canvas, _vGrid, _rGrid, _vSwipeGrid, _rSwipeGrid, _vTranslationGrid, _rTranslationGrid, _vBoxesGrid, _rBoxesGrid;
        public Image _verso, _recto;
        public int _page, numFingersVerso, numFingersRecto;
        public List<BoundingBox> _rGhostBoxes, _vGhostBoxes;
        public List<Point> fingerPos;
        public List<TextBlock> _textBlocksV, _textBlocksR;
        public List<TranslationBox> _translationBoxesV, _translationBoxesR;
        public Point avgTouchPoint;
        public ScatterView _SV;
        public ScatterViewItem _SVI;
        public SurfaceWindow1.language _currentLanguage, _previousLanguage;
        public TabItem _tab;
        public TextBlock _headerTB;
        public Workers _worker;



        public MainTab(int page, TabItem newTab, Image newVerso, Image newRecto, Grid canvas, Grid vGrid, Grid rGrid, Button delBtn, ScatterView SV, ScatterViewItem si, Grid vSwipeGrid, Grid rSwipeGrid, Grid vTranslationGrid, Grid rTranslationGrid, Grid vBoxesGrid, Grid rBoxesGrid, TextBlock headerText, SurfaceWindow1.language language)
        {
            _page = page;
            _tab = newTab;
            _verso = newVerso;
            _recto = newRecto;
            _canvas = canvas;
            _vGrid = vGrid;
            _rGrid = rGrid;
            _SVI = si;
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
            _SV = SV;
            _headerTB = headerText;
            _currentLanguage = language;
            _previousLanguage = _currentLanguage;
            _worker = new Workers(this);
        }

    }
}
