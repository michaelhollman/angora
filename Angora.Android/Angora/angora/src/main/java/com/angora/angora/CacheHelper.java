package com.angora.angora;

import android.app.Activity;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;

import org.json.JSONException;
import org.json.JSONObject;

import java.io.File;
import java.io.FileInputStream;
import java.io.FileOutputStream;
import java.io.IOException;
import java.io.ObjectInputStream;
import java.io.ObjectOutput;
import java.io.ObjectOutputStream;

/**
 * Created by Alex on 4/22/2014.
 */
public class CacheHelper {
    private final String ANGORA_STORAGE = "angora";
    private final String USER_ENTRY = "user";
    private final String EVENTS_ENTRY = "events";
    private final String PIC_ENTRY = "profile_pic";
    private Activity mActivity;

    public CacheHelper (Activity context){
        mActivity = context;
    }

    public void storeUser(JSONObject user) throws IOException{
        ObjectOutput out = new ObjectOutputStream(new FileOutputStream(new File(mActivity.getCacheDir(),USER_ENTRY)+ ANGORA_STORAGE));
        out.writeObject(user.toString());
        out.close();
    }

    public JSONObject getStoredUser() throws IOException, ClassNotFoundException, JSONException{
        ObjectInputStream in = new ObjectInputStream(new FileInputStream(new File(new File(mActivity.getCacheDir(),USER_ENTRY)+ ANGORA_STORAGE)));
        JSONObject user = new JSONObject((String) in.readObject());
        in.close();
        return user;
    }

    public void storeEvents(AngoraEvent[] events) throws IOException{
        ObjectOutput out = new ObjectOutputStream(new FileOutputStream(new File(mActivity.getCacheDir(),EVENTS_ENTRY)+ ANGORA_STORAGE));
        out.writeObject(events);
        out.close();
    }

    public AngoraEvent[] getStoredEvents() throws IOException, ClassNotFoundException{
        ObjectInputStream in = new ObjectInputStream(new FileInputStream(new File(new File(mActivity.getCacheDir(),EVENTS_ENTRY)+ ANGORA_STORAGE)));
        AngoraEvent[] events = (AngoraEvent[]) in.readObject();
        in.close();
        return events;
    }

    public void storeProfilePic(Bitmap image) throws IOException{
        FileOutputStream out = new FileOutputStream(new File(mActivity.getCacheDir(),PIC_ENTRY)+ ANGORA_STORAGE);
        image.compress(Bitmap.CompressFormat.JPEG, 100, out);
        out.flush();
        out.close();
    }

    public Bitmap getStoredProfilePic() throws IOException, ClassNotFoundException{
        FileInputStream in = new FileInputStream(new File(new File(mActivity.getCacheDir(),PIC_ENTRY)+ ANGORA_STORAGE));
        Bitmap image = BitmapFactory.decodeStream(in);
        in.close();
        return image;
    }
}
