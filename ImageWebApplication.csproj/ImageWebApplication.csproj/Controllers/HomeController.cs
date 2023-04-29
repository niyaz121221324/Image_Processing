using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.DataVisualization.Charting;

namespace ImageWebApplication.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddFrame(int frameSize)
        {
            // загрузка изображения
            string imagePath = Server.MapPath("~/Content/image.jpg");
            Bitmap originalImage = new Bitmap(imagePath);

            // создание нового изображения с рамкой
            Bitmap newImage = new Bitmap(originalImage.Width + frameSize * 2, originalImage.Height + frameSize * 2);
            Graphics g = Graphics.FromImage(newImage);
            g.FillRectangle(Brushes.White, 0, 0, newImage.Width, newImage.Height);
            g.DrawImage(originalImage, frameSize, frameSize, originalImage.Width, originalImage.Height);

            // отправка изображения пользователю
            Response.ContentType = "image/jpeg";
            newImage.Save(Response.OutputStream, System.Drawing.Imaging.ImageFormat.Jpeg);

            // подсчет распределения цветов
            Dictionary<Color, int> colorDistribution = new Dictionary<Color, int>();
            for (int x = 0; x < originalImage.Width; x++)
                for (int y = 0; y < originalImage.Height; y++)
                {
                    Color pixelColor = originalImage.GetPixel(x, y);
                    if (!colorDistribution.ContainsKey(pixelColor))
                        colorDistribution.Add(pixelColor, 1);
                    else
                        colorDistribution[pixelColor]++;
                }

            // создание графика распределения цветов
            Chart chart = new Chart();
            chart.Width = 500;
            chart.Height = 300;
            chart.Titles.Add("Color Distribution");
            chart.Series.Add("Colors");
            chart.Series["Colors"].ChartType = SeriesChartType.Column;
            chart.ChartAreas.Add("Colors");
            chart.ChartAreas["Colors"].AxisX.MajorGrid.Enabled = false;
            chart.ChartAreas["Colors"].AxisY.MajorGrid.Enabled = false;
            chart.ChartAreas["Colors"].AxisX.LabelStyle.Enabled = false;
            chart.ChartAreas["Colors"].AxisY.LabelStyle.Enabled = false;
            chart.ChartAreas["Colors"].BackColor = Color.White;
            int i = 0;
            foreach (KeyValuePair<Color, int> pair in colorDistribution
                .OrderByDescending(p => p.Value))
            {
                chart.Series["Colors"].Points.AddXY(i, pair.Value);
                chart.Series["Colors"].Points[i].Color = pair.Key;
                i++;
            }

            // отправка графика пользователю
            Response.ContentType = "image/jpeg";
            chart.SaveImage(Response.OutputStream, ChartImageFormat.Jpeg);

            return null;
        }
    }
}