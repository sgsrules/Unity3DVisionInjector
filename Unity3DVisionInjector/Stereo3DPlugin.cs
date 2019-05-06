using IllusionPlugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

namespace Stereo3D
{
	internal class Stereo3DPlugin : IPlugin
	{
		private static Settings settings = Settings.Instance;
		//public static bool isRight = false;

		public static float Separation
		{
			get { return settings.Separation; }
			set { settings.Separation = value; }
		}

		public static float Convergence
		{
			get { return settings.Convergence; }
			set { settings.Convergence = value; }
		}

		// public static float Separation = .60f;
		//  public static float Convergence = 15f;

		public string Name
		{
			get { return "Unity 3D Vision Plugin"; }
		}

		public string Version
		{
			get { return "1.0"; }
		}

		//  [DllImport("3DVisionDirectUnity")]
		//  private static extern int SetSeparation(float s);

		public void OnApplicationStart()
		{
			Console.WriteLine("Starting my plugin");
			QualitySettings.vSyncCount = 0;
			Screen.fullScreen = true;

			if (settings.OverrideResolution)
			{
				Screen.SetResolution(settings.Width, settings.Heigth, true);
			}
			Console.WriteLine(Screen.currentResolution.width + "x" + Screen.currentResolution.height);
			Console.WriteLine("Orignal FrameRate: " + Application.targetFrameRate);
			Application.targetFrameRate = settings.UpdateHz;
			Console.WriteLine("New FrameRate: " + Application.targetFrameRate);
			uiGameObject = new GameObject();
			ui = uiGameObject.AddComponent<UIDisplay>();
			ui.ShowText("Unity 3DVision Plugin by SGS");
			//  SetSeparation(0);
			//    int i = InitializeStereo();
			//  if (i > 0) init = true;
			//   Console.WriteLine("Stereo init: " + i);
		}

		private GameObject uiGameObject;
		private UIDisplay ui;

		private bool init;

		public void OnApplicationQuit()
		{
		}

		public void OnFixedUpdate()
		{
			/*  if (isRight)
              {
                  Time.timeScale = 2f;
              }
              else
              {
                  Time.timeScale = 0.01f;
              }*/
		}

		public void OnLevelWasInitialized(int level)
		{
			AddStereoCameras();
			//RefreshCameras();
		}

		private void ListCameras()
		{
			List<GameObject> cameras = GetCameras();
			if (cameras.Count < 1) return;
			StringBuilder sb = new StringBuilder();
			sb.AppendLine("Active cameras found:");
			foreach (GameObject cam in cameras)
			{
				var camstring = GetCameraInfo(cam);
				sb.AppendLine(camstring);
			}
			ui.ShowText(sb.ToString());
		}

		private List<GameObject> GetCameras()
		{
			var objects = Resources.FindObjectsOfTypeAll<GameObject>().Where(obj => obj.GetComponent<Camera>() != null);
			List<GameObject> cameras = new List<GameObject>();
			foreach (var gameObject in objects)
			{
				if (gameObject.name != null && gameObject.name.Equals("reflectStereoCameraObject")) continue;
				if (gameObject.name != null && gameObject.name.Equals("stereoGameObject")) continue;
				if (gameObject.name != null && gameObject.name.Equals("tempGameObject")) continue;
				if (cameras.Contains(gameObject)) continue;
				Camera cam = gameObject.GetComponent<Camera>();
				if (cam == null) continue;
				//	if(!cam.enabled)continue;
				cameras.Add(gameObject);
			}

			if (cameras.Count < 1)
			{
				ui.ShowText("No cameras found");
			}
			return cameras;
		}

		private GameObject selectedCamera;
		private int selectedCameraIndex;

		private void SelectNextCamera()
		{
			List<GameObject> cameras = GetCameras();
			if (selectedCameraIndex > cameras.Count - 1) selectedCameraIndex = 0;
			selectedCamera = cameras[selectedCameraIndex];
			selectedCameraIndex++;

			var camstring = GetCameraInfo(selectedCamera);
			ui.ShowText(camstring);
		}

