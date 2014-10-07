About
=====

This utility generates an "address book" and "absences" file for SchoolConnects / Synrevoice autodialer system. It is designed to be run as an automated task in Windows every week day.

This utility /should/ work for any school using SchoolLogic / SIRS, but since databases can differ quite drastically between schools, it may not work for you. 


How to use
==========

* Create a folder that contains the following files:
 * SCAbsenceFile.exe
 * SCAddressBook.exe
 * SLDataLib.dll
* From a command prompt, run one of the executables to generate a new configuration file (it will be called "SCRConfig.xml")
* Edit the SCRConfig.xml file and modify the database connection string to point to your SchoolLogic database. 
* Running an EXE again, with the proper syntax should create the files, or display an error message.


Syntax
======

In the future running an exe will display the syntax, but as of this time that is not implemented.

scabsencefile.exe /schoolid:XXXXXXX /filename:Absence.csv /date:today

scaddressbook.exe /schoolid:XXXXXXX /filename:Addressbook.csv /date:today

Where "schoolid" is the Government ID number of the school (or whatever is in the "cCode" field in the school's record in the SchoolLogic database).

The "date" can be a specific date like "2014-10-23, "today", "now", "yesterday". It is not case sensitive.