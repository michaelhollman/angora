package com.angora.angora;

import android.app.Activity;
import android.content.Intent;
import android.content.SharedPreferences;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.os.AsyncTask;
import android.support.v7.app.ActionBarActivity;
import android.support.v7.app.ActionBar;
import android.support.v4.app.Fragment;
import android.content.Context;
import android.os.Bundle;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.Menu;
import android.view.MenuItem;
import android.view.View;
import android.view.ViewGroup;
import android.support.v4.widget.DrawerLayout;
import android.widget.AdapterView;
import android.widget.BaseAdapter;
import android.widget.ImageView;
import android.widget.ListView;
import android.widget.TextView;
import android.widget.Toast;

import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;
import org.json.JSONTokener;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.net.HttpURLConnection;
import java.net.MalformedURLException;
import java.net.URL;
import java.text.SimpleDateFormat;
import java.util.Arrays;
import java.util.Comparator;
import java.util.concurrent.ExecutionException;

public class MainActivity extends ActionBarActivity
        implements NavigationDrawerFragment.NavigationDrawerCallbacks {

    public final static String EXTRA_EVENT_ID = "com.angora.angora.EVENT_ID";
    public final static String PREFS_NAME = "MyPrefs";
    public final static String PREFS_KEY_LOGGED_IN = "IsLoggedIn";
    public final static String PREFS_KEY_LAST_REFRESH = "LastRefresh";
    public final static String PREFS_KEY_LOGIN_PROVIDER = "LoginProvider";
    public final static String PREFS_KEY_PROVIDER_KEY = "ProviderKey";
    public final static String PREFS_KEY_ANGORA_ID = "AngoraId";

    public final static String USER_KEY_PROFILE_PIC_URL = "ProfilePictureUrl";
    public final static String USER_KEY_ID = "Id";
    public final static String USER_KEY_FIRST_NAME = "FirstName";
    public final static String USER_KEY_LAST_NAME = "LastName";
    public final static String USER_KEY_LOCATION = "Location";

    public final static String EVENT_KEY_NAME = "Name";
    public final static String EVENT_KEY_DESC = "Description";
    public final static String EVENT_KEY_ID = "Id";
    public final static String EVENT_KEY_LOCATION = "Location";
    public final static String EVENT_KEY_TIME_JSON = "EventTime";
    public final static String EVENT_KEY_CREATOR_JSON = "Creator";
    public final static String EVENT_DATE_FORMAT = "yyyy-MM-dd'T'HH:mm:ss";

    public final static String TIME_KEY_START_TIME = "StartTime";

    public final static String LOCATION_KEY_NAME_ADDRESS = "NameOrAddress";

    public final static String API_URL = "http://seteam4.azurewebsites.net/api/";

    
    private final long REFRESH_RATE = 60000; //60 seconds, in milliseconds
    private final String TAG = "MainActivity";
    
    private static JSONObject userJson;
    private static AngoraEvent[] userEvents;
    private static Bitmap userProfilePic;
    private static SharedPreferences preferences;
    
    private NavigationDrawerFragment mNavigationDrawerFragment;
    private CharSequence mTitle;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);


        preferences = getApplicationContext().getSharedPreferences(PREFS_NAME, 0);
        if (!preferences.getBoolean(PREFS_KEY_LOGGED_IN, false)){
            Intent intent = new Intent(this, LoginActivity.class);
            startActivity(intent);
            finish();
            return;
        }

        mNavigationDrawerFragment = (NavigationDrawerFragment)
                getSupportFragmentManager().findFragmentById(R.id.navigation_drawer);
        mTitle = getTitle();

        // Set up the drawer.
        mNavigationDrawerFragment.setUp(
                R.id.navigation_drawer,
                (DrawerLayout) findViewById(R.id.drawer_layout));

        CacheHelper cacheHelper = new CacheHelper(this);

        if (System.currentTimeMillis() - preferences.getLong(PREFS_KEY_LAST_REFRESH, 0) >= REFRESH_RATE){
            refreshData();
        } else {
            try{
                userJson = cacheHelper.getStoredUser();
                userEvents = cacheHelper.getStoredEvents();
                if (!userJson.getString(USER_KEY_PROFILE_PIC_URL).startsWith("/")) {
                    userProfilePic = cacheHelper.getStoredProfilePic();
                }
            }catch (IOException ie){
                Toast.makeText(getApplicationContext(), "Error: "+ ie.getMessage(), Toast.LENGTH_SHORT).show();
                Log.e(TAG, ie.getMessage());
            }catch (ClassNotFoundException ce){
                Toast.makeText(getApplicationContext(), "Error: "+ ce.getMessage(), Toast.LENGTH_SHORT).show();
                Log.e(TAG, ce.getMessage());
            }catch (JSONException je){
                Toast.makeText(getApplicationContext(), "Error: "+ je.getMessage(), Toast.LENGTH_SHORT).show();
                Log.e(TAG, je.getMessage());
            }
        }

    }

    @Override
    public void onBackPressed() {
        //super.onBackPressed();
    }

    private void refreshData(){
        CacheHelper ch = new CacheHelper(this);
        SharedPreferences.Editor editor = preferences.edit();
        try{
            userJson = new LoginUserTask().execute(preferences.getString(PREFS_KEY_LOGIN_PROVIDER, null), preferences.getString(PREFS_KEY_PROVIDER_KEY, null)).get();
            if (userJson == null){
                //user hasn't registered with the site
                SharedPreferences.Editor prefEditor = preferences.edit();
                prefEditor.putBoolean(PREFS_KEY_LOGGED_IN, false);
                prefEditor.commit();

                Toast.makeText(getApplicationContext(), "Please register before using the app!", Toast.LENGTH_SHORT).show();

                Intent intent = new Intent(this, LoginActivity.class);
                startActivity(intent);
                finish();
                return;
            }
            editor.putString(PREFS_KEY_ANGORA_ID, userJson.getString(USER_KEY_ID));
            ch.storeUser(userJson);
            userEvents = new GetEventsTask().execute(userJson.getString(USER_KEY_ID)).get();
            ch.storeEvents(userEvents);

            String profilePicUrl = userJson.getString(USER_KEY_PROFILE_PIC_URL);
            if (profilePicUrl != null){
                if (!profilePicUrl.startsWith("/")){
                    userProfilePic = new GetProfilePicTask().execute(profilePicUrl).get();
                    ch.storeProfilePic(userProfilePic);
                }
            }

        }catch (IOException ie){
            Toast.makeText(getApplicationContext(), "Error Refreshing Data: " + ie.getMessage(), Toast.LENGTH_SHORT).show();
            Log.e(TAG, ie.getMessage());
            ie.printStackTrace();
        }catch (InterruptedException ine){
            Toast.makeText(getApplicationContext(), "Error Refreshing Data: " + ine.getMessage(), Toast.LENGTH_SHORT).show();
            Log.e(TAG, ine.getMessage());
            ine.printStackTrace();
        }catch (ExecutionException ee){
            Toast.makeText(getApplicationContext(), "Error Refreshing Data: " + ee.getMessage(), Toast.LENGTH_SHORT).show();
            Log.e(TAG, ee.getMessage());
            ee.printStackTrace();
        }catch (JSONException je){
            Toast.makeText(getApplicationContext(), "Error Refreshing Data: " + je.getMessage(), Toast.LENGTH_SHORT).show();
            Log.e(TAG, je.getMessage());
        }

        editor.putLong(PREFS_KEY_LAST_REFRESH, System.nanoTime());
        editor.commit();

    }

    @Override
    public void onNavigationDrawerItemSelected(int position) {
        // update the main content by replacing fragments
        switch (position)   {

            case 0:
                //FragmentManager fm = getSupportFragmentManager();
                getSupportFragmentManager().beginTransaction().replace(R.id.container, FeedFragment.newInstance(position)).commit();
                mTitle = getString(R.string.title_feed_section);
                getSupportActionBar().setTitle(mTitle);
                break;
            case 1:
                //FragmentManager fm2 = getSupportFragmentManager();
                getSupportFragmentManager().beginTransaction().replace(R.id.container, ProfileFragment.newInstance(position)).commit();
                mTitle = getString(R.string.title_profile_section);
                getSupportActionBar().setTitle(mTitle);
                break;
            case 2:
                //open the friends fragment
                break;

        }
    }


    public void restoreActionBar() {
        ActionBar actionBar = getSupportActionBar();
        actionBar.setNavigationMode(ActionBar.NAVIGATION_MODE_STANDARD);
        actionBar.setDisplayShowTitleEnabled(true);
        actionBar.setTitle(mTitle);
    }


    @Override
    public boolean onCreateOptionsMenu(Menu menu) {
        getMenuInflater().inflate(R.menu.main, menu);

        return super.onCreateOptionsMenu(menu);
    }

    @Override
    public boolean onOptionsItemSelected(MenuItem item) {
        // Handle action bar item clicks here. The action bar will
        // automatically handle clicks on the Home/Up button, so long
        // as you specify a parent activity in AndroidManifest.xml.
        int id = item.getItemId();
        if (id == R.id.action_logout) {
            SharedPreferences pref = getApplicationContext().getSharedPreferences(PREFS_NAME, 0);
            SharedPreferences.Editor prefEditor = pref.edit();
            prefEditor.putBoolean(PREFS_KEY_LOGGED_IN, false);
            prefEditor.commit();
            Intent intent = new Intent(this, LoginActivity.class);
            startActivity(intent);
            finish();
            return true;
        }else if (id == R.id.action_refresh){
            refreshData();
            //open the feed again
            onNavigationDrawerItemSelected(0);
        }
        return super.onOptionsItemSelected(item);
    }

    public static class FeedFragment extends Fragment {

        private ListView listView;
        private CustomAdapter customAdapter;

        private static final String ARG_SECTION_NUMBER = "section_number";

        public static FeedFragment newInstance(int sectionNumber) {
            FeedFragment fragment = new FeedFragment();

            Bundle args = new Bundle();
            args.putInt(ARG_SECTION_NUMBER, sectionNumber);
            fragment.setArguments(args);


            return fragment;
        }

        public FeedFragment() {
        }


        @Override
        public View onCreateView(LayoutInflater inflater, ViewGroup container,
                Bundle savedInstanceState) {
            View rootView = inflater.inflate(R.layout.fragment_feed, container, false);

            TextView noEventsTextView = (TextView) rootView.findViewById(R.id.textView_noEvents);

            if (userEvents != null) {
                if (userEvents.length > 0) {
                    customAdapter = new CustomAdapter(getActivity(), userEvents);
                    listView = (ListView) rootView.findViewById(R.id.list);
                    listView.setAdapter(customAdapter);
                    listView.setOnItemClickListener(new AdapterView.OnItemClickListener() {
                        @Override
                        public void onItemClick(AdapterView<?> adapterView, View view, int i, long l) {
                            Intent intent = new Intent(getActivity(), SnapAndGoBuiltIn.class);
                            intent.putExtra(EXTRA_EVENT_ID, userEvents[i].getId());
                            startActivity(intent);
                        }
                    });

                    noEventsTextView.setVisibility(View.GONE);
                } else if (preferences.getBoolean(PREFS_KEY_LOGGED_IN, false)) {
                    //if the user is logged in but has no events
                    noEventsTextView.setVisibility(View.VISIBLE);
                    Toast.makeText(getActivity().getApplicationContext(), "No Events to show! Go make some!", Toast.LENGTH_SHORT).show();
                }
            }else if (preferences.getBoolean(PREFS_KEY_LOGGED_IN, false)) {
                //if the user is logged in but has no events
                noEventsTextView.setVisibility(View.VISIBLE);
                Toast.makeText(getActivity().getApplicationContext(), "No Events to show! Go make some!", Toast.LENGTH_SHORT).show();
            }

            return rootView;
        }

        @Override
        public void onAttach(Activity activity) {

            super.onAttach(activity);
        }

    }

    public static class ProfileFragment extends Fragment {

        private static final String ARG_SECTION_NUMBER = "section_number";

        private final String TAG = "ProfileFragment";

        public static ProfileFragment newInstance(int sectionNumber) {
            ProfileFragment fragment = new ProfileFragment();

            Bundle args = new Bundle();
            args.putInt(ARG_SECTION_NUMBER, sectionNumber);
            fragment.setArguments(args);


            return fragment;
        }

        public ProfileFragment() {
        }


        @Override
        public View onCreateView(LayoutInflater inflater, ViewGroup container,
                                 Bundle savedInstanceState) {
            View rootView = inflater.inflate(R.layout.fragment_profile, container, false);

            TextView nameTextView = (TextView) rootView.findViewById(R.id.textView_name);
            TextView locationTextView = (TextView) rootView.findViewById(R.id.textView_location);
            ImageView profilePicImageView = (ImageView) rootView.findViewById(R.id.imageView_profilePic);
            try {
                nameTextView.setText(userJson.getString(USER_KEY_FIRST_NAME) + " " + userJson.getString(USER_KEY_LAST_NAME));
                locationTextView.setText((userJson.getString(USER_KEY_LOCATION)));

                if (userProfilePic != null){
                    profilePicImageView.setImageBitmap(userProfilePic);
                }

            }catch (JSONException je) {
                //TODO Handle
                je.printStackTrace();
                Toast.makeText(getActivity().getApplicationContext(), "Error Refreshing Profile: " + je.getMessage(), Toast.LENGTH_SHORT).show();
                Log.e(TAG, je.getMessage());
            }
            return rootView;
        }

        @Override
        public void onAttach(Activity activity) {

            super.onAttach(activity);

            /*
            ((MainActivity) activity).onSectionAttached(
                    getArguments().getInt(ARG_SECTION_NUMBER));
            */
        }
    }

    public class LoginUserTask extends AsyncTask<String, Void, JSONObject>
    {
        @Override
        protected JSONObject doInBackground(String... strings) {
            StringBuilder urlString = new StringBuilder();
            urlString.append(API_URL).append("user/login?");
            urlString.append("provider=").append(strings[0]);
            urlString.append("&providerKey=").append(strings[1]);

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
                e.printStackTrace();
            }

            return (object);
        }


    }

    public class GetEventsTask extends AsyncTask<String, Void, AngoraEvent[]> {


        private final String SITE_URL = "http://seteam4.azurewebsites.net/api/user/getEvents?";

        @Override
        protected AngoraEvent[] doInBackground(String... strings) {
            StringBuilder urlString = new StringBuilder();
            urlString.append(API_URL).append("/user/getEvents?");
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
                SimpleDateFormat parser = new SimpleDateFormat(EVENT_DATE_FORMAT);
                if (eventsJSON != null) {
                    AngoraEvent[] events = new AngoraEvent[eventsJSON.length()];
                    for (int i = 0; i < eventsJSON.length(); i++) {
                        JSONObject currentEvent = eventsJSON.getJSONObject(i);
                        JSONObject currentEventTime = currentEvent.getJSONObject(EVENT_KEY_TIME_JSON);
                        JSONObject currentEventCreator = currentEvent.getJSONObject(EVENT_KEY_CREATOR_JSON);
                        JSONObject currentEventLocation = currentEvent.getJSONObject(EVENT_KEY_LOCATION);
                        events[i] = new AngoraEvent(currentEvent.getString(EVENT_KEY_ID),
                                currentEvent.getString(EVENT_KEY_NAME),
                                currentEventCreator.getString(USER_KEY_FIRST_NAME) + " " + currentEventCreator.getString(USER_KEY_LAST_NAME),
                                currentEvent.getString(EVENT_KEY_DESC),
                                currentEventLocation.getString(LOCATION_KEY_NAME_ADDRESS),
                                parser.parse(currentEventTime.getString(TIME_KEY_START_TIME)));
                    }

                    Arrays.sort(events, new Comparator<AngoraEvent>() {
                        @Override
                        public int compare(AngoraEvent e1, AngoraEvent e2) {
                            return e1.getStartDate().compareTo(e2.getStartDate());
                        }
                    });

                    return events;
                }
            }catch (Exception e){
                //todo handle
                e.printStackTrace();

            }


            return null;
        }
    }

    public class GetProfilePicTask extends AsyncTask<String, Void, Bitmap>{

        @Override
        protected Bitmap doInBackground(String... strings) {
            Bitmap image = null;
            try {
                InputStream is = (InputStream) new URL(strings[0]).getContent();
                image = BitmapFactory.decodeStream(is);
            } catch (MalformedURLException me) {
                //todo handle
                Log.e(TAG, me.getMessage());
                me.printStackTrace();
            } catch (IOException ie){
                Log.e(TAG, ie.getMessage());
                ie.printStackTrace();
            }
            return image;
        }
    }
}

class CustomAdapter extends BaseAdapter{
    Context context;
    AngoraEvent[] userEvent;
    private static LayoutInflater inflater = null;

    public CustomAdapter(Context context, AngoraEvent[] userEvent){
        this.context = context;
        this.userEvent = userEvent;
        inflater = (LayoutInflater) context.getSystemService(Context.LAYOUT_INFLATER_SERVICE);
    }

    @Override
    public int getCount() {
        return userEvent.length;
    }

    @Override
    public Object getItem(int position) {
        return userEvent[position];
    }

    @Override
    public long getItemId(int position) {
        //this obviously does nothing
        return position;
    }

    @Override
    public View getView(int position, View convertView, ViewGroup parent) {
        View vi = convertView;
        if (vi == null){
            vi = inflater.inflate(R.layout.event_list_row, null);
        }
        TextView name = (TextView) vi.findViewById(R.id.textView_Event_Name);
        TextView creator = (TextView) vi.findViewById(R.id.textView_Event_Creator);
        TextView location = (TextView) vi.findViewById(R.id.textView_Event_Location);
        TextView date = (TextView) vi.findViewById(R.id.textView_Event_Date);
        TextView time = (TextView) vi.findViewById(R.id.textView_Event_Time);

        name.setText(userEvent[position].getName());
        creator.setText(userEvent[position].getCreator());
        location.setText(userEvent[position].getLocation());
        date.setText(userEvent[position].getStartDateString());
        time.setText(userEvent[position].getStartTimeString());

        return vi;
    }


}
