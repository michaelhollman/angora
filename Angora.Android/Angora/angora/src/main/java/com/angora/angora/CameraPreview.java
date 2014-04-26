package com.angora.angora;

import android.content.Context;
import android.hardware.Camera;
import android.util.Log;
import android.view.SurfaceHolder;
import android.view.SurfaceView;


import java.io.IOException;
import java.util.List;

public class CameraPreview extends SurfaceView implements SurfaceHolder.Callback{

    private SurfaceHolder mSurfaceHolder;
    private Camera mCamera;
    private boolean isPaused;
    private int cameraNum;

    public int getCameraNum() {
        return cameraNum;
    }

    public void setCameraNum(int cameraNum) {
        this.cameraNum = cameraNum;
    }

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
        if (isPaused || mCamera == null){
            mCamera = getCameraInstance(cameraNum);
            isPaused = false;
        }

        try {
            mCamera.setPreviewDisplay(surfaceHolder);
            //mCamera.startPreview();
        } catch (IOException e) {
            //todo handle
        }

        Camera.Parameters parameters = mCamera.getParameters();
        parameters.setFlashMode(Camera.Parameters.FLASH_MODE_AUTO);
        parameters.setFocusMode(Camera.Parameters.FOCUS_MODE_AUTO);

        mCamera.setParameters(parameters);
        mCamera.startPreview();
    }

    @Override
    public void surfaceChanged(SurfaceHolder surfaceHolder, int i, int i2, int i3) {

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
            //todo handle
        }


        Camera.Parameters parameters = mCamera.getParameters();
        parameters.setFlashMode(Camera.Parameters.FLASH_MODE_AUTO);
        parameters.setFocusMode(Camera.Parameters.FOCUS_MODE_AUTO);

        mCamera.setParameters(parameters);

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

    public void refresh() {
        if (mCamera != null){
            mCamera.stopPreview();
            mCamera.release();

        }
        mCamera = getCameraInstance(cameraNum);
        Log.d("Camera Preview", "Getting Camera " + cameraNum);
        try {
            Camera.Parameters parameters = mCamera.getParameters();
            parameters.setFlashMode(Camera.Parameters.FLASH_MODE_AUTO);
            parameters.setFocusMode(Camera.Parameters.FOCUS_MODE_AUTO);
            mCamera.setParameters(parameters);
            mCamera.setPreviewDisplay(mSurfaceHolder);
            mCamera.startPreview();
        } catch (IOException e) {
            e.printStackTrace();
        }
    }

    public void capture(){
        Camera.PictureCallback callback = new Camera.PictureCallback() {
            @Override
            public void onPictureTaken(byte[] bytes, Camera camera) {
                camera.stopPreview();

            }
        };
        if (mCamera != null){
            mCamera.takePicture(null, callback, null);
        }
    }

}
