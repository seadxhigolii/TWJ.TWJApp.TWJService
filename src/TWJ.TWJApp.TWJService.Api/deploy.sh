#!/bin/bash

# Define variables
SERVER_USER=ec2-user
SERVER_IP=52.202.73.100
PMK_FILE=/c/Users/Sead/Desktop/TWJ/AWS\ EC2\ Key/AWS-TWJ-Key.pmk
REMOTE_PATH=/var/www/twjapp

echo "Starting deployment..."

# Transfer the files
scp -i "$PMK_FILE" -r ./publish/* $SERVER_USER@$SERVER_IP:$REMOTE_PATH

# SSH into the server and restart the application
ssh -i "$PMK_FILE" $SERVER_USER@$SERVER_IP << EOF
   cd $REMOTE_PATH
   nohup dotnet TWJ.TWJApp.TWJService.Api.dll &
   exit
EOF

echo "Deployment finished!"
