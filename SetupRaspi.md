# Setup Raspberry Pi
## Install OS
Download RaspberryPI OS32-bit Lite (previously called Raspbian) from officialraspberry.org site. Use a flash tool e.g. [balenaEtcher](https://www.balena.io/etcher/) flash your micro-SD card.

## SSH & Wifi
Enable SSH by placing an empty file with the name ssh (see subfolderfiles/ssh_wifi) to the mounted boot-partition of the previously flashed SD card with Raspberry PI OS Lite.

*The Next stepis only necessary if you connect to the network viaWifi. You can skipthis if you use wired ethernet only.*

Enable WIFI by placing a file with the name wpa_supplicant.conf (see subfolderfiles/ssh_wifi) and add the following content to the boot-partition of the previously flashed SD card with Raspberry PI OS Lite. Please enter you WIFI SSID and password to the corresponding lines.

```
ctrl_interface = DIR = /var/run/wpa_supplicant GROUP=netdev
update_config = 1
country = <Insert 2 letter ISO 3166-1 country code here>
network = {
ssid="yourSSID"
psk="yourPWD"
}
```
Please ensure that line breaks are "LF" and not "CRLF" (e.g. with NotePad++ or Visual Studio Code).

## Boot in Raspberry PI OS
Insert micro-SD card into the Raspberry Pi and let it boot. Find IP-address of the Pi and note it down.

## Install requirements

Connect to the Raspberry Pi over SSH with ```ssh pi@<IP>```. Default password is *raspberry*.

Install required packages via 

```pip python3 -m pip install --upgrade wiringpi python-osc```


## Create and copy files
Create a file called pwmout.py with the following content:

```
# Code originally writen by
# Tom David Eibich, contact: tom-david.eibich@h-brs.de
import argparse
import math
import time
import signal
import wiringpi

from pythonosc import dispatcher
from pythonosc import osc_server

port = 5005
PINS = [0,1,2,3,4,5,6]

wiringpi.wiringPiSetup()

for pin in PINS:
    wiringpi.pinMode(pin,1)
    wiringpi.softPwmCreate(pin,0,100)


def set_actuator(unused_addr, args, value):
    print("/actuator not implemented")

def set_actuators(unused_addr, *args):
    print(str(args[0]))
    for j in range(0, len(args)):
      for i in range(0, len(args[j]),2):
          if args[j][i] in PINS:
              wiringpi.softPwmWrite(args[j][i],args[j][i+1])

if __name__ == "__main__":
    parser = argparse.ArgumentParser()
    parser.add_argument("--ip",
        default="127.0.0.1", help="The ip to listen on")
    parser.add_argument("--port",
        type=int, default=5005, help="The port to listen on")
    args = parser.parse_args()

    dispatcher = dispatcher.Dispatcher()
    dispatcher.map("/actuators", set_actuators)

    server = osc_server.ThreadingOSCUDPServer(
        ("0.0.0.0", args.port), dispatcher)
    print("Serving on {}".format(server.server_address))
    server.serve_forever()
```

Copy pwmout.py to the home directory of user pi with the command:

```scp pwmout.py pi@<IP>:~/```

You can run the script with ```python3 ./pwmout.py``` inside the home directory.

## Add service to run script after boot

Connect via ssh. Create a file *pwmout.service* at location
```/etc/systemd/system/ ```

with the command:

```sudo nano /etc/systemd/system/pwmout.service```

Add the following content (change username and file location ifrequired):

```
[Unit]
Description=Python script with osc-listener for unity
After=network.target

[Service]
ExecStart=/usr/bin/python3 -u /home/pi/pwmout.py
WorkingDirectory=/home/pi
StandardOutput=inherit
StandardError=inherit
Restart=always
User=pi

[Install]
WantedBy=multi-user.target
```

*CTRL-X* and *Return* to save.

Enable service:

```sudo systemctl daemon-reload```
```sudo systemctl enable pwmout.service```
```sudo systemctl start pwmout.service```

Check status

```sudo systemctl status pwmout.service```

Reboot Pi and check if service is running.

## Troubleshoot
- Correct IP?
- Correct Port?
- Devices see each other? Try to ping.
- Vibration motors correct attached? Check wiring and circuit.
- Is the script running?

You are able to "debug" the script. Stop the service, if running

```sudo systemctl start pwmout.service```

Change the python-script according to your needs (prints etc) and run manually by python3 

```./pwmout.py```

Check the output.