#!/bin/bash

LUAJIT_VER="LuaJIT-2.0.4"

# !!
# !! Change to your own path !!
# !!
# On Windows
NDK=/f/Application/android-ndk-r10e
NDKP=$(NDKVER)/prebuilt/windows-x86_64/bin/arm-linux-androideabi-
# On Mac OSX
#NDK=/Users/jo3l/android-ndk-r10e
#NDKP=$NDKVER/prebuilt/darwin-x86_64/bin/arm-linux-androideabi-

# Android/ARM, armeabi-v7a (ARMv7 VFP), Android 4.0+ (ICS)
NDKABI=14
NDKVER=$NDK/toolchains/arm-linux-androideabi-4.9
NDKF="--sysroot $NDK/platforms/android-$NDKABI/arch-arm"
NDKARCH="-march=armv7-a -mfloat-abi=softfp -Wl,--fix-cortex-a8"
# Build libluajit.a
make -C $LUAJIT_VER clean
make -C $LUAJIT_VER HOST_CC="gcc -m32" CC="gcc -fPIC" CROSS=$NDKP TARGET_FLAGS="$NDKF $NDKARCH" TARGET_SYS=Linux
# Build lua_wrap.o and pb.o
${NDKP}gcc $ISDKF -c lua_wrap.c -o ios/lua_wrap.o -I$LUAJIT_VER/src
${NDKP}gcc $ISDKF -c pb.c -o ios/pb.o -I$LUAJIT_VER/src
# Build libulua.so
${NDKP}gcc -fno-stack-protector -I$LUAJIT_VER/src $NDKF -fPIC -shared \
	-Wl,--whole-archive $LUAJIT_VER/src/libluajit.a lua_wrap.o pb.o -Wl,--no-whole-archive -lm \
	-o Plugins/Android/libulua.so
