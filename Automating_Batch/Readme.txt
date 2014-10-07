This is an example of how you could automate uploading this report via SFTP. It is not the only way to do it, it is just the first way that I've set it up.

I used PSFTP.exe (by the makers of PuTTY: http://www.chiark.greenend.org.uk/~sgtatham/putty/download.html) to do the SFTP upload, and a batch file to control everything.


run_automated_task.bat

This is an example of a batch file that would be set up as an automated task in Windows. My scheduled task in Windows runs this file.

sftp_batch_commands.txt
This is a list of commands that PSFTP.exe will use once logged in, to upload the CSV files and then close the connection.