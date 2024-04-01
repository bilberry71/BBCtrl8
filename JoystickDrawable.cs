using Microsoft.Maui.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBCtrl8
{
    public class JoystickDrawable : IDrawable
    {
        private const float _spotRadius = 40f;
        //private const float _areaRadius = 120f;
        private readonly RadialGradientPaint gradSpot = new(new PaintGradientStop[] {
                new (0.1f, Colors.DarkBlue),
                new (0.2f, Colors.AliceBlue),
                new (0.5f, Colors.Red),
                new (1.0f, Colors.Orange),
        });
        private readonly RadialGradientPaint gradArea = new(new PaintGradientStop[] {
                new (0.1f, Colors.DarkGreen),
                new (0.5f, Colors.Green),
                new (0.7f, Colors.LightSeaGreen),
                new (1.0f, Colors.GreenYellow),
        });
        private readonly LinearGradientPaint gradBack = new(new PaintGradientStop[] {
                new (0.1f, Colors.Yellow),
                new (1.0f, Colors.Green)
        }, new(0.5, 0), new(0.5, 1));
        private float _joyAreaRadius = 120f;
        private PointF _spot;

        public JoystickDrawable(float joyAreaRadius)
        {
            _joyAreaRadius = joyAreaRadius;
        }

        public JoystickDrawable()
        {
        }

        public PointF Spot
        {
            get => _spot;
            set
            {
                if (Spot.X < 0 || Spot.Y < 0 || Spot.Distance(JoyCenter) > _joyAreaRadius)
                    _spot = JoyCenter;
                else _spot = value;
            }
        } //= new PointF(-1f, -1f);
        public PointF JoyCenter { get; set; } = new PointF(-1f, -1f);
        public float JoyAreaRadius
        {
            get
            {
                return _joyAreaRadius;
            }
            set
            {
                _joyAreaRadius = value;
            }
        }

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            JoyCenter = dirtyRect.Center;
            canvas.StrokeColor = Colors.Green;
            canvas.StrokeSize = 3;
            canvas.SetFillPaint(gradBack, dirtyRect);
            canvas.FillRectangle(dirtyRect);

            canvas.SetFillPaint(gradArea, new(JoyCenter, new(_joyAreaRadius)));
            canvas.FillCircle(JoyCenter, _joyAreaRadius);

            canvas.StrokeColor = Colors.DeepSkyBlue;
            canvas.StrokeSize = 1;
            canvas.DrawLine(JoyCenter.X, dirtyRect.Top, JoyCenter.X, dirtyRect.Bottom);
            canvas.DrawLine(dirtyRect.Left, JoyCenter.Y, dirtyRect.Right, JoyCenter.Y);


            if (Spot.X == 0 && Spot.Y == 0) //|| Spot.Distance(JoyCenter) > _joyAreaRadius
            {
                canvas.StrokeSize = 10;
                canvas.FillColor = Colors.BlueViolet;
                canvas.StrokeColor = Colors.DarkBlue;
                canvas.DrawCircle(JoyCenter, _spotRadius);
            }
            else
            {
                canvas.SetFillPaint(gradSpot, new(JoyCenter, new(_joyAreaRadius)));
                canvas.FillCircle(Spot, _spotRadius);
            }
        }
    }
}
