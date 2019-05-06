using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using UnityEngine;

public class Settings
{
	private static Settings _instance;
	public static Settings Instance
	{
		get
		{
			if(_instance==null) _instance = new Settings();
			return _instance;
		}
	}
    private IniFile iniFile;
    private float _separation = .06f;
    private float _convergence = -10.87f;
	private float UIDisplayTime = 10;
	private KeyCode _previousCameraHotKey = KeyCode.Alpha7;
	private KeyCode _nextCameraHotKey = KeyCode.Alpha8;
	private KeyCode _listAllCamerasHotKey = KeyCode.Alpha6;
	private KeyCode _toggleStereoCameraHotKey = KeyCode.Alpha9;
	private KeyCode _toggleSkipBlitCameraHotKey = KeyCode.Alpha0;
	private KeyCode _toggleSwapLRCameraHotKey = KeyCode.P;
	private KeyCode _increaseSeparationHotKey = KeyCode.Alpha2;
    private KeyCode _decreaseSeparationHotKey = KeyCode.Alpha1;
    private KeyCode _increaseConvergenceHotKey = KeyCode.Alpha4;
    private KeyCode _decreaseConvergenceHotKey = KeyCode.Alpha3;
    private KeyCode _saveSettingsHotKey = KeyCode.Alpha5;
	private bool _overrideResolution = false;
	private int _width =1280;
	private int _heigth = 720;
	private int _updateHz = 120;
	private bool _enableSequentialMode = false;
	List<int> _stereoCameraIDs = new List<int>();
	List<int> _skipBlitCameraIDs = new List<int>();
	List<int> _swapLRCameraIDs = new List<int>();
	List<string> _stereoCameraNames = new List<string>();
	private Settings()
    {
        LoadSettings();
    }


    public KeyCode IncreaseSeparationHotKey
    {
        get { return _increaseSeparationHotKey; }
        set { _increaseSeparationHotKey = value; }
    }

    public KeyCode DecreaseSeparationHotKey
    {
        get { return _decreaseSeparationHotKey; }
        set { _decreaseSeparationHotKey = value; }
    }

    public KeyCode IncreaseConvergenceHotKey
    {
        get { return _increaseConvergenceHotKey; }
        set { _increaseConvergenceHotKey = value; }
    }

    public KeyCode DecreaseConvergenceHotKey
    {
        get { return _decreaseConvergenceHotKey; }
        set { _decreaseConvergenceHotKey = value; }
    }

    public KeyCode SaveSettingsHotKey
    {
        get { return _saveSettingsHotKey; }
        set { _saveSettingsHotKey = value; }
    }

    public float Separation
    {
        get { return _separation; }
        set { _separation = Mathf.Clamp(value, 0, 20); }
    }

    public float Convergence
    {
        get { return _convergence; }
        set { _convergence = value; }
    }

	public bool OverrideResolution
	{
		get { return _overrideResolution; }
		set { _overrideResolution = value; }
	}

	public int Width
	{
		get { return _width; }
		set { _width = value; }
	}

	public int Heigth
	{
		get { return _heigth; }
		set { _heigth = value; }
	}

	public int UpdateHz
	{
		get { return _updateHz; }
		set { _updateHz = value; }
	}

	public List<int> StereoCameraIDs
	{
		get { return _stereoCameraIDs; }
		set { _stereoCameraIDs = value; }
	}

	public KeyCode ListAllCamerasHotKey
	{
		get { return _listAllCamerasHotKey; }
		set { _listAllCamerasHotKey = value; }
	}

	public KeyCode NextCameraHotKey
	{
		get { return _nextCameraHotKey; }
		set { _nextCameraHotKey = value; }
	}

	public KeyCode PreviousCameraHotKey
	{
		get { return _previousCameraHotKey; }
		set { _previousCameraHotKey = value; }
	}

	public KeyCode ToggleStereoCameraHotKey
	{
		get { return _toggleStereoCameraHotKey; }
		set { _toggleStereoCameraHotKey = value; }
	}

	public float UiDisplayTime
	{
		get { return UIDisplayTime; }
		set { UIDisplayTime = value; }
	}

	public List<string> StereoCameraNames
	{
		get { return _stereoCameraNames; }
		set { _stereoCameraNames = value; }
	}

	public bool EnableSequentialMode
	{
		get { return _enableSequentialMode; }
		set { _enableSequentialMode = value; }
	}

	public List<int> SkipBlitCameraIDs
	{
		get { return _skipBlitCameraIDs; }
		set { _skipBlitCameraIDs = value; }
	}

	public KeyCode ToggleSkipBlitCameraHotKey
	{
		get { return _toggleSkipBlitCameraHotKey; }
		set { _toggleSkipBlitCameraHotKey = value; }
	}

	public List<int> SwapLRCameraIDs
	{
		get { return _swapLRCameraIDs; }
		set { _swapLRCameraIDs = value; }
	}

