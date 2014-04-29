package com.angora.angora;

import android.app.Activity;
import android.app.ListFragment;
import android.content.Intent;
import android.content.SharedPreferences;
import android.graphics.drawable.Drawable;
import android.os.AsyncTask;
import android.provider.ContactsContract;
import android.support.v7.app.ActionBarActivity;
import android.support.v7.app.ActionBar;
import android.support.v4.app.Fragment;
import android.support.v4.app.FragmentManager;
import android.content.Context;
import android.os.Build;
import android.os.Bundle;
import android.util.Log;
import android.view.Gravity;
import android.view.LayoutInflater;
import android.view.Menu;
import android.view.MenuItem;
import android.view.View;
import android.view.ViewGroup;
import android.support.v4.widget.DrawerLayout;
import android.widget.ArrayAdapter;
import android.widget.BaseAdapter;
import android.widget.ImageView;
import android.widget.ListView;
import android.widget.ProgressBar;
import android.widget.TextView;
import android.widget.Toast;

import org.apache.http.protocol.HTTP;
import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;

import java.io.BufferedInputStream;
import java.io.BufferedReader;
import java.io.DataInputStream;
import java.io.DataOutputStream;
import java.io.File;
import java.io.FileInputStream;
import java.io.IOException;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.net.HttpURLConnection;
import java.net.MalformedURLException;
import java.net.URL;
import java.text.ParseException;
import java.text.SimpleDateFormat;
import java.util.Date;
import java.util.concurrent.ExecutionException;

