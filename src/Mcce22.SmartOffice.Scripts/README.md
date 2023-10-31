# IoT device scripts

To control the desk and the picture frame, as well as sending sensor data, call the deskcontrol.py script.

Make sure to call it from a terminal within the X-session to make showing the pictures work.

Example:
```bash
python3 deskcontrol.py --endpoint mqtt-mcce22extreme.westeurope.cloudapp.azure.com --username myusername --password mypassword --workspace workspace-002

# set "--action 0" to disable calling the scripts for local changes. For testing purposes
python3 deskcontrol.py --endpoint mqtt-mcce22extreme.westeurope.cloudapp.azure.com --username myusername --password mypassword --workspace workspace-002 --action 0
```
