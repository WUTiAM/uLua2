using LuaInterface;
using System.Text;
using UnityEngine;

public class HelloWorld : MonoBehaviour
{
	void Start()
	{
		LuaState l = new LuaState();
		l.DoString( Encoding.UTF8.GetBytes( "print('hello world 世界')" ) );
	}
}
