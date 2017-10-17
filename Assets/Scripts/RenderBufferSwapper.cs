using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class RenderBufferSwapper : MonoBehaviour {
    /*
    public List<RenderTexture> buffers;
	public int index;
	public long seenFrames;
	public Material cameraViewMaterial;
    public RawImage rawOutputDisplay;
    ,,
    0.26 spill
    0.15 tol
    0.32 threshold
    alpha blur
    */ 
    [RangeAttribute(15, 60)]
    public float cameraFPS;
    [RangeAttribute(0, 1)]
    public float cameraOffset;
    public Material targetMaterial;
    public Camera fullCamera;
    public Camera frontCamera;
	public Camera stencilCamera;
    public Camera lightCamera;
	public string materialFullFieldName;
	public string materialFrontFieldName;
	public string materialWebcamFieldName;
    public string materialWebcamMaskFieldName;
    public string materialLightFieldName;


    private float frameWindow;
    private float delay;
    private int IntDelay { get { return (int)delay;} }
    private float frameDelay;
    private int IntFrameDelay { get { return (int)frameDelay;} }
    private float fractionDelay;
    private float innerTimer;
    private float absoluteTimer;
    private float initialDelay;
    private List<RenderTexture> colorBuffers;
    private List<RenderTexture> alphaBuffers;
	private List<RenderTexture> stencilBuffers;
    private List<RenderTexture> lightBuffers;
	public WebcamEnabler webcamEnabler;
    private WebCamTexture webcamTexture;
    public Material unlit3Display;
    public RenderTexture greenscreenResult;
    public int index;


    void Start () {
        frameWindow = 1.0f / cameraFPS;
        delay = cameraOffset / frameWindow;
        frameDelay = (int) delay * frameWindow;
        fractionDelay = delay % 1 * frameWindow;

        innerTimer = 0.0f;
        absoluteTimer = 0.0f;
        initialDelay = frameDelay + fractionDelay;


        greenscreenResult = new RenderTexture(Screen.width, Screen.height, 0);
        greenscreenResult.name = "Greenscreen Result (Generated)";
        unlit3Display.mainTexture = greenscreenResult;
		RebuildRenderBuffers();
        ResetWebcam();
	}
	
	void Update () {
        innerTimer += Time.deltaTime;
        absoluteTimer += Time.deltaTime;
        var localTime = innerTimer - fractionDelay;
        if(localTime < frameWindow || absoluteTimer < initialDelay) {
            return;
        }

        SwapRenderBuffer();
        Graphics.Blit(targetMaterial.GetTexture("_MainTex"), greenscreenResult, targetMaterial);
        innerTimer %= frameWindow;
        absoluteTimer = absoluteTimer % (2f) + initialDelay;
	}

    void SwapRenderBuffer() {
        index = index % IntDelay;
        if(fullCamera) {
            fullCamera.targetTexture = colorBuffers[index];       
            var frameTex = colorBuffers[(index + 1) % IntDelay];
            targetMaterial.SetTexture(materialFullFieldName, frameTex);
        }

        if(frontCamera) {
            frontCamera.targetTexture = alphaBuffers[index];
            var alphaTex = alphaBuffers[(index + 1) % IntDelay];
            targetMaterial.SetTexture(materialFrontFieldName, alphaTex);
        }

        if(stencilCamera) {
            stencilCamera.targetTexture = stencilBuffers[index];
            var stencilTex = stencilBuffers[(index + 1) % IntDelay];
            targetMaterial.SetTexture(materialWebcamMaskFieldName, stencilTex);
        }

        if(lightCamera) {
            lightCamera.targetTexture = lightBuffers[index];
            var lightTex = lightBuffers[(index + 1) % IntDelay];
            targetMaterial.SetTexture(materialLightFieldName, lightTex);
        }

        index++;

        if(webcamTexture != webcamEnabler.webcamTexture) {
            ResetWebcam();
        }
    }

    public void ResetWebcam() {
        webcamTexture = webcamEnabler.webcamTexture;
        targetMaterial.SetTexture(materialWebcamFieldName, webcamTexture);
    }

	void RebuildRenderBuffers() {
        colorBuffers = new List<RenderTexture>();
        alphaBuffers = new List<RenderTexture>();
		stencilBuffers = new List<RenderTexture>();
        lightBuffers = new List<RenderTexture>();
		for(int i = 0; i < IntDelay; i++) {
            var cBuf = new RenderTexture(Screen.width, Screen.height, 16, RenderTextureFormat.ARGB32);
            cBuf.name = "Color Buffer " + i;
            colorBuffers.Add(cBuf);

            var aBuf = new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.ARGB32);
            aBuf.name = "Front Buffer " + i;
            alphaBuffers.Add(aBuf);

			var sBuf = new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.ARGB32);
			sBuf.name = "Stencil Buffer " + i;
			stencilBuffers.Add(sBuf);

            var lBuf = new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.ARGB32);
            lBuf.name = "Light Buffer " + 1;
            lightBuffers.Add(lBuf);
		}
		Debug.Log("Rebuilt buffers");
	}
}