package com.angora.angora;

import java.text.SimpleDateFormat;
import java.util.Date;
import java.text.DateFormat;

/**
 * Created by Alex on 3/19/14.
 */
public class AngoraEvent {
    public String getName() {
        return name;
    }

    public void setName(String name) {
        this.name = name;
    }

    public String getCreator() {
        return creator;
    }

    public void setCreator(String creator) {
        this.creator = creator;
    }

    public String getLocation() {
        return location;
    }

    public void setLocation(String location) {
        this.location = location;
    }

    public Date getStartDate() {
        return startDate;
    }

    public void setStartDate(Date startDate) {
        this.startDate = startDate;
    }

    public String getStartTimeString(){
        DateFormat df = new SimpleDateFormat("hh:mm a");
        return df.format(this.startDate);
    }

    public String getStartDateString(){
        DateFormat df = new SimpleDateFormat("MM/dd/yyyy");
        return df.format(this.startDate);
    }

    private String name;
    private String creator;
    private String location;
    private Date startDate;

    public AngoraEvent(){};

    public AngoraEvent(String name, String creator, String location, Date startDate){
        this.name = name;
        this.creator = creator;
        this.location = location;
        this.startDate = startDate;
    }


}
