using Caliburn.Micro;

namespace Ivao.It.Aurora.FlightStripPrinter.Models;

public class SettingsModel : PropertyChangedBase
{
    private string _areaIcaoCode;
    public string AreaIcaoCode
    {
        get { return _areaIcaoCode; }
        set { _areaIcaoCode = value; NotifyOfPropertyChange(); }
    }

    private int _stripWidth;
    public int StripWidth
    {
        get { return _stripWidth; }
        set { _stripWidth = value; NotifyOfPropertyChange(); }
    }

    private int _stripHeigth;
    public int StripHeigth
    {
        get { return _stripHeigth; }
        set { _stripHeigth = value; NotifyOfPropertyChange(); }
    }

    private int _printZoom;
    public int PrintZoom
    {
        get { return _printZoom; }
        set { _printZoom = value; NotifyOfPropertyChange(); }
    }

    private int _marginTop;
    public int MarginTop
    {
        get { return _marginTop; }
        set { _marginTop = value; NotifyOfPropertyChange(); }
    }

    private int _marginRight;
    public int MarginRight
    {
        get { return _marginRight; }
        set { _marginRight = value; NotifyOfPropertyChange(); }
    }

    private int _marginLeft;
    public int MarginLeft
    {
        get { return _marginLeft; }
        set { _marginLeft = value; NotifyOfPropertyChange(); }
    }

    private int _marginBottom;
    public int MarginBottom
    {
        get { return _marginBottom; }
        set { _marginBottom = value; NotifyOfPropertyChange(); }
    }
}
