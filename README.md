<img align="right" src="https://raw.githubusercontent.com/WUTiAM/uLua2/master/ulua2logo.png" width="100" />
# uLua²

uLua² 是一套在 Unity 中使用 Lua（实现代码热更新）的手机游戏开发解决方案，并在已上线的 3D 卡牌 RPG 手游大作中表现优异。  
uLua² is a solution for mobile game development using Lua in Unity. It has been verified in a famous onlined 3D mobile game.

uLua² 源于已故 Unity 插件 [uLua](https://www.assetstore.unity3d.com/en/#!/content/13887) 最后的 1.03 版本，继续保持使用反射机制来完成 Lua 对 C# 的访问控制，并扩充为一套完整的 Unity + Lua 手游开发框架。当然，我们也修正了一些 uLua 旧有的问题，并扩展了少量功能（如支持 ARM64 等）。  
uLua² is derived from the deprecated Unity plugin [uLua](https://www.assetstore.unity3d.com/en/#!/content/13887) (the last version is 1.03). We keeps the original reflection for Lua to C# as uLua did, and is a complete framework for mobile game development using Unity + Lua. We also fixed some bugs in uLua and added a few functions (i.e. ARM64 support).

***

特性：  
Features:
- 在 Unity 中无缝编写 Lua/C# 代码，控制所有你想控制的 / Coding in Lua & C# seamlessly in Unity, to control everything you want
- 代码随时热更新，无需依赖 C# 代码改动 / Code updating at anytime with NO binding C# code generated
- 协程、错误处理等更多功能支持 / Supporting Coroutine, error handling, etc
- 全面支持 Android/iOS 32位/64位，未来将支持 WP / Supporting Android and iOS (32/64bit), and WP in the future

要求：  
Requires:
- Lua 5.1.4
- Unity 4.6.9 or higher (require Unity Pro)

支持平台：  
Suported Platforms:
- Android (32bit/64bit)
- iOS (32bit/64bit)

已集成第三方组件：  
Third-Party Module Integration:
- [dkjson](http://dkolf.de/src/dkjson-lua.fsl/home)
- [Protocol Buffers](https://github.com/google/protobuf)
- [protoc-gen-lua](https://github.com/paynechu/protoc-gen-lua)

***

uLua² 是已故 Unity 插件 [uLua](http://forum.unity3d.com/threads/ulua-lua-for-unity.221310/) 的延续，一次重生。仅仅是对于 uLua 这个插件的热爱和感恩，我们决定开启这样一个开源项目，希望能和更多使用 Unity + Lua 的小伙伴们切磋交流。  
uLua² is based on the deprecated Unity plugin [uLua](https://www.assetstore.unity3d.com/en/#!/content/13887), a new begining. We decided to start the project due to our love and appreciation to uLua, and hope to exchange experiences with other game developers who use Unity + Lua.  
> 我们于2014年在 Unity Asset Store 付费购买了这个插件。  
> We bought the plugin on Unity Asset Store in 2014.
