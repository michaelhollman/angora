package com.angora.angora;

import android.app.Activity;
import android.content.Context;
import android.content.Intent;
import android.content.SharedPreferences;
import android.content.pm.ActivityInfo;
import android.hardware.Camera;
import android.net.Uri;
import android.os.AsyncTask;
import android.os.Environment;
import android.support.v7.app.ActionBarActivity;
import android.support.v7.app.ActionBar;
import android.support.v4.app.Fragment;
import android.os.Bundle;
import android.test.InstrumentationTestRunner;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.Menu;
import android.view.MenuItem;
import android.view.View;
import android.view.ViewGroup;
import android.os.Build;
import android.view.Window;
import android.view.WindowManager;
import android.widget.Button;
import android.widget.FrameLayout;
import android.widget.ImageView;
import android.widget.Toast;

import java.io.File;
import java.io.FileOutputStream;
import java.io.IOException;

public class SnapAndGoActivity extends Activity {

    private CameraPreview mPreview;
    private int CurrentCamera;
    private Activity mActivity;
    private byte[] image;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);

        if (!isExternalStorageWritable()){
            Log.i("SnapAndGo", "External Media Not Writable");
            Intent intent = new Intent(this, MainActivity.class);
            startActivity(intent);
            finish();
        }

        mActivity = this;

        SharedPreferences pref = getApplicationContext().getSharedPreferences("MyPrefs", 0);
        CurrentCamera = pref.getInt("preferredCamera", 0);

        requestWindowFeature(Window.FEATURE_NO_TITLE);
        getWindow().setFlags(WindowManager.LayoutParams.FLAG_FULLSCREEN,
                WindowManager.LayoutParams.FLAG_FULLSCREEN);

        setContentView(R.layout.activity_snap_and_go);

        mPreview = new CameraPreview(this, CurrentCamera);
        FrameLayout preview = (FrameLayout) findViewById(R.id.camera_preview);
        preview.addView(mPreview);

        ImageView nextCameraButton = (ImageView) findViewById(R.id.imageView_nextCamera);
        Button captureButton = (Button) findViewById(R.id.button_capture);


        nextCameraButton.setOnClickListener(new View.OnClickListener(){
            public void onClick(View v) {
                switchCameras();
            }
        });

        captureButton.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                Log.i("SnapAndGo", "Taking photo");
                mPreview.capture();

            }
        });
    }

    public void switchCameras() {
        int numCameras = Camera.getNumberOfCameras();
        if (numCameras > 1){

            SharedPreferences pref = getApplicationContext().getSharedPreferences("MyPrefs", 0);
            SharedPreferences.Editor prefEditor = pref.edit();
            if (CurrentCamera + 1 <= numCameras - 1){
                CurrentCamera++;
            }else{
                CurrentCamera = 0;
            }
            prefEditor.putInt("preferredCamera", CurrentCamera);
            prefEditor.commit();

            mPreview.setCameraNum(CurrentCamera);
            mPreview.restart();
        }
    }

    /* Checks if external storage is available for read and write */
    public boolean isExternalStorageWritable() {
        String state = Environment.getExternalStorageState();
        if (Environment.MEDIA_MOUNTED.equals(state)) {
            return true;
        }
        return false;
    }




}
