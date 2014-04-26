package com.angora.angora;

import android.app.Activity;
import android.content.Context;
import android.content.Intent;
import android.content.SharedPreferences;
import android.content.pm.ActivityInfo;
import android.hardware.Camera;
import android.support.v7.app.ActionBarActivity;
import android.support.v7.app.ActionBar;
import android.support.v4.app.Fragment;
import android.os.Bundle;
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

import java.io.IOException;

public class SnapAndGoActivity extends Activity {

    private CameraPreview mPreview;
    private int CurrentCamera;
    private Context context;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        context = this;

        SharedPreferences pref = getApplicationContext().getSharedPreferences("MyPrefs", 0);
        CurrentCamera = pref.getInt("preferredCamera", 0);

        requestWindowFeature(Window.FEATURE_NO_TITLE);
        getWindow().setFlags(WindowManager.LayoutParams.FLAG_FULLSCREEN,
                WindowManager.LayoutParams.FLAG_FULLSCREEN);

        setContentView(R.layout.activity_snap_and_go);

        mPreview = new CameraPreview(this, CurrentCamera);
        FrameLayout preview = (FrameLayout) findViewById(R.id.camera_preview);
        preview.addView(mPreview);

        final ImageView nextCameraButton = (ImageView) findViewById(R.id.imageView_nextCamera);
        final Button captureButton = (Button) findViewById(R.id.button_capture);
        final ImageView cancelButton = (ImageView) findViewById(R.id.imageView_cancel);
        final ImageView acceptButton = (ImageView) findViewById(R.id.imageView_accept);

        nextCameraButton.setOnClickListener(new View.OnClickListener(){
            public void onClick(View v) {
                switchCameras();
            }
        });

        captureButton.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                mPreview.capture();
                view.setVisibility(View.INVISIBLE);
                nextCameraButton.setVisibility(View.INVISIBLE);
                cancelButton.setVisibility(View.VISIBLE);
                acceptButton.setVisibility(View.VISIBLE);
            }
        });

        cancelButton.setOnClickListener(new View.OnClickListener(){
            @Override
            public void onClick(View view) {
                mPreview.refresh();
                nextCameraButton.setVisibility(View.VISIBLE);
                captureButton.setVisibility(View.VISIBLE);
                view.setVisibility(View.INVISIBLE);
                acceptButton.setVisibility(View.INVISIBLE);
            }
        });

        acceptButton.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                //todo do something with the picture

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
            mPreview.refresh();

        }
    }
}
