package com.angora.angora;

import android.content.Context;
import android.content.pm.PackageManager;
import android.hardware.Camera;
import android.support.v7.app.ActionBarActivity;
import android.support.v7.app.ActionBar;
import android.support.v4.app.Fragment;
import android.os.Bundle;
import android.util.Log;
import android.view.KeyEvent;
import android.view.LayoutInflater;
import android.view.Menu;
import android.view.MenuItem;
import android.view.SurfaceHolder;
import android.view.SurfaceView;
import android.view.View;
import android.view.ViewGroup;
import android.os.Build;
import android.view.Window;
import android.widget.Toast;

import java.io.IOException;

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
                mCamera.startPreview();
        } catch (IOException e) {

        }
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
            //BAD THING
        }
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
