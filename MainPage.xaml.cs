using System.Net.Sockets;
using System.Text;

namespace BBCtrl8;

public partial class MainPage : ContentPage
{
    private const string TEXT = "Dane z MAUI :)";
    JoystickDrawable joy;

    public MainPage()
	{
		InitializeComponent();
        joy = (JoystickDrawable)grView.Drawable;
        joy.JoyAreaRadius = 150;
        //var gestureRecognizer = new TapGestureRecognizer();
        //      gestureRecognizer.Tapped += GestureRecognizer_Tapped;
        //grView.GestureRecognizers.Add(gestureRecognizer);
        grView.StartInteraction += GrView_StartInteraction;
        grView.EndInteraction += GrView_EndInteraction;
        grView.DragInteraction += GrView_DragInteraction;
        //grView.StartHoverInteraction += GrView_StartHoverInteraction;
        //grView.MoveHoverInteraction += GrView_MoveHoverInteraction;
        //grView.EndHoverInteraction += GrView_EndHoverInteraction;
    }

    DateTime last = DateTime.Now;

    private void GrView_StartInteraction(object sender, TouchEventArgs e)
    {
        lblPos3.Text = $"START {string.Join('|', e.Touches.Select(t => $"({t.X:F0},{t.Y:F0})"))}";
        SemanticScreenReader.Announce(lblPos3.Text);
        UpdateRemote();
    }
    private void GrView_DragInteraction(object sender, TouchEventArgs e)
    {
        var touch = e.Touches.Select(t => new PointF(t.X, t.Y)).FirstOrDefault();
        string txt = $"DRAG {string.Join('|', e.Touches.Select(t => $"({t.X:F0},{t.Y:F0})"))}";
        lblPos3.Text = txt;
        SemanticScreenReader.Announce(lblPos3.Text);
        SetSpot(e);
        UpdateRemote();
    }

    private void UpdateRemote(bool updateNow = false)
    {
        if (!updateNow && last.AddMilliseconds(250) >= DateTime.Now)
        {
            return;
        }

        last = DateTime.Now;
        int dx = (int)((joy.Spot.X - joy.JoyCenter.X));
        float dxf = (dx / joy.JoyAreaRadius);
        sbyte x = (sbyte)(dxf * 128);

        int dy = (int)((joy.JoyCenter.Y - joy.Spot.Y));
        float dyf = (dy / joy.JoyAreaRadius);
        sbyte y = (sbyte)(dyf * 128);

        SendUdp("J", x, y);

        lblPos.Text = $"J{x},{y}  C{joy.JoyCenter.X:F0},{joy.JoyCenter.Y:F0} t{joy.Spot.X:F0},{joy.Spot.Y:F0} R{joy.JoyAreaRadius}";
        SemanticScreenReader.Announce(lblPos.Text);

        lblPos2.Text = $"d{dx:F0},{dy:F0} dF{dxf:F2},{dyf:F2}";
        SemanticScreenReader.Announce(lblPos2.Text);
    }

    private void GrView_EndInteraction(object sender, TouchEventArgs e)
    {
        lblPos2.Text = $"END {string.Join('|', e.Touches.Select(t => $"({t.X:F0},{t.Y:F0})"))}";
        SemanticScreenReader.Announce(lblPos2.Text);
        SetSpot(0, 0);
    }


    private void OnCounterClicked(object sender, EventArgs e)
    {
        lblPos.Text = "----";
        SemanticScreenReader.Announce(lblPos.Text);
        lblPos2.Text = "----";
        SemanticScreenReader.Announce(lblPos2.Text);
        lblPos3.Text = "----";
        SemanticScreenReader.Announce(lblPos3.Text);
        SetSpot(0, 0);
        SemanticScreenReader.Announce(CounterBtn.Text);
    }

    private void OnSendClicked(object sender, EventArgs e)
    {
        SendUdp(TEXT);
        lblPos.Text = TEXT;
        SemanticScreenReader.Announce(lblPos.Text);
    }

    private static void SendUdp(string msg, sbyte b1 = (sbyte)' ', sbyte b2 = (sbyte)' ')
    {
        byte[] dtxt = Encoding.ASCII.GetBytes(msg);
        byte[] buff = new byte[dtxt.Length+2];
        dtxt.CopyTo(buff,0);
        buff[dtxt.Length + 0] = (byte)b1;
        buff[dtxt.Length + 1] = (byte)b2;
        using var c = new UdpClient(2300);
        c.Send(buff, buff.Length, "192.168.1.18", 4444);
    }

    private void SetSpot(TouchEventArgs e)
    {
        var ft = e.Touches[0];
        SetSpot(ft.X, ft.Y);
    }
    private void SetSpot(float x, float y)
    {
        joy.Spot = new PointF(x, y);
        grView.Invalidate();
    }
}

