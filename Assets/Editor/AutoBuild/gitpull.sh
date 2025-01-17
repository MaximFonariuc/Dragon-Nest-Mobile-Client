#!/bin/sh
cd ~/Desktop/DN/dragon-nest

git config --global credential.helper store

git branch -a

echo Input checkout branch name or Input enter continue:
read BRANCH

if [ "$BRANCH" == "" ];then
	git checkout $BRANCH
    if [ $? -ne 0 ];then
        msg="Checkout branch $BRANCH failed."
        echo $msg
        exit
    fi
fi

git pull
if [ $? -ne 0 ];then
    msg="Pull branch $BRANCH failed."
    echo $msg
    exit
fi
