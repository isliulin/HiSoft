sc create PhoneWatcherService binPath= "%~dp0�������ѷ���.exe" start= auto 
sc config PhoneWatcherService type= interact
net start PhoneWatcherService
pause