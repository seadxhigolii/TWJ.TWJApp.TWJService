#!/bin/bash

# Define variables
SERVER_USER=ec2-user
SERVER_IP=52.202.73.100
PEM_FILE="$HOME/.ssh/aws-key.pem"  # Path where the PEM file will be saved from GitHub secrets
REMOTE_PATH=/var/www/twjapp
LOCAL_PUBLISH_PATH="./src/TWJ.TWJApp.TWJService.Api/bin/Release/net6.0/publish"

echo "Starting deployment..."

# Ensure the publish directory exists and has files
if [ ! -d "$LOCAL_PUBLISH_PATH" ]; then
  echo "Error: The publish directory does not exist."
  exit 1
fi

# Transfer the files from the correct publish directory
scp -i "$PEM_FILE" -r "$LOCAL_PUBLISH_PATH/*" $SERVER_USER@$SERVER_IP:$REMOTE_PATH

# SSH into the server to stop the running process and restart the app
ssh -i "$PEM_FILE" $SERVER_USER@$SERVER_IP << EOF
   # Find the process ID and kill it
   PID=\$(ps aux | grep 'dotnet TWJ.TWJApp.TWJService.Api.dll' | grep -v grep | awk '{print \$2}')
   if [ ! -z "\$PID" ]; then
     kill -9 \$PID
     echo "Stopped running application."
   fi

   # Navigate to the application directory
   cd $REMOTE_PATH

   # Start the application in the background
   nohup dotnet TWJ.TWJApp.TWJService.Api.dll > output.log 2>&1 &
   echo "Restarted application."
   exit
EOF

echo "Deployment finished!"