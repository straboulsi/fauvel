using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Media.Imaging;

namespace SurfaceApplication1
{
    class Workers
    {
        public bool bigV, bigR;
        public Tab tabV, tabR;
        public int slideInt;

        public BackgroundWorker slideImageChange = new BackgroundWorker();
        public BackgroundWorker versoImageChange = new BackgroundWorker();
        public BackgroundWorker rectoImageChange = new BackgroundWorker();

        public void updateSlideImage(int pageNum)
        {
            slideInt = pageNum;
            slideImageChange.RunWorkerAsync();
        }

        public void updateVersoImage(Tab newTab, bool big)
        {
            if (newTab != null)
            {
                tabV = newTab;
                bigV = big;
                versoImageChange.RunWorkerAsync();
            }
        }

        public void updateRectoImage(Tab newTab, bool big)
        {
            if (newTab != null)
            {
                tabR = newTab;
                bigR = big;
                rectoImageChange.RunWorkerAsync();
            }
        }

        public Workers()
        {
            bigV = false;
            bigR = false;

            versoImageChange.WorkerSupportsCancellation = true;
            rectoImageChange.WorkerSupportsCancellation = true;
            slideImageChange.WorkerSupportsCancellation = true;

            versoImageChange.DoWork += (s, e) =>
            {
                e.Result = null;
                int pn = 2 * tabV.pageNumber + 10;
                BitmapImage image = new BitmapImage();
                image.BeginInit();
                if (!bigV)
                    image.DecodePixelWidth = SurfaceWindow1.minPageWidth;
                image.CacheOption = BitmapCacheOption.OnLoad;
                if (versoImageChange.CancellationPending) return;
                image.UriSource = new Uri("pages/" + pn.ToString() + ".jpg", UriKind.Relative);
                if (versoImageChange.CancellationPending) return;
                image.EndInit();
                image.Freeze();
                e.Result = image;
            };
            versoImageChange.RunWorkerCompleted += (s, e) =>
            {
                if (e.Result != null)
                {
                    BitmapImage bitmapImage = e.Result as BitmapImage;
                    tabV._verso.Source = bitmapImage;
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
                int pn = 2 * tabR.pageNumber + 11;
                BitmapImage image = new BitmapImage();
                image.BeginInit();
                if (!bigR)
                    image.DecodePixelWidth = SurfaceWindow1.minPageWidth;
                image.CacheOption = BitmapCacheOption.OnLoad;
                if (rectoImageChange.CancellationPending) return;
                image.UriSource = new Uri("pages/" + pn.ToString() + ".jpg", UriKind.Relative);
                if (rectoImageChange.CancellationPending) return;
                image.EndInit();
                image.Freeze();
                e.Result = image;
            };
            rectoImageChange.RunWorkerCompleted += (s, e) =>
            {
                if (e.Result != null)
                {
                    BitmapImage bitmapImage = e.Result as BitmapImage;
                    tabR._recto.Source = bitmapImage;
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
                if (slideImageChange.CancellationPending) return;
                image1.UriSource = new Uri("smallpages/" + pn.ToString() + ".jpg", UriKind.Relative);
                if (slideImageChange.CancellationPending) return;
                image2.UriSource = new Uri("smallpages/" + (pn + 1).ToString() + ".jpg", UriKind.Relative);
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
