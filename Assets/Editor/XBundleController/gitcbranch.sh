#!/bin/sh

git config --global credential.helper store

current=`git rev-parse --abbrev-ref HEAD`

if [ $? -ne 0 ];then
    echo "error"
    exit
fi

echo $current