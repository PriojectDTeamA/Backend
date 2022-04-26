#!/bin/bash

cd $1
docker build -t $2 . 
echo built file!