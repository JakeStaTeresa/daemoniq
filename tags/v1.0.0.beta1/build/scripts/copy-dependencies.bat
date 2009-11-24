cd /d %~dp0
call setEnv
"../nant-%NANT_VERSION%/bin/NAnt.exe" -buildfile:nant.build copy-dependencies > build.log