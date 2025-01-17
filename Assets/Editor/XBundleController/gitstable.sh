#!/bin/sh

git config --global credential.helper store

git checkout -b $1

if [ $? -ne 0 ];then
    echo $?
    exit
fi

git push -v --set-upstream origin $1:$1

if [ $? -ne 0 ];then
    echo $?
    exit
fi

cd ../../../src/

git checkout -b $1

if [ $? -ne 0 ];then
    echo $?
    exit
fi

git push -v --set-upstream origin $1:$1

if [ $? -ne 0 ];then
    echo $?
    exit
fi