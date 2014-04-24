package com.angora.angora;

import android.content.Context;
import android.os.AsyncTask;
import android.util.Log;
import android.widget.Toast;

import org.json.JSONObject;
import org.json.JSONTokener;

import java.io.BufferedReader;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.net.HttpURLConnection;
import java.net.URL;

/**
 * Created by Alex on 4/21/2014.
 */
public class LoginUserTask extends AsyncTask<String, Void, JSONObject>
{

    private final String SITE_URL = "http://seteam4.azurewebsites.net/api/user/login?";
    @Override
    protected JSONObject doInBackground(String... strings) {
        StringBuilder urlString = new StringBuilder();
        urlString.append(SITE_URL);
        urlString.append("provider=").append(strings[0]);
        urlString.append("&providerKey=").append(strings[1]);

        Log.d("LoginUserTask", urlString.toString());

        HttpURLConnection urlConnection = null;
        URL url = null;
        JSONObject object = null;

        try
        {
            url = new URL(urlString.toString());
            urlConnection = (HttpURLConnection) url.openConnection();
            urlConnection.setRequestMethod("GET");
            urlConnection.connect();
            InputStream inStream = null;
            inStream = urlConnection.getInputStream();
            BufferedReader bReader = new BufferedReader(new InputStreamReader(inStream));
            String temp, response = "";
            while ((temp = bReader.readLine()) != null)
                response += temp;
            bReader.close();
            inStream.close();
            urlConnection.disconnect();
            object = (JSONObject) new JSONTokener(response).nextValue();
        }
        catch (Exception e)
        {
            //TODO handle

        }

        return (object);
    }

    @Override
    protected void onPreExecute() {
        super.onPreExecute();

    }
}
