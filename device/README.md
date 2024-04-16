
## Device

### Fabrics and Electronics
TODO - device description

### Setting up the raspberrypi

Install the required dependencies system wide on the raspberry and configure it to boot on CLI with no password to login.

```bash
sudo apt install python3-hid python3-aiohttp python3-asyncio
sudo raspi-config # Make sure to select the CLI+no-password option. 
```

Make the project boot on startup. Make sure the raspberrypi connects itself to your local network with a fixed IP. 

```bash
echo "\n\n\n" >> ~/.bashrc
echo "cd <path-to-the-project>" >> ~/.bashrc
echo "sudo python main.py" >> ~/.bashrc
```

Run `source ~/.bashrc` to launch the server. Next time the raspberry will boot, the server will start automatically.

## Debug

A debug HTML page is available on port 8001 of the server. You can also run the [analytics app](../analytics-app/README.md) to get a more advanced tool. 