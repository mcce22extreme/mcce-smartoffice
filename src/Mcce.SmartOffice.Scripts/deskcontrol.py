import argparse
import paho.mqtt.client as mqtt
import sys
import threading
import os
import time
from uuid import uuid4
import json
from gpiozero import CPUTemperature
import subprocess

#from grove_dht import Dht # from a custom made grovepi-based library import our needed class
import datetime # that's for printing the current date

g_userdata = {
    "received_count": 0,
    "sent_count": 0,
}

curdir = os.path.dirname(os.path.abspath(__file__))

# Bash script paths
set_desk_script = curdir + "/bash_scripts/move_desk.sh"
set_picture_script = curdir + "/bash_scripts/display_picture.sh"

black_picture_url = "https://www.solidbackgrounds.com/images/1920x1080/1920x1080-black-solid-color-background.jpg"

print ("setting script paths to\n" + set_desk_script + "\n" + set_picture_script)

def on_connect(client, userdata, flags, rc):
    if rc == 0:
        print("Connected successfully.")
    else:
        print(f"Connect returned result code: {rc}")

def on_message(client, userdata, message):
    global g_userdata
    global args
    
    # Decoding the message payload
    payload = message.payload.decode()
    topic = message.topic
    print(f"Received message '{payload}' on topic '{topic}'")

    # Basic error handling for JSON loading
    try:
        data = json.loads(payload)
    except json.JSONDecodeError as e:
        print(f"Failed to decode JSON: {e}")
        return

    # Increment the received count
    g_userdata["received_count"] += 1
    
    # Check if the received count has reached its maximum value
    if 0 < g_userdata["max_rcvcount"] <= g_userdata["received_count"]:
        client.loop_stop()
        
    # Check if the topic ends with 'activate/userimages'
    if topic.endswith('activate/userimages'):
        # Make sure the 'WorkspaceNumber' key exists and has the correct value
        #if data.get('WorkspaceNumber') == 'workspace-002':
            # Extract image URLs and handle potential issues
            try:
                cmd = [set_picture_script] + data['UserImages']
                print (cmd)
                if (args.action):
                    subprocess.Popen(cmd)
            except KeyError as e:
                print(f"Key error: {e} - Check the structure of 'UserImages'")
            except TypeError as e:
                print(f"Type error: {e} - 'UserImages' must be a list of dictionaries with 'Url' keys")
            except subprocess.SubprocessError as e:
                print(f"Subprocess error: {e} - Failed to execute shell script")
    elif topic.endswith('activate/workspaceconfiguration'):
        try:
            deskHeight = data['DeskHeight']
            cmd = [set_desk_script, str(deskHeight)]
            print (cmd)
            if (args.action):
                subprocess.Popen(cmd)
        except KeyError as e:
            print(f"Key error: {e} - Check the structure of 'DeskHeight'")
        except subprocess.SubprocessError as e:
            print(f"Subprocess error: {e} - Failed to execute shell script")

def main():
    global g_userdata
    global args

    parser = argparse.ArgumentParser(description="MQTT publisher and subscriber")
    parser.add_argument("--endpoint", required=True, help="MQTT broker endpoint")
    parser.add_argument("--username", required=True, help="MQTT broker username")
    parser.add_argument("--password", required=True, help="MQTT broker password")
    parser.add_argument("--client_id", default="mcce-smart-office-python", help="Client ID for MQTT session")
    parser.add_argument("--basetopic", default="mcce-smartoffice", help="Base-topic to subscribe to and publish on")
    parser.add_argument("--workspace", default="workspace-001", help="Workspace number to subscribe to and publish on")
    parser.add_argument("--rcvcount", type=int, default=0, help="Number of messages to receive (0 for infinite loop)")
    parser.add_argument("--count", type=int, default=0, help="Number of messages to receive (0 for infinite loop)")
    parser.add_argument("--action", type=int, default=1, help="Do action, like showing a picture or setting the height of the desk")

    args = parser.parse_args()

    g_userdata["max_rcvcount"] = args.rcvcount
    g_userdata["max_sndcount"] = args.count

    #dht_pin = 7 # use Digital Port 4 found on GrovePi
    #dht_sensor = Dht(dht_pin) # instantiate a dht class with the appropriate pin

    #dht_sensor.start() # start collecting from the DHT sensor

    client = mqtt.Client(client_id=args.client_id)
    client.username_pw_set(args.username, args.password)
    client.on_connect = on_connect
    client.on_message = on_message

    client.connect(args.endpoint)

    client.loop_start()

    mqttSubscribe = [(args.basetopic+"/workspace/"+args.workspace+"/activate/userimages",0),
                     (args.basetopic+"/workspace/"+args.workspace+"/activate/workspaceconfiguration",0)]
    mqttPublish = args.basetopic+"/dataingress"

    print("Subscribing to: ")
    print(*mqttSubscribe, sep=", ")
    print("Send data to " + mqttPublish)

    if (args.action):
        cmd = [set_picture_script, black_picture_url]
        subprocess.Popen(cmd)

    client.subscribe(mqttSubscribe)

    cpu = CPUTemperature()
    message_string = "unused"
    temperature_old = 0
    humidity_old = 0

    try:
        while (g_userdata["sent_count"] < g_userdata["max_sndcount"]) or (g_userdata["max_sndcount"] == 0):
            #temperature, humidity = dht_sensor.feedMe() # try to read values

            temperature = cpu.temperature
            humidity = 0
            # if any of the read values is a None type, then it means there're no available values
            if not temperature is None:
    #            string += '[temperature = {:.01f}][humidity = {:.01f}]'.format(temperature, humidity)
                if (abs(temperature_old - temperature) > 1 or abs(humidity_old - humidity) > 1):
                    temperature_old = temperature
                    humidity_old = humidity
                    custommsg = ("{\n"
                                "  \"WorkspaceNumber\": " + "{}".format("\""+args.workspace+"\"") + ",\n"
                                "  \"Temperature\": " + "{:.01f}".format(23.2) + ",\n"
                                "  \"NoiseLevel\": " + "{:.01f}".format(20) + ",\n"
                                "  \"Co2Level\": " + "{:.01f}".format(700) + ",\n"
                                "  \"Humidity\": " + "{:.01f}".format(70) + "\n"
                                "}"
                                )
                    print(custommsg)

                    message_json = custommsg
                    client.publish(mqttPublish, message_json)

                    g_userdata["sent_count"] += 1
            time.sleep(1)
    except KeyboardInterrupt:
        print("Interrupted by user. Disconnecting from broker...")

    print("Disconnecting...")

    client.loop_stop()

#    dht_sensor.stop() # stop gathering data

    client.disconnect()

if __name__ == "__main__":
    main()
