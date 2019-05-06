using System.Collections;
using UnityEngine;

namespace Stereo3D
{
	public class UIDisplay:MonoBehaviour
	{
		private static Settings settings = Settings.Instance;
		private static bool created = false;
		void Awake()
		{
			if (!created)
			{
				DontDestroyOnLoad(this.gameObject);
				created = true;
				Debug.Log("Created UI Canvas");
			}
		}
		public void ShowText(string text)
		{
			OutputText = text;
		}
		private string OutputText;
		private bool hideTextStarted;
		void OnGUI()
		{
			if (string.IsNullOrEmpty(OutputText)) return;
			/*	int width = 200;
				int height = 30;
				Rect r = new Rect(Screen.width/2, Screen.height - 50, 0, height);
				GUI.skin.label.wordWrap = false;
				GUI.skin.label.clipping = TextClipping.Overflow;
			GUI.Label(r, OutputText);
				*/

			GUIContent content = new GUIContent(OutputText);

			GUIStyle style = GUI.skin.label;
			style.fontSize = 20;
			style.alignment = TextAnchor.LowerCenter;

			// Compute how large the button needs to be.
			Vector2 size = style.CalcSize(content);
			Rect r = new Rect(Screen.width / 2- size.x/2, Screen.height -(size.y+10 ), size.x,size.y);
			GUI.Label(r, OutputText);
		}

		public void HideText()
		{
			if(hideTextStarted)return;
			hideTextStarted = true;
			StartCoroutine("HideTextCoroutine");
		}

		IEnumerator HideTextCoroutine()
		{
			yield return new WaitForSeconds(settings.UiDisplayTime);
			hideTextStarted = false;
			OutputText = null;
		}
	}
}
