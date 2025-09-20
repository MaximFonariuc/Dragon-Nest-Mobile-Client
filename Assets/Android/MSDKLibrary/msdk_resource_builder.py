# MSDK string resource menu
# coding: utf-8
import os
import hashlib
import sys
import re

print '------------------ start check-----------------------'
s = os.sep


res_root = os.curdir+s+'res'
# res_root = 'res'
# check_file = open(os.curdir+s+'OpenId'+s+'MSDKLibrary'+s+'msdk_resource_md5.txt' , 'w')
check_file = open('msdk_resource_md5.txt' , 'w+')

# 写入版本号
if len(sys.argv) <= 1:
    raise RuntimeError('Can not find md5 check file')
    pass
version = sys.argv[1]
print 'version : ', version
check_file.write('version : '+ version +'\n')

# 写入资源MD5
print 'msdk_resource_checker root path : ', res_root 
for rt, dirs, files in os.walk(res_root):
    for f in files:
        if not ".DS_Store" in f :            
            # 计算md5 写入文件
            path = os.path.join(rt,f)
            regex=re.compile("/+[\w-]+")
            path_list = regex.findall(path)
            s_path = path_list[-2]+path_list[-1]
            print 'checking file : ',s_path
            md5file = open(path,'rb')
            md5 = hashlib.md5(md5file.read()).hexdigest()
            md5file.close()
            check_file.write(''+s_path +' : '+md5+'\n')
            pass

check_file.close()

print '------------------ end check-------------------------'


