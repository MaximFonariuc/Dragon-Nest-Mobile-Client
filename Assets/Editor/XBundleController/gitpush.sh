#!/bin/sh

git config --global credential.helper store

path=`pwd`
git add ${path}/Bundle
if [ $? -ne 0 ];then
    echo $?
    exit
fi

git add ${path}/Resources/ios-version.bytes
git add ${path}/Resources/android-version.bytes
git add ${path}/ABSystem
git add ${path}/AssetBundles

git commit -m 'push version '${1}
if [ $? -ne 0 ];then
    echo $?
    exit
fi

git tag -a $1 -m 'v'${1}
if [ $? -ne 0 ];then
    echo $?
    exit
fi

current=`git rev-parse --abbrev-ref HEAD`
git push origin $current
if [ $? -ne 0 ];then
    echo $?
    exit
fi

git push origin --tags
if [ $? -ne 0 ];then
    echo $?
    exit
fi