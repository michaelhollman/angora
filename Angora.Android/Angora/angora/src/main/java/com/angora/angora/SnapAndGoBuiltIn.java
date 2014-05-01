package com.angora.angora;

import android.content.Intent;
import android.content.SharedPreferences;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.graphics.Matrix;
import android.media.ExifInterface;
import android.net.Uri;
import android.os.AsyncTask;
import android.os.Environment;
import android.provider.MediaStore;
import android.support.v7.app.ActionBarActivity;
import android.os.Bundle;
import android.util.Log;
import android.view.Menu;
import android.view.MenuItem;
import android.widget.ImageView;
import android.widget.ProgressBar;
import android.widget.Toast;

import java.io.ByteArrayOutputStream;
import java.io.DataOutputStream;
import java.io.File;
import java.io.FileInputStream;
import java.io.IOException;
import java.net.HttpURLConnection;
import java.net.URL;
import java.text.SimpleDateFormat;
import java.util.Date;
import java.util.Locale;

/*
 * Heavily adapted from http://www.androidhive.info/2013/09/android-working-with-camera-api/
 */
public class SnapAndGoBuiltIn extends ActionBarActivity {
    // Activity request codes
    private static final int CAMERA_CAPTURE_IMAGE_REQUEST_CODE = 100;
    private static final int MEDIA_TYPE_IMAGE = 1;

    // directory name to store captured images and videos
    private static final String IMAGE_DIRECTORY_NAME = "Auderus";
    private static final String FILE_URI = "file_uri";
    private static final String TIME_STAMP_FORMAT = "yyyyMMdd_HHmmss";
    private static final String NEEDS_ROTATION_90_D = "6";
    private static final String NEEDS_ROTATION_270_D = "8";
    private static final String TAG = "SnapNGo";

    private Uri fileUri; // file url to store image/video
    private String eventId;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_snap_and_go_built_in);

        Intent intent = getIntent();
        eventId = intent.getStringExtra(MainActivity.EXTRA_EVENT_ID);

        captureImage();

    }


    /*
 * Capturing Camera Image will lauch camera app requrest image capture
 */
    private void captureImage() {
        Intent intent = new Intent(MediaStore.ACTION_IMAGE_CAPTURE);

        fileUri = getOutputMediaFileUri(MEDIA_TYPE_IMAGE);

        intent.putExtra(MediaStore.EXTRA_OUTPUT, fileUri);

        // start the image capture Intent
        startActivityForResult(intent, CAMERA_CAPTURE_IMAGE_REQUEST_CODE);
    }

    /**
     * Here we store the file url as it will be null after returning from camera
     * app
     */
    @Override
    protected void onSaveInstanceState(Bundle outState) {
        super.onSaveInstanceState(outState);

        // save file url in bundle as it will be null on screen orientation
        // changes
        outState.putParcelable(FILE_URI, fileUri);
    }

    @Override
    protected void onRestoreInstanceState(Bundle savedInstanceState) {
        super.onRestoreInstanceState(savedInstanceState);

        // get the file url
        fileUri = savedInstanceState.getParcelable(FILE_URI);
    }


    @Override
    public boolean onCreateOptionsMenu(Menu menu) {
        
        // Inflate the menu; this adds items to the action bar if it is present.
        getMenuInflater().inflate(R.menu.snap_and_go_built_in, menu);
        return true;
    }

    @Override
    public boolean onOptionsItemSelected(MenuItem item) {
        // Handle action bar item clicks here. The action bar will
        // automatically handle clicks on the Home/Up button, so long
        // as you specify a parent activity in AndroidManifest.xml.
        int id = item.getItemId();
        if (id == R.id.action_settings) {
            return true;
        }
        return super.onOptionsItemSelected(item);
    }

    /**
     * Receiving activity result method will be called after closing the camera
     * */
    @Override
    protected void onActivityResult(int requestCode, int resultCode, Intent data) {
        // if the result is capturing Image
        if (requestCode == CAMERA_CAPTURE_IMAGE_REQUEST_CODE) {
            if (resultCode == RESULT_OK) {
                // successfully captured the image

                SharedPreferences pref = getApplicationContext().getSharedPreferences(MainActivity.PREFS_NAME, 0);

                String angoraId = pref.getString(MainActivity.PREFS_KEY_ANGORA_ID, null);
                if (angoraId != null) {
                    new UploadLastImageTask().execute(angoraId);
                }else{
                    //the user somehow managed to log in without an angora id...
                    Toast.makeText(getApplicationContext(), "Error: Please log in again. Sorry!", Toast.LENGTH_SHORT).show();
                    SharedPreferences.Editor prefEditor = pref.edit();
                    prefEditor.putBoolean(MainActivity.PREFS_KEY_LOGGED_IN, false);
                    prefEditor.commit();

                    Intent intent = new Intent(this, LoginActivity.class);
                    startActivity(intent);
                }

            } else if (resultCode == RESULT_CANCELED) {
                // user cancelled Image capture
                Intent intent = new Intent(getApplicationContext(), MainActivity.class);
                startActivity(intent);
                finish();

            } else {
                // failed to capture image
                Toast.makeText(getApplicationContext(),
                        "Sorry! Failed to capture image", Toast.LENGTH_SHORT)
                        .show();
            }
        }
    }

    /**
     * ------------ Helper Methods ----------------------
     * */

    /**
     * Creating file uri to store image/video
     */
    public Uri getOutputMediaFileUri(int type) {
        return Uri.fromFile(getOutputMediaFile(type));
    }

    /**
     * returning image / video
     */
    private static File getOutputMediaFile(int type) {

        // External sdcard location
        File mediaStorageDir = new File(
                Environment
                        .getExternalStoragePublicDirectory(Environment.DIRECTORY_PICTURES),
                IMAGE_DIRECTORY_NAME);

        // Create the storage directory if it does not exist
        if (!mediaStorageDir.exists()) {
            if (!mediaStorageDir.mkdirs()) {
                Log.d(IMAGE_DIRECTORY_NAME, "Oops! Failed create "
                        + IMAGE_DIRECTORY_NAME + " directory");
                return null;
            }
        }

        // Create a media file name
        String timeStamp = new SimpleDateFormat(TIME_STAMP_FORMAT,
                Locale.getDefault()).format(new Date());
        File mediaFile;
        if (type == MEDIA_TYPE_IMAGE) {
            mediaFile = new File(mediaStorageDir.getPath() + File.separator
                    + "IMG_" + timeStamp + ".jpg");
        } else {
            return null;
        }

        return mediaFile;
    }

    public class UploadLastImageTask extends AsyncTask<String, Void, String> {

        @Override
        protected String doInBackground(String... strings) {
            HttpURLConnection connection = null;
            DataOutputStream outputStream = null;
            StringBuilder urlServer = new StringBuilder();
            String lineEnd = "\r\n";
            String twoHyphens = "--";
            String boundary =  "*****";


            int bytesRead, bytesAvailable, bufferSize;
            byte[] buffer;
            int maxBufferSize = 1*1024*1024;

            urlServer.append(MainActivity.API_URL).append("user/");
            urlServer.append(strings[0]);
            urlServer.append("/upload/");
            urlServer.append(eventId);

            try
            {
                ExifInterface exif = new ExifInterface(fileUri.getPath());
                String orientation = exif.getAttribute(ExifInterface.TAG_ORIENTATION);

                BitmapFactory.Options options = new BitmapFactory.Options();
                options.inPreferredConfig = Bitmap.Config.ARGB_8888;
                Bitmap bitmap = BitmapFactory.decodeFile(fileUri.getPath(), options);

                URL url = new URL(urlServer.toString());
                connection = (HttpURLConnection) url.openConnection();

                // Allow Inputs &amp; Outputs.
                connection.setDoInput(true);
                connection.setDoOutput(true);
                connection.setUseCaches(false);

                // Set HTTP method to POST.
                connection.setRequestMethod("POST");

                connection.setRequestProperty("Connection", "Keep-Alive");
                connection.setRequestProperty("Content-Type", "multipart/form-data; boundary="+boundary);

                outputStream = new DataOutputStream( connection.getOutputStream() );
                outputStream.writeBytes(twoHyphens + boundary + lineEnd);
                outputStream.writeBytes("Content-Disposition: form-data; name=\"picture\"; filename=\"" + fileUri.getPath() +"\"" + lineEnd);
                outputStream.writeBytes("Content-Type: image/jpeg" + lineEnd);
                outputStream.writeBytes(lineEnd);

                if (orientation.equals(NEEDS_ROTATION_90_D)){
                    outputStream.write(rotateAndConvert(bitmap, 90));
                }else if (orientation.equals(NEEDS_ROTATION_270_D)){
                    outputStream.write(rotateAndConvert(bitmap, -90));
                }else {
                    //image is fine as is
                    FileInputStream fileInputStream = new FileInputStream(new File(fileUri.getPath()) );
                    bytesAvailable = fileInputStream.available();
                    bufferSize = Math.min(bytesAvailable, maxBufferSize);
                    buffer = new byte[bufferSize];

                    // Read file
                    bytesRead = fileInputStream.read(buffer, 0, bufferSize);

                    while (bytesRead > 0) {
                        outputStream.write(buffer, 0, bufferSize);
                        bytesAvailable = fileInputStream.available();
                        bufferSize = Math.min(bytesAvailable, maxBufferSize);
                        bytesRead = fileInputStream.read(buffer, 0, bufferSize);
                    }
                    fileInputStream.close();

                }

                outputStream.writeBytes(lineEnd);
                outputStream.writeBytes(twoHyphens + boundary + twoHyphens + lineEnd);

                outputStream.flush();
                outputStream.close();

                int response = connection.getResponseCode();
                String responseMessage = connection.getResponseMessage();
                Log.i(TAG, "Server Response is " + response + ":" + responseMessage);
            }
            catch (IOException ie)
            {
                Toast.makeText(getApplicationContext(), "Error Uploading File: " + ie.getMessage(), Toast.LENGTH_SHORT).show();
            }

            return null;
        }

        private byte[] rotateAndConvert(Bitmap bitmap, int degrees){
            Matrix matrix = new Matrix();
            matrix.postRotate(degrees);
            Bitmap rotatedBitmap = Bitmap.createBitmap(bitmap , 0, 0, bitmap.getWidth(), bitmap.getHeight(), matrix, true);
            ByteArrayOutputStream bao = new ByteArrayOutputStream();
            rotatedBitmap.compress(Bitmap.CompressFormat.JPEG, 100, bao);
            return bao.toByteArray();
        }

        @Override
        protected void onPostExecute(String s) {
            super.onPostExecute(s);
            Intent intent = new Intent(getApplicationContext(), MainActivity.class);
            startActivity(intent);
        }
    }

}
