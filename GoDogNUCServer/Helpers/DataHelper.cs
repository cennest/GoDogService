using System;
using System.Collections.Generic;
//using StackExchange.Redis;

using GoDogCommon;
using GoDogNUCServer.Models;

namespace GoDogNUCServer.Helpers
{
    public sealed class DataHelper
    {
        private static DataHelper dataHelper;
        private static readonly object locker = new object();
        //private static Lazy<ConnectionMultiplexer> redisConnection;

        public DataHelper()
        {
            /*redisConnection = new Lazy<ConnectionMultiplexer>(() =>
            {
                return ConnectionMultiplexer.Connect("GoDogManager");
            });*/
        }

        /*public static ConnectionMultiplexer RedisConnection
        {
            get
            {
                return redisConnection.Value;
            }
        }*/

        public static DataHelper GetDataHelper()
        {
            lock (locker)
            {
                if (dataHelper == null)
                {
                    dataHelper = new DataHelper();
                }
                return dataHelper;
            }
        }

        public Field GetField()
        {
            Field field = null;
            Facility facility = GetFacility();

            foreach (Field _field in facility.Fields)
            {
                if (field == null)
                {
                    string biosId = Configuration.BiosID();
                    if (_field.NUC.ID == biosId)
                    {
                        field = _field;
                    }
                }
                else
                {
                    break;
                }
            }

            return field;
        }

        private Facility GetFacility()
        {
            return new Facility()
            {
                ID = 1,
                Name = "Facility1",
                Fields = new List<Field>()
                {
                    new Field()
                    {
                        ID= 1,
                        Name = "Field 1",
                        NUC = new NUCDevice()
                        {
                            ID = "C07L394NDWYL",
                            Name = "NUC Device 1",
                            Cameras = new List<Camera>()
                            {
                                new Camera()
                                {
                                    ID =1,
                                    Name = "Camera 1",
                                    IPAddress= "192.168.1.64",
                                    Username = "admin",
                                    Password = "Foxhound1",
                                    ArchivalPeriod = 1,
                                    CameraURL = "rtsp://admin:Foxhound1!@192.168.1.64/Streaming/Channels/1",
                                    StreamingURL = "rtmp://104.248.182.51/live/2"
                                },
                                new Camera()
                                {
                                    ID =2,
                                    Name = "Camera 2",
                                    IPAddress= "192.168.1.64",
                                    Username = "admin",
                                    Password = "Foxhound1",
                                    ArchivalPeriod = 1,
                                    CameraURL = "rtsp://admin:Foxhound1!@192.168.1.64/Streaming/Channels/2",
                                    StreamingURL = "rtmp://104.248.182.51/live/3"
                                },
                                new Camera()
                                {
                                    ID =3,
                                    Name = "Camera 3",
                                    IPAddress= "192.168.1.120:554",
                                    Username = "admin",
                                    Password = "admin",
                                    ArchivalPeriod = 1,
                                    CameraURL = "rtsp://admin:admin@192.168.1.120:554/live/main",
                                    StreamingURL = "rtmp://104.248.182.51/live/4"
                                },
                                new Camera()
                                {
                                    ID =4,
                                    Name = "Camera 4",
                                    IPAddress= "192.168.1.120:554",
                                    Username = "admin",
                                    Password = "admin",
                                    ArchivalPeriod = 1,
                                    CameraURL = "rtsp://admin:admin@192.168.1.120:554/live/sub",
                                    StreamingURL = "rtmp://104.248.182.51/live/5"
                                }
                            }
                        }
                    },
                    new Field()
                    {
                        ID= 1,
                        Name = "Field 1",
                        NUC = new NUCDevice()
                        {
                            ID = "C07L394NDWYL",
                            Name = "NUC Device 1",
                            Cameras = new List<Camera>()
                            {
                                new Camera()
                                {
                                    ID =1,
                                    Name = "Camera 1",
                                    IPAddress= "192.168.1.64",
                                    Username = "admin",
                                    Password = "Foxhound1",
                                    ArchivalPeriod = 1,
                                    CameraURL = "rtsp://admin:Foxhound1!@192.168.1.64/Streaming/Channels/1",
                                    StreamingURL = "rtmp://104.248.182.51/live/2"
                                },
                                new Camera()
                                {
                                    ID =2,
                                    Name = "Camera 2",
                                    IPAddress= "192.168.1.64",
                                    Username = "admin",
                                    Password = "Foxhound1",
                                    ArchivalPeriod = 1,
                                    CameraURL = "rtsp://admin:Foxhound1!@192.168.1.64/Streaming/Channels/2",
                                    StreamingURL = "rtmp://104.248.182.51/live/3"
                                },
                                new Camera()
                                {
                                    ID =3,
                                    Name = "Camera 3",
                                    IPAddress= "192.168.1.120:554",
                                    Username = "admin",
                                    Password = "admin",
                                    ArchivalPeriod = 1,
                                    CameraURL = "rtsp://admin:admin@192.168.1.120:554/live/main",
                                    StreamingURL = "rtmp://104.248.182.51/live/4"
                                },
                                new Camera()
                                {
                                    ID =4,
                                    Name = "Camera 4",
                                    IPAddress= "192.168.1.120:554",
                                    Username = "admin",
                                    Password = "admin",
                                    ArchivalPeriod = 1,
                                    CameraURL = "rtsp://admin:admin@192.168.1.120:554/live/sub",
                                    StreamingURL = "rtmp://104.248.182.51/live/5"
                                }
                            }
                        }
                    }
                }
            };
        }
    }
}
