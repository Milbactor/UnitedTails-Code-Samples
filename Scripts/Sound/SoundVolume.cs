// 音量クラス
using System;


[Serializable]
public class SoundVolume{
	public float BGM = 1.0f;
	public float Voice = 1.0f;
	public float SE = 1.0f;
	public bool Mute = false;
	public bool BGM_Mute = false;
	public bool SE_Mute = false;
	
	public void Init(){
		BGM = 1.0f;
		Voice = 1.0f;
		SE = 1.0f;
		Mute = false;
		BGM_Mute = false;
		SE_Mute = false;
	}
}
