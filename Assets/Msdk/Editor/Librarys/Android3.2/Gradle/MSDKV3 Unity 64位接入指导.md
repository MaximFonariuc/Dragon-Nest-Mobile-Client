# MSDKV3 Unity 64位接入指导

### 使用方法

#### 步骤一

MSDKV3 Unity 额外提供两个 64 位 adapter 库：arm64-v8 和 x86_64，当前 Unity 暂时还不支持打 X86_64 的库, 为避免出现机型兼容问题，请根据需要拷贝 arm64-v8a 覆盖到 MSDK/Editor/Librarys/Android 3.2/libs 中，修改之后需要重新 Deploy 部署。

> 您在部署MSDK之前，如果在"Assets/Plugin/Android"目录有"AndroidManifest.xml"、"project.properties"这两个文件，则在部署MSDK时不会替换掉，而是生成副本"Copy_AndroidManifest.xml"、"Copy_project.properties"。您需要参考副本文件手动修改"AndroidManifest.xml"、"project.properties"。

#### 步骤二

需要手动修改 /Assets/Msdk/Editor/Scripts/Deploy/DeployAndroid.cs 中 DeployLibrary 各版本 so 的部署限制，具体操作如下所示

- 从3.3.18版本开始，信鸽推送被单独抽离成TPNSSDK插件提供，如果业务侧有接入TPNS的需求，则DeployLibrary() 和 DeployTPNSSDK() 中都需要对以下代码做处理

```java
    // 3.3.7u 开始支持 64 ，为了避免非 64 的业务出现兼容问题，需要 64 位库的业务要手动修改 Android 支持库部署代码 
    // 把引号中的内容复制到下面 if 判断中“ || abiName.Equals("arm64-v8a")”
    // 当前 Unity 版本暂不支持 "x86_64"，后续 Unity 支持后，如果需要再把该库在 if 中添加
    if (abiName.Equals("armeabi-v7a") || abiName.Equals("x86")) {
        continue;
    }
```

#### 已知问题说明

1、Unity 自版本 2018.2 和 2017.4.16 开始提供 64 位支持。

2、确保编译设置能够输出 64 位库

依次转到 Player Settings Panel > Settings for Android > Other Settings > Configuration，将 Scripting Backend 设为 IL2CPP，依次选择“Target Architectures”> ARM64 复选框。

> 更多详情参见官网说明：[https://developer.android.com/distribute/best-practices/develop/64-bit?hl=zh-cn](https://developer.android.com/distribute/best-practices/develop/64-bit?hl=zh-cn)



