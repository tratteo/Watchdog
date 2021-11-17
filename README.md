# Watchdog
A simple process watchdog system.   
Launch the Watchdog process passing the observed process and eventually its arguments.

Arguments:   
```
p0    path to the observed process executable   

-arg  observed process arguments   
-mi   max number of reboots   
-w    observed working directory   

/k    kill the observed upon closing the watchdog   
/e    embed the watchdog terminal into the observed process terminal
```
