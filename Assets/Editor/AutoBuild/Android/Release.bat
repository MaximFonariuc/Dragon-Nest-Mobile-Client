::@echo off

::-----------Config-----------::
set UNITY_PATH=%XUnityPath%\Unity.exe
set PROJECT_PATH=%XResourcePath%\XProject
set USER_PATH=%XUserPath%
set APP_NAME=DragonNest
set /p VERSION=<%XResourcePath%\XProject\Assets\Resources\android-version.bytes
::-----------Config-----------::

::echo "Git operation"

::sh %PROJECT_PATH%/Assets/Editor/AutoBuild/gitpull.sh

echo "Editor.log"

rm "%USER_PATH%\AppData\Local\Unity\Editor\Editor.log"

echo "Unity to apk"

echo "ProjectPath:"%PROJECT_PATH%

"%UNITY_PATH%" -projectPath "%PROJECT_PATH%" -executeMethod SelectPlatformEditor.BuildAndroidRelease -quit

echo "apk finish"

echo "start ResourceStatistics"

python "%PROJECT_PATH%\Assets\Editor\ResourceStatistics\ResourceStatistics.py"

mv %PROJECT_PATH%\Android\DragonNest.apk %PROJECT_PATH%\Android\longzhigu_android_sdo_%VERSION%.apk

pause