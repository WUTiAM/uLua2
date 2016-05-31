#!/bin/bash
make TARGET_OS=x86_64 LUAJIT_VER=luajit2.0.3 clean
make TARGET_OS=x86_64 LUAJIT_VER=luajit2.0.3

cp Plugins/x86_64/ulua.dll ../../Client/Assets/Plugins/x86_64/
