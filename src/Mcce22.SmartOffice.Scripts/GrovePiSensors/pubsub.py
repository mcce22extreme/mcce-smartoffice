# Copyright Amazon.com, Inc. or its affiliates. All Rights Reserved.
# SPDX-License-Identifier: Apache-2.0.

from awscrt import mqtt
import sys
import threading
import time
from uuid import uuid4
import json
from gpiozero import CPUTemperature
import subprocess

from grove_dht import Dht # from a custom made grovepi-based library import our needed class
import datetime # that's for printing the current date

dht_pin = 7 # use Digital Port 4 found on GrovePi
dht_sensor = Dht(dht_pin) # instantiate a dht class with the appropriate pin

dht_sensor.start() # start collecting from the DHT sensor

# This sample uses the Message Broker for AWS IoT to send and receive messages
# through an MQTT connection. On startup, the device connects to the server,
# subscribes to a topic, and begins publishing messages to that topic.
# The device should receive those same messages back from the message broker,
# since it is subscribed to that same topic.

# Parse arguments
import utils.command_line_utils as command_line_utils
cmdUtils = command_line_utils.CommandLineUtils("PubSub - Send and recieve messages through an MQTT connection.")
cmdUtils.add_common_mqtt_commands()
cmdUtils.add_common_topic_message_commands()
cmdUtils.add_common_proxy_commands()
cmdUtils.add_common_logging_commands()
cmdUtils.register_command("key", "<path>", "Path to your key in PEM format.", True, str)
cmdUtils.register_command("cert", "<path>", "Path to your client certificate in PEM format.", True, str)
cmdUtils.register_command("port", "<int>", "Connection port. AWS IoT supports 443 and 8883 (optional, default=auto).", type=int)
cmdUtils.register_command("client_id", "<str>", "Client ID to use for MQTT connection (optional, default='test-*').", default="test-" + str(uuid4()))
cmdUtils.register_command("count", "<int>", "The number of messages to send (optional, default='10').", default=10, type=int)
cmdUtils.register_command("is_ci", "<str>", "If present the sample will run in CI mode (optional, default='None')")
# Needs to be called so the command utils parse the commands
cmdUtils.get_args()

received_count = 0
received_all_event = threading.Event()
is_ci = cmdUtils.get_command("is_ci", None) != None

# Callback when connection is accidentally lost.
def on_connection_interrupted(connection, error, **kwargs):
    print("Connection interrupted. error: {}".format(error))


# Callback when an interrupted connection is re-established.
def on_connection_resumed(connection, return_code, session_present, **kwargs):
    print("Connection resumed. return_code: {} session_present: {}".format(return_code, session_present))

    if return_code == mqtt.ConnectReturnCode.ACCEPTED and not session_present:
        print("Session did not persist. Resubscribing to existing topics...")
        resubscribe_future, _ = connection.resubscribe_existing_topics()

        # Cannot synchronously wait for resubscribe result because we're on the connection's event-loop thread,
        # evaluate result with a callback instead.
        resubscribe_future.add_done_callback(on_resubscribe_complete)


def on_resubscribe_complete(resubscribe_future):
        resubscribe_results = resubscribe_future.result()
        print("Resubscribe results: {}".format(resubscribe_results))

        for topic, qos in resubscribe_results['topics']:
            if qos is None:
                sys.exit("Server rejected resubscribe to topic: {}".format(topic))


# Callback when the subscribed topic receives a message
def on_message_received(topic, payload, dup, qos, retain, **kwargs):
    print("Received message from topic '{}': {}".format(topic, payload))

    data = json.loads(payload)
    if data['WorkspaceNumber'] == 'workspace-002':
        cmd = ['/home/pi/display_picture.sh'] + data['UserImageUrls']
        subprocess.Popen(cmd)

    global received_count
    received_count += 1
    if received_count == cmdUtils.get_command("count"):
        received_all_event.set()

if __name__ == '__main__':
    mqtt_connection = cmdUtils.build_mqtt_connection(on_connection_interrupted, on_connection_resumed)

    if is_ci == False:
        print("Connecting to {} with client ID '{}'...".format(
            cmdUtils.get_command(cmdUtils.m_cmd_endpoint), cmdUtils.get_command("client_id")))
    else:
        print("Connecting to endpoint with client ID")
    connect_future = mqtt_connection.connect()

    # Future.result() waits until a result is available
    connect_future.result()
    print("Connected!")

    cpu = CPUTemperature()
    message_count = cmdUtils.get_command("count")
    message_topic = cmdUtils.get_command(cmdUtils.m_cmd_topic)
