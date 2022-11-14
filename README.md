# Ivao Italy - Aurora FlightStrip Printer

Print your flight strips directly from Aurora!
A cheap POS thermal printer is strongly recommended. Prints with no costs and is really cheap to buy! And also, with a little effort you can print on a correct size paper.

## How the strip data binding works?
Look into "templates" folder, there are a few files:
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
