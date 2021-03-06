﻿using System.Collections.Generic;
using System.Linq;
using Nancy;
using PactNet.Mocks.MockHttpService.Models;

namespace PactNet.Mocks.MockHttpService.Nancy
{
    public static class NancyContextExtensions
    {
        public static void SetMockInteraction(this NancyContext context, IEnumerable<ProviderServiceInteraction> interactions)
        {
            context.Items[Constants.PactMockInteractionsKey] = interactions;
        }

        public static ProviderServiceInteraction GetMatchingMockInteraction(this NancyContext context, HttpVerb method, string path)
        {
            if (!context.Items.ContainsKey(Constants.PactMockInteractionsKey))
            {
                throw new PactFailureException("No mock interactions have been registered");
            }

            var interactions = (IEnumerable<ProviderServiceInteraction>)context.Items[Constants.PactMockInteractionsKey];

            if (interactions == null)
            {
                throw new PactFailureException("No matching mock interaction has been registered for the current request");
            }

            var matchingInteractions = interactions.Where(x =>
                x.Request.Method == method &&
                x.Request.Path == path).ToList();

            if (matchingInteractions == null || !matchingInteractions.Any())
            {
                throw new PactFailureException("No matching mock interaction has been registered for the current request");
            }

            if (matchingInteractions.Count() > 1)
            {
                throw new PactFailureException("More than one matching mock interaction has been registered for the current request");
            }

            return matchingInteractions.Single();
        }

        public static IEnumerable<ProviderServiceInteraction> GetMockInteractions(this NancyContext context)
        {
            if (context.Items.ContainsKey(Constants.PactMockInteractionsKey))
            {
                return (IEnumerable<ProviderServiceInteraction>)context.Items[Constants.PactMockInteractionsKey];
            }

            return new List<ProviderServiceInteraction>();
        }
    }
}
