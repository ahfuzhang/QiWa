#!/bin/bash

# Find the PID of the Http1EchoServer process
PID=$(pgrep -f "Http1EchoServer" | head -n 1)

if [ -z "$PID" ]; then
  echo "Http1EchoServer process not found."
  exit 1
fi

echo "Found Http1EchoServer process with PID: $PID"
echo "Starting CPU profiling for 30 seconds..."

# Collect trace using speedscope format for easy viewing
dotnet-trace collect -p $PID --duration 00:00:30 --format speedscope -o ./trace.speedscope.json

echo "Profiling complete. Trace saved to trace.speedscope.json"
echo "You can view it at: https://www.speedscope.app/"
