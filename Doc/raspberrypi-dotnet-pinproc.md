
## Raspberry Pi Software Setup Notes

### Net5.0 Raspberry Pi
---

- Get a download link for Linux ARM32 https://dotnet.microsoft.com/en-us/download/dotnet/5.0 Runtime is 
- SSH into Raspberry PI. `cd Downloads` and download the link using the following curl
- `curl -SL -o dotnet.tar.gz {downloadlink}`
- Create dir for dotnet install `sudo mkdir -p /usr/share/dotnet`
- Extract download `sudo tar -zxf dotnet.tar.gz -C /usr/share/dotnet`
- Create link to use `dotnet`. `sudo ln -s /usr/share/dotnet/dotnet /usr/bin/dotnet`
- Test with `dotnet --list-runtimes` or `dotnet --list-sdks`

### Samba File share

---

- install samba `sudo apt-get install samba samba-common-bin`
- edit the config to add shares `sudo nano /etc/samba/smb.conf`

```
[procshare]
path = /home/pi
writeable=Yes
create mask=0777
directory mask=0777
public=no
```

- Ctrl X to save file
- Set password for share `sudo smbpasswd -a pi`
- Restart the samba service `sudo systemctl restart smbd`
- Windows: Map a network drive to the ip `//192.168.1.69/procshare`

### Publish Test App
---

- `dotnet publish -r linux-arm`
- Copy files to Pi and run with `./NetProcGameTest`

### Building PinProc on Pi

- Download libusb and libftdi as per instructions but place these in `/usr/include`
- `sudo apt-get install libusb-1.0-0-dev`
- Install guide and dependencies for libftdi https://github.com/nblock/libftdi/blob/master/README.build