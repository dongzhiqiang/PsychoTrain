set buildingMakeFileName=%cd%\���ڴ��PC
rem echo %buildingMakeFileName%

if exist %buildingMakeFileName% goto exitFlg
mkdir %buildingMakeFileName%

set binPath=%cd%\bin_pc
rd /q /s %binPath%
call "E:/MySvn/GOWPackPC/build.bat" %binPath%
rd /q /s %buildingMakeFileName%

:exitFlg
exit