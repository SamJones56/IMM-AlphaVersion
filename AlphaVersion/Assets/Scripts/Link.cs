using UnityEngine;
using System.Runtime.InteropServices;
using UnityEngine.UI;

public class Link : MonoBehaviour 
{

	public void OpenRepo()
	{
	#if !UNITY_EDITOR
		openWindow("https://github.com/SamJones56/IMM-AlphaVersion");
	#endif
    }

    [DllImport("__Internal")]
	private static extern void openWindow(string url);

}