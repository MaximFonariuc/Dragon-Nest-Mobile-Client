#!/bin/sh

git config --global credential.helper store

git -c diff.mnemonicprefix=false -c core.quotepath=false reset -q --hard HEAD --

if [ $? -ne 0 ];then
    echo "error"
    exit
fi

echo $?