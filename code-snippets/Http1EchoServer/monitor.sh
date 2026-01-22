#!/bin/bash

# Find the PID of the Http1EchoServer process
PID=$(pgrep -f "Http1EchoServer" | head -n 1)

if [ -z "$PID" ]; then
  echo "Http1EchoServer process not found."
  exit 1
fi

echo "Found Http1EchoServer process with PID: $PID"
echo "Monitoring counters..."

# Monitor relevant counters
dotnet-counters monitor -p $PID --counters System.Runtime,Microsoft.AspNetCore.Hosting,Microsoft.AspNetCore.Http.Connections,Microsoft.AspNetCore.Server.Kestrel,Http1EchoServer
