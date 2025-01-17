@echo off

::-----------Config-----------::
set UNITY_PATH=%UnityPath%\Editor\Unity.exe
set PROJECT_PATH=%XResourcePath%\XProject
::-----------Config-----------::

echo "Unity to apk"

echo "ProjectPath:"%PROJECT_PATH%

"%UNITY_PATH%" -projectPath "%PROJECT_PATH%" -executeMethod SelectPlatformEditor.ShellBuildiOS -quit -batchmode

echo "apk finish"

pause