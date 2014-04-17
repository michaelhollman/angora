package com.angora.angora;

import android.app.Activity;
import android.content.Intent;
import android.content.SharedPreferences;
import android.content.pm.PackageInfo;
import android.content.pm.PackageManager;
import android.content.pm.Signature;
import android.support.v7.app.ActionBarActivity;
import android.support.v7.app.ActionBar;
import android.support.v4.app.Fragment;
import android.os.Bundle;
import android.util.Base64;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.Menu;
import android.view.MenuItem;
import android.view.View;
import android.view.ViewGroup;
import android.os.Build;
import android.widget.Button;
import android.widget.Toast;

import com.facebook.*;
import com.facebook.model.*;
import com.facebook.widget.LoginButton;

import java.security.MessageDigest;
import java.security.NoSuchAlgorithmException;

public class LoginActivity extends ActionBarActivity {

    private Activity mActivity;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_login);
        mActivity = this;
        registerButtons();

    }

    @Override
    protected void onActivityResult(int requestCode, int resultCode, Intent data) {
        super.onActivityResult(requestCode, resultCode, data);
    }

    @Override
    public boolean onCreateOptionsMenu(Menu menu) {
        
        // Inflate the menu; this adds items to the action bar if it is present.
        getMenuInflater().inflate(R.menu.login, menu);
        return true;
    }

    @Override
    public boolean onOptionsItemSelected(MenuItem item) {
        // Handle action bar item clicks here. The action bar will
        // automatically handle clicks on the Home/Up button, so long
        // as you specify a parent activity in AndroidManifest.xml.

        return super.onOptionsItemSelected(item);
    }
    public void registerButtons(){
        View.OnClickListener fbClickListener = new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                // start Facebook Login
                Session.openActiveSession(mActivity, true, new Session.StatusCallback() {

                    // callback when session changes state
                    @Override
                    public void call(Session session, SessionState state, Exception exception) {
                        Log.d("LoginActivity", "session.opened = "+session.isOpened());
                        if (session.isOpened()) {

                            // make request to the /me API
                            Request.newMeRequest(session, new Request.GraphUserCallback() {

                                // callback after Graph API response with user object
                                @Override
                                public void onCompleted(GraphUser user, Response response) {
                                    Log.e("LoginActivity", "Request Completed");
                                    if (user != null) {
                                        SharedPreferences pref = getApplicationContext().getSharedPreferences("MyPrefs", 0);
                                        SharedPreferences.Editor prefEditor = pref.edit();
                                        prefEditor.putBoolean("IsLoggedIn", true);
                                        prefEditor.commit();
                                        //TODO pass the user...or maybe the ID...
                                        openMainActivity();
                                    }
                                }
                            }).executeAsync();
                        }
                    }
                });
            }
        };
        Button facebookButton = (Button) findViewById(R.id.button_facebook);
        facebookButton.setOnClickListener(fbClickListener);

        //Button twitterButton = (Button) findViewById(R.id.button_twitter);
        //twitterButton.setOnClickListener(onClickListener);
    }

    public void openMainActivity(){
        Intent intent = new Intent(this, MainActivity.class);
        startActivity(intent);
        finish();
    }

}
