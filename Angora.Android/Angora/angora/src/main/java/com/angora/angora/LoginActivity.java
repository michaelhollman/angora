package com.angora.angora;

import android.app.Activity;
import android.content.Intent;
import android.content.SharedPreferences;
import android.net.Uri;
import android.os.AsyncTask;
import android.support.v7.app.ActionBarActivity;
import android.os.Bundle;
import android.util.Log;
import android.view.Menu;
import android.view.MenuItem;
import android.view.View;
import android.webkit.WebView;
import android.webkit.WebViewClient;
import android.widget.Button;
import android.widget.Toast;

import com.facebook.*;
import com.facebook.model.*;


import java.util.concurrent.ExecutionException;

import twitter4j.Twitter;
import twitter4j.TwitterException;
import twitter4j.TwitterFactory;
import twitter4j.auth.RequestToken;
import twitter4j.conf.Configuration;
import twitter4j.conf.ConfigurationBuilder;
import twitter4j.auth.AccessToken;

public class LoginActivity extends ActionBarActivity {

    private final String TWITTER_CONSUMER_KEY = "o8QTwfzt6CdfDGndyqvLrg";
    private final String TWITTER_CONSUMER_SECRET = "jqU2tq5QVUkK6JdFA22wtXZNrTumatvG9VpPAfK5M";
    private final String TWITTER_CALLBACK_URL = "http://seteam4.azurewebsites.net";
    private final String URL_TWITTER_OAUTH_VERIFIER = "oauth_verifier";
    private final String PROVIDER_NAME_FACEBOOK = "Facebook";
    private final String PROVIDER_NAME_TWITTER = "Twitter";

    private Activity activity;
    private RequestToken requestToken;
    private Twitter twitter;
    private AccessToken accessToken;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_login);
        activity = this;
        registerButtons();

    }

    @Override
    protected void onNewIntent(Intent intent) {
        super.onNewIntent(intent);

    }

    @Override
    protected void onActivityResult(int requestCode, int resultCode, Intent data) {
        super.onActivityResult(requestCode, resultCode, data);
        Session.getActiveSession().onActivityResult(this, requestCode, resultCode, data);
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
        //Facebook Button
        View.OnClickListener fbClickListener = new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                // start Facebook Login
                Session.openActiveSession(activity, true, new Session.StatusCallback() {

                    // callback when session changes state
                    @Override
                    public void call(Session session, SessionState state, Exception exception) {
                        if (session.isOpened()) {
                            // make request to the /me API
                            Request.newMeRequest(session, new Request.GraphUserCallback() {

                                // callback after Graph API response with user object
                                @Override
                                public void onCompleted(GraphUser user, Response response) {
                                    if (user != null) {
                                        SharedPreferences pref = getApplicationContext().getSharedPreferences(MainActivity.PREFS_NAME, 0);
                                        SharedPreferences.Editor prefEditor = pref.edit();
                                        prefEditor.putBoolean(MainActivity.PREFS_KEY_LOGGED_IN, true);
                                        prefEditor.putString(MainActivity.PREFS_KEY_LOGIN_PROVIDER, PROVIDER_NAME_FACEBOOK);
                                        prefEditor.putString(MainActivity.PREFS_KEY_PROVIDER_KEY, user.getId());

                                        prefEditor.commit();

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

        //Twitter Button
        View.OnClickListener twitClickListener = new View.OnClickListener() {

            @Override
            public void onClick(View view) {
                loginToTwitter();
            }
        };

        Button twitterButton = (Button) findViewById(R.id.button_twitter);
        twitterButton.setOnClickListener(twitClickListener);
    }

    public void openMainActivity(){
        Intent intent = new Intent(this, MainActivity.class);
        startActivity(intent);
        finish();
    }

    private void loginToTwitter() {
        ConfigurationBuilder builder = new ConfigurationBuilder();
        builder.setOAuthConsumerKey(TWITTER_CONSUMER_KEY);
        builder.setOAuthConsumerSecret(TWITTER_CONSUMER_SECRET);
        Configuration configuration = builder.build();

        TwitterFactory factory = new TwitterFactory(configuration);
        twitter = factory.getInstance();

        WebView webview = new WebView(this);
        webview.setWebViewClient(new WebViewClient(){

            @Override
            public boolean shouldOverrideUrlLoading(WebView view, String url) {
                if (url.startsWith(TWITTER_CALLBACK_URL)) {
                    setContentView(R.layout.activity_login);

                    Uri uri = Uri.parse(url);
                    // oAuth verifier
                    String verifier = uri.getQueryParameter(URL_TWITTER_OAUTH_VERIFIER);

                    try {
                        // Get the access token
                        accessToken = new GetTwitterAccessTokenTask().execute(verifier).get();

                        // Getting user details from twitter
                        long userID = accessToken.getUserId();

                        SharedPreferences pref = getApplicationContext().getSharedPreferences(MainActivity.PREFS_NAME, 0);
                        SharedPreferences.Editor prefEditor = pref.edit();

                        prefEditor.putBoolean(MainActivity.PREFS_KEY_LOGGED_IN, true);
                        prefEditor.putString(MainActivity.PREFS_KEY_LOGIN_PROVIDER, PROVIDER_NAME_TWITTER);
                        prefEditor.putString(MainActivity.PREFS_KEY_PROVIDER_KEY, String.valueOf(userID));

                        prefEditor.commit();

                        openMainActivity();

                    } catch (InterruptedException ie) {
                        Toast.makeText(getApplicationContext(), "Twitter Error: " + ie.getMessage(), Toast.LENGTH_SHORT).show();
                    } catch (ExecutionException ee){
                        Toast.makeText(getApplicationContext(), "Twitter Error: " + ee.getMessage(), Toast.LENGTH_SHORT).show();
                    }
                }else{
                    view.loadUrl(url);
                }
                return false;
            }
        });

        setContentView(webview);

        try {
            requestToken = new GetTwitterRequestTokenTask().execute().get();
            webview.loadUrl(requestToken.getAuthenticationURL());
        }catch (InterruptedException ie){
            Toast.makeText(getApplicationContext(), "Twitter Error: " + ie.getMessage(), Toast.LENGTH_SHORT).show();
        }catch (ExecutionException ee){
            Toast.makeText(getApplicationContext(), "Twitter Error: " + ee.getMessage(), Toast.LENGTH_SHORT).show();
        }

    }

    public class GetTwitterRequestTokenTask extends AsyncTask<String, Void, RequestToken> {

        @Override
        protected RequestToken doInBackground(String... strings) {
            try {
                return twitter.getOAuthRequestToken(TWITTER_CALLBACK_URL);
            } catch (TwitterException te) {
                Toast.makeText(getApplicationContext(), "Twitter Error: " + te.getMessage(), Toast.LENGTH_SHORT).show();
            }
            return null;
        }
    }

    public class GetTwitterAccessTokenTask extends AsyncTask<String, Void, AccessToken> {

        @Override
        protected AccessToken doInBackground(String... strings) {
            try {
                return twitter.getOAuthAccessToken(requestToken, strings[0]);
            } catch (TwitterException te) {
                Toast.makeText(getApplicationContext(), "Twitter Error: " + te.getMessage(), Toast.LENGTH_SHORT).show();
            }
            return null;
        }
    }

}
