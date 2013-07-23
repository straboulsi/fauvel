using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using System.Windows;

namespace SurfaceApplication1
{
    class Workers
    {
        private Tab tab;
        public bool bigV, bigR;
        public int slideInt;

        public BackgroundWorker versoGhostBoxes = new BackgroundWorker();
        public BackgroundWorker rectoGhostBoxes = new BackgroundWorker();
        public BackgroundWorker versoTranslations = new BackgroundWorker();
        public BackgroundWorker rectoTranslations = new BackgroundWorker();
        public BackgroundWorker slideImageChange = new BackgroundWorker();
        public BackgroundWorker versoImageChange = new BackgroundWorker();
        public BackgroundWorker rectoImageChange = new BackgroundWorker();

        public void updateTranslations()
        {
            if (!versoTranslations.IsBusy)
                versoTranslations.RunWorkerAsync();
            if (!rectoTranslations.IsBusy)
                rectoTranslations.RunWorkerAsync();
        }

        public void updateGhostBoxes()
        {
            if (!versoGhostBoxes.IsBusy)
                versoGhostBoxes.RunWorkerAsync();
            if (!rectoGhostBoxes.IsBusy)
                rectoGhostBoxes.RunWorkerAsync();
        }

        public void setTranslateText(SurfaceWindow1.language language)
        {
            setTranslateTextVerso(language);
            setTranslateTextRecto(language);
        }

