// Intended to implement https://github.com/EricMay256/HighScoreServer
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace UBear.Leaderboard
{
    /// <summary>
    /// Handles all communication with the leaderboard API.
    /// 
    /// Usage: attach to a persistent GameObject, or access via a singleton.
    /// All public methods are coroutines — start them with StartCoroutine().
    /// Callbacks follow the pattern Action&lt;ApiResult&lt;T&gt;&gt; so callers always
    /// receive either data or a human-readable error, never an unhandled exception.
    /// </summary>
    public class LeaderboardService : MonoBehaviour
    {
        [Header("Configuration")]
        [Tooltip("Responsible for base URL and API Key. Create via Assets → Create → UBear → LeaderboardConfig.")]
        [SerializeField] private LeaderboardConfig _config;

        private const int TimeoutSeconds = 10;

        // ── Public API ─────────────────────────────────────────────────────────

        /// <summary>
        /// Fetches the top scores for a given game mode and period.
        /// Period is one of: "alltime", "daily", "weekly".
        /// </summary>
        public IEnumerator GetScores(
            string gameMode,
            Action<ApiResult<List<ScoreResponse>>> callback,
            string period = "alltime")
        {
            string url = $"{_config.BaseUrl}/api/scores?game_mode={gameMode}&period={period}";
            yield return Get(url, callback);
        }

        /// <summary>
        /// Fetches all registered game modes and their configuration.
        /// Useful for populating a mode selector in the UI.
        /// </summary>
        public IEnumerator GetGameModes(Action<ApiResult<List<GameModeConfig>>> callback)
        {
            string url = $"{_config.BaseUrl}/api/game_modes";
            yield return Get(url, callback);
        }

        /// <summary>
        /// Submits a score. Requires a valid API key set in the Inspector.
        /// The server upserts — if the player already has a better score, 
        /// their existing record is preserved and returned.
        /// </summary>
        public IEnumerator SubmitScore(
            string player,
            int    score,
            string gameMode,
            Action<ApiResult<ScoreResponse>> callback)
        {
            string url  = $"{_config.BaseUrl}/api/scores";
            var    body = new ScoreSubmission { Player = player, Score = score, GameMode = gameMode };
            yield return Post(url, body, callback);
        }

        // ── Private HTTP helpers ───────────────────────────────────────────────

        /// <summary>
        /// Generic GET — deserializes the response body into T.
        /// </summary>
        private IEnumerator Get<T>(string url, Action<ApiResult<T>> callback)
        {
            using UnityWebRequest request = UnityWebRequest.Get(url);
            request.timeout = TimeoutSeconds;

            yield return request.SendWebRequest();

            callback(ParseResponse<T>(request));
        }

        /// <summary>
        /// Generic POST — serializes body to JSON, deserializes response into T.
        /// Attaches the API key header automatically.
        /// </summary>
        private IEnumerator Post<TBody, TResponse>(
            string                        url,
            TBody                         body,
            Action<ApiResult<TResponse>>  callback)
        {
            string json    = JsonConvert.SerializeObject(body);
            byte[] encoded = Encoding.UTF8.GetBytes(json);

            using UnityWebRequest request = new UnityWebRequest(url, "POST");
            request.uploadHandler   = new UploadHandlerRaw(encoded);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("x-api-key", _config.ApiKey);
            request.timeout = TimeoutSeconds;

            yield return request.SendWebRequest();

            callback(ParseResponse<TResponse>(request));
        }

        /// <summary>
        /// Interprets a completed UnityWebRequest as ApiResult&lt;T&gt;.
        /// Network errors, HTTP errors (4xx/5xx), and JSON parse failures
        /// all surface as ApiResult.Fail with a message rather than exceptions.
        /// </summary>
        private static ApiResult<T> ParseResponse<T>(UnityWebRequest request)
        {
            if (request.result != UnityWebRequest.Result.Success)
            {
                // Network failure or HTTP error — the server's error body may
                // contain a useful "detail" field; include it if present.
                string serverDetail = TryExtractDetail(request.downloadHandler?.text);
                string message = string.IsNullOrEmpty(serverDetail)
                    ? $"Request failed: {request.error}"
                    : $"Request failed ({request.responseCode}): {serverDetail}";

                return ApiResult<T>.Fail(message);
            }

            string responseBody = request.downloadHandler.text;

            try
            {
                T data = JsonConvert.DeserializeObject<T>(responseBody);
                return ApiResult<T>.Ok(data);
            }
            catch (JsonException ex)
            {
                Debug.LogError($"[LeaderboardService] JSON parse error: {ex.Message}\nBody: {responseBody}");
                return ApiResult<T>.Fail("Unexpected response format from server.");
            }
        }

        /// <summary>
        /// FastAPI wraps validation/HTTP errors in {"detail": "..."}.
        /// Extract that message when available.
        /// </summary>
        private static string TryExtractDetail(string json)
        {
            if (string.IsNullOrEmpty(json)) return null;
            try
            {
                var obj = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
                obj.TryGetValue("detail", out string detail);
                return detail;
            }
            catch { return null; }
        }
    }
}