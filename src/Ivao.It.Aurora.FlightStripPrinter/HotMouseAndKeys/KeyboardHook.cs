using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;

namespace Ivao.It.Aurora.FlightStripPrinter.HotMouseAndKeys;

/// <summary>
/// Captures global keyboard events
/// </summary>
public class KeyboardHook : GlobalHook
{
    private PresentationSource _presentationSource;
    public void SetPresentationSource(PresentationSource ps)
    {
        _presentationSource = ps;
    }

    #region Events

    public event KeyEventHandler KeyDown;
    public event KeyEventHandler KeyUp;
    //public event KeyPressEventHandler KeyPress;

    #endregion

    #region Constructor

    public KeyboardHook()
    {
        _hookType = WH_KEYBOARD_LL;
    }

    #endregion

    #region Methods

    protected override int HookCallbackProcedure(int nCode, int wParam, IntPtr lParam)
    {

        bool handled = false;

        if (nCode > -1 && (KeyDown != null || KeyUp != null))
        {

            KeyboardHookStruct keyboardHookStruct =
                (KeyboardHookStruct)Marshal.PtrToStructure(lParam, typeof(KeyboardHookStruct));
            if (keyboardHookStruct == null) return 0;

            // Is Control being held down?
            bool control = (GetKeyState(VK_LCONTROL) & 0x80) != 0 ||
                           (GetKeyState(VK_RCONTROL) & 0x80) != 0;

            // Is Shift being held down?
            bool shift = (GetKeyState(VK_LSHIFT) & 0x80) != 0 ||
                         (GetKeyState(VK_RSHIFT) & 0x80) != 0;

            // Is Alt being held down?
            bool alt = (GetKeyState(VK_LALT) & 0x80) != 0 ||
                       (GetKeyState(VK_RALT) & 0x80) != 0;

            // Is CapsLock on?
            bool capslock = GetKeyState(VK_CAPITAL) != 0;

            // Create event using keycode and control/shift/alt values found
            Key key = ((FormsKeys)keyboardHookStruct.vkCode).ToKey();

            KeyEventArgs e = new KeyEventArgs(InputManager.Current.PrimaryKeyboardDevice, _presentationSource, 0, key);

            // Handle KeyDown and KeyUp events
            switch (wParam)
            {

                case WM_KEYDOWN:
                case WM_SYSKEYDOWN:
                    if (KeyDown != null)
                    {
                        KeyDown(this, e);
                        handled = handled || e.Handled;
                    }
                    break;
                case WM_KEYUP:
                case WM_SYSKEYUP:
                    if (KeyUp != null)
                    {
                        KeyUp(this, e);
                        handled = handled || e.Handled;
                    }
                    break;

            }
        }

        if (handled)
        {
            return 1;
        }
        else
        {
            return CallNextHookEx(_handleToHook, nCode, wParam, lParam);
        }

    }

    #endregion

}

