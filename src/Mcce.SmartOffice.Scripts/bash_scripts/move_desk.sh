#!/bin/bash

echo "Bluetooth-Table Script started"

target_height=$1

if [[ $target_height -gt 1280 ]]
then target_height=1280
fi

if [[ $target_height -lt 620 ]]
then target_height=620
fi

echo "Target Height: $target_height"

idasen-controller --move-to $target_height

echo "Moving complete"