        private void setTranslateTextVerso(SurfaceWindow1.language language)
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
                    tab._textBlocksV[i].Text = tab._translationBoxesV[i].getOldFr();
                if (language == SurfaceWindow1.language.English)
                    tab._textBlocksV[i].Text = tab._translationBoxesV[i].getEng();
            }
        }

        private void setTranslateTextRecto(SurfaceWindow1.language language)
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
                    tab._textBlocksR[i].Text = tab._translationBoxesR[i].getOldFr();
                if (language == SurfaceWindow1.language.English)
                    tab._textBlocksR[i].Text = tab._translationBoxesR[i].getEng();
            }
        }

        public void updateSlideImage(int pageNum)
        {
            slideInt = pageNum;
            if(!slideImageChange.IsBusy)
                slideImageChange.RunWorkerAsync();
        }

        public void updateVersoImage(bool big)
        {
            bigV = big;
            if(!versoImageChange.IsBusy)
                versoImageChange.RunWorkerAsync();
        }

        public void updateRectoImage(bool big)
        {
            bigR = big;
            if (!rectoImageChange.IsBusy)
                rectoImageChange.RunWorkerAsync();
        }

        public Workers(Tab newtab)
        {
            tab = newtab;
            bigV = false;
            bigR = false;

            rectoGhostBoxes.WorkerSupportsCancellation = false;
            versoGhostBoxes.WorkerSupportsCancellation = false;
            versoImageChange.WorkerSupportsCancellation = false;
            rectoImageChange.WorkerSupportsCancellation = false;
            slideImageChange.WorkerSupportsCancellation = false;
            versoTranslations.WorkerSupportsCancellation = false;
            rectoTranslations.WorkerSupportsCancellation = false;

            versoGhostBoxes.DoWork += (s, e) =>
            {
                e.Result = tab._page;
                tab._vBoxesGrid.Children.Clear();

                List<Rect> vGhostBoxes = Translate.getGhostBoxes(PageNamer.getOnePageText(tab._page), SurfaceWindow1.xml, SurfaceWindow1.engXml, SurfaceWindow1.layoutXml);
                
                foreach (Rect r in vGhostBoxes)
                {
                    Grid g = Translate.getGhostGrid(r.X, r.Y, r.Width, r.Height);
                    tab._vBoxesGrid.Children.Add(g);
                }
            };
            versoGhostBoxes.RunWorkerCompleted += (s, e) =>
            {
                if ((int)(e.Result) != tab._page)
                {
                    versoGhostBoxes.Dispose();
                    versoGhostBoxes.RunWorkerAsync();
                }
            };
            
            versoTranslations.DoWork += (s, e) =>
            {
                e.Result = tab._page;
                tab._translationBoxesV = Translate.getBoxes(PageNamer.getOnePageText(tab._page), SurfaceWindow1.xml, SurfaceWindow1.engXml, SurfaceWindow1.layoutXml);
                tab._textBlocksV = new List<TextBlock>();

                foreach (TranslationBox tb in tab._translationBoxesV)
                {
                    double width, x, y, height;
                    x = tb.getTopLeft().X;
                    y = tb.getTopLeft().Y;
                    width = (tb.getBottomRight().X - tb.getTopLeft().X);
                    height = (tb.getBottomRight().Y - tb.getTopLeft().Y);

                    TextBlock t = new TextBlock();
                    Grid g = Translate.getGrid(x, y, width, height, t);
                    t.Foreground = Translate.textBrush;
                    t.Background = Translate.backBrush;
                    tab._textBlocksV.Add(t);
                    tab._vTranslationGrid.Children.Add(g);
                }
            };
            versoTranslations.RunWorkerCompleted += (s, e) =>
            {
                if ((int)(e.Result) == tab._page)
                {
                    setTranslateTextVerso(SurfaceWindow1.currentLanguage);
                    versoImageChange.Dispose();
                } else {
                    versoImageChange.Dispose();
                    versoImageChange.RunWorkerAsync();
                }
            };

            rectoTranslations.DoWork += (s, e) =>
            {
                tab._translationBoxesR = Translate.getBoxes(PageNamer.getOnePageText(tab._page + 1), SurfaceWindow1.xml, SurfaceWindow1.engXml, SurfaceWindow1.layoutXml);
                tab._textBlocksR = new List<TextBlock>();

                foreach (TranslationBox tb in tab._translationBoxesR)
                {
                    e.Result = tab._page + 1;
                    double width, x, y, height;
                    x = tb.getTopLeft().X;
                    y = tb.getTopLeft().Y;
                    width = (tb.getBottomRight().X - tb.getTopLeft().X);
                    height = (tb.getBottomRight().Y - tb.getTopLeft().Y);

                    TextBlock t = new TextBlock();
                    Grid g = Translate.getGrid(x, y, width, height, t);
                    t.Foreground = Translate.textBrush;
                    t.Background = Translate.backBrush;
                    tab._textBlocksR.Add(t);
                    tab._rTranslationGrid.Children.Add(g);
                }
            };
            rectoTranslations.RunWorkerCompleted += (s, e) =>
            {
                if ((int)(e.Result) == tab._page + 1)
                {
                    setTranslateTextRecto(SurfaceWindow1.currentLanguage);
                    rectoImageChange.Dispose();
                } else {
                    rectoImageChange.Dispose();
                    rectoImageChange.RunWorkerAsync();
                }
            };

            versoImageChange.DoWork += (s, e) =>
            {
                e.Result = null;
                int pn = tab._page + 10;
                BitmapImage image = new BitmapImage();
                image.BeginInit();
                if (!bigV)
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
                if (!bigR)
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
                int pn = slideInt + 10;
                BitmapImage image1 = new BitmapImage();
                BitmapImage image2 = new BitmapImage();
                image1.BeginInit();
                image2.BeginInit();
                image1.CacheOption = BitmapCacheOption.OnLoad;
                image2.CacheOption = BitmapCacheOption.OnLoad;
                if (slideImageChange.CancellationPending) return;
                image1.UriSource = new Uri("smallpages/" + pn.ToString() + ".jpg", UriKind.Relative);
                if (slideImageChange.CancellationPending) return;
                if(slideInt != SurfaceWindow1.maxPage)
                    image2.UriSource = new Uri("smallpages/" + (pn + 1).ToString() + ".jpg", UriKind.Relative);
                else
                    image2.UriSource = new Uri("smallpages/" + pn.ToString() + ".jpg", UriKind.Relative);
                if (slideImageChange.CancellationPending) return;
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