#    message_string = cmdUtils.get_command(cmdUtils.m_cmd_message)
    message_string = "unused"
    temperature_old = 0
    humidity_old = 0

    # Subscribe
    print("Subscribing to topic '{}'...".format(message_topic))
    subscribe_future, packet_id = mqtt_connection.subscribe(
        topic=message_topic+"/activate",
        qos=mqtt.QoS.AT_LEAST_ONCE,
        callback=on_message_received)

    subscribe_result = subscribe_future.result()
    print("Subscribed with {}".format(str(subscribe_result['qos'])))


#    demomsg='{"WorkspaceNumber":"workspace-002","UserId":"n7mEXnzoc1ga","BookingId":"C8ac--1Kxhwv","DeskHeight":90,"UserImageUrls":["https://mcce-smart-office-userimage-bbvd.s3.amazonaws.com/n7mEXnzoc1ga/3c0c86d1-e76b-4bc2-bf57-ff0a27b2c868.jpg","https://mcce-smart-office-userimage-bbvd.s3.amazonaws.com/n7mEXnzoc1ga/75f826d2-d3f4-4e9e-8ee0-5798105ca274.jpg","https://mcce-smart-office-userimage-bbvd.s3.amazonaws.com/n7mEXnzoc1ga/47b1ec1b-d2dd-45cd-9690-3785226a970c.jpg"]}'
    demomsg='{"WorkspaceNumber":"workspace-002","UserId":"n7mEXnzoc1ga","BookingId":"C8ac--1Kxhwv","DeskHeight":90,"UserImageUrls":["https://cdn.shopify.com/s/files/1/0071/8946/3091/files/adolescent-dog-with-stick.jpg","https://www.rd.com/wp-content/uploads/2021/03/GettyImages-1133605325-scaled-e1617227898456.jpg","https://hips.hearstapps.com/wdy.h-cdn.co/assets/17/39/1600x1066/gallery-1506709524-cola-0247.jpg"]}'
    mqtt_connection.publish(
        topic=message_topic+"/activate",
        payload=demomsg,
        qos=mqtt.QoS.AT_LEAST_ONCE)

    # Publish message to server desired number of times.
    # This step is skipped if message is blank.
    # This step loops forever if count was set to 0.
    if message_string:
        if message_count == 0:
            print ("Sending messages until program killed")
        else:
            print ("Sending {} message(s)".format(message_count))

        publish_count = 1
        while (publish_count <= message_count) or (message_count == 0):
#            message = "{} [{}]".format(message_string, publish_count)

            temperature, humidity = dht_sensor.feedMe() # try to read values

            # if any of the read values is a None type, then it means there're no available values
            if not temperature is None:
    #            string += '[temperature = {:.01f}][humidity = {:.01f}]'.format(temperature, humidity)


                if (abs(temperature_old - temperature) > 1 or abs(humidity_old - humidity) > 1):
                    temperature_old = temperature
                    humidity_old = humidity
                    #print("Publishing message to topic '{}': {}".format(message_topic+"/dataingress/pi1black/temp", temperature))
                    #message_json = json.dumps(temperature)
                    #mqtt_connection.publish(
                    #    topic=message_topic+"/pi1black/temp",
                    #    payload=message_json,
                    #    qos=mqtt.QoS.AT_LEAST_ONCE)
                    #print("Publishing message to topic '{}': {}".format(message_topic+"/dataingress/pi1black/hum", humidity))
                    #message_json = json.dumps(humidity)
                    #mqtt_connection.publish(
                    #    topic=message_topic+"/pi1black/hum",
                    #    payload=message_json,
                    #    qos=mqtt.QoS.AT_LEAST_ONCE)

                    custommsg = ("{\n"
                                "  \"WorkspaceNumber\": " + "{}".format("\"workspace-001\"") + ",\n"
                                "  \"Temperature\": " + "{:.01f}".format(temperature) + ",\n"
                                "  \"NoiseLevel\": " + "{:.01f}".format(20) + ",\n"
                                "  \"Co2Level\": " + "{:.01f}".format(700) + ",\n"
                                "  \"Humidity\": " + "{:.01f}".format(humidity) + "\n"
                                "}"
                                )
                    print(custommsg)

                    message_json = custommsg
                    mqtt_connection.publish(
                        topic=message_topic+"/dataingress",
                        payload=message_json,
                        qos=mqtt.QoS.AT_LEAST_ONCE)

                    publish_count += 1
            time.sleep(1)

    # Wait for all messages to be received.
    # This waits forever if count was set to 0.
    if message_count != 0 and not received_all_event.is_set():
        print("Waiting for all messages to be received...")

    received_all_event.wait()
    print("{} message(s) received.".format(received_count))

    # Disconnect
    print("Disconnecting...")
    dht_sensor.stop() # stop gathering data
    disconnect_future = mqtt_connection.disconnect()
    disconnect_future.result()
    print("Disconnected!")

#except KeyboardInterrupt:
#    dht_sensor.stop() # stop gathering data
