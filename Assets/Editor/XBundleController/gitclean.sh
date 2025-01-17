#!/bin/sh

git config --global credential.helper store

git status

if [ $? -ne 0 ];then
    echo $?
    exit
fi

echo $?