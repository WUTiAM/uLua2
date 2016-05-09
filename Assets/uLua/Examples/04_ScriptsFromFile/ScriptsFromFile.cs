using LuaInterface;
using System.Text;
using UnityEngine;

public class ScriptsFromFile : MonoBehaviour
{
	public TextAsset scriptFile;

	void Start()
	{
		LuaState l = new LuaState();
		l.DoString( Encoding.UTF8.GetBytes( scriptFile.text ) );
	}
}
