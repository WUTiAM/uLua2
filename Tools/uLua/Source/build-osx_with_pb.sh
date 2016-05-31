#!/bin/bash

LUAJIT_VER="lua-5.1.5"

# Build liblua.a for x86_64
make -C $LUAJIT_VER clean
make -j2 -C $LUAJIT_VER BUILDMODE=static
cp $LUAJIT_VER/src/liblua.a osx/liblua-x86_64.a

cd osx/
xcodebuild
cd ..

cp -r osx/Build/Release/ulua.bundle Plugins/
