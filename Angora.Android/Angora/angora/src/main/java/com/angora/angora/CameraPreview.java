package com.angora.angora;

import android.content.Context;
import android.hardware.Camera;
import android.view.SurfaceHolder;
import android.view.SurfaceView;


import java.io.IOException;
import java.util.List;

public class CameraPreview extends SurfaceView implements SurfaceHolder.Callback{

    private SurfaceHolder mSurfaceHolder;
    private Camera mCamera;
    private boolean isPaused;

    public int getCameraNum() {
        return cameraNum;
    }

    public void setCameraNum(int cameraNum) {
        this.cameraNum = cameraNum;
    }

    private int cameraNum;

    public CameraPreview(Context context, int cameraNum) {
        super(context);
        this.cameraNum = cameraNum;
        mCamera = getCameraInstance(this.cameraNum);

        mSurfaceHolder = getHolder();
        mSurfaceHolder.addCallback(this);

        isPaused = false;
    }


    @Override
    public void surfaceCreated(SurfaceHolder surfaceHolder) {
        if (isPaused){
            mCamera = getCameraInstance(cameraNum);
            isPaused = false;
        }

        try {
            mCamera.setPreviewDisplay(surfaceHolder);
            //mCamera.startPreview();
        } catch (IOException e) {

        }

        Camera.Parameters parameters = mCamera.getParameters();
        List<Camera.Size> previewSizes = parameters.getSupportedPreviewSizes();
        Camera.Size previewSize = previewSizes.get(4); //480h x 720w

        parameters.setPreviewSize(previewSize.width, previewSize.height);
        parameters.setFlashMode(Camera.Parameters.FLASH_MODE_AUTO);
        parameters.setFocusMode(Camera.Parameters.FOCUS_MODE_AUTO);

        mCamera.setParameters(parameters);

        //Display display = ((WindowManager)getSystemService(Context.WINDOW_SERVICE)).getDefaultDisplay();
        /*
        Configuration config = getResources().getConfiguration();
        int orientation = config.orientation;
        if(orientation == config.ORIENTATION_PORTRAIT) {
            mCamera.setDisplayOrientation(90);
        } else if(orientation == config.ORIENTATION_LANDSCAPE) {
            mCamera.setDisplayOrientation(180);
        }
        */

        mCamera.startPreview();


    }

    @Override
    public void surfaceChanged(SurfaceHolder surfaceHolder, int i, int i2, int i3) {
        /*
        if (mSurfaceHolder.getSurface() != null){
            return;
        }

        try{
            mCamera.stopPreview();
        }catch (Exception e){
            //ignore
        }

        try {
            mCamera.setPreviewDisplay(mSurfaceHolder);
            mCamera.startPreview();
        } catch (Exception e){
            //BAD THING
        }
        */
        Camera.Parameters parameters = mCamera.getParameters();
        List<Camera.Size> previewSizes = parameters.getSupportedPreviewSizes();
        Camera.Size previewSize = previewSizes.get(4); //480h x 720w

        parameters.setPreviewSize(previewSize.width, previewSize.height);
        parameters.setFlashMode(Camera.Parameters.FLASH_MODE_AUTO);
        parameters.setFocusMode(Camera.Parameters.FOCUS_MODE_AUTO);

        mCamera.setParameters(parameters);
    /*
        //Display display = ((WindowManager)getSystemService(Context.WINDOW_SERVICE)).getDefaultDisplay();
        Configuration config = getResources().getConfiguration();
        int orientation = config.orientation;
        if(orientation == config.ORIENTATION_PORTRAIT) {
            mCamera.setDisplayOrientation(90);
        } else if(orientation == config.ORIENTATION_LANDSCAPE) {
            mCamera.setDisplayOrientation(180);
        }
*/
        mCamera.startPreview();
    }

    @Override
    public void surfaceDestroyed(SurfaceHolder surfaceHolder) {
        if (mCamera != null){
            mCamera.stopPreview();
            mCamera.release();
        }
        isPaused = true;
    }


    public Camera getCameraInstance(int id){
        Camera c = null;
        try {
            c = Camera.open(id); // attempt to get a Camera instance
        }
        catch (Exception e){
            // Camera is not available (in use or does not exist)
        }
        return c; // returns null if camera is unavailable
    }

    public void refresh(){
        if (mCamera != null){
            mCamera.stopPreview();
            mCamera.release();
            mCamera = getCameraInstance(cameraNum);

            try {
                mCamera.setPreviewDisplay(mSurfaceHolder);
                mCamera.startPreview();
            } catch (IOException e) {

            }
        }
    }

}
