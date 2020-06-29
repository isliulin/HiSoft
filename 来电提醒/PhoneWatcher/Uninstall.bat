net stop HiSoftPhoneWatcher
sc delete HiSoftPhoneWatcher binPath= "%~dp0PhoneWatcher.exe"
pause