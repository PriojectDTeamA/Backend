#!/bin/bash

cd $1
echo --start of output--
docker run --rm $2 2>&1
echo --end of output--
docker rmi $2