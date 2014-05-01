package com.angora.angora;

import java.io.Serializable;
import java.text.SimpleDateFormat;
import java.util.Date;
import java.text.DateFormat;

/**
 * Created by Alex on 3/19/14.
 * Serializable so that it can be cached
 */
public class AngoraEvent implements Serializable {
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

    public String getDescription() {
        return description;
    }

    public void setDescription(String description) {
        this.description = description;
    }

    public String getId() {
        return id;
    }

    public void setId(String id) {
        this.id = id;
    }


    private String id;
    private String name;
    private String creator;
    private String location;
    private String description;
    private Date startDate;


    public AngoraEvent(){};


    public AngoraEvent(String id, String name, String creator, String description, String location, Date startDate){
        this.id = id;
        this.name = name;
        this.creator = creator;
        this.description = description;
        this.location = location;
        this.startDate = startDate;
    }

}
