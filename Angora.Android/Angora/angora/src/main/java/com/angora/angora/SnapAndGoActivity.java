package com.angora.angora;

import android.content.Intent;
import android.content.SharedPreferences;
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
import android.widget.FrameLayout;
import android.widget.ImageView;
import android.widget.Toast;

public class SnapAndGoActivity extends ActionBarActivity {

    private CameraPreview mPreview;
    private int CurrentCamera;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);

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
        nextCameraButton.setOnClickListener(new View.OnClickListener(){
            public void onClick(View v) {
                switchCameras();
            }
        });
    }


    @Override
    public boolean onCreateOptionsMenu(Menu menu) {
        
        // Inflate the menu; this adds items to the action bar if it is present.
        getMenuInflater().inflate(R.menu.snap_and_go, menu);
        return true;
    }

    @Override
    public boolean onOptionsItemSelected(MenuItem item) {
        // Handle action bar item clicks here. The action bar will
        // automatically handle clicks on the Home/Up button, so long
        // as you specify a parent activity in AndroidManifest.xml.
        int id = item.getItemId();

        return super.onOptionsItemSelected(item);
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

            startCameraAndPreview();
        }
    }


    public void startCameraAndPreview(){



    }
}
