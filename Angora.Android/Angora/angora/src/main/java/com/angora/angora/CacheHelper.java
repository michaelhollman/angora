package com.angora.angora;

import android.app.Activity;
import android.content.Context;

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
        out.writeObject(user);
        out.close();
    }

    public JSONObject getStoredUser() throws IOException, ClassNotFoundException{
        ObjectInputStream in = new ObjectInputStream(new FileInputStream(new File(new File(mActivity.getCacheDir(),"")+USER_STORAGE)));
        JSONObject user = (JSONObject) in.readObject();
        in.close();
        return user;
    }
}
