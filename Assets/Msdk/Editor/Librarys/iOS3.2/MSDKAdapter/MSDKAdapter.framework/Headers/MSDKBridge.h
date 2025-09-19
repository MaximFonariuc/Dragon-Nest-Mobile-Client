//
//  MSDKBridge.hpp
//  Unity-iPhone
//
//  Created by Luox on 16/4/25.
//
//

#ifndef MSDKBRIDGE_H
#define MSDKBRIDGE_H

typedef char* (*SendToUnity)(const char* jsonStr);

class MSDKBridge {
public:
    static SendToUnity sendToUnity;
    static void setBridge(SendToUnity bridge);
};

#endif /* MSDKBRIDGE_H */
