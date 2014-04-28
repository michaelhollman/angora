package com.angora.angora;

import android.os.AsyncTask;

import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;
import org.json.JSONTokener;

import java.io.BufferedReader;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.net.HttpURLConnection;
import java.net.URL;
import java.text.SimpleDateFormat;

/**
 * Created by Alex on 4/24/2014.
 */
public class GetEventsTask extends AsyncTask<String, Void, AngoraEvent[]> {

    private final String SITE_URL = "http://seteam4.azurewebsites.net/api/user/getEvents?";

    @Override
    protected AngoraEvent[] doInBackground(String... strings) {
        StringBuilder urlString = new StringBuilder();
        urlString.append(SITE_URL);
        urlString.append("userId=").append(strings[0]);

        HttpURLConnection urlConnection = null;
        URL url = null;
        JSONObject object = null;
        JSONArray eventsJSON = null;

        try{
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
            //return (JSONObject) new JSONTokener(response).nextValue();
            eventsJSON = new JSONArray(response);
            SimpleDateFormat parser = new SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss");
            if (eventsJSON != null) {
                AngoraEvent[] events = new AngoraEvent[eventsJSON.length()];
                for (int i = 0; i < eventsJSON.length(); i++) {
                    JSONObject currentEvent = eventsJSON.getJSONObject(i);
                    //todo get creator (API change)
                    events[i] = new AngoraEvent(currentEvent.getString("Id"),
                            currentEvent.getString("Name"),
                            "CREATOR",
                            currentEvent.getString("Description"),
                            currentEvent.getString("Location"),
                            parser.parse(currentEvent.getString("StartDateTime")));
                }
                return events;
            }
        }catch (Exception e){
            //todo handle
            e.printStackTrace();
        }


        return null;
    }
}
