using LuaInterface;
using System.Text;
using UnityEngine;

public class CreateGameObject : MonoBehaviour
{
	private string script = @"
			luanet.load_assembly('UnityEngine')
			GameObject = luanet.import_type('UnityEngine.GameObject')

			local newGameObj = GameObject('NewObj')
			newGameObj:AddComponent('ParticleSystem')
		";

	void Start()
	{
		LuaState l = new LuaState();
		l.DoString( Encoding.UTF8.GetBytes( script ) );
	}
}
