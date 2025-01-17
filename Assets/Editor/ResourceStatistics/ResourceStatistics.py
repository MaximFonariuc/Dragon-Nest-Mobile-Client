import platform
import os
import sys
import re
import shutil

class CKeyWord:
    origin = ""
    regular = ""
    pattern = re.compile("")

def HasKeyword(text, keywords):
    res = []
    for keyword in keywords:
        if keyword.pattern.search(text):
            res.append(keyword)
    return res

def ProcessMatch(match, keywords):
    size = float(match.group(1))
    uint = match.group(2)
    text = match.group(4)
    if uint == "mb":
        size *= 1024;

    matchedKeywords = HasKeyword(text, keywords)
    return (matchedKeywords, size)

def ProcessLine(line, pattern, keywords):
    matchObj = pattern.search(line)
    if matchObj:
        #print matchObj.group()
        matchedKeywords, size = ProcessMatch(matchObj, keywords)
        return (True, matchedKeywords, size)
    else:
        return (False, [], 0)

def ParseSize(kb):
    if kb > 1024.0:
        kb = kb / 1024.0;
        return ("%.2fMB" % kb)
    
    return ("%.2fKB" % kb)

def GetTotalSize(line, pattern):
    matchObj = pattern.search(line)
    size = 0.0
    if matchObj:
        size = float(matchObj.group(1))
        if matchObj.group(2) == "mb":
            size = size * 1024
        #size = 432432543765
    else:
        size = 0
    return size

def GetVersion(line, pattern):
    matchObj = pattern.search(line)

    if matchObj:
        return matchObj.group()

    return ""

def CreateKeywordsPatterns(keywords):
    for keyword in keywords:
        keyword.pattern = re.compile(keyword.regular, re.IGNORECASE)
    return

def ParseFile(logPath, keyWords):
    if not os.path.exists(strLogPath):
        sys.stdout.write("File not exit: " + strLogPath + '\n')
        return False
        
    fLogFile = open(strLogPath)

    bHasValidInfo = False
    dicStatistics = {}

    CreateKeywordsPatterns(keyWords)

    pattern = re.compile(r"^\s*([1-9]\d*\.\d*|0\.\d+|[1-9]\d*|0)\s+(mb|kb)\s+([1-9]\d*\.\d*|0\.\d+|[1-9]\d*|0)%\s+(.*)")

    totalSizePattern = re.compile(r"(?<=^Complete\ssize)(?:\s+)([1-9]\d*\.\d*|0\.\d+|[1-9]\d*|0)\s+(mb|kb)")
    versionPattern = re.compile(r"(?<=^LICENSE\sSYSTEM\s\[).*(?=\])")

    versionInfo = ""
    totalSize = 0.0
    for line in fLogFile:
        bValid, sKeywords, fKB = ProcessLine(line, pattern, keyWords)
        bHasValidInfo = bHasValidInfo | bValid
        #if bHasValidInfo:
        if not bValid and bHasValidInfo:
            break

        if not bValid and versionInfo == "":
            versionInfo = GetVersion(line, versionPattern)

        if not bValid and totalSize == 0.0:
            size = GetTotalSize(line, totalSizePattern)
            if size != 0.0:
                totalSize = size

        if bValid:
            for sKeyword in sKeywords:
                if dicStatistics.has_key(sKeyword.origin):
                    dicStatistics[sKeyword.origin] = dicStatistics[sKeyword.origin] + fKB
                else:
                    dicStatistics[sKeyword.origin] = fKB

    sys.stdout.write("Success\n")
    sys.stdout.write("Version: " + versionInfo + "\n")
    
    if totalSize == 0.0:
        totalSize = 1.0
    for (k,v) in dicStatistics.items():
        sys.stdout.write(k + "\t" + ParseSize(v) + "_" + "%.1f" % (v / totalSize * 100) + "%\n")
            
    fLogFile.close()

    return bHasValidInfo

def GetDefaultPath():
    strPlatform = platform.system();
    path = ""
    if strPlatform == "Windows":
        path = os.environ['HOMEDRIVE'] + os.environ['HOMEPATH'] + "/AppData/Local/Unity/Editor/Editor.log"
    else:
        path = "/Users/liming/Library/Logs/Unity/Editor.log"
    return path

def GetPreservedLogPath():
    strPlatform = platform.system();
    path = ""
    if strPlatform == "Windows":
        path = os.environ['XResourcePath'] + "/XProject/Assets/Editor/ResourceStatistics/Editor.log"
    else:
        path = "/Users/liming/Desktop/DN/dragon-nest/XProject/Assets/Editor/ResourceStatistics/Editor.log"
    return path

# Convert non-regular keywords to regular ones, e.g.
# \ -> \\
#. -> \.
# * -> .*
# space -> \s
def PreprocessKeywords(keywords):
    for keyword in keywords:
        keyword.regular = keyword.origin.replace("\\", "\\\\")
        keyword.regular = keyword.regular.replace(".", "\\.")
        keyword.regular = keyword.regular.replace("*", ".*")
        keyword.regular = keyword.regular.replace(" ", "\\s")
        #print(keyword.regular)
    return

# Get Key Words
listKeywords = sys.argv[3:]
if not listKeywords or len(listKeywords) == 0:
    listKeywords = ["Resources/Effects/", "Resources/Animation/", "atlas/UI", "XScene"]
    #listKeywords = [".png"]

listKeywordDatas = []
for keyword in listKeywords:
    newData = CKeyWord()
    newData.origin = keyword
    newData.regular = keyword
    listKeywordDatas.append(newData)

# Get Log Path
if len(sys.argv) < 2:
    strLogPath = "default"
else:
    strLogPath = sys.argv[1]

if len(sys.argv) < 3:
    bRegularKeywords = False
else:
    bRegularKeywords = (int(sys.argv[2]) == 1 and True or False)

bCustomPath = False
if(strLogPath == "default"):
    strLogPath = GetDefaultPath()
else:
    bCustomPath = True

if not bRegularKeywords:
    PreprocessKeywords(listKeywordDatas)

bRes = ParseFile(strLogPath, listKeywordDatas)
if bRes and not bCustomPath:
    shutil.copy(strLogPath, GetPreservedLogPath())

if not bRes and bCustomPath:
    strLogPath = GetDefaultPath()
    ParseFile(strLogPath, listKeywordDatas)


