# CPU Shutdown

### CpuShutdown.Service

Windows service that polls the CPU temperature and sends data to a system tray application. It also shuts down the system when a critical temperature is reached. Since this is a Windows service, even if there is no logged user, the system is protected in the event of a CPU cooler failure.

### CpuShutdown.UI.Tray

System tray application that receives data from the Windows service and displays a colored icon to inform the user of the CPU temperature status. It also displays a notification when the temperature is close to a system shutdown.

### CpuShutdown.UI.Settings

Program to change application settings.

| Setting              | Description                                                                                                |
|----------------------|------------------------------------------------------------------------------------------------------------|
| Polling Interval     | CPU temperature reading interval                                                                           |
| Yellow Temperature   | Temperature that displays a yellow icon in the system tray                                                 |
| Red Temperature      | Temperature that displays a red icon in the system tray                                                    |
| Alert Temperature    | Temperature that displays a notification in the system tray                                                |
| Alert Hysteresis     | Temperature delta below the alert temperature for a new notification to be displayed                       |
| Shutdown Temperature | Temperature that shuts down the system                                                                     |
| Shutdown Timeout     | Timeout period before the system shutdown                                                                  |

### CpuShutdown

Library that wraps the main functionalities of the application.

### OpenHardwareMonitorLib

For now, this is the library used to read the temperature of the CPU. Finding a signed and free driver to use doesn't give much of a choice.
This library uses WinRing0x64.sys to read the temperature but this driver is no longer maintained. Be warned.

https://github.com/openhardwaremonitor