		private void SelectPreviousCamera()
		{
			List<GameObject> cameras = GetCameras();
			if (selectedCameraIndex > cameras.Count - 1) selectedCameraIndex = 0;
			if (selectedCameraIndex < 0) selectedCameraIndex = cameras.Count - 1;
			selectedCamera = cameras[selectedCameraIndex];
			selectedCameraIndex--;

			var camstring = GetCameraInfo(selectedCamera);
			ui.ShowText(camstring);
		}

		private string GetCameraInfo(GameObject camera)
		{
			int id = camera.GetInstanceID();
			var cam = camera.GetComponent<Camera>();

			string camstring = string.Format("Name={0} ID={1} Enabled={2} Stereo={3} SkipBlit={4} Swapped={5}", camera.name, id, cam.enabled,
				settings.StereoCameraIDs.Contains(id), settings.SkipBlitCameraIDs.Contains(id), settings.SwapLRCameraIDs.Contains(id));
			return camstring;
		}

		private void AddStereoCameras()
		{
			IEnumerable<GameObject> objects = Resources.FindObjectsOfTypeAll<GameObject>().Where(obj => obj.GetComponent<Camera>() != null);

			foreach (var gameObject in objects)
			{
				StereoizeCamera(gameObject);
			}
		}

		private Dictionary<int, StereoCamera> stereoCamDictionary = new Dictionary<int, StereoCamera>();

		private void StereoizeCamera(GameObject gameObject)
		{
			if (gameObject.name.Equals("reflectStereoCameraObject")) return;
			if (gameObject.name.Equals("stereoGameObject")) return;
			if (gameObject.name.Equals("tempGameObject")) return;
			int id = gameObject.GetInstanceID();
			if (!settings.StereoCameraIDs.Contains(id) && !settings.StereoCameraNames.Contains(gameObject.name)) return;

			Camera c = gameObject.GetComponent<Camera>();
			if (c == null) return;
			//	if(!c.enabled)return;
			if (StereoizedCameraList.Contains(c)) return;
			StereoizedCameraList.Add(c);

			//    continue;
			GameObject stereoGameObject = new GameObject("stereoGameObject");
			StereoCamera sc = stereoGameObject.AddComponent<StereoCamera>();
			sc.SetCameraTarget(c);
			if (!stereoCamDictionary.ContainsKey(id)) stereoCamDictionary.Add(id, sc);
		}

		private void DeStereoizeCamera(GameObject gameObject)
		{
			if (gameObject.name.Equals("reflectStereoCameraObject")) return;
			if (gameObject.name.Equals("stereoGameObject")) return;
			if (gameObject.name.Equals("tempGameObject")) return;
			int id = gameObject.GetInstanceID();
			if (!settings.StereoCameraIDs.Contains(id)) return;
			settings.StereoCameraIDs.Remove(id);
			Camera c = gameObject.GetComponent<Camera>();
			if (c == null) return;
			if (StereoizedCameraList.Contains(c)) StereoizedCameraList.Remove(c);

			if (stereoCamDictionary.ContainsKey(id))
			{
				stereoCamDictionary[id].RemoveStereoTarget();
				stereoCamDictionary.Remove(id);
			}
		}

		public void OnLevelWasLoaded(int level)
		{
			AddStereoCameras();
			//	RefreshCameras();
		}

		private int count = 0;

		[DllImport("3DVisionDirectUnity")]
		private static extern IntPtr SetLeftEye();

		[DllImport("3DVisionDirectUnity")]
		private static extern IntPtr SetRightEye();

		private List<Camera> StereoizedCameraList = new List<Camera>();
		//	private GameObject stereoGameObject;

		//	private List<GameObject> otherCameraObjects = new List<GameObject>();
		public static bool isRight = false;

