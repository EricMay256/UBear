using System;
using Newtonsoft.Json;

namespace UBear.Leaderboard
{
    /// <summary>
    /// Maps to the API's ScoreResponse shape.
    /// [JsonProperty] bridges C# naming conventions to the API's snake_case keys.
    /// </summary>
    [Serializable]
    public class ScoreResponse
    {
        [JsonProperty("id")]          public int    Id          { get; set; }
        [JsonProperty("player")]      public string Player      { get; set; }
        [JsonProperty("score")]       public int    Score       { get; set; }
        [JsonProperty("game_mode")]   public string GameMode    { get; set; }
        [JsonProperty("period")]      public string Period      { get; set; }
        [JsonProperty("submitted_at")] public string SubmittedAt { get; set; }
    }

    /// <summary>
    /// Maps to the API's GameModeConfig shape.
    /// </summary>
    [Serializable]
    public class GameModeConfig
    {
        [JsonProperty("name")]       public string Name      { get; set; }
        [JsonProperty("sort_order")] public string SortOrder { get; set; }
        [JsonProperty("label")]      public string Label     { get; set; }
    }

    /// <summary>
    /// Body for POST /api/scores. Serialized and sent as JSON.
    /// </summary>
    [Serializable]
    public class ScoreSubmission
    {
        [JsonProperty("player")]    public string Player   { get; set; }
        [JsonProperty("score")]     public int    Score    { get; set; }
        [JsonProperty("game_mode")] public string GameMode { get; set; }
    }

    /// <summary>
    /// Wraps a successful result or an error message.
    /// Callers check Success before reading Data.
    /// </summary>
    public class ApiResult<T>
    {
        public bool   Success { get; }
        public T      Data    { get; }
        public string Error   { get; }

        private ApiResult(bool success, T data, string error)
        {
            Success = success;
            Data    = data;
            Error   = error;
        }

        public static ApiResult<T> Ok(T data)             => new ApiResult<T>(true,  data,    null);
        public static ApiResult<T> Fail(string message)   => new ApiResult<T>(false, default, message);
    }
}