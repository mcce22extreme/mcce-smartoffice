# GrovePi sensors and AWS IoT Core

## Setup
Use legacy Raspbian on RPI 1 with minimum 8GB SD card.

```bash
cd aws-iot/
python3 -m pip install ./aws-iot-device-sdk-python-v2
python3 -m pip install gpiozero
curl -kL dexterindustries.com/update_grovepi | bash -s -- --user-local --bypass-gui-installation
git clone https://github.com/DexterInd/GrovePi.git
cd GrovePi/Software/Python/
python3 -m pip install numpy
sudo python setup.py install
sudo python3 setup.py install
```

## Example commands
```bash
cd ~/aws-iot/
python3 aws-iot-device-sdk-python-v2/samples/pubsub.py --endpoint ahnd3bakiu5r9-ats.iot.us-east-1.amazonaws.com --ca_file root-CA.crt --cert raspberryTemp.cert.pem --key raspberryTemp.private.key --client_id basicPubSub --topic mcce22-smart-office --count 0
python3 aws-iot-device-sdk-python-v2/samples/pubsub.py --endpoint a1635o3379l6to-ats.iot.us-east-1.amazonaws.com --ca_file root-CA.crt --cert 4651112b375c1cd03485c96e089d62510f8d0783806aaf7306a43abf13e7d366-certificate.pem.crt --key 4651112b375c1cd03485c96e089d62510f8d0783806aaf7306a43abf13e7d366-private.pem.key --client_id basicPubSub --topic mcce22-smart-office --count 0
```
