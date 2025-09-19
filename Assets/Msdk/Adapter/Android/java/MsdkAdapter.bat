DirRoot
cd "MSDKUnityLibrary"
mkdir classes
javac -source 1.6 -target 1.6 -nowarn -encoding utf8 -cp "MSDKLibraryJar;AndroidSdkJar;UnityJar;BuglyJar" -d classes ./src/GamePackage/*.java ./src/GamePackage/wxapi/*.java ./src/com/tencent/msdk/adapter/*.java
cd classes
jar cvf MSDKUnityJar com
exit
