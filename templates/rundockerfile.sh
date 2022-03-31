#!/bin/bash

cd $1
docker build -t "session" . 
echo starting output here --
docker run --rm "session"