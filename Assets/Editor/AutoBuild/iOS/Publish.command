#!/bin/sh

#-----------Config-----------#
UNITY_PATH=/Applications/Unity/Unity.app/Contents/MacOS/Unity
PROJECT_PATH=/Users/liming/Desktop/DN/dragon-nest/XProject
PACK_PATH=/Users/liming/Desktop/DN/dragon-nest/XProject/IOS/DragonNest_publish
IPA_NAME=DragonNest
CODE_SIGN_IDENTITY="iPhone Distribution: Shanghai Jiang You Information Technology Company Limited"
PROVISIONING_PROFILE_NAME="DragonNest"
VERSION=`cat /Users/liming/Desktop/DN/dragon-nest/XProject/Assets/Resources/ios-version.bytes`
#-----------Config-----------#

#echo "Git拉取资源"

#sh $PROJECT_PATH/Assets/Editor/AutoBuild/gitpull.sh

echo "清空Editor.log"

rm ~/Library/Logs/Unity/Editor.log

echo "将Unity导出成Xcode工程"

echo "ProjectPath:"/$PROJECT_PATH

$UNITY_PATH -projectPath $PROJECT_PATH -executeMethod SelectPlatformEditor.BuildiOSPublish -quit -batchmode

echo "Xcode工程生成完毕"

cd $PACK_PATH

xcodebuild clean -project Unity-iPhone.xcodeproj -configuration Release -alltargets CODE_SIGN_IDENTITY="$CODE_SIGN_IDENTITY"

xcodebuild archive -project Unity-iPhone.xcodeproj -scheme Unity-iPhone -archivePath Unity-iPhone.xcarchive CODE_SIGN_IDENTITY="$CODE_SIGN_IDENTITY"

xcodebuild -exportArchive -archivePath Unity-iPhone.xcarchive -exportPath ${PACK_PATH}/longzhigu_ios_sdo_${VERSION}.ipa -exportFormat ipa CODE_SIGN_IDENTITY=$CODE_SIGN_IDENTITY -exportProvisioningProfile "$PROVISIONING_PROFILE_NAME"

echo "ipa完毕"

echo "调用分析脚本"

python ${PROJECT_PATH}/Assets/Editor/ResourceStatistics/ResourceStatistics.py

open $PACK_PATH

if [[ $1 == "1" ]]; then
	echo "Start Upload"
elif [[ $1 == "0" ]]; then
	echo "Upload ipa to FtpServer(y/n) Or wait 60 seconds to don't upload:"
	UploadToFtpServer = "n"
	read -t 60 UploadToFtpServer
	if [[ "$UploadToFtpServer" != "y" && "$UploadToFtpServer" != "Y" ]];then
	    exit
	fi
else
	echo "Upload ipa to FtpServer(y/n) Or wait 60 seconds to automatically upload:"
	UploadToFtpServer = "y"
	read -t 60 UploadToFtpServer
	if [[ "$UploadToFtpServer" != "y" && "$UploadToFtpServer" != "Y" ]];then
	    exit
	fi
fi

sed 's/@version@/sdo_${VERSION}/g' /Users/liming/longzhigu_ios_version.plist > ${PACK_PATH}/longzhigu_ios_sdo_${VERSION}.plist

ftp -n 10.1.3.120 <<END
user gameapk 123u123u.gameapk
binary
cd longzhigu
prompt
mput longzhigu_ios_sdo_${VERSION}.ipa
mput longzhigu_ios_sdo_${VERSION}.plist
bye
END

cd ${PROJECT_PATH}/Assets/Bundle/IOS

ftp -n 42.62.22.46 2121 <<END
user anonymous 123
binary
cd Patch/Live/IOS
prompt
mput manifest.assetbundle
bye
END
