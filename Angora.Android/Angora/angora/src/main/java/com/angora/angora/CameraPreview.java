package com.angora.angora;

import android.app.Activity;
import android.content.Context;
import android.content.Intent;
import android.content.SharedPreferences;
import android.hardware.Camera;
import android.location.Location;
import android.net.Uri;
import android.os.AsyncTask;
import android.os.Environment;
import android.util.Log;
import android.view.SurfaceHolder;
import android.view.SurfaceView;


import java.io.File;
import java.io.FileOutputStream;
import java.io.IOException;
import java.util.List;
import java.util.concurrent.ExecutionException;

public class CameraPreview extends SurfaceView implements SurfaceHolder.Callback{

    private SurfaceHolder mSurfaceHolder;
    private Camera mCamera;
    private boolean isPaused;
    private int cameraNum;
    private Activity mActivity;

    public byte[] getImage() {
        return image;
    }

    private byte[] image;

    public int getCameraNum() {
        return cameraNum;
    }

    public void setCameraNum(int cameraNum) {
        this.cameraNum = cameraNum;
    }

    public CameraPreview(Context context, int cameraNum) {
        super(context);
        mActivity = (Activity) context;
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
        //parameters.setFlashMode(Camera.Parameters.FLASH_MODE_AUTO);
        //parameters.setFocusMode(Camera.Parameters.FOCUS_MODE_AUTO);

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
            e.printStackTrace();
        }


        Camera.Parameters parameters = mCamera.getParameters();
        //parameters.setFlashMode(Camera.Parameters.FLASH_MODE_AUTO);
       // parameters.setFocusMode(Camera.Parameters.FOCUS_MODE_AUTO);
        mCamera.setParameters(parameters);


        //mCamera.startPreview();
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

    public void restart()  {
        if (mCamera != null){
            mCamera.stopPreview();
            mCamera.release();

        }
        mCamera = getCameraInstance(cameraNum);
        Log.d("Camera Preview", "Getting Camera " + cameraNum);
        try {

            Camera.Parameters parameters = mCamera.getParameters();
            //parameters.setFlashMode(Camera.Parameters.FLASH_MODE_AUTO);
            //parameters.setFocusMode(Camera.Parameters.FOCUS_MODE_AUTO);
            mCamera.setParameters(parameters);

            mCamera.setPreviewDisplay(mSurfaceHolder);
            mCamera.startPreview();
        } catch (IOException e) {
            e.printStackTrace();
        }
    }

    public void capture() {
        /*
        try {
            image = new TakePhotoTask().execute(mCamera).get();
        }catch (InterruptedException ie){
            Log.e("CameraPreview", "Cannot take photo");
            ie.printStackTrace();
        }catch (ExecutionException ee){
            Log.e("CameraPreview", "Cannot take photo");
            ee.printStackTrace();
        }
        */
        Camera.PictureCallback firstCallback = new Camera.PictureCallback() {
            @Override
            public void onPictureTaken(byte[] bytes, Camera camera) {
                //camera.stopPreview();
                image = bytes;
            }
        };

        if (mCamera != null) {
            mCamera.takePicture(null, null, null, firstCallback);
            /*
            if (image != null){
                //new SavePhotoTask().execute(image);
                SharedPreferences pref = mActivity.getSharedPreferences("MyPrefs", 0);
                SharedPreferences.Editor prefEditor = pref.edit();

                File photo=new File(Environment.getExternalStorageDirectory(), "eventphoto_"+pref.getInt("LastPhoto", 0)+".jpg");

                if (pref.getInt("LastPhoto", 0)>= Integer.MAX_VALUE){
                    prefEditor.putInt("LastPhoto", 0);
                    Log.i("SnapAndGo", "Restarting LastPhoto count. Images will be deleted!");
                }else{
                    prefEditor.putInt("LastPhoto", pref.getInt("LastPhoto", 0) + 1);
                }
                prefEditor.commit();

                if (photo.exists()) {
                    photo.delete();
                    Log.i("SnapAndGo", "Deleted a photo, hope it wasn't important...");
                }

                try {
                    FileOutputStream fos=new FileOutputStream(photo.getPath());
                    fos.write(image);
                    fos.close();
                }
                catch (java.io.IOException e) {
                    Log.e("SnapAndGo", "Exception in photoCallback", e);
                }
                prefEditor.putBoolean("NewPhoto", true);
                prefEditor.commit();
            } else {
                Log.e("CameraPreview", "Image is null");
            }
            */
        }
    }

    class TakePhotoTask extends AsyncTask<Camera, Void, byte[]>{
        byte[] image = null;
        @Override
        protected byte[] doInBackground(Camera... cameras) {
            Camera.PictureCallback firstCallback = new Camera.PictureCallback() {
                @Override
                public void onPictureTaken(byte[] bytes, Camera camera) {
                    //camera.stopPreview();
                    image = bytes;
                }
            };

            if (mCamera != null) {
                mCamera.takePicture(null, null, null, firstCallback);

            }
            return image;
        }

        @Override
        protected void onPostExecute(byte[] bytes) {
            super.onPostExecute(bytes);
        }


    }

    class SavePhotoTask extends AsyncTask<byte[], Void, String> {
        @Override
        protected String doInBackground(byte[]... jpeg) {
            SharedPreferences pref = mActivity.getSharedPreferences("MyPrefs", 0);
            SharedPreferences.Editor prefEditor = pref.edit();

            File photo=new File(Environment.getExternalStorageDirectory(), "eventphoto_"+pref.getInt("LastPhoto", 0)+".jpg");

            if (pref.getInt("LastPhoto", 0)>= Integer.MAX_VALUE){
                prefEditor.putInt("LastPhoto", 0);
                Log.i("SnapAndGo", "Restarting LastPhoto count. Images will be deleted!");
            }else{
                prefEditor.putInt("LastPhoto", pref.getInt("LastPhoto", 0) + 1);
            }
            prefEditor.commit();

            if (photo.exists()) {
                photo.delete();
                Log.i("SnapAndGo", "Deleted a photo, hope it wasn't important...");
            }

            try {
                FileOutputStream fos=new FileOutputStream(photo.getPath());
                fos.write(jpeg[0]);
                fos.close();
            }
            catch (java.io.IOException e) {
                Log.e("SnapAndGo", "Exception in photoCallback", e);
            }
            prefEditor.putBoolean("NewPhoto", true);
            prefEditor.commit();

            return(null);
        }
    }

}
