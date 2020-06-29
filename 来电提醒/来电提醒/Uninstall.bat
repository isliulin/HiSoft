net stop PhoneWatcherService
sc delete PhoneWatcherService binPath= "%~dp0来电提醒服务.exe"
pause