﻿using Angora.Data.Models;
using System;
using System.Collections.Generic;
using System.ServiceModel;


namespace Angora.Services
{
    [ServiceContract]
    public interface IEventService
    {
        [OperationContract]
        long Create(Event newEvent);

        [OperationContract]
        long Edit(Event oldEvent);

        //not sure if we should be returning anythang
        [OperationContract]
        void Delete(Event oldEvent);

        [OperationContract]
        Event FindById(long id);

        [OperationContract]
        List<Event> FindEventsByUserId(string userId);

    }
}