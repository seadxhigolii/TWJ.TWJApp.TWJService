#!/bin/bash

# Define variables
SERVER_USER=ec2-user
SERVER_IP=52.202.73.100
PEM_FILE="/c/Users/Sead/Desktop/TWJ/AWS EC2 Key/AWS-TWJ-Key.pem"
REMOTE_PATH=/var/www/twjapp
LOCAL_PUBLISH_PATH="/c/Sources/thewellnessjunction/bin/Release/net6.0/publish"

echo "LOCAL_PUBLISH_PATH: $LOCAL_PUBLISH_PATH"
echo "Starting deployment..."

# Ensure the publish directory exists and has files
if [ ! -d "$LOCAL_PUBLISH_PATH" ]; then
  echo "Error: The publish directory does not exist."
  exit 1
fi

# Transfer the files and directories from the publish folder (without using dots)
scp -i "$PEM_FILE" -r "$LOCAL_PUBLISH_PATH/"* $SERVER_USER@$SERVER_IP:$REMOTE_PATH

# SSH into the server and restart the application in the background
ssh -i "$PEM_FILE" $SERVER_USER@$SERVER_IP << EOF
   cd $REMOTE_PATH
   nohup dotnet TWJ.TWJApp.TWJService.Api.dll > output.log 2>&1 &
   exit
EOF

echo "Deployment finished!"
