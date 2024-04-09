# Kongsberg-Maritime-recruitment-task
Kongsberg Maritime Software Engineer Recruitment Task

1. Tech:
    - JS/TS
    - React
    - .NET
    - Azure
    - SQL / NoSQL
    - WebSocets
    - REST API
2. Create simulator for each sensor.  
- The simulator should transmit messages wiht defined frequency.  
- The simulator should transmit messages that follows the scheme:  
$FIX, [ID], [TYPE], [VALUE], [QUALITY] *, i.e.  
$FIX, 3, Speed, 192, Normal *
3. Create classifier that signals the quality of each message's value.
4. Create receiver that should accept and analyze the messages sent from a sensor.  
- The receiver should be able to analyze the contents of each telegram.  
- The receiver should present the data in human readable format.  
- The receiver should highlight the Warnings and Alarms
5. Each sensor can be listened by 0 to N receivers.
6. Each receiver can listen to only one sensor.
7. Create configuration file for receivers.
- Determine which sensor is listened to by which receiver.
- Determine which receiver is active.