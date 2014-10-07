@echo off

REM Delete previous day's file if exists
IF EXIST Absence.csv DEL Absence.csv
IF EXIST Addressbook.csv DEL Addressbook.csv

REM Generate address book file
scabsencefile.exe /schoolid:XXXXXX /filename:Absence.csv /date:today

REM Generate absence file
scaddressbook.exe /schoolid:XXXXXX /filename:Addressbook.csv /date:today

REM Upload it to Schoolconnects
REM You need to do "echo y |" the first time this is run so that it accepts the key
Rem After you've run it once with echo y, you should remove that part
psftp.exe SFTPHOSTNAME -l USERNAME -pw PASSWORD -b stfp_batch_commands.txt