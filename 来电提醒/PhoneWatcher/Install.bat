sc create HiSoftPhoneWatcher binPath= "%~dp0PhoneWatcher.exe" start= auto 
sc config HiSoftPhoneWatcher type= interact
net start HiSoftPhoneWatcher
pause