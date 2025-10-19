using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using BarangayTech.Models;
using System.Text;

namespace BarangayTech.Services
{
    public static class MobileApiService
    {
        // Android emulator loopback to host machine
    private static string BaseUrl = "http://10.0.2.2:5000"; // adjust if your API listens on a different port
    private static string? _apiKey;
        private static readonly HttpClient _http = new HttpClient { Timeout = TimeSpan.FromSeconds(20) };
        private static readonly JsonSerializerOptions _json = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        // Optional: call at app startup to override BaseUrl or provide an API key for AI endpoints
        public static void Configure(string? baseUrl = null, string? apiKey = null, TimeSpan? timeout = null)
        {
            if (!string.IsNullOrWhiteSpace(baseUrl))
            {
                BaseUrl = baseUrl.TrimEnd('/');
            }
            _apiKey = apiKey;
            if (timeout.HasValue)
            {
                _http.Timeout = timeout.Value;
            }
        }

        // --- Generic helpers ---
        private static void AttachAuthHeaders(HttpRequestMessage req)
        {
            if (!string.IsNullOrWhiteSpace(_apiKey))
            {
                // Common pattern: use Bearer token. Adjust if your AI API expects a different header.
                if (!req.Headers.Contains("Authorization"))
                    req.Headers.Add("Authorization", $"Bearer {_apiKey}");
            }
        }

        private static async Task<T> GetJsonAsync<T>(string relativeUrl, CancellationToken ct = default)
        {
            var req = new HttpRequestMessage(HttpMethod.Get, $"{BaseUrl}{relativeUrl}");
            AttachAuthHeaders(req);
            var res = await _http.SendAsync(req, ct);
            res.EnsureSuccessStatusCode();
            var body = await res.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(body, _json) ?? throw new InvalidOperationException($"Empty response body for {relativeUrl}");
        }

        private static async Task<T> PostJsonAsync<T>(string relativeUrl, object payload, CancellationToken ct = default)
        {
            var json = JsonSerializer.Serialize(payload, _json);
            using var req = new HttpRequestMessage(HttpMethod.Post, $"{BaseUrl}{relativeUrl}")
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
            AttachAuthHeaders(req);
            var res = await _http.SendAsync(req, ct);
            res.EnsureSuccessStatusCode();
            var body = await res.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(body, _json) ?? throw new InvalidOperationException($"Empty response body for {relativeUrl}");
        }

        private static async Task PutJsonAsync(string relativeUrl, object payload, CancellationToken ct = default)
        {
            var json = JsonSerializer.Serialize(payload, _json);
            using var req = new HttpRequestMessage(HttpMethod.Put, $"{BaseUrl}{relativeUrl}")
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
            AttachAuthHeaders(req);
            var res = await _http.SendAsync(req, ct);
            res.EnsureSuccessStatusCode();
        }

        private static async Task DeleteAsync(string relativeUrl, CancellationToken ct = default)
        {
            var req = new HttpRequestMessage(HttpMethod.Delete, $"{BaseUrl}{relativeUrl}");
            AttachAuthHeaders(req);
            var res = await _http.SendAsync(req, ct);
            res.EnsureSuccessStatusCode();
        }

        // --- Read endpoints (existing) ---
        public static async Task<List<Announcement>> GetAnnouncementsAsync(CancellationToken ct = default)
        {
            var list = await GetJsonAsync<List<Announcement>>("/api/announcements", ct);
            return list ?? new List<Announcement>();
        }

        public static async Task<List<Event>> GetEventsAsync(CancellationToken ct = default)
        {
            var list = await GetJsonAsync<List<Event>>("/api/events", ct);
            return list ?? new List<Event>();
        }

        public static async Task<List<Service>> GetServicesAsync(CancellationToken ct = default)
        {
            var list = await GetJsonAsync<List<Service>>("/api/services", ct);
            return list ?? new List<Service>();
        }

        public static async Task<List<Official>> GetOfficialsAsync(CancellationToken ct = default)
        {
            var list = await GetJsonAsync<List<Official>>("/api/officials", ct);
            return list ?? new List<Official>();
        }

        // --- Announcement CRUD ---
        public static async Task<Announcement> CreateAnnouncementAsync(Announcement dto, CancellationToken ct = default)
        {
            return await PostJsonAsync<Announcement>("/api/announcements", dto, ct);
        }

        public static async Task UpdateAnnouncementAsync(int id, Announcement dto, CancellationToken ct = default)
        {
            await PutJsonAsync($"/api/announcements/{id}", dto, ct);
        }

        public static async Task DeleteAnnouncementAsync(int id, CancellationToken ct = default)
        {
            await DeleteAsync($"/api/announcements/{id}", ct);
        }

        // --- Event CRUD ---
        public static async Task<Event> CreateEventAsync(Event dto, CancellationToken ct = default)
        {
            return await PostJsonAsync<Event>("/api/events", dto, ct);
        }

        public static async Task UpdateEventAsync(int id, Event dto, CancellationToken ct = default)
        {
            await PutJsonAsync($"/api/events/{id}", dto, ct);
        }

        public static async Task DeleteEventAsync(int id, CancellationToken ct = default)
        {
            await DeleteAsync($"/api/events/{id}", ct);
        }

        // --- Service CRUD ---
        public static async Task<Service> CreateServiceAsync(Service dto, CancellationToken ct = default)
        {
            return await PostJsonAsync<Service>("/api/services", dto, ct);
        }

        public static async Task UpdateServiceAsync(int id, Service dto, CancellationToken ct = default)
        {
            await PutJsonAsync($"/api/services/{id}", dto, ct);
        }

        public static async Task DeleteServiceAsync(int id, CancellationToken ct = default)
        {
            await DeleteAsync($"/api/services/{id}", ct);
        }

        // --- Official CRUD ---
        public static async Task<Official> CreateOfficialAsync(Official dto, CancellationToken ct = default)
        {
            return await PostJsonAsync<Official>("/api/officials", dto, ct);
        }

        public static async Task UpdateOfficialAsync(int id, Official dto, CancellationToken ct = default)
        {
            await PutJsonAsync($"/api/officials/{id}", dto, ct);
        }

        public static async Task DeleteOfficialAsync(int id, CancellationToken ct = default)
        {
            await DeleteAsync($"/api/officials/{id}", ct);
        }

        // --- Administrative tasks (existing DTO preserved) ---
        public class AdministrativeTaskDto
        {
            public string Id { get; set; } = string.Empty;
            public string Title { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
            public string Category { get; set; } = string.Empty;
            public string Priority { get; set; } = string.Empty;
            public string Status { get; set; } = string.Empty;
            public string AssignedTo { get; set; } = string.Empty;
            public DateTime? DueDate { get; set; }
            public DateTime CreatedAt { get; set; }
            public DateTime UpdatedAt { get; set; }
        }

        public static async Task<List<AdministrativeTaskDto>> GetAdminTasksAsync(CancellationToken ct = default)
        {
            var res = await GetJsonAsync<List<AdministrativeTaskDto>>("/api/administrative-tasks", ct);
            return res ?? new List<AdministrativeTaskDto>();
        }

        public static async Task<AdministrativeTaskDto> CreateAdminTaskAsync(AdministrativeTaskDto dto, CancellationToken ct = default)
        {
            return await PostJsonAsync<AdministrativeTaskDto>("/api/administrative-tasks", dto, ct);
        }

        public static async Task UpdateAdminTaskAsync(string id, AdministrativeTaskDto dto, CancellationToken ct = default)
        {
            await PutJsonAsync($"/api/administrative-tasks/{id}", dto, ct);
        }

        public static async Task DeleteAdminTaskAsync(string id, CancellationToken ct = default)
        {
            await DeleteAsync($"/api/administrative-tasks/{id}", ct);
        }

        // --- AI Endpoints ---
        // Input / output DTOs
    public class TextInput { public string Text { get; set; } = string.Empty; }
    public class TranslateInput { public string Text { get; set; } = string.Empty; public string TargetLang { get; set; } = string.Empty; }
    public class FeedbackOutput { public string Sentiment { get; set; } = string.Empty; public string[] Topics { get; set; } = Array.Empty<string>(); }
    public class SuggestTaskInput { public string Title { get; set; } = string.Empty; public string Description { get; set; } = string.Empty; }
    public class SuggestTaskOutput { public string SuggestedPriority { get; set; } = string.Empty; public string SuggestedCategory { get; set; } = string.Empty; }

        // Summarize: returns the "summary" property if present, otherwise the raw response as fallback
        public static async Task<string> SummarizeAsync(string text, CancellationToken ct = default)
        {
            try
            {
                var resp = await PostJsonAsync<JsonElement>("/api/ai/summarize", new TextInput { Text = text }, ct);
                if (resp.ValueKind == JsonValueKind.Object && resp.TryGetProperty("summary", out var p) && p.ValueKind == JsonValueKind.String)
                    return p.GetString() ?? string.Empty;
                // fallback: try "result" or "output"
                if (resp.TryGetProperty("result", out var r) && r.ValueKind == JsonValueKind.String) return r.GetString() ?? string.Empty;
                if (resp.TryGetProperty("output", out var o) && o.ValueKind == JsonValueKind.String) return o.GetString() ?? string.Empty;
                return resp.ToString();
            }
            catch
            {
                // Do not throw for AI helper; surface empty string so UI can handle it
                return string.Empty;
            }
        }

        // Translate: returns the "translated" property or empty string on failure
        public static async Task<string> TranslateAsync(string text, string lang, CancellationToken ct = default)
        {
            try
            {
                var resp = await PostJsonAsync<JsonElement>("/api/ai/translate", new TranslateInput { Text = text, TargetLang = lang }, ct);
                if (resp.ValueKind == JsonValueKind.Object && resp.TryGetProperty("translated", out var p) && p.ValueKind == JsonValueKind.String)
                    return p.GetString() ?? string.Empty;
                // fallback checks
                if (resp.TryGetProperty("translation", out var t) && t.ValueKind == JsonValueKind.String) return t.GetString() ?? string.Empty;
                return resp.ToString();
            }
            catch
            {
                return string.Empty;
            }
        }

        // Analyze feedback: expects FeedbackOutput-shaped JSON
        public static async Task<FeedbackOutput> AnalyzeFeedbackAsync(string text, CancellationToken ct = default)
        {
            try
            {
                return await PostJsonAsync<FeedbackOutput>("/api/ai/analyze-feedback", new TextInput { Text = text }, ct);
            }
            catch
            {
                return new FeedbackOutput { Sentiment = string.Empty, Topics = Array.Empty<string>() };
            }
        }

        // Suggest task: expects SuggestTaskOutput-shaped JSON
        public static async Task<SuggestTaskOutput> SuggestTaskAsync(string title, string description, CancellationToken ct = default)
        {
            try
            {
                return await PostJsonAsync<SuggestTaskOutput>("/api/ai/suggest-task", new SuggestTaskInput { Title = title, Description = description }, ct);
            }
            catch
            {
                return new SuggestTaskOutput { SuggestedCategory = string.Empty, SuggestedPriority = string.Empty };
            }
        }
    }       
}
