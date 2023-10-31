#!/bin/bash

# Check if feh is installed
command -v feh >/dev/null 2>&1 || { echo >&2 "Please install 'feh' to run this script."; exit 1; }

# Check if a web link parameter is provided
if [ $# -eq 0 ]; then
    echo "Please provide a web link to a picture as a parameter."
    exit 1
fi

# Check if a previous picture is displayed
if pgrep -x feh >/dev/null; then
    # Kill the previous feh process
    pkill -x feh

    # Wait for the process to exit
    sleep 1
fi

echo "Parameters $@"
# Make a temporary directory to store images
tmpdir=$(mktemp -d)

echo "created temp dir $tmpdir"

# Loop over all arguments
for url in "$@"; do
    # Generate a random filename
    filename=$(mktemp -p "$tmpdir")

    # Download the image
    curl -s "$url" -o "$filename"
done

ls -la $tmpdir

# Display the new picture in fullscreen with a transition effect using feh
feh --auto-zoom --fullscreen --quiet --hide-pointer --image-bg black --zoom fill --no-menus --slideshow-delay 5 --no-fehbg "$tmpdir"/*

# Remove the temporary directory
rm -r "$tmpdir"
