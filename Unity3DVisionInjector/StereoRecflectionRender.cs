using System;
using UnityEngine;

namespace Stereo3D
{
    public class StereoRecflectionRender : MonoBehaviour
    {

        // Use this for initialization


        void Start()
        {

        }


        private GameObject tempGameObject;
        private Camera originalCamera;

        private Camera tempCamera;


        private GameObject originalObject;
        private Camera MainCamForReflect;

        public void SetReflectionCamera(Camera mainCamera)
        {
            MainCamForReflect = mainCamera;
        }

        void Awake()
        {








        }

        public void SetCameraTarget(Camera targetCamera)
        {
            originalCamera = targetCamera;
            originalObject = originalCamera.gameObject;


          //  originalCamera.useOcclusionCulling = false;
          //  originalCamera.
            tempGameObject = new GameObject("tempGameObject");
            tempCamera = tempGameObject.AddComponent<Camera>();
            tempCamera.CopyFrom(originalCamera);
            tempCamera.enabled = false;




            initialized = true;
        }

        private bool initialized = false;



        // Update is called once per frame
        void Update()
        {
            try
            {
            

                if (!initialized)
                {

                    Console.WriteLine("Not Initialized!");
                    return;
                }




                tempCamera.CopyFrom(originalCamera);
            




            }
            catch (Exception e)
            {

                Console.WriteLine(e.Message);
            }

   
        }


 

    


        public void OnPostRender()
        {
            if (!initialized)
            {

                Console.WriteLine("Not Initialized!");
                return;
            }



       
            originalCamera.CopyFrom(tempCamera);
        }

     

    
        private Vector3 op;



        private float fudge = .117f;



        void OnPreRender()
        {

            try
            {
              

                if (!initialized)
                {
                    Console.WriteLine("Not Initialized!");
                    return;
                }

                tempCamera.CopyFrom(originalCamera);
                GetStereoMatrices(Stereo3DPlugin.isRight, tempCamera, originalCamera);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }


            /* 



              float ipd2 = Stereo3DPlugin.Separation;
              float hsep = ipd2 * .5f;
              float dist = hsep;
              if (!Stereo3DPlugin.isRight) dist *= -1;

              Vector4 right = originalCamera.worldToCameraMatrix.GetColumn(0);
              Vector4 up = originalCamera.worldToCameraMatrix.GetColumn(1);
              Vector4 forward = originalCamera.worldToCameraMatrix.GetColumn(2);
              Vector4 pos = originalCamera.worldToCameraMatrix.GetColumn(3);
              Vector4 otpos = originalCamera.transform.position;
              //   Vector4 pos = originalCamera.worldToCameraMatrix.GetPosition();

              Vector4 rightVector = right.normalized * dist; // Each unit vectors
              Vector4 opos = pos - rightVector;
              Vector4 cpos = otpos + rightVector + rightVector;
              Vector4 cameraPosition = opos - rightVector;


              Vector4 zaxis = ((opos + forward * Stereo3DPlugin.Convergence) - cameraPosition).normalized;
              Vector4 xaxis = Vector3.Cross(up, zaxis).normalized;
              Vector4 yaxis = Vector3.Cross(zaxis, xaxis).normalized;

              Matrix4x4 vm = Matrix4x4.identity;
              vm.SetColumn(0, xaxis);
              vm.SetColumn(1, yaxis);
              vm.SetColumn(2, zaxis);

              Matrix4x4 tm = Matrix4x4.identity;
              tm.SetColumn(3, -cpos);
              tm.m33 = 1;

               originalCamera.worldToCameraMatrix = vm * tm;
              */
            /*
                Matrix4x4 p = originalCamera.projectionMatrix;
                float d = dist*fudge;

                p.m02 -= d;
                //   p.m03 = dist * Stereo3DPlugin.Convergence;
                originalCamera.projectionMatrix = p;


                originalCamera.transform.position = opos;
                Matrix4x4 newvm = originalCamera.worldToCameraMatrix;
                newvm.SetColumn(3, opos);
                originalCamera.worldToCameraMatrix = newvm;
                */




        }



        public void GetStereoMatrices(bool isRightEye, Camera sourceCamera, Camera targetCamera)
        {

            float dist = Stereo3DPlugin.Separation;
            if (!isRightEye) dist *= -1;


            Matrix4x4 proj = sourceCamera.projectionMatrix;
            proj.m02 += dist;
               proj.m03= -dist * Stereo3DPlugin.Convergence;

            Matrix4x4 vmr = sourceCamera.cameraToWorldMatrix;
            vmr.m03 = 0;
            vmr.m13 = 0;
            vmr.m23 = 0;
            Vector3 vo = new Vector3(dist * Stereo3DPlugin.Convergence / proj.m00, 0, 0);
            Vector3 wo = vmr.MultiplyVector(vo);

            op = targetCamera.transform.position;
            Matrix4x4 vm = sourceCamera.worldToCameraMatrix.inverse;
            //  Vector3 rv = vm.GetPosition();
            //   rv -= rightVector;
            vm.m03 -= wo.x;
            vm.m13 -= wo.y;
            vm.m23 -= wo.z;
            vm = vm.inverse;



            targetCamera.worldToCameraMatrix = vm;
            targetCamera.transform.position = op + wo;
            targetCamera.projectionMatrix = proj;
        }




    }
}