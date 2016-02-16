# Kiilogger
C# based keylogger for general purpose keyboard logging and computer usage debugging :stuck_out_tongue:

### Working Fluid
Uses GetAsyncKeyState() with a Timer :alarm_clock:  
Polls all keys :key: on en-us qwerty keyboard.  
Might not be the most efficient code. But we don't have so weak computers anymore.

Its just a Proof of Concept Project.

### Achievements
VirusTotal Detection 1/52 :cop: [Result](https://www.virustotal.com/en/file/1822f0aa34f705ac3902d46827c8677944e0465c3a7b4e9ff8140921ede09a5b/analysis/1453916060/)  
For this Binary [Commit](https://github.com/XEonAX/Kiilogger/commit/4f5995e92bebcd301ca20dfec8c62fdb04ff9e7a)

### How To:
To use pass the Following Parameters:  
`"nohint path=PathToLogFile keyword=StringKeyword"`

`nohint`: Hides Hint/How to use  
`path`:Path where log file be appended  
`StringKeywordstat`<-- when typed anywhere while KiiLogger is running will cause it to open Log File.  
`StringKeywordstat`<-- when typed anywhere while KiiLogger is running will cause it to Exit.

### Exercises for the Reader:
Hiding in plain sight:neckbeard:, Delivery at the target :package: and Extraction after the OP :runner:
