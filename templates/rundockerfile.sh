#!/bin/bash

cd $1
docker build -t $2 . 
echo --start of output--
docker run --rm $2
echo --end of output--
docker rmi $2