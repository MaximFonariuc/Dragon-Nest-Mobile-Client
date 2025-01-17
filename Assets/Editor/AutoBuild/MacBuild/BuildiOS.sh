#!/bin/sh

#-----------Config-----------#
UNITY_PATH=/Applications/Unity/Unity.app/Contents/MacOS/Unity
PROJECT_PATH=/Users/liming/Desktop/DN/dragon-nest/XProject
#-----------Config-----------#

echo "将Unity导出成Xcode工程"

echo "ProjectPath:"/$PROJECT_PATH

$UNITY_PATH -projectPath $PROJECT_PATH -executeMethod SelectPlatformEditor.ShellBuildiOS -quit -batchmode

echo "Xcode工程生成完毕"