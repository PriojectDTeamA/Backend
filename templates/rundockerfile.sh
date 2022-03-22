#!/bin/bash

cd $1
docker build -t "session" . 
docker run --rm "session"