using System;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Rendering;

namespace Stereo3D
{
	public class StereoCamera : MonoBehaviour
	{
		private Settings settings = Settings.Instance;
		public Camera originalCamera;
		private Camera stereoCamera;
		private StereoRender sr;

		private void Awake()
		{
		//	stereoCamera = gameObject.AddComponent<Camera>();
			//stereoCamera.cullingMask = 0;
			//stereoCamera.aspect = (stereoCamera.pixelWidth / (float)stereoCamera.pixelHeight) * 2f;
			//   gameObject.AddComponent<GUILayer>();

			
	
		}

		private bool initialized;
		public RenderTexture leftTex;
		public RenderTexture rightTex;
		public RenderTexture originalTargetTexture;
		private bool initialEnableStatus;

		public void SetCameraTarget(Camera targetCamera)
		{
			originalCamera = targetCamera;
			originalObject = originalCamera.gameObject;
			initialEnableStatus = originalCamera.enabled;
			//stereoCamera.depth = Camera.main.depth + originalCamera.depth;
			//stereoCamera.enabled = true;
			if (!settings.EnableSequentialMode)
			{
				if (!settings.SkipBlitCameraIDs.Contains(originalObject.GetInstanceID()))
				{
					stereoCamera = gameObject.AddComponent<Camera>();
					stereoCamera.cullingMask = 0;
					stereoCamera.aspect = (stereoCamera.pixelWidth/(float) stereoCamera.pixelHeight)*2f;
					stereoCamera.depth = Camera.main.depth + originalCamera.depth;
					leftTex = new RenderTexture(originalCamera.pixelWidth, originalCamera.pixelHeight, 24);
					rightTex = new RenderTexture(originalCamera.pixelWidth, originalCamera.pixelHeight, 24);
				stereoCamera.enabled = true;
					originalCamera.enabled = false;



					var vertices = new Vector3[4];

					vertices[0] = new Vector3(-1, 1, 0);
					vertices[1] = new Vector3(1, 1, 0);
					vertices[2] = new Vector3(-1, -1, 0);
					vertices[3] = new Vector3(1, -1, 0);

					mesh.vertices = vertices;

					var tri = new int[6];

					tri[0] = 0;
					tri[1] = 2;
					tri[2] = 1;

					tri[3] = 2;
					tri[4] = 3;
					tri[5] = 1;

					mesh.triangles = tri;

					var normals = new Vector3[4];

					normals[0] = -Vector3.forward;
					normals[1] = -Vector3.forward;
					normals[2] = -Vector3.forward;
					normals[3] = -Vector3.forward;

					mesh.normals = normals;

					var uv = new Vector2[4];

					uv[0] = new Vector2(0, 0);
					uv[1] = new Vector2(1, 0);
					uv[2] = new Vector2(0, 1);
					uv[3] = new Vector2(1, 1);

					mesh.uv = uv;

					Shader shader = UnityHelper.GetShader("StereoShader");
					if (shader == null)
					{
						Console.Write("Failed To Load Shader");
					}
					else
					{
						Console.Write("Loaded Shader");
					}
					rmat = new Material(shader);
					lmat = new Material(shader);
					
					rmat.SetTexture("mainTex", rightTex);
					lmat.SetTexture("mainTex", leftTex);
					// rmat = new Material(Shader.Find("Unlit/Texture"));

					//rmat.SetTexture("Base", leftTex);
					// lmat = new Material(Shader.Find("Unlit/Texture"));

					//lmat.SetTexture("Base", leftTex);
				}
			}

			originalTargetTexture = originalCamera.targetTexture;
			sr = originalObject.AddComponent<StereoRender>();
			initialized = true;

			Console.WriteLine("Added StereoCam to: " + originalObject.name);
		}

		public void RemoveStereoTarget()
		{
			Debug.Log("Removing Stereo from " + originalCamera.name);
			sr.RevertCamera();
			Destroy(sr);
			//	originalCamera.targetTexture = originalTargetTexture;
			Debug.Log("Reverted Texture to " + originalTargetTexture);
			originalCamera.enabled = initialEnableStatus;
			Debug.Log("Reverted enable to " + initialEnableStatus);
			Destroy(gameObject);
		}

		[DllImport("3DVisionDirectUnity")]
		private static extern IntPtr SetLeftEye();

		[DllImport("3DVisionDirectUnity")]
		private static extern IntPtr SetRightEye();