	public KeyCode ToggleSwapLRCameraHotKey
	{
		get { return _toggleSwapLRCameraHotKey; }
		set { _toggleSwapLRCameraHotKey = value; }
	}

	//   string iniPath = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location)+"\\Unity3DVisionPlugin.ini";
    public void LoadSettings()
    {
        string assemblyFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
		Console.WriteLine("assemblyFolder Path =" + assemblyFolder);
		string iniPath = Path.Combine(assemblyFolder, "Unity3DVisionPlugin.ini");
		Console.WriteLine("Ini File Path ="+iniPath);
		iniFile = new IniFile(iniPath);
        if (!File.Exists(iniPath))
        {
            CreateIni();
            return;
        }
        iniFile = new IniFile(iniPath);

        string ish = iniFile.Read("HotKeys", "IncreaseSeparation");
        if (!string.IsNullOrEmpty(ish)) IncreaseSeparationHotKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), ish, true);
        string dsh = iniFile.Read("HotKeys", "DecreaseSeparation");
        if (!string.IsNullOrEmpty(dsh)) DecreaseSeparationHotKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), dsh, true);
        string ich = iniFile.Read("HotKeys", "IncreaseConvergence");
        if (!string.IsNullOrEmpty(ich)) IncreaseConvergenceHotKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), ich, true);
        string dch = iniFile.Read("HotKeys", "DecreaseConvergence");
        if (!string.IsNullOrEmpty(dch)) DecreaseConvergenceHotKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), dch, true);
        string ssh = iniFile.Read("HotKeys", "SaveSettings");
        if (!string.IsNullOrEmpty(ssh)) SaveSettingsHotKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), ssh, true);
		string lchk = iniFile.Read("HotKeys", "ListCameras");
		if (!string.IsNullOrEmpty(lchk)) ListAllCamerasHotKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), lchk, true);
		string pchk = iniFile.Read("HotKeys", "PreviousCamera");
		if (!string.IsNullOrEmpty(pchk)) PreviousCameraHotKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), pchk, true);
		string nchk = iniFile.Read("HotKeys", "NextCamera");
		if (!string.IsNullOrEmpty(nchk)) NextCameraHotKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), nchk, true);
		string tschk = iniFile.Read("HotKeys", "ToggleStereoCamera");
		if (!string.IsNullOrEmpty(tschk)) ToggleStereoCameraHotKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), tschk, true);
		string skip = iniFile.Read("HotKeys", "ToggleSkipBlitCamera");
		if (!string.IsNullOrEmpty(skip)) ToggleSkipBlitCameraHotKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), skip, true);
		string swap = iniFile.Read("HotKeys", "ToggleSwapLRCamera");
		if (!string.IsNullOrEmpty(swap)) ToggleSwapLRCameraHotKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), swap, true);

		string uidt = iniFile.Read("General", "UIDisplayTime");
		if (!string.IsNullOrEmpty(uidt)) float.TryParse(uidt, out UIDisplayTime);

		string sep = iniFile.Read("Stereo", "Separation");
        if (!string.IsNullOrEmpty(sep)) float.TryParse(sep, out _separation);
        string con = iniFile.Read("Stereo", "Convergence");
        if (!string.IsNullOrEmpty(con)) float.TryParse(con, out _convergence);
		string cams = iniFile.Read("Stereo", "StereoCameraIDs");
	    if (!string.IsNullOrEmpty(cams))
	    {
		    string[] scams = cams.Split(',');
		    foreach (string cam in scams)
		    {
			    string tc = cam.Trim();
				int camnum = -1;
			    bool suc = int.TryParse(tc, out camnum);
			    if (suc)
			    {
				    if(!StereoCameraIDs.Contains(camnum)) StereoCameraIDs.Add(camnum);
			    }
		    }
	    }

		string camss = iniFile.Read("Stereo", "SkipBlitCameraIDs");
		if (!string.IsNullOrEmpty(camss))
		{
			string[] scams = camss.Split(',');
			foreach (string cam in scams)
			{
				string tc = cam.Trim();
				int camnum = -1;
				bool suc = int.TryParse(tc, out camnum);
				if (suc)
				{
					if (!SkipBlitCameraIDs.Contains(camnum)) SkipBlitCameraIDs.Add(camnum);
				}
			}
		}

		string camssa = iniFile.Read("Stereo", "SwapLRCameraIDs");
		if (!string.IsNullOrEmpty(camssa))
		{
			string[] scams = camssa.Split(',');
			foreach (string cam in scams)
			{
				string tc = cam.Trim();
				int camnum = -1;
				bool suc = int.TryParse(tc, out camnum);
				if (suc)
				{
					if (!SwapLRCameraIDs.Contains(camnum)) SwapLRCameraIDs.Add(camnum);
				}
			}
		}

		cams = iniFile.Read("Stereo", "StereoCameraNames");
		if (!string.IsNullOrEmpty(cams))
		{
			string[] scams = cams.Split(',');
			foreach (string cam in scams)
			{
				string tc = cam.Trim();
			
				
					if (!StereoCameraNames.Contains(tc)) StereoCameraNames.Add(tc);
				
			}
		}

		string mode = iniFile.Read("Stereo", "Mode2");
		if (!string.IsNullOrEmpty(con)) bool.TryParse(mode, out _enableSequentialMode);

		string ow = iniFile.Read("Override", "OverrideWidth");
		if (!string.IsNullOrEmpty(sep)) int.TryParse(ow, out _width);
		string oh = iniFile.Read("Override", "OverrideHeight");
		if (!string.IsNullOrEmpty(con)) int.TryParse(oh, out _heigth);
		string uh = iniFile.Read("Override", "UpdateHz");
		if (!string.IsNullOrEmpty(con)) int.TryParse(uh, out _updateHz);
		string or = iniFile.Read("Override", "OverrideResolution");
		if (!string.IsNullOrEmpty(con)) bool.TryParse(or, out _overrideResolution);
	}

    public void SaveSettings()
    {
        iniFile.Write("Stereo", "Separation", Separation.ToString());
        iniFile.Write("Stereo", "Convergence", Convergence.ToString());
		StringBuilder sb = new StringBuilder();
	    bool first = false;
	    foreach (int cam in StereoCameraIDs)
	    {
		    if (first) sb.Append(",");
		    first = true;
		    sb.Append(cam);
	    }
		iniFile.Write("Stereo", "StereoCameraIDs", sb.ToString());

		 first = false;
		sb = new StringBuilder();
		foreach (int cam in SkipBlitCameraIDs)
		{
			if (first) sb.Append(",");
			first = true;
			sb.Append(cam);
		}
		iniFile.Write("Stereo", "SkipBlitCameraIDs", sb.ToString());
		first = false;
		sb = new StringBuilder();
		foreach (int cam in SwapLRCameraIDs)
		{
			if (first) sb.Append(",");
			first = true;
			sb.Append(cam);
		}
		iniFile.Write("Stereo", "SwapLRCameraIDs", sb.ToString());
		sb = new StringBuilder();
		 first = false;
		foreach (string cam in StereoCameraNames)
		{
			if (first) sb.Append(",");
			first = true;
			sb.Append(cam);
		}
		iniFile.Write("Stereo", "StereoCameraNames", sb.ToString());
		iniFile.Write("Stereo", "Mode2", EnableSequentialMode.ToString());
	}

    public void CreateIni()
    {
        iniFile.Write("Stereo", "Separation", Separation.ToString());
        iniFile.Write("Stereo", "Convergence", Convergence.ToString());
		iniFile.Write("Stereo", "StereoCameraIDs", "");
		iniFile.Write("Stereo", "StereoCameraNames", "");
		iniFile.Write("Stereo", "SkipBlitCameraIDs", "");
		iniFile.Write("Stereo", "SwapLRCameraIDs", "");
		iniFile.Write("Stereo", "Mode2", EnableSequentialMode.ToString());

		iniFile.Write("HotKeys", "IncreaseSeparation", IncreaseSeparationHotKey.ToString());
        iniFile.Write("HotKeys", "DecreaseSeparation", DecreaseSeparationHotKey.ToString());
        iniFile.Write("HotKeys", "IncreaseConvergence", IncreaseConvergenceHotKey.ToString());
        iniFile.Write("HotKeys", "DecreaseConvergence", DecreaseConvergenceHotKey.ToString());
        iniFile.Write("HotKeys", "SaveSettings", SaveSettingsHotKey.ToString());
		iniFile.Write("HotKeys", "ListCameras",ListAllCamerasHotKey.ToString());
		iniFile.Write("HotKeys", "PreviousCamera", PreviousCameraHotKey.ToString());
		iniFile.Write("HotKeys", "NextCamera", NextCameraHotKey.ToString());
		iniFile.Write("HotKeys", "ToggleStereoCamera", ToggleStereoCameraHotKey.ToString());
		iniFile.Write("HotKeys", "ToggleSkipBlitCamera", ToggleSkipBlitCameraHotKey.ToString());
		iniFile.Write("HotKeys", "ToggleSwapLRCamera", ToggleSwapLRCameraHotKey.ToString());
		iniFile.Write("Override", "OverrideResolution", OverrideResolution.ToString());
		iniFile.Write("Override", "OverrideWidth", Width.ToString());
		iniFile.Write("Override", "OverrideHeight", Heigth.ToString());
		iniFile.Write("Override", "UpdateHz", UpdateHz.ToString());
		iniFile.Write("General", "UIDisplayTime", UIDisplayTime.ToString());
	}
}