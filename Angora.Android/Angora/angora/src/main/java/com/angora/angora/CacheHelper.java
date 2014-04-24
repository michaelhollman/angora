package com.angora.angora;

import android.app.Activity;
import android.content.Context;
import android.util.Log;

import org.json.JSONException;
import org.json.JSONObject;

import java.io.File;
import java.io.FileInputStream;
import java.io.FileNotFoundException;
import java.io.FileOutputStream;
import java.io.IOException;
import java.io.ObjectInputStream;
import java.io.ObjectOutput;
import java.io.ObjectOutputStream;

/**
 * Created by Alex on 4/22/2014.
 */
public class CacheHelper {
    private final String USER_STORAGE = "angora_user";
    private Activity mActivity;

    public CacheHelper (Activity context){
        mActivity = context;
    }

    public void storeUser(JSONObject user) throws IOException{
        ObjectOutput out = new ObjectOutputStream(new FileOutputStream(new File(mActivity.getCacheDir(),"")+USER_STORAGE));
        out.writeObject(user.toString());
        out.close();
    }

    public JSONObject getStoredUser() throws IOException, ClassNotFoundException, JSONException{
        ObjectInputStream in = new ObjectInputStream(new FileInputStream(new File(new File(mActivity.getCacheDir(),"")+USER_STORAGE)));
        JSONObject user = new JSONObject((String) in.readObject());
        in.close();
        return user;
    }

    public void storeEvents(AngoraEvent[] events) throws IOException{
        ObjectOutput out = new ObjectOutputStream(new FileOutputStream(new File(mActivity.getCacheDir(),"")+USER_STORAGE));
        out.writeObject(events);
        out.close();
    }

    public AngoraEvent[] getStoredEvents() throws IOException, ClassNotFoundException{
        ObjectInputStream in = new ObjectInputStream(new FileInputStream(new File(new File(mActivity.getCacheDir(),"")+USER_STORAGE)));
        AngoraEvent[] events = (AngoraEvent[]) in.readObject();
        in.close();
        return events;
    }
}
