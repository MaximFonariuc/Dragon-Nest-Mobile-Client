#!/usr/bin/env python
# -*- coding: utf-8 -*-

from Tkinter import *
import os

class Application(Frame):

    def android_release(self):
        os.system("start Android\\Release.bat" + " " + str(self.upload))

    def android_test(self):
        os.system("start Android\\Test.bat" + " " + str(self.upload))

    def android_publish(self):
        os.system("start Android\\Publish.bat" + " " + str(self.upload))

    def ios_release(self):
        os.system("~/Desktop/DN/dragon-nest/XProject/Assets/Editor/AutoBuild/iOS/Release.command" + " " + str(self.upload))

    def ios_test(self):
        os.system("~/Desktop/DN/dragon-nest/XProject/Assets/Editor/AutoBuild/iOS/Test.command" + " " + str(self.upload))

    def ios_publish(self):
        os.system("~/Desktop/DN/dragon-nest/XProject/Assets/Editor/AutoBuild/iOS/Publish.command" + " " + str(self.upload))

    def change_flag(self):
		self.upload = 1 - self.upload

    def createWidgets(self):
        self.QUIT = Button(self)
        self.QUIT["text"] = "QUIT"
        self.QUIT["fg"]   = "red"
        self.QUIT["command"] =  self.quit
        self.QUIT.grid(row=5,column=4)

        self.UploadToServer = Checkbutton(self)
        self.UploadToServer["text"] = "是否上传服务器"
        self.UploadToServer["anchor"] = "w"
        self.UploadToServer["command"] = self.change_flag
        self.UploadToServer.grid(row=5,column=0)

	#iOS
        self.iOSRelease = Button(self)
        self.iOSRelease["text"] = "iOS外网版本"
        self.iOSRelease["command"] = self.ios_release
        self.iOSRelease["anchor"]="w"
        self.iOSRelease.grid(row=0,column=0)

        self.iOSPublish = Button(self)
        self.iOSPublish["text"] = "iOS盛大版本"
        self.iOSPublish["command"] = self.ios_publish
        self.iOSPublish["anchor"]="w"
        self.iOSPublish.grid(row=1,column=0)

        self.iOSTest = Button(self)
        self.iOSTest["fg"]="red"
        self.iOSTest["text"] = "iOS测试版本"
        self.iOSTest["command"] = self.ios_test
        self.iOSTest["anchor"]="w"
        self.iOSTest.grid(row=2,column=0)

        #Android
        self.AndroidRelease = Button(self)
        self.AndroidRelease["text"] = "Android外网版本"
        self.AndroidRelease["command"] = self.android_release
        self.AndroidRelease["anchor"]="w"
        self.AndroidRelease.grid(row=0,column=1)

        self.AndroidPublish = Button(self)
        self.AndroidPublish["text"] = "Android盛大版本"
        self.AndroidPublish["command"] = self.android_publish
        self.AndroidPublish["anchor"]="w"
        self.AndroidPublish.grid(row=1,column=1)

        self.AndroidTest = Button(self)
        self.AndroidTest["fg"]="red"
        self.AndroidTest["text"] = "Android测试版本"
        self.AndroidTest["command"] = self.android_test
        self.AndroidTest["anchor"]="w"
        self.AndroidTest.grid(row=2,column=1)

    def __init__(self, master=None):
        Frame.__init__(self, master)
        self.pack()
        self.createWidgets()

    upload = 0

root = Tk()
app = Application(master=root)

app.master.title("DragonNest AutoBuild")

app.mainloop()
root.destroy()