public class MainActivity extends ActionBarActivity
        implements NavigationDrawerFragment.NavigationDrawerCallbacks {

    /**
     * Fragment managing the behaviors, interactions and presentation of the navigation drawer.
     */
    private NavigationDrawerFragment mNavigationDrawerFragment;

    /**
     * Used to store the last screen title. For use in {@link #restoreActionBar()}.
     */
    private CharSequence mTitle;

    private static JSONObject mUser;
    private static AngoraEvent[] mEvents;
    private static SharedPreferences pref;
    private static Context appContext;

    private CacheHelper mCacheHelper;

    private ProgressBar progBar;

    private final long REFRESH_RATE = 60000; //60 seconds, in milliseconds

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);


        pref = getApplicationContext().getSharedPreferences("MyPrefs", 0);
        if (!pref.getBoolean("IsLoggedIn", false)){
            Intent intent = new Intent(this, LoginActivity.class);
            startActivity(intent);
            finish();
            return;
        }

        appContext = getApplicationContext();
        progBar = (ProgressBar) findViewById(R.id.progressBar_main);

        mNavigationDrawerFragment = (NavigationDrawerFragment)
                getSupportFragmentManager().findFragmentById(R.id.navigation_drawer);
        mTitle = getTitle();

        // Set up the drawer.
        mNavigationDrawerFragment.setUp(
                R.id.navigation_drawer,
                (DrawerLayout) findViewById(R.id.drawer_layout));

        mCacheHelper = new CacheHelper(this);

        if (System.currentTimeMillis() - pref.getLong("LastRefresh", 0) >= REFRESH_RATE){
            refreshData();
        } else {
            try{
                mUser = mCacheHelper.getStoredUser();
                mEvents = mCacheHelper.getStoredEvents();
            }catch (IOException ie){
                Toast.makeText(appContext, "Error: "+ ie.getMessage(), Toast.LENGTH_SHORT).show();
                Log.e("Main Activity", ie.getMessage());
            }catch (ClassNotFoundException ce){
                Toast.makeText(appContext, "Error: "+ ce.getMessage(), Toast.LENGTH_SHORT).show();
                Log.e("Main Activity", ce.getMessage());
            }catch (JSONException je){
                Toast.makeText(appContext, "Error: "+ je.getMessage(), Toast.LENGTH_SHORT).show();
                Log.e("Main Activity", je.getMessage());
            }
        }

    }

    private void refreshData(){
        progBar.setVisibility(View.VISIBLE);
        CacheHelper ch = new CacheHelper(this);
        SharedPreferences.Editor editor = pref.edit();
        try{
            mUser = new LoginUserTask().execute(pref.getString("LoginProvider", null), pref.getString("ProviderKey", null)).get();
            if (mUser == null){
                //user hasn't registered with the site
                SharedPreferences.Editor prefEditor = pref.edit();
                prefEditor.putBoolean("IsLoggedIn", false);
                prefEditor.commit();

                Toast.makeText(appContext, "Please register before using the app!", Toast.LENGTH_SHORT).show();

                Intent intent = new Intent(this, LoginActivity.class);
                startActivity(intent);
                finish();
                return;
            }
            ch.storeUser(mUser);
            mEvents = new GetEventsTask().execute(mUser.getString("Id")).get();
            ch.storeEvents(mEvents);
        }catch (IOException ie){
            Toast.makeText(appContext, "Error Refreshing Data: " + ie.getMessage(), Toast.LENGTH_SHORT).show();
            Log.e("Main Activity", ie.getMessage());
            ie.printStackTrace();
        }catch (InterruptedException ine){
            Toast.makeText(appContext, "Error Refreshing Data: " + ine.getMessage(), Toast.LENGTH_SHORT).show();
            Log.e("Main Activity", ine.getMessage());
            ine.printStackTrace();
        }catch (ExecutionException ee){
            Toast.makeText(appContext, "Error Refreshing Data: " + ee.getMessage(), Toast.LENGTH_SHORT).show();
            Log.e("Main Activity", ee.getMessage());
            ee.printStackTrace();
        }catch (JSONException je){
            Toast.makeText(appContext, "Error Refreshing Data: " + je.getMessage(), Toast.LENGTH_SHORT).show();
            Log.e("Main Activity", je.getMessage());
        }

        editor.putLong("LastRefresh", System.nanoTime());
        progBar.setVisibility(View.INVISIBLE);
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
        /*
        if (!mNavigationDrawerFragment.isDrawerOpen()) {
            // Only show items in the action bar relevant to this screen
            // if the drawer is not showing. Otherwise, let the drawer
            // decide what to show in the action bar.
            getMenuInflater().inflate(R.menu.main, menu);
            restoreActionBar();
            return true;
        }
        */
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
            SharedPreferences pref = getApplicationContext().getSharedPreferences("MyPrefs", 0);
            SharedPreferences.Editor prefEditor = pref.edit();
            prefEditor.putBoolean("IsLoggedIn", false);
            prefEditor.commit();
            Intent intent = new Intent(this, LoginActivity.class);
            startActivity(intent);
            finish();
            return true;
        }
        return super.onOptionsItemSelected(item);
    }

    public static class FeedFragment extends Fragment {
        private ListView mListView;
        private CustomAdapter mAdapter;
        /**
         * The fragment argument representing the section number for this
         * fragment.
         */
        private static final String ARG_SECTION_NUMBER = "section_number";

        /**
         * Returns a new instance of this fragment for the given section
         * number.
         */
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
            try {
                mEvents = new GetEventsTask().execute(mUser.getString("Id")).get();
            }catch (Exception je){
                 je.printStackTrace();
            }
            if (mEvents != null) {
                mAdapter = new CustomAdapter(getActivity(), mEvents);
                mListView = (ListView) rootView.findViewById(R.id.list);
                mListView.setAdapter(mAdapter);
            }else if (pref.getBoolean("IsLoggedIn", false)){
                //if the user is logged in but has no events
                Toast.makeText(appContext, "No Events to show! Go make some!", Toast.LENGTH_SHORT).show();
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

    public static class ProfileFragment extends Fragment {

        /**
         * The fragment argument representing the section number for this
         * fragment.
         */
        private static final String ARG_SECTION_NUMBER = "section_number";

        /**
         * Returns a new instance of this fragment for the given section
         * number.
         */
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
            try {
                nameTextView.setText(mUser.getString("FirstName") + " " + mUser.getString("LastName"));
                locationTextView.setText((mUser.getString("Location")));
                String profilePicUrl = mUser.getString("ProfilePictureUrl");
                if (profilePicUrl != null){
                    //todo use picture
                }
            }catch (JSONException je) {
                //TODO Handle
                je.printStackTrace();
                Toast.makeText(appContext, "Error Refreshing Profile: " + je.getMessage(), Toast.LENGTH_SHORT).show();
                Log.e("Feed Activity", je.getMessage());
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
