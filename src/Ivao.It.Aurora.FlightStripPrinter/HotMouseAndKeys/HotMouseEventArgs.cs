using System.Windows.Input;

namespace Ivao.It.Aurora.FlightStripPrinter.HotMouseAndKeys;

public class HotMouseEventArgs : MouseEventArgs
{
    public MouseButton Button { get; }

    public HotMouseEventArgs(MouseEventArgs args, MouseButton button) : base(args.MouseDevice, args.Timestamp)
    {
        Button = button;
    }

    public HotMouseEventArgs(MouseDevice mouse, int timestamp, MouseButton button) : base(mouse, timestamp)
    {
        Button = button;
    }
}
