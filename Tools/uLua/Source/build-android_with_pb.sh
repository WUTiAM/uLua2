#!/bin/bash

LUAJIT_VER = LuaJIT-2.0.4

make TARGET_OS=Android LUAJIT_VER=${LUAJIT_VER} clean
make TARGET_OS=Android LUAJIT_VER=${LUAJIT_VER}

cp Plugins/Android/libulua.so ../../Client/Assets/Plugins/Android/
