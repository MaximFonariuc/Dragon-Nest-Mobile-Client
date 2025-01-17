#!/bin/sh

git config --global credential.helper store

cd ../../../src/
git tag -a $1 -m 'v'${1}

if [ $? -ne 0 ];then
    echo $?
    exit
fi

git push origin --tags