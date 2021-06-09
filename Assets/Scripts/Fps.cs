using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fps : MonoBehaviour
{
	float deltaTime = 0.0f;
	//游戏的FPS，可在属性窗口中修改
	public int targetFrameRate = 60;
	float CurFps;
	//当程序唤醒时
	void Awake()
	{
		//修改当前的FPS
		//Application.targetFrameRate = targetFrameRate;
	}
	void Update()
	{
		deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
	}

	void OnGUI()
	{
		int w = Screen.width, h = Screen.height;

		GUIStyle style = new GUIStyle();

		Rect rect = new Rect(0, 0, w, h * 2 / 100);
		style.alignment = TextAnchor.UpperLeft;
		style.fontSize = h * 2 / 100;
		//new Color (0.0f, 0.0f, 0.5f, 1.0f);
		style.normal.textColor = Color.white;
		float msec = deltaTime * 1000.0f;
		CurFps = 1.0f / deltaTime;
		string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, CurFps);
		GUI.Label(rect, text, style);
	}
}