		public void OnUpdate()
		{
			if (settings.EnableSequentialMode)
			{
				isRight = !isRight;
				if (!isRight) GL.IssuePluginEvent(SetRightEye(), 1);
				else GL.IssuePluginEvent(SetLeftEye(), 1);
			}

			if (Input.GetKey(settings.DecreaseSeparationHotKey))
			{
				Separation -= .0005f;
				ui.ShowText("Separation: " + Separation);
			}
			if (Input.GetKey(settings.IncreaseSeparationHotKey))
			{
				Separation += .0005f;
				ui.ShowText("Separation: " + Separation);
			}
			if (Input.GetKey(settings.DecreaseConvergenceHotKey))
			{
				Convergence -= .05f;
				ui.ShowText("Convergence: " + Convergence);
			}
			if (Input.GetKey(settings.IncreaseConvergenceHotKey))
			{
				Convergence += .05f;
				ui.ShowText("Convergence: " + Convergence);
			}
			if (Input.GetKey(settings.SaveSettingsHotKey))

			{
				ui.ShowText("Settings saved");
				settings.SaveSettings();
			}
			if (Input.GetKeyDown(settings.ListAllCamerasHotKey))
			{
				ListCameras();
			}
			if (Input.GetKeyDown(settings.NextCameraHotKey))
			{
				SelectNextCamera();
			}
			if (Input.GetKeyDown(settings.PreviousCameraHotKey))
			{
				SelectPreviousCamera();
			}
			if (Input.GetKeyDown(settings.ToggleStereoCameraHotKey))
			{
				ToggleStereoCamera(selectedCamera);
			}
			if (Input.GetKeyDown(settings.ToggleSkipBlitCameraHotKey))
			{
				ToggleSkipBlitCamera();//ToggleSkipBlitCamera();
			}
			if (Input.GetKeyDown(settings.ToggleSwapLRCameraHotKey))
			{
				ToggleSwapLRCamera();
			}

			if (!Input.anyKey)
			{
				ui.HideText();
			}

			var camera = UnityEngine.Camera.main;
			if (camera != null)
			{
				StereoizeCamera(camera.gameObject);
			}
		}

		private void ToggleSwapLRCamera()
		{
			if (selectedCamera == null) return;
			int id = selectedCamera.GetInstanceID();
			if (settings.SwapLRCameraIDs.Contains(id))
			{
				settings.SwapLRCameraIDs.Remove(id);
			}
			else
			{
				settings.SwapLRCameraIDs.Add(id);
			}
			string camstring = GetCameraInfo(selectedCamera);
			ui.ShowText(camstring);
		}

		private void ToggleSkipBlitCamera()
		{
			if (selectedCamera == null) return;
			int id = selectedCamera.GetInstanceID();
			if (settings.SkipBlitCameraIDs.Contains(id))
			{
				settings.SkipBlitCameraIDs.Remove(id);
			}
			else
			{
				settings.SkipBlitCameraIDs.Add(id);
			}
			string camstring = GetCameraInfo(selectedCamera);
			ui.ShowText(camstring);
		}

		private void RefreshCameras()
		{
			var cams = GetCameras();
			foreach (GameObject cam in cams)
			{
				ToggleStereoCamera(cam);
				ToggleStereoCamera(cam);
			}
		}

		private void ToggleStereoCamera(GameObject tcam)
		{
			if (tcam == null) return;
			int id = tcam.GetInstanceID();
			if (!settings.StereoCameraIDs.Contains(id))
			{
				//	Camera cam = tcam.GetComponent<Camera>();
				//	if (!cam.enabled)return;
				settings.StereoCameraIDs.Add(id);
				StereoizeCamera(tcam);
				string camstring = GetCameraInfo(tcam);
				ui.ShowText(camstring);
			}
			else
			{
				DeStereoizeCamera(tcam);
				string camstring = GetCameraInfo(tcam);
				ui.ShowText(camstring);
			}
		}
	}
}