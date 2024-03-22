# Ivao Italy - Aurora FlightStrip Printer

Print your flight strips directly from Aurora!
A cheap POS thermal printer is strongly recommended. Prints with no costs and is really cheap to buy! And also, with a little effort you can print on a correct size paper.

> [!CAUTION]
> **.NET 8.0 Desktop Runtime needed!**
> 
> [You can download it here](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)

* [How the strip data binding works?](#how-the-strip-data-binding-works)
* [Which printer do I need?](#which-printer-do-i-need)
    * [What data is bindable to my flightstrips](#what-data-is-bindable-to-my-flightstrips)
    * [How to use this app](#how-to-use-this-app)
        * [Control bar and how to print](#control-bar-and-how-to-print)
        * [Settings](#settings)
* [What if i want to collaborate](#what-if-i-want-to-collaborate)


### Which printer do I need?

The app is designed to be printer agnostic. It actually uses any printer on your system.
By the way, what i prefer to use is a cheap POS thermal printer bought on Amazon or AliExpress with 58mm paper rolls. This size is quite perfect for being folded in an half and fit a 30mm-ish tall stip holder.
![Example printer](https://i.imgur.com/BcXvPyU.jpeg)

Talking about strip holders, if you find companies that sells those items for everyone, good to you. But you can even think to 3d print them down.

## How the strip data binding works?

> [!TIP]
> All the templates are installed in `%appdata%\Ivao Italy\Aurora Flight Strip Printer\Templates`

Looking into `Templates` folder, there are a few files:
* `template_any_in.html`
* `template_any_out.html`
* `template_vfr.html`
* `template_trans.html`

This is the minimum templates set to run the app.
The app determines if a traffic is departure, arrival or transit by looking into your Aurora controlled airports. So be careful setting them up.

#### What if I want a custom template for a specific airport?
Transit and VFR are not customizable. In/out strip can be customized.

If you want to build a template for LIRF arrivals, build your HTML file and save it as: `template_lirf_in.html`

For LIRF departures:  `template_lirf_out.html`

### What data is bindable to my flightstrips?
Aurora provides data to bind the following data:


| Placeholder | Description                                                                                                                                              |
|-------------|----------------------------------------------------------------------------------------------------------------------------------------------------------|
| [cs]        | Callsign                                                                                                                                                 |
| [assr]      | Assigned SSR Code                                                                                                                                        |
| [ssr]       | Current SSR code                                                                                                                                         |
| [acft-type] | Flightplan Aircraft Type (ICAO Code)                                                                                                                     |
| [acft-cat]  | Flightplan Aircraft WTC                                                                                                                                  |
| [equip]     | Flightplan Equipment                                                                                                                                     |
| [rules]     | Flightplan Flight rules                                                                                                                                  |
| [rfl]       | Flightplan Requested flight level/altitude with identifier (A or F)                                                                                      |
| [fl]        | Flightpane Requested flight level/altitude. If is FL, F is removed.                                                                                      |
| [dep]       | Flightplan Departure                                                                                                                                     |
| [dep2]      | Flightplan Departure 2 letters (only if the first 2 letters are matching ICAO Area code saved in settings). Eg: Settings = LI, LIRF =>RF, EDDM => EDDM   |
| [dest]      | Flightplan Destination                                                                                                                                   |
| [dest2]     | Flightplan Destination 2 letters (only if the first 2 letters are matching ICAO Area code saved in settings). Eg: Settings = LI, LIRF =>RF, EDDM => EDDM |
| [tas]       | Flightplan cruising speed                                                                                                                                |
| [alt]       | Flightplan Alternate                                                                                                                                     |
| [rte]       | Flightplan Route shortened like: first 3 identifiers ... last 3 identifiers                                                                              |
| [rmk]       | Flightplan Remaks                                                                                                                                        |
| [eobt]      | Flightplan Departure Time                                                                                                                                |
| [eta]       | Flightplan Estimated time of arrival (by FPL, EOBT+EET)                                                                                                  |
| [eet]       | Flightplan Estimated enroute time                                                                                                                        |
| [endur]     | Flightplan Endurance                                                                                                                                     |
| [proc-wpt]  | Aurora Label procedure/waipoint assigned.                                                                                                                |
| [afl]       | Aurora Label assigned FL/Alt                                                                                                                             |
| [exit-fix]  | Flightplan Route last point                                                                                                                              |
| [entry-fix] | Flightplan Route first point                                                                                                                             |
| [stand]     | Aurora Label assigned gate                                                                                                                               |
| [no-fpl]    | Flightplan route "NO FPL" to represent a no flight plan flight. It will contains a "check" icon flag a no-fpl box in the strip.                          |
| [p-time]    | Strip print UTC time, format HHMM                                                                                                                        |

### How to use this app?
App is based on IVAO Aurora 3rd party integration.

In order to have the integration working, this integration should be enabled in Aurora settings view, in the Other tab:
![Aurora 3rd party access](https://i.imgur.com/sGhdbsV.png)

The app has a main window formed as follows:
![App UI sample](https://i.imgur.com/fvaklaB.png)
1. Title bar with settings button. Use this icon to open strip generation settings view (described below).
2. App version.
3. Generated flight strip preview. Once the app creates a new flight strip, a preview is shown in this box.
4. App logs: a glance on what is going on in the app, you can use it to analyze error message or the status of the app as well.
5. Controls: here you can control the app with your mouse.

#### Control bar and how to print
![Control bar at app startup](https://i.imgur.com/5J0VEPd.png)
Once the app has been started you can find this status of the control. 
Ensure that you have Aurora running, then you can connect to it clicking on **Connect to Aurora**.

When successfully connected, buttons are shown as follows:
![Control bar when connected to Aurora](https://i.imgur.com/vpUhxVZ.png)
Then you have 4 options:
1. **Generate Strip**: this generates the strip of the selected label in Aurora and shows it in the preview pane. **Remember that you have to select a label on Aurora prior hitting this key!** No print will be sent using this key. Useful when you're customizing a strip template and you just wand to take a look to this without any print cost.
2. **Print Selected Label (F9)**: This key will generate the strip as in step nr1, but also sends this to the printer. **If this is the first print request of the session, system print dialog will be shown.** Select you favorite printer and options, then the app stores them to reuse for every print request until you close the app. 
    
    **You can then minimize the App and simply printing your stip hitting F9 key from within Aurora.**

3. **Print with printer setup**: If you want to change printer preferences, you can use this button to recall system print dialog. **Then those new options are stored to be used with step 3 or F9 key as well.**

#### Settings
![Settings](https://i.imgur.com/iqmFOph.png)

Settings view is used to setup print zoom, margins, and sizes. 

The **area ICAO code filter** is used to truncate ICAO codes. Some real life systems are using a shortening of ICAO codes removing the local area ICAO code. 
For example, in Italy (LI) for LIRF on the strip is only reported **RF**. So you are able to request the app to remove **LI** using this field.
Every code that starts with something different than the value in this box will be left untouched.


## What if I want to collaborate?
 
This project uses [Syncfusion](https://www.syncfusion.com/) for PDF handling and printing strips. So if you need to build and run from your IDE this app, you need to grab a proper license key.

You can find an implementation of a custom ConfigProvider for IConfiguration abstractions in order to provide a license key into the config collecion of the app (Asp.Net Core like).
You just need to wrap your key in a static class for grabbing the key in `SyncfusionLicenseKeyProvider`.

**Remember to avoid committing your key on GitHiub!**
