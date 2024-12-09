#!/bin/bash

SERVER_USER=ec2-user
SERVER_IP=44.212.18.216
PEM_FILE="$HOME/.ssh/aws-key.pem"
REMOTE_PATH=/var/www/twjapp
LOCAL_PUBLISH_PATH="./downloaded-artifacts/publish-folder"

echo "Starting deployment..."

if [ ! -d "$LOCAL_PUBLISH_PATH" ]; then
  echo "Error: The publish directory does not exist."
  exit 1
fi

if [ -z "$(ls -A $LOCAL_PUBLISH_PATH)" ]; then
  echo "Error: The publish directory is empty."
  exit 1
fi

echo "Listing files in $LOCAL_PUBLISH_PATH:"
ls -l $LOCAL_PUBLISH_PATH

ssh-keyscan -H $SERVER_IP >> ~/.ssh/known_hosts

scp -i "$PEM_FILE" -r $LOCAL_PUBLISH_PATH/* $SERVER_USER@$SERVER_IP:$REMOTE_PATH/

ssh -i "$PEM_FILE" $SERVER_USER@$SERVER_IP "ls -l $REMOTE_PATH"

ssh -i "$PEM_FILE" $SERVER_USER@$SERVER_IP << EOF
   # Find the process ID and kill it
   PID=\$(ps aux | grep 'dotnet TWJ.TWJApp.TWJService.Api.dll' | grep -v grep | awk '{print \$2}')
   if [ ! -z "\$PID" ]; then
     kill -9 \$PID
     echo "Stopped running application."
   fi

   # Navigate to the application directory
   cd $REMOTE_PATH

   # Set the environment variable for Selenium Manager
   export SELENIUM_MANAGER_CACHE=/var/selenium

   # Check if the DLL exists before trying to run it
   if [ -f "TWJ.TWJApp.TWJService.Api.dll" ]; then
       # Start the application in the background
       nohup dotnet TWJ.TWJApp.TWJService.Api.dll > output.log 2>&1 &
       echo "Restarted application."
   else
       echo "Error: TWJ.TWJApp.TWJService.Api.dll not found in $REMOTE_PATH"
       exit 1
   fi
EOF

echo "Deployment finished!"
