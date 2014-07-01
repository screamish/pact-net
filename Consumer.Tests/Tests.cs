﻿using System;
using System.Collections.Generic;
using System.Linq;
using Concord;
using Xunit;

namespace Consumer.Tests
{
    public class Tests
    {
        //TODO: Test order is important here atm, refactor so it isn't
        //TODO: Refactor the code, it needs a big cleanup

        //TODO:! Implement a new test and share the server for both consumer and provider
        //TODO:! Assertions in library (Look how other testing tools do it)


        private const string BaseUri = "http://localhost:1234";

        /*private Pact _pact;
        private PactProvider _pactProviderMock;
        private TestServer _testServer;*/

        /*public Tests()
        {
            _pact = 

            _pact.StartServer();

            _testServer = TestServer.Create<Startup>();
            _testServer.HttpClient.BaseAddress = new Uri(BaseUri); //Don't think we really need to do this
        }

        public void Dispose()
        {
            _pact.StopServer();
            _testServer.Dispose();
        }*/

        [Fact]
        public void GetAllEvents_WhenCalled_ReturnsEvents()
        {
            var pact = new Pact().ServiceConsumer("Source System")
                .HasPactWith("Event API")
                .MockService(1234);

            var pactProviderMock = pact.GetMockProvider();

            pactProviderMock.UponReceiving("A GET request to retrieve all events")
                .With(new PactProviderRequest
                {
                    Method = HttpVerb.Get,
                    Path = "/events",
                    Headers = new Dictionary<string, string>
                    {
                        { "Content-Type", "application/json" }
                    }
                })
                .WillRespondWith(new PactProviderResponse
                {
                    Status = 200,
                    Headers = new Dictionary<string, string>
                    {
                        { "Content-Type", "application/json; charset=utf-8" }
                    },
                    Body = new List<dynamic>
                    {
                        new 
                        {
                            EventId = Guid.Parse("45D80D13-D5A2-48D7-8353-CBB4C0EAABF5"),
                            Timestamp = "2014-06-30T01:37:41.0660548Z",
                            EventType = "JobSearchView"
                        },
                        new
                        {
                            EventId = Guid.Parse("83F9262F-28F1-4703-AB1A-8CFD9E8249C9"),
                            Timestamp = "2014-06-30T01:37:52.2618864Z",
                            EventType = "JobDetailsView"
                        },
                        new
                        {
                            EventId = Guid.Parse("3E83A96B-2A0C-49B1-9959-26DF23F83AEB"),
                            Timestamp = "2014-06-30T01:38:00.8518952Z",
                            EventType = "JobSearchView"
                        }
                    }
                });

            var consumer = new TestApiConsumer(BaseUri);

            //Act
            pact.StartServer();
            var events = consumer.GetAllEvents();
            pact.StopServer();

            Assert.NotEmpty(events);
            Assert.Equal(3, events.Count());
        }
    }
}