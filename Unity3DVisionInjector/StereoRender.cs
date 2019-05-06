using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace Stereo3D
{
    public class StereoRender : MonoBehaviour
    {
        // Use this for initialization

      //  public RenderTexture leftTex;
       // public RenderTexture rightTex;
		Settings settings = Settings.Instance;

	    public bool IsRight
	    {
		    get {
			    if (settings.EnableSequentialMode)
			    {
					if (settings.SkipBlitCameraIDs.Contains(originalObject.GetInstanceID()))
						return Stereo3DPlugin.isRight;
			    }
				return _isRight; }
		    set { _isRight = value; }
	    }

	
        private GameObject tempGameObject;
        private Camera originalCamera;

        private Camera tempCamera;

        private GameObject originalObject;

         void Start()
        {
            Camera thisCamera = GetComponent<Camera>();
            if (thisCamera != null) SetCameraTarget(thisCamera);
        }

	    public bool _isRight;
        public void SetCameraTarget(Camera targetCamera)
        {
			if(targetCamera==null)return;
            originalCamera = targetCamera;
			   originalObject = originalCamera.gameObject;

			/*    var comps = originalCamera.gameObject.GetComponents<Component>();
				foreach (var comp in comps)
				{
					//  Console.WriteLine("old: " + comp.name + comp.GetType());
				}

			  //  DeleteFX(originalCamera.gameObject);

				comps = originalCamera.gameObject.GetComponents<Component>();
				foreach (var comp in comps)
				{
					//  Console.WriteLine("new: " + comp.name + comp.GetType());
				}
				*/
			//if (settings.Mode2) leftTex = new RenderTexture(originalCamera.pixelWidth, originalCamera.pixelHeight, 24);
			//if (settings.Mode2) rightTex = new RenderTexture(originalCamera.pixelWidth, originalCamera.pixelHeight, 24);

            tempGameObject = new GameObject("tempGameObject");
            tempCamera = tempGameObject.AddComponent<Camera>();
            tempCamera.CopyFrom(originalCamera);
            tempCamera.enabled = false;
	

		
			//   Console.WriteLine("Awakened");

			initialized = true;
        }

        private bool initialized = false;
	    public bool SkipStereoMatrix;
     //   public static came unmodifiedCamera ;
        // Update is called once per frame
        private void OnPreRender()
        {
            try
            {
              

                if (!initialized)
                {
                 //   Console.WriteLine("Not Initialized!");
                    return;
                }
				if(originalCamera==null)return;
				if(tempCamera==null)return;
                tempCamera.CopyFrom(originalCamera);
	            if (!SkipStereoMatrix)
	            {
					if(settings.SwapLRCameraIDs.Contains(originalCamera.gameObject.GetInstanceID()))  GetStereoMatrices(!IsRight, tempCamera, originalCamera);
					else  GetStereoMatrices(IsRight, tempCamera, originalCamera);
				}
				//   if (IsRight) originalCamera.targetTexture = rightTex;
				//   else originalCamera.targetTexture = leftTex;

				/*	originalCamera.RemoveAllCommandBuffers();
	            if (IsRight)
	            {
					CommandBuffer rightCommandBuffer = new CommandBuffer();
					rightCommandBuffer.Blit(BuiltinRenderTextureType.CurrentActive, rightTex);
					originalCamera.AddCommandBuffer(CameraEvent.AfterEverything,  rightCommandBuffer);
				}
	            else
	            {
					CommandBuffer leftCommandBuffer = new CommandBuffer();
					leftCommandBuffer.Blit(BuiltinRenderTextureType.CurrentActive, leftTex);
					originalCamera.AddCommandBuffer(CameraEvent.AfterEverything, leftCommandBuffer);
				}*/

			}
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            //	cam.Render();
        }

  

        private void DeleteFX(GameObject source)
        {
            // Rebuild
            foreach (var fx in source.GetComponents())
            {
                if (fx.GetType().Name.Contains("D11PDTemporalReprojection3")) DestroyImmediate(fx);
            }
        }

     /*   private void OnRenderImage(RenderTexture source, RenderTexture destination)

        {
            //  if (DontRender) return;
            if (!initialized)
            {
                Console.WriteLine("Not Initialized!");
                return;
            }
            try
            {
	            if (settings.Mode2)
	            {
		            if (!settings.SkipBlitCameraIDs.Contains(originalCamera.gameObject.GetInstanceID()))
		            {
			            if (IsRight) Graphics.Blit(source, rightTex);
			            else Graphics.Blit(source, leftTex);
		            }
	            }
	            Graphics.Blit(source, destination);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }*/
		
        public void OnPostRender()
        {
	        if (!initialized)
            {
                return;
            }


	        RevertCamera();
        }

	    public void RevertCamera()
		{
			if (originalCamera == null) return;
			if (tempCamera == null) return;
			originalCamera.CopyFrom(tempCamera);
	    }

	    //  Matrix4x4 newProjMatrix = Matrix4x4.identity;
        //  Matrix4x4 oriProjMatrix = Matrix4x4.identity;

        private Matrix4x4 nvm = Matrix4x4.identity;
        private Vector3 op;

        public void GetStereoMatrices(bool isRightEye, Camera sourceCamera, Camera targetCamera)
        {
   
            float dist = Stereo3DPlugin.Separation;
            if (!isRightEye) dist *= -1;


            Matrix4x4 proj = sourceCamera.projectionMatrix;
            proj.m02 += dist;
         //   proj.m03= -dist * Stereo3DPlugin.Convergence;

            Matrix4x4 vmr = sourceCamera.cameraToWorldMatrix;
            vmr.m03 = 0;
            vmr.m13 = 0;
            vmr.m23 = 0;
            Vector3 vo = new Vector3(dist * Stereo3DPlugin.Convergence/proj.m00,0,0);
            Vector3 wo = vmr.MultiplyVector(vo);
      
            op = targetCamera.transform.position;
            Matrix4x4 vm = sourceCamera.worldToCameraMatrix.inverse;
            //  Vector3 rv = vm.GetPosition();
            //   rv -= rightVector;
            vm.m03 += wo.x;
            vm.m13 += wo.y;
            vm.m23 += wo.z;
            vm = vm.inverse;

  

            targetCamera.worldToCameraMatrix = vm;
            targetCamera.transform.position = op + wo;
            targetCamera.projectionMatrix = proj;
        }

        public void GetStereoMatrices2(bool isRightEye, Camera sourceCamera, Camera targetCamera)
        {
            float fovRadians = Mathf.Deg2Rad * sourceCamera.fieldOfView;
            float widthdiv2 = sourceCamera.nearClipPlane * Mathf.Tan(fovRadians / 2); // aperture in radians
            //  float ipd2 = (Convergence/30)*Separation;
            float ipd2 = Stereo3DPlugin.Separation;
            float hsep = ipd2 * .5f;
            float dist = hsep;
            if (isRightEye) dist *= -1;
            float fo = Stereo3DPlugin.Convergence; // delta.Length;

            Vector3 rightVector = sourceCamera.transform.right.normalized * dist; // Each unit vectors

            float top = widthdiv2;
            float bottom = -widthdiv2;

            float left = -sourceCamera.aspect * widthdiv2 - dist * sourceCamera.nearClipPlane / fo;
            float right = sourceCamera.aspect * widthdiv2 - dist * sourceCamera.nearClipPlane / fo;

            Matrix4x4 proj;
            proj = PerspectiveOffCenter(left, right, bottom, top, sourceCamera.nearClipPlane, sourceCamera.farClipPlane);
            op = targetCamera.transform.position;
            Matrix4x4 vm = sourceCamera.worldToCameraMatrix.inverse;
            //  Vector3 rv = vm.GetPosition();
            //   rv -= rightVector;
            vm.m03 += rightVector.x;
            vm.m13 += rightVector.y;
            vm.m23 += rightVector.z;
            vm = vm.inverse;

            float width = (right - left);
            float height = (top - bottom);
            targetCamera.aspect = width / height;
            float fov = (Mathf.Atan(top / targetCamera.nearClipPlane) - Mathf.Atan(bottom / targetCamera.nearClipPlane)) *
                        57.2957795f;
            targetCamera.fieldOfView = fov;

            targetCamera.worldToCameraMatrix = vm;
            targetCamera.transform.position = op + rightVector;
            targetCamera.projectionMatrix = proj;
        }

        private static Matrix4x4 PerspectiveOffCenter(float left, float right, float bottom, float top, float near, float far)
        {
            Matrix4x4 m = new Matrix4x4();
            m[0, 0] = 2f * near / (right - left);
            m[0, 1] = 0;
            m[0, 2] = (right + left) / (right - left);
            m[0, 3] = 0;
            m[1, 0] = 0;
            m[1, 1] = 2f * near / (top - bottom);
            m[1, 2] = (top + bottom) / (top - bottom);
            m[1, 3] = 0;
            m[2, 0] = 0;
            m[2, 1] = 0;
            m[2, 2] = -(far + near) / (far - near);
            m[2, 3] = -(2f * far * near) / (far - near);
            m[3, 0] = 0;
            m[3, 1] = 0;
            m[3, 2] = -1;
            m[3, 3] = 0;
            return m;
        }
    }
}