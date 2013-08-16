using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Threading;

namespace DigitalFauvel
{
    /**
     * This class sets up the background workers for the application.
     * Primary Coder: Brendan Chou
     * */
    class Workers
    {

        public BackgroundWorker slideImageChange;
        public BackgroundWorker versoImageChange;
        public BackgroundWorker rectoImageChange;
        public bool largeRectoLoaded, largeVersoLoaded;
        public int slideInt;
        private Tab tab;



        public void updateTranslations()
        {
            var translationDispatcher = tab._canvas.Dispatcher.BeginInvoke(new Action(() =>
            {
                tab._translationBoxesV = Overlay.getTranslationOverlay(PageNamer.getOnePageText(tab._page));
                tab._textBlocksV = new List<TextBlock>();
                tab._translationBoxesR = Overlay.getTranslationOverlay(PageNamer.getOnePageText(tab._page + 1));
                tab._textBlocksR = new List<TextBlock>();
            }));
            translationDispatcher.Completed += (s, e) =>
            {
                foreach (TranslationBox tb in tab._translationBoxesV)
                {
                    TextBlock t = new TextBlock();
                    Grid g = Overlay.getGrid(tb, t);
                    tab._textBlocksV.Add(t);
                    tab._vTranslationGrid.Children.Add(g);
                }
                foreach (TranslationBox tb in tab._translationBoxesR)
                {
                    TextBlock t = new TextBlock();
                    Grid g = Overlay.getGrid(tb, t);
                    tab._textBlocksR.Add(t);
                    tab._rTranslationGrid.Children.Add(g);
                }
                setTranslateTextRecto(tab._currentLanguage);
                setTranslateTextVerso(tab._currentLanguage);
            };
        }

        /**
         * Creates the bounding boxes for the current page
         **/
        public void updateGhostBoxes()
        {
            var ghostBoxDispatcher = tab._canvas.Dispatcher.BeginInvoke(new Action(() =>
            {
                tab._vGhostBoxes = Overlay.getGhostBoxes(PageNamer.getOnePageText(tab._page));
                tab._rGhostBoxes = Overlay.getGhostBoxes(PageNamer.getOnePageText(tab._page + 1));
            }));
            ghostBoxDispatcher.Completed += (s, e) =>
            {
                foreach (BoundingBox r in tab._vGhostBoxes)
                {
                    Grid g = Overlay.getGrid(r);
                    tab._vBoxesGrid.Children.Add(g);
                }
                foreach (BoundingBox r in tab._rGhostBoxes)
                {
                    Grid g = Overlay.getGrid(r);
                    tab._rBoxesGrid.Children.Add(g);
                }
            };
        }

        public void setTranslateText(SurfaceWindow1.language language)
        {
            setTranslateTextVerso(language);
            setTranslateTextRecto(language);
        }

        private void setTranslateTextVerso(SurfaceWindow1.language language)
        {
            if (tab._textBlocksV != null)
            {
                int leftCount = tab._textBlocksV.Count;
                for (int i = 0; i < leftCount; i++)
                {
                    tab._textBlocksV[i].Visibility = System.Windows.Visibility.Visible;
                    if (language == SurfaceWindow1.language.None)
                        tab._textBlocksV[i].Visibility = System.Windows.Visibility.Hidden;
                    if (language == SurfaceWindow1.language.OldFrench)
                        tab._textBlocksV[i].Text = tab._translationBoxesV[i].getOldFr();
                    if (language == SurfaceWindow1.language.French)
                    tab._textBlocksV[i].Text = tab._translationBoxesV[i].getModFr(); 
                    if (language == SurfaceWindow1.language.English)
                        tab._textBlocksV[i].Text = tab._translationBoxesV[i].getEng();
                }
            }
        }

        private void setTranslateTextRecto(SurfaceWindow1.language language)
        {
            if (tab._textBlocksR != null)
            {
                int rightCount = tab._textBlocksR.Count;
                for (int i = 0; i < rightCount; i++)
                {
                    tab._textBlocksR[i].Visibility = System.Windows.Visibility.Visible;
                    if (language == SurfaceWindow1.language.None)
                        tab._textBlocksR[i].Visibility = System.Windows.Visibility.Hidden;
                    if (language == SurfaceWindow1.language.OldFrench)
                        tab._textBlocksR[i].Text = tab._translationBoxesR[i].getOldFr();
                    if (language == SurfaceWindow1.language.French)
                    tab._textBlocksR[i].Text = tab._translationBoxesR[i].getModFr(); 
                    if (language == SurfaceWindow1.language.English)
                        tab._textBlocksR[i].Text = tab._translationBoxesR[i].getEng();
                }
            }
        }

        /**
         * updates the images on the page slider
         **/
        public void updateSlideImage(int pageNum)
        {
            slideInt = pageNum;
            if(!slideImageChange.IsBusy)
                slideImageChange.RunWorkerAsync();
        }

        public void updateVersoImage(bool big)
        {
            largeVersoLoaded = big;
            if(!versoImageChange.IsBusy)
                versoImageChange.RunWorkerAsync();
        }

        public void updateRectoImage(bool big)
        {
            largeRectoLoaded = big;
            if (!rectoImageChange.IsBusy)
                rectoImageChange.RunWorkerAsync();
        }

        /**
         * Constructor. The Workers object is owned by newtab and will update the stuff for this tab.
         **/
        public Workers(Tab newtab)
        {
            slideImageChange = new BackgroundWorker();
            versoImageChange = new BackgroundWorker();
            rectoImageChange = new BackgroundWorker();

            tab = newtab;
            largeVersoLoaded = false;
            largeRectoLoaded = false;

            versoImageChange.WorkerSupportsCancellation = true;
            rectoImageChange.WorkerSupportsCancellation = true;
            slideImageChange.WorkerSupportsCancellation = true;
            
            versoImageChange.DoWork += (s, e) =>
            {
                e.Result = null;
                int pn = tab._page + 10;
                BitmapImage image = new BitmapImage();
                image.BeginInit();
                if (!largeVersoLoaded)
                    image.DecodePixelWidth = SurfaceWindow1.minPageWidth;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = new Uri("pages/" + pn.ToString() + ".jpg", UriKind.Relative);
                image.EndInit();
                image.Freeze();
                if (pn == tab._page + 10)
                    e.Result = image;
                else
                    return;
            };
            versoImageChange.RunWorkerCompleted += (s, e) =>
            {
                if (e.Result != null)
                {
                    BitmapImage bitmapImage = e.Result as BitmapImage;
                    tab._verso.Source = bitmapImage;
                    versoImageChange.Dispose();
                }
                else
                {
                    versoImageChange.Dispose();
                    versoImageChange.RunWorkerAsync();
                }
            };

            rectoImageChange.DoWork += (s, e) =>
            {
                int pn = tab._page + 11;
                BitmapImage image = new BitmapImage();
                image.BeginInit();
                if (!largeRectoLoaded)
                    image.DecodePixelWidth = SurfaceWindow1.minPageWidth;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = new Uri("pages/" + pn.ToString() + ".jpg", UriKind.Relative);
                image.EndInit();
                image.Freeze();
                if (pn == tab._page + 11)
                    e.Result = image;
                else
                    return;
            };
            rectoImageChange.RunWorkerCompleted += (s, e) =>
            {
                if (e.Result != null)
                {
                    BitmapImage bitmapImage = e.Result as BitmapImage;
                    tab._recto.Source = bitmapImage;
                    rectoImageChange.Dispose();
                }
                else
                {
                    rectoImageChange.Dispose();
                    rectoImageChange.RunWorkerAsync();
                }
            };

            slideImageChange.DoWork += (s, e) =>
            {
                e.Result = null;
                int pn = 2 * slideInt + 10;
                BitmapImage image1 = new BitmapImage();
                BitmapImage image2 = new BitmapImage();
                image1.BeginInit();
                image2.BeginInit();
                image1.CacheOption = BitmapCacheOption.OnLoad;
                image2.CacheOption = BitmapCacheOption.OnLoad;
                image1.UriSource = new Uri("smallpages/" + pn.ToString() + ".jpg", UriKind.Relative);
                image2.UriSource = new Uri("smallpages/" + (pn + 1).ToString() + ".jpg", UriKind.Relative);
                image1.EndInit();
                image2.EndInit();
                image1.Freeze();
                image2.Freeze();
                e.Result = new BitmapImage[] {image1, image2};
            };
            slideImageChange.RunWorkerCompleted += (s, e) =>
            {
                if (e.Result != null)
                {
                    BitmapImage[] result = (BitmapImage[])e.Result;
                    BitmapImage image1 = result[0] as BitmapImage;
                    BitmapImage image2 = result[1] as BitmapImage;
                    SurfaceWindow1.slideImage1.Source = image1;
                    SurfaceWindow1.slideImage2.Source = image2;
                    slideImageChange.Dispose();
                }
                else
                {
                    slideImageChange.Dispose();
                    slideImageChange.RunWorkerAsync();
                }
            };
        }

            
    }
}
