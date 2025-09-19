# MSDK string resource menu
# coding: utf-8
import os
import hashlib
import sys
import re

# check input
res_root = sys.argv[1]
md5file_root = sys.argv[2]

# get resource file md5 dict
info_dict = {'':''}

check_file = open(md5file_root, 'r')
lines = check_file.readlines()
for line in lines:
	file_name_end_index = line.index(' : ')
	key = line[0:file_name_end_index]
	value = line[file_name_end_index+3:-1]

	if not key == 'version':
		info_dict[key] = value
		pass
	pass

check_file.close()

# check resource file
error_list = []
key_list = info_dict.keys()
for rt, dirs, files in os.walk(res_root):
    for f in files:
    	path = os.path.join(rt,f)
    	regex=re.compile("/+[\w-]+")
    	path_list = regex.findall(path)
    	s_path = path_list[-2]+path_list[-1]
    	if s_path in key_list:
    		print 'checking path : ',s_path
	    	md5file = open(path,'rb')
	    	md5 = hashlib.md5(md5file.read()).hexdigest()
	    	md5file.close()
	    	if not info_dict[s_path] == md5:
	    		error_list.append(s_path)
	    		pass
    		pass
       	pass
    pass

# report error

if len(error_list) > 0:
	print '---------------- error list ---------------------'
	for file in error_list:		
		print file
		pass
	print '---------------- error list end ---------------------'
	raise RuntimeError('Some resource files are wrong . Please check them')
	pass
else:
 	print ' Check success !!! '

