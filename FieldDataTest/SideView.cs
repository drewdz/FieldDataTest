using DataFactory.Model;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace FieldDataTest
{
    public partial class SideView : Form
    {
        #region Fields

        private readonly List<EventData> _Data;

        #endregion Fields

        #region Constructors

        public SideView()
        {
            InitializeComponent();
        }

        public SideView(List<EventData> data)
            : this()
        {
            _Data = data;
        }

        #endregion Constructors

        private void MainPic_Paint(object sender, PaintEventArgs e)
        {
            //  get graphics context
            var width = (float)MainPic.ClientRectangle.Width;
            var height = (float)MainPic.ClientRectangle.Height;

            var margin = 5;

            var _ScaleX = (width - (2 * margin)) / 360f;
            var _ScaleY = (height - (2 * margin)) / 160f;

            Debug.WriteLine($"Width: {width}, Height: {height}, Scale X: {_ScaleX}, Scale Y: {_ScaleY}");

            var bmp = new Bitmap(MainPic.ClientRectangle.Width, MainPic.ClientRectangle.Height);
            using (var gc = Graphics.FromImage(bmp))
            {
                gc.Clear(Color.White);
                //  X/Z Axis
                gc.DrawLine(Pens.Gray, margin, margin, margin, height - margin);
                gc.DrawLine(Pens.Gray, margin, height - margin, width - margin, height - margin);
                //  plot data
                if ((_Data != null) && (_Data.Count > 0))
                {
                    float xMin = _Data.Min(d => d.X), xMax = _Data.Max(d => d.X);
                    var domain = xMax - xMin;
                    var xFactor = (width - (2 * margin)) / domain;
                    float zMin = _Data.Min(d => d.Z), zMax = _Data.Max(d => d.Z);
                    var range = zMax - zMin;
                    var zFactor = (height - (2 * margin)) / range;

                    foreach (var data in _Data)
                    {
                        gc.DrawRectangle(Pens.Black, margin + ((data.X - xMin) * xFactor), height - margin - ((data.Z - zMin) * zFactor), 1, 1);
                    }

                    //xFactor = (width - (2 * margin)) / 10;
                    //zFactor = (height - (2 * margin)) / 10;
                    //for (int i = 0; i < 10; i++)
                    //{
                    //    gc.DrawRectangle(Pens.Black, margin + (i * xFactor), height - margin - (i * zFactor), 1, 1);
                    //}
                }
                gc.Flush();
            }
            MainPic.Image = bmp;
        }
    }
}
