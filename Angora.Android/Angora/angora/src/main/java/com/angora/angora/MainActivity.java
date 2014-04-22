package com.angora.angora;

import android.app.Activity;
import android.app.ListFragment;
import android.content.Intent;
import android.content.SharedPreferences;
import android.graphics.drawable.Drawable;
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
import android.widget.TextView;
import android.widget.Toast;

import org.apache.http.protocol.HTTP;
import org.json.JSONException;
import org.json.JSONObject;

import java.io.BufferedInputStream;
import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.net.HttpURLConnection;
import java.net.MalformedURLException;
import java.net.URL;
import java.text.ParseException;
import java.text.SimpleDateFormat;
import java.util.Date;

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


    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);


        SharedPreferences pref = getApplicationContext().getSharedPreferences("MyPrefs", 0);
        if (!pref.getBoolean("IsLoggedIn", false)){
            Intent intent = new Intent(this, LoginActivity.class);
            startActivity(intent);
            finish();
        }
        mNavigationDrawerFragment = (NavigationDrawerFragment)
                getSupportFragmentManager().findFragmentById(R.id.navigation_drawer);
        mTitle = getTitle();

        // Set up the drawer.
        mNavigationDrawerFragment.setUp(
                R.id.navigation_drawer,
                (DrawerLayout) findViewById(R.id.drawer_layout));

        try {
            mUser = new LoginUserTask().execute(pref.getString("LoginProvider", null), pref.getString("ProviderKey", null)).get();
        }catch (Exception e){
            //todo something
        }
    }

    @Override
    public void onNavigationDrawerItemSelected(int position) {
        // update the main content by replacing fragments
        //This isn't being used in Beta Release.


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
/*
    public void onSectionAttached(int number) {
        switch (number) {
            case 0:
                mTitle = getString(R.string.title_feed_section);
                break;
            case 1:
                mTitle = getString(R.string.title_profile_section);
                break;
            case 2:
                mTitle = getString(R.string.title_friends_section);
                break;
        }
    }
    */

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
            AngoraEvent[] userEvent = generateFakeEvents();

            mAdapter = new CustomAdapter(getActivity(), userEvent);

            mListView = (ListView) rootView.findViewById(R.id.list);
            mListView.setAdapter(mAdapter);

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

        /*
        this is purely for testing
         */
        public AngoraEvent[] generateFakeEvents(){
            AngoraEvent[] userEvent = new AngoraEvent[15];
            SimpleDateFormat parser = new SimpleDateFormat("MM/dd/yyy hh:mm a");
            try{
                userEvent[0] = new AngoraEvent("Birthday Party", "Alex Bainter", "Seward, NE", parser.parse("02/02/2014 4:00 PM"));
                userEvent[1] = new AngoraEvent("Dance Party", "Billy Bob", "Lincoln, NE", parser.parse("02/08/2014 7:00 AM"));
                userEvent[2] = new AngoraEvent("Hoe Down", "Dirty Dan", "Lincoln, NE", parser.parse("03/01/2014 5:00 PM"));
                userEvent[3] = new AngoraEvent("Hootenanny", "Pinhead Larry", "Lincoln, NE", parser.parse("03/01/2014 5:00 PM"));
                userEvent[4] = new AngoraEvent("\"Product\" Cook", "Walter White", "Albuquerque, NM", parser.parse("04/05/2014 6:00 AM"));
                userEvent[5] = new AngoraEvent("Breakfast Party", "Walter White Jr.", "Alburquerque, NM", parser.parse("05/30/2014 11:00 AM"));
                userEvent[6] = new AngoraEvent("Lil' Bringus Concert", "Mikey Mike", "Lincoln, NE", parser.parse("06/1/2014 6:00 PM"));
                userEvent[7] = new AngoraEvent("Birthday Party", "Alex Bainter", "Seward, NE", parser.parse("02/02/2015 4:00 PM"));
                userEvent[8] = new AngoraEvent("Dance Party", "Billy Bob", "Lincoln, NE", parser.parse("02/08/2015 7:00 AM"));
                userEvent[9] = new AngoraEvent("Hoe Down", "Dirty Dan", "Lincoln, NE", parser.parse("03/01/2015 5:00 PM"));
                userEvent[10] = new AngoraEvent("Hootenanny", "Pinhead Larry", "Lincoln, NE", parser.parse("03/01/2015 5:00 PM"));
                userEvent[11] = new AngoraEvent("\"Product\" Cook", "Walter White", "Albuquerque, NM", parser.parse("04/05/2015 6:00 AM"));
                userEvent[12] = new AngoraEvent("Breakfast Party", "Walter White Jr.", "Alburquerque, NM", parser.parse("05/30/2015 11:00 AM"));
                userEvent[13] = new AngoraEvent("Lil' Bringus Concert", "Mikey Mike", "Lincoln, NE", parser.parse("06/1/2015 6:00 PM"));
                userEvent[14] = new AngoraEvent("Movie Marathon", "Sarah Elmwood", "Omaha, NE", parser.parse("04/15/2015 7:00 PM"));

            }catch(ParseException e){
                this.getActivity().finish();
            }

            return userEvent;
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
            try {
                nameTextView.setText(mUser.getString("FirstName") + mUser.getString("LastName"));
            }catch (JSONException e){
                //TODO Handle
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
