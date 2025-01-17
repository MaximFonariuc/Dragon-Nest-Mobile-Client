#!/bin/sh

git config --global credential.helper store

git log --name-status head...$1

if [ $? -ne 0 ];then
    echo $?
    exit
fi