/*
 *  ARToolKitFunctions.cs
 *  ARToolKit for Unity
 *
 *  This file is part of ARToolKit for Unity.
 *
 *  ARToolKit for Unity is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU Lesser General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  ARToolKit for Unity is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU Lesser General Public License for more details.
 *
 *  You should have received a copy of the GNU Lesser General Public License
 *  along with ARToolKit for Unity.  If not, see <http://www.gnu.org/licenses/>.
 *
 *  As a special exception, the copyright holders of this library give you
 *  permission to link this library with independent modules to produce an
 *  executable, regardless of the license terms of these independent modules, and to
 *  copy and distribute the resulting executable under terms of your choice,
 *  provided that you also meet, for each linked independent module, the terms and
 *  conditions of the license of that module. An independent module is a module
 *  which is neither derived from nor based on this library. If you modify this
 *  library, you may extend this exception to your version of the library, but you
 *  are not obligated to do so. If you do not wish to do so, delete this exception
 *  statement from your version.
 *
 *  Copyright 2015 Daqri, LLC.
 *  Copyright 2010-2015 ARToolworks, Inc.
 *
 *  Author(s): Philip Lamb, Julian Looser
 *
 */

using System;
using System.Runtime.InteropServices;
using System.Text;

/// <summary>
/// This class makes the functions of the ARWrapper accessible in C#. For function documentation please 
/// refer to the ARToolKitWrapperExportedAPI.h file located in %ARTOOLKIT5_ROOT%/include/ARWrapper.
/// For the implementation the start point is the coresponding ARToolKitWrapperExportedAPI.cpp file located in 
/// %ARTOOLKIT5_ROOT%/lib/SRC/ARWrapper
/// </summary>
public class ARToolKitFunctions
{
	[NonSerialized]
	private bool inited = false;

	// Delegate type declaration.
	public delegate void LogCallback([MarshalAs(UnmanagedType.LPStr)] string msg);

	// Delegate instance.
	private LogCallback logCallback = null;
	private GCHandle logCallbackGCH;

    private ARToolKitFunctions() { }

    public static ARToolKitFunctions Instance { get { return Nested.instance; } }

    private class Nested
    {
        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static Nested()
        {
        }

        internal static readonly ARToolKitFunctions instance = new ARToolKitFunctions();
    }

    public void arwRegisterLogCallback(LogCallback lcb)
	{
		if (lcb != null) {
			logCallback = lcb;
			logCallbackGCH = GCHandle.Alloc(logCallback); // Does not need to be pinned, see http://stackoverflow.com/a/19866119/316487 
		}
		else ARNativePlugin.arwRegisterLogCallback(logCallback);
		if (lcb == null) {
			logCallback = null;
			logCallbackGCH.Free();
		}
	}

	public  void arwSetLogLevel(int logLevel)
	{
		 ARNativePlugin.arwSetLogLevel(logLevel);
	}

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    static extern bool SetDllDirectory(string lpPathName);

    public  bool arwInitialiseAR(int pattSize = 16, int pattCountMax = 25)
	{
        string dllDir = "";
        dllDir += Environment.GetEnvironmentVariable("ARTOOLKIT5_ROOT");
        if (dllDir.Equals(""))
        {
            throw new System.Exception("ARToolKitCSharpIntegration.ARToolKitFunctions.arwInitialiseAR(): ARTOOLKIT5_ROOT not set. Please set ARTOOLKIT5_ROOT in your environment variables to the path where you installed ARToolKit to.");
        }
        else
        {
            SetDllDirectory(dllDir+"\\bin\\");
        }

        bool ok;
        ok = ARNativePlugin.arwInitialiseARWithOptions(pattSize, pattCountMax);
		if (ok) inited = true;
		return ok;
	}
	
	public  string arwGetARToolKitVersion()
	{
		StringBuilder sb = new StringBuilder(128);
		bool ok;
		ok = ARNativePlugin.arwGetARToolKitVersion(sb, sb.Capacity);
		if (ok) return sb.ToString();
		else return "unknown";
	}

	public  int arwGetError()
	{
	 return ARNativePlugin.arwGetError();
	}

    public  bool arwShutdownAR()
	{
		bool ok;
		 ok = ARNativePlugin.arwShutdownAR();
		if (ok) inited = false;
		return ok;
	}

    /**
     * Initialises and starts video capture.
     * @param vconf		The video configuration string
     * @param cparaName	The camera parameter file, which is used to form the projection matrix
     * @param nearPlane	The distance to the near plane of the viewing frustum formed by the camera parameters.
     * @param farPlane	The distance to the far plane of the viewing frustum formed by the camera parameters.
     * @return			true if successful, false if an error occurred
     * @see				arwStopRunning()
     */
    public bool arwStartRunning(string vconf, string cparaName, float nearPlane, float farPlane)
    {
        return ARNativePlugin.arwStartRunning(vconf, cparaName, nearPlane, farPlane);
    }

    /**
     * Initialises and starts video capture.
     * @param vconf		The video configuration string
     * @param cparaBuff	A string containing the contents of a camera parameter file, which is used to form the projection matrix.
     * @param cparaBuffLen	Number of characters in cparaBuff.
     * @param nearPlane	The distance to the near plane of the viewing frustum formed by the camera parameters.
     * @param farPlane	The distance to the far plane of the viewing frustum formed by the camera parameters.
     * @return			true if successful, false if an error occurred
     * @see				arwStopRunning()
     */
    public bool arwStartRunningB(string vconf, byte[] cparaBuff, int cparaBuffLen, float nearPlane, float farPlane)
	{
		 return ARNativePlugin.arwStartRunningB(vconf, cparaBuff, cparaBuffLen, nearPlane, farPlane);
	}

    public bool arwStartRunningStereoB(string vconfL, byte[] cparaBuffL, int cparaBuffLenL, string vconfR, byte[] cparaBuffR, int cparaBuffLenR, byte[] transL2RBuff, int transL2RBuffLen, float nearPlane, float farPlane)
	{
		return ARNativePlugin.arwStartRunningStereoB(vconfL, cparaBuffL, cparaBuffLenL, vconfR, cparaBuffR, cparaBuffLenR, transL2RBuff, transL2RBuffLen, nearPlane, farPlane);
	}

	public  bool arwIsRunning()
	{
		return ARNativePlugin.arwIsRunning();
	}

	public  bool arwStopRunning()
	{
		return ARNativePlugin.arwStopRunning();
	}

	public  bool arwGetProjectionMatrix(float[] matrix)
	{
	    return ARNativePlugin.arwGetProjectionMatrix(matrix);
	}

	public  bool arwGetProjectionMatrixStereo(float[] matrixL, float[] matrixR)
	{
		return ARNativePlugin.arwGetProjectionMatrixStereo(matrixL, matrixR);
	}

	public  bool arwGetVideoParams(out int width, out int height, out int pixelSize, out String pixelFormatString)
	{
		StringBuilder sb = new StringBuilder(128);
		bool ok;
		ok = ARNativePlugin.arwGetVideoParams(out width, out height, out pixelSize, sb, sb.Capacity);
		if (!ok) pixelFormatString = "";
		else pixelFormatString = sb.ToString();
		return ok;
	}

	public  bool arwGetVideoParamsStereo(out int widthL, out int heightL, out int pixelSizeL, out String pixelFormatL, out int widthR, out int heightR, out int pixelSizeR, out String pixelFormatR)
	{
		StringBuilder sbL = new StringBuilder(128);
		StringBuilder sbR = new StringBuilder(128);
		bool ok;
		ok = ARNativePlugin.arwGetVideoParamsStereo(out widthL, out heightL, out pixelSizeL, sbL, sbL.Capacity, out widthR, out heightR, out pixelSizeR, sbR, sbR.Capacity);
		if (!ok) {
			pixelFormatL = "";
			pixelFormatR = "";
		} else {
			pixelFormatL = sbL.ToString();
			pixelFormatR = sbR.ToString();
		}
		return ok;
	}

	public  bool arwCapture()
	{
		return ARNativePlugin.arwCapture();
	}

	public  bool arwUpdateAR()
	{
		return ARNativePlugin.arwUpdateAR();
	}
	
    public  bool arwUpdateTexture([In, Out]Color[] colors)
	{
		bool ok;
		GCHandle handle = GCHandle.Alloc(colors, GCHandleType.Pinned);
		IntPtr address = handle.AddrOfPinnedObject();
		ok = ARNativePlugin.arwUpdateTexture(address);
		handle.Free();
		return ok;
	}

	public  bool arwUpdateTextureStereo([In, Out]Color[] colorsL, [In, Out]Color[] colorsR)
	{
		bool ok;
		GCHandle handle0 = GCHandle.Alloc(colorsL, GCHandleType.Pinned);
		GCHandle handle1 = GCHandle.Alloc(colorsR, GCHandleType.Pinned);
		IntPtr address0 = handle0.AddrOfPinnedObject();
		IntPtr address1 = handle1.AddrOfPinnedObject();
		ok = ARNativePlugin.arwUpdateTextureStereo(address0, address1);
		handle0.Free();
		handle1.Free();
		return ok;
	}
	
	public  bool arwUpdateTexture32([In, Out]Color32[] colors32)
	{
		bool ok;
		GCHandle handle = GCHandle.Alloc(colors32, GCHandleType.Pinned);
		IntPtr address = handle.AddrOfPinnedObject();
		ok = ARNativePlugin.arwUpdateTexture32(address);
		handle.Free();
		return ok;
	}
	
	public  bool arwUpdateTexture32Stereo([In, Out]Color32[] colors32L, [In, Out]Color32[] colors32R)
	{
		bool ok;
		GCHandle handle0 = GCHandle.Alloc(colors32L, GCHandleType.Pinned);
		GCHandle handle1 = GCHandle.Alloc(colors32R, GCHandleType.Pinned);
		IntPtr address0 = handle0.AddrOfPinnedObject();
		IntPtr address1 = handle1.AddrOfPinnedObject();
		ok = ARNativePlugin.arwUpdateTexture32Stereo(address0, address1);
		handle0.Free();
		handle1.Free();
		return ok;
	}
	
	public  bool arwUpdateTextureGL(int textureID)
	{
		return ARNativePlugin.arwUpdateTextureGL(textureID);
	}
	
	public  bool arwUpdateTextureGLStereo(int textureID_L, int textureID_R)
	{
		return ARNativePlugin.arwUpdateTextureGLStereo(textureID_L, textureID_R);
	}

	public  void arwSetUnityRenderEventUpdateTextureGLTextureID(int textureID)
	{
		ARNativePlugin.arwSetUnityRenderEventUpdateTextureGLTextureID(textureID);
	}

	public  void arwSetUnityRenderEventUpdateTextureGLStereoTextureIDs(int textureID_L, int textureID_R)
	{
		ARNativePlugin.arwSetUnityRenderEventUpdateTextureGLStereoTextureIDs(textureID_L, textureID_R);
	}
	
	public  int arwGetMarkerPatternCount(int markerID)
	{
		return ARNativePlugin.arwGetMarkerPatternCount(markerID);
	}

	public  bool arwGetMarkerPatternConfig(int markerID, int patternID, float[] matrix, out float width, out float height, out int imageSizeX, out int imageSizeY)
	{
		return ARNativePlugin.arwGetMarkerPatternConfig(markerID, patternID, matrix, out width, out height, out imageSizeX, out imageSizeY);
	}
	
	public  bool arwGetMarkerPatternImage(int markerID, int patternID, [In, Out]Color[] colors)
	{
		bool ok;
		ok = ARNativePlugin.arwGetMarkerPatternImage(markerID, patternID, colors);
		return ok;
	}
	
	public  bool arwGetMarkerOptionBool(int markerID, int option)
	{
		 return ARNativePlugin.arwGetMarkerOptionBool(markerID, option);
	}
	
	public  void arwSetMarkerOptionBool(int markerID, int option, bool value)
	{
		 ARNativePlugin.arwSetMarkerOptionBool(markerID, option, value);
	}

	public  int arwGetMarkerOptionInt(int markerID, int option)
	{
		 return ARNativePlugin.arwGetMarkerOptionInt(markerID, option);
	}
	
	public  void arwSetMarkerOptionInt(int markerID, int option, int value)
	{
		 ARNativePlugin.arwSetMarkerOptionInt(markerID, option, value);
	}

	public  float arwGetMarkerOptionFloat(int markerID, int option)
	{
		 return ARNativePlugin.arwGetMarkerOptionFloat(markerID, option);
	}
	
	public  void arwSetMarkerOptionFloat(int markerID, int option, float value)
	{
		 ARNativePlugin.arwSetMarkerOptionFloat(markerID, option, value);
	}

	public  void arwSetVideoDebugMode(bool debug)
	{
		ARNativePlugin.arwSetVideoDebugMode(debug);
	}

	public  bool arwGetVideoDebugMode()
	{
		 return ARNativePlugin.arwGetVideoDebugMode();
	}

	public  void arwSetVideoThreshold(int threshold)
	{
		 ARNativePlugin.arwSetVideoThreshold(threshold);
	}

	public  int arwGetVideoThreshold()
	{
		 return ARNativePlugin.arwGetVideoThreshold();
	}

	public  void arwSetVideoThresholdMode(int mode)
	{
		ARNativePlugin.arwSetVideoThresholdMode(mode);
	}

	public  int arwGetVideoThresholdMode()
	{
		 return ARNativePlugin.arwGetVideoThresholdMode();
	}

	public  void arwSetLabelingMode(int mode)
	{
		 ARNativePlugin.arwSetLabelingMode(mode);
	}

	public  int arwGetLabelingMode()
	{
		 return ARNativePlugin.arwGetLabelingMode();
	}

	public  void arwSetBorderSize(float size)
	{
		 ARNativePlugin.arwSetBorderSize(size);
	}

	public  float arwGetBorderSize()
	{
		 return ARNativePlugin.arwGetBorderSize();
	}

	public  void arwSetPatternDetectionMode(int mode)
	{
		 ARNativePlugin.arwSetPatternDetectionMode(mode);
	}

	public  int arwGetPatternDetectionMode()
	{
		 return ARNativePlugin.arwGetPatternDetectionMode();
	}

	public  void arwSetMatrixCodeType(int type)
	{
		 ARNativePlugin.arwSetMatrixCodeType(type);
	}

	public  int arwGetMatrixCodeType()
	{
		 return ARNativePlugin.arwGetMatrixCodeType();
	}

	public  void arwSetImageProcMode(int mode)
	{
		 ARNativePlugin.arwSetImageProcMode(mode);
	}

	public  int arwGetImageProcMode()
	{
		 return ARNativePlugin.arwGetImageProcMode();
	}
	
	public  void arwSetNFTMultiMode(bool on)
	{
		 ARNativePlugin.arwSetNFTMultiMode(on);
	}

	public  bool arwGetNFTMultiMode()
	{
		 return ARNativePlugin.arwGetNFTMultiMode();
	}

    /// <summary>
    /// Takes the marker configuration string
    /// </summary>
    /// <param name="cfg">Sample configurations:
    /// single;data/hiro.patt;80
    /// single_buffer;80;buffer=234 221 237...
    /// single_barcode;0;80
    /// multi;data/multi/marker.dat
    /// nft;data/nft/pinball</param>
    /// <returns> marker id for further usage</returns>
    public int arwAddMarker(string cfg)
	{
		 return ARNativePlugin.arwAddMarker(cfg);
	}
	
	public  bool arwRemoveMarker(int markerID)
	{
		 return ARNativePlugin.arwRemoveMarker(markerID);
	}

	public  int arwRemoveAllMarkers()
	{
		 return ARNativePlugin.arwRemoveAllMarkers();
	}


	public  bool arwQueryMarkerVisibility(int markerID)
	{
		 return ARNativePlugin.arwQueryMarkerVisibility(markerID);
	}

	public  bool arwQueryMarkerTransformation(int markerID, float[] matrix)
	{
		 return ARNativePlugin.arwQueryMarkerTransformation(markerID, matrix);
	}

	public  bool arwQueryMarkerTransformationStereo(int markerID, float[] matrixL, float[] matrixR)
	{
		 return ARNativePlugin.arwQueryMarkerTransformationStereo(markerID, matrixL, matrixR);
	}
	
	public  bool arwLoadOpticalParams(string optical_param_name, byte[] optical_param_buff, int optical_param_buffLen, out float fovy_p, out float aspect_p, float[] m, float[] p)
	{
		 return ARNativePlugin.arwLoadOpticalParams(optical_param_name, optical_param_buff, optical_param_buffLen, out fovy_p, out aspect_p, m, p);
	}

}
