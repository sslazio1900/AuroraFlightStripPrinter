using System;
using System.Runtime.InteropServices;
using System.Windows.Input;

namespace Ivao.It.Aurora.FlightStripPrinter.HotMouseAndKeys;

/// <summary>
/// Captures global mouse events
/// </summary>
public class MouseHook : GlobalHook
{

    #region MouseEventType Enum

    private enum MouseEventType
    {
        None,
        MouseDown,
        MouseUp,
        DoubleClick,
        MouseWheel,
        MouseMove
    }

    #endregion

    #region Events

    public event MouseEventHandler MouseDown;
    public event MouseEventHandler MouseUp;
    public event MouseEventHandler MouseMove;
    public event MouseEventHandler MouseWheel;

    public event MouseEventHandler Click;
    public event MouseEventHandler DoubleClick;

    #endregion

    #region Constructor

    public MouseHook()
    {

        _hookType = WH_MOUSE_LL;

    }

    #endregion

    #region Methods

    protected override int HookCallbackProcedure(int nCode, int wParam, IntPtr lParam)
    {

        if (nCode > -1 && (MouseDown != null || MouseUp != null || MouseMove != null || DoubleClick != null))
        {
            MouseLLHookStruct mouseHookStruct = (MouseLLHookStruct)Marshal.PtrToStructure(lParam, typeof(MouseLLHookStruct))!;

            MouseButton button = GetButton(wParam);
            //if (button is null) return 0;
            MouseEventType eventType = GetEventType(wParam);

            //MouseEventArgs e = new MouseEventArgs(
            //    button,
            //    eventType == MouseEventType.DoubleClick ? 2 : 1,
            //    mouseHookStruct.pt.x,
            //    mouseHookStruct.pt.y,
            //    eventType == MouseEventType.MouseWheel ? (short)(mouseHookStruct.mouseData >> 16 & 0xffff) : 0);
            MouseEventArgs e = new MouseEventArgs(InputManager.Current.PrimaryMouseDevice, mouseHookStruct.time);


            // Prevent multiple Right Click events (this probably happens for popup menus)
            if (button == MouseButton.Right && mouseHookStruct.flags != 0)
            {
                eventType = MouseEventType.None;
            }

            switch (eventType)
            {
                case MouseEventType.MouseDown:
                    MouseDown?.Invoke(this, new HotMouseEventArgs(e, button));
                    break;
                case MouseEventType.MouseUp:
                    Click?.Invoke(this, new HotMouseEventArgs(e, button));
                    MouseUp?.Invoke(this, new HotMouseEventArgs(e, button));
                    break;
                case MouseEventType.DoubleClick:
                    DoubleClick?.Invoke(this, new HotMouseEventArgs(e, button));
                    break;
                case MouseEventType.MouseWheel:
                    MouseWheel?.Invoke(this, e);
                    break;
                case MouseEventType.MouseMove:
                    MouseMove?.Invoke(this, e);
                    break;
                default:
                    break;
            }

        }

        return CallNextHookEx(_handleToHook, nCode, wParam, lParam);

    }

    private MouseButton GetButton(int wParam)
    {

        switch (wParam)
        {

            case WM_LBUTTONDOWN:
            case WM_LBUTTONUP:
            case WM_LBUTTONDBLCLK:
                return MouseButton.Left;
            case WM_RBUTTONDOWN:
            case WM_RBUTTONUP:
            case WM_RBUTTONDBLCLK:
                return MouseButton.Right;
            case WM_MBUTTONDOWN:
            case WM_MBUTTONUP:
            case WM_MBUTTONDBLCLK:
                return MouseButton.Middle;
            default:
                return MouseButton.XButton1;
        }

    }

    private MouseEventType GetEventType(int wParam)
    {

        switch (wParam)
        {

            case WM_LBUTTONDOWN:
            case WM_RBUTTONDOWN:
            case WM_MBUTTONDOWN:
                return MouseEventType.MouseDown;
            case WM_LBUTTONUP:
            case WM_RBUTTONUP:
            case WM_MBUTTONUP:
                return MouseEventType.MouseUp;
            case WM_LBUTTONDBLCLK:
            case WM_RBUTTONDBLCLK:
            case WM_MBUTTONDBLCLK:
                return MouseEventType.DoubleClick;
            case WM_MOUSEWHEEL:
                return MouseEventType.MouseWheel;
            case WM_MOUSEMOVE:
                return MouseEventType.MouseMove;
            default:
                return MouseEventType.None;

        }
    }

    #endregion

}

