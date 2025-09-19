D:
cd "D:\Git\SoloProjects\Dragon-Nest-Mobile-Client\Assets/Msdk/Editor/Temp/java"
mkdir classes
javac -source 1.6 -target 1.6 -nowarn -encoding utf8 -cp "D:\Git\SoloProjects\Dragon-Nest-Mobile-Client\Assets\Msdk\Editor\Librarys\Android3.2/MSDKLibrary/libs\MSDK_Android_3.3.30a_6.jar;C:\Program Files\Unity\Hub\Editor\2022.3.50f1\Editor\Data\PlaybackEngines\AndroidPlayer\SDK/platforms\android-35\android.jar;D:\Git\SoloProjects\Dragon-Nest-Mobile-Client\Assets\Msdk\Editor\Librarys\Android3.2/UnityClasses.jar;D:\Git\SoloProjects\Dragon-Nest-Mobile-Client\Assets\Msdk\Editor\Librarys\Android3.2/MSDKLibrary/libs\bugly_crash_release.jar" -d classes ./src/com/example/wegame/*.java ./src/com/example/wegame/wxapi/*.java ./src/com/tencent/msdk/adapter/*.java
cd classes
jar cvf msdk_unity_adapter_3.3.30u.jar com
exit
