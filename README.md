![GitHub release (latest SemVer including pre-releases)](https://img.shields.io/github/v/release/tratteo/Watchdog?include_prereleases&label=Release)
![GitHub](https://img.shields.io/github/license/tratteo/Watchdog?color=orange&label=License)
![GitHub top language](https://img.shields.io/github/languages/top/tratteo/Watchdog?color=5027d5&label=C%23&logo=.net)
![GitHub last commit (branch)](https://img.shields.io/github/last-commit/tratteo/Watchdog/main?label=Last%20commit&color=brightgreen&logo=github)
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