		private GameObject originalObject;
		// Update is called once per frame

		void Update()
		{
		//	if (settings.Mode2) stereoCamera.enabled = originalCamera.enabled;
		//	if (settings.SkipBlitCameraIDs.Contains(originalCamera.GetInstanceID())) stereoCamera.enabled = false;
		}
		/*  void OnPostRender()
		  {
			try
			  {
				  if (!isRight) GL.IssuePluginEvent(SetRightEye(), 1);
				  else GL.IssuePluginEvent(SetLeftEye(), 1);
			  }
			  catch (Exception e)
			  {
				  Debug.LogException(e);
			  }
		  }*/
			CommandBuffer rcommandBuffer = new CommandBuffer();
		//	CommandBuffer leftCommandBuffer = new CommandBuffer();
		Mesh mesh = new Mesh();
		private Material rmat;
		private Material lmat;
	

		private void OnPreRender()
		{
			if (settings.EnableSequentialMode)
			{
				 sr.IsRight = Stereo3DPlugin.isRight;
			}
			else
			{
			//	if (!Stereo3DPlugin.isRight)
				{
					if (!settings.SkipBlitCameraIDs.Contains(originalObject.GetInstanceID()))
					{
						originalCamera.enabled = false;
						sr.IsRight = false;
						originalCamera.targetTexture = leftTex;
						originalCamera.Render();
						sr.IsRight = true;
						originalCamera.targetTexture = rightTex;
						originalCamera.Render();
						stereoCamera.RemoveAllCommandBuffers();

					/*	stereoCamera.RemoveAllCommandBuffers();
						CommandBuffer commandBuffer = new CommandBuffer();
						commandBuffer.IssuePluginEvent(SetRightEye(), 1);
					
						commandBuffer.DrawMesh(mesh,Matrix4x4.identity,rmat );
						//commandBuffer.Blit(rightTex, BuiltinRenderTextureType.CurrentActive);
						commandBuffer.IssuePluginEvent(SetLeftEye(), 1);
						commandBuffer.DrawMesh(mesh, Matrix4x4.identity, lmat);
					//	commandBuffer.Blit(rightTex, BuiltinRenderTextureType.CameraTarget);
						stereoCamera.AddCommandBuffer(CameraEvent.AfterEverything, commandBuffer);
						*/
						
						rcommandBuffer = new CommandBuffer();
						rcommandBuffer.IssuePluginEvent(SetRightEye(), 1);
						rcommandBuffer.DrawMesh(mesh, Matrix4x4.identity, rmat);
						CommandBuffer	lcommandBuffer = new CommandBuffer();
						lcommandBuffer.IssuePluginEvent(SetLeftEye(), 1);
						lcommandBuffer.DrawMesh(mesh, Matrix4x4.identity, lmat);
						//CommandBuffer rbcommandBuffer = new CommandBuffer();
						// rbcommandBuffer.Blit(rightTex, BuiltinRenderTextureType.None);
						//CommandBuffer lbcommandBuffer = new CommandBuffer();

						//lbcommandBuffer.Blit(leftTex, BuiltinRenderTextureType.None);
						stereoCamera.AddCommandBuffer(CameraEvent.AfterImageEffects, lcommandBuffer);
						stereoCamera.AddCommandBuffer(CameraEvent.AfterEverything, rcommandBuffer);
						//stereoCamera.AddCommandBuffer(CameraEvent.AfterImageEffects, rbcommandBuffer);
					
					//	stereoCamera.AddCommandBuffer(CameraEvent.AfterEverything, lbcommandBuffer);
					}
				}
			}
		}
/*
		private void OnRenderImage(RenderTexture source, RenderTexture destination)

		{
			if (!initialized)
			{
				Graphics.Blit(source, destination);
				Console.WriteLine("Not Initialized!");
				return;
			}
			try
			{
				if (settings.EnableSequentialMode)
				{
					Graphics.Blit(source, destination);
					//if (Stereo3DPlugin.isRight) Graphics.Blit(sr.rightTex, destination);
					//else Graphics.Blit(sr.leftTex, destination);
				}
				else
				{
					if (settings.SkipBlitCameraIDs.Contains(originalObject.GetInstanceID()))
					{
						Graphics.Blit(source, destination);
						
					}
					else
					{
						if (Stereo3DPlugin.isRight) Graphics.Blit(rightTex, destination);
						else Graphics.Blit(leftTex, destination);
					}
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
			}
		}*/
	}
}