#!/bin/sh

git config --global credential.helper store

git -c diff.mnemonicprefix=false -c core.quotepath=false fetch origin

if [ $? -ne 0 ];then
    echo "error"
    exit
fi

git -c diff.mnemonicprefix=false -c core.quotepath=false pull origin $1

if [ $? -ne 0 ];then
    echo "error"
    exit
fi