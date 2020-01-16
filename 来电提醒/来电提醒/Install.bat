sc create PhoneWatcherService binPath= "%~dp0来电提醒服务.exe" start= auto 
sc config PhoneWatcherService type= interact
net start PhoneWatcherService
pause