﻿namespace SurveyBasket.API.Contracts.cs.Responses
{
    public class PollResponse
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
