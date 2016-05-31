#!/bin/bash
make TARGET_OS=x86 LUAJIT_VER=luajit2.0.3 clean
make TARGET_OS=x86 LUAJIT_VER=luajit2.0.3

cp Plugins/x86/ulua.dll ../../Client/Assets/Plugins/x86/
