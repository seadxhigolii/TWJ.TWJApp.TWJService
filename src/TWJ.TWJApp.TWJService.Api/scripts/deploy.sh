#!/bin/bash

# Define variables
SERVER_USER=ec2-user
SERVER_IP=52.202.73.100
PEM_FILE="$HOME/.ssh/aws-key.pem"  # Path where the PEM file will be saved from GitHub secrets
REMOTE_PATH=/var/www/twjapp
LOCAL_PUBLISH_PATH="./src/TWJ.TWJApp.TWJService.Api/bin/Release/net6.0/publish"

echo "Starting deployment..."

# Ensure the publish directory exists
if [ ! -d "$LOCAL_PUBLISH_PATH" ]; then
  echo "Error: The publish directory does not exist."
  exit 1
fi

# Transfer the files
scp -i "$PEM_FILE" -r "$LOCAL_PUBLISH_PATH"/* $SERVER_USER@$SERVER_IP:$REMOTE_PATH

# SSH into the server and restart the application
ssh -i "$PEM_FILE" $SERVER_USER@$SERVER_IP << EOF
   cd $REMOTE_PATH
   nohup dotnet TWJ.TWJApp.TWJService.Api.dll > output.log 2>&1 &
   exit
EOF

echo "Deployment finished!"
