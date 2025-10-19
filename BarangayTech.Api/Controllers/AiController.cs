using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace BarangayTech.Api.Controllers
{
    [ApiController]
    [Route("api/ai")]
    public class AiController : ControllerBase
    {
        public record TextInput(string Text);
        public record TranslateInput(string Text, string TargetLang);
        public record FeedbackOutput(string Sentiment, string[] Topics);
        public record SuggestTaskInput(string Title, string? Description);
        public record SuggestTaskOutput(string SuggestedPriority, string SuggestedCategory);

        [HttpPost("summarize")]
        public ActionResult<object> Summarize([FromBody] TextInput input)
        {
            if (string.IsNullOrWhiteSpace(input.Text)) return BadRequest("text required");
            // Simple extractive summary: first 1-2 sentences
            var sentences = Regex.Split(input.Text.Trim(), @"(?<=[.!?])\s+");
            var take = Math.Min(2, sentences.Length);
            var summary = string.Join(" ", sentences.Take(take));
            return Ok(new { summary });
        }

        [HttpPost("translate")]
        public ActionResult<object> Translate([FromBody] TranslateInput input)
        {
            if (string.IsNullOrWhiteSpace(input.Text) || string.IsNullOrWhiteSpace(input.TargetLang))
                return BadRequest("text and targetLang required");
            // Stub translation: echo text with a note (replace with real provider later)
            var translated = $"[Translated to {input.TargetLang}] {input.Text}";
            return Ok(new { translated });
        }

        [HttpPost("analyze-feedback")]
        public ActionResult<FeedbackOutput> AnalyzeFeedback([FromBody] TextInput input)
        {
            if (string.IsNullOrWhiteSpace(input.Text)) return BadRequest("text required");
            var text = input.Text.ToLowerInvariant();
            // Very basic sentiment
            int score = 0;
            var pos = new[] { "salamat", "thank", "ayos", "galing", "maganda", "helpful", "great", "love" };
            var neg = new[] { "reklamo", "bad", "delay", "bagal", "mabagal", "hassle", "hate", "poor" };
            foreach (var w in pos) if (text.Contains(w)) score++;
            foreach (var w in neg) if (text.Contains(w)) score--;
            var sentiment = score > 0 ? "positive" : score < 0 ? "negative" : "neutral";

            // Topic keywords
            var topics = new List<string>();
            if (text.Contains("permit") || text.Contains("business")) topics.Add("permit");
            if (text.Contains("health") || text.Contains("clinic") || text.Contains("medical")) topics.Add("health");
            if (text.Contains("garbage") || text.Contains("waste") || text.Contains("collection")) topics.Add("sanitation");
            if (text.Contains("police") || text.Contains("security") || text.Contains("safety")) topics.Add("safety");
            if (text.Contains("event") || text.Contains("announcement") || text.Contains("news")) topics.Add("communication");
            if (!topics.Any()) topics.Add("general");

            return Ok(new FeedbackOutput(sentiment, topics.ToArray()));
        }

        [HttpPost("suggest-task")]
        public ActionResult<SuggestTaskOutput> SuggestTask([FromBody] SuggestTaskInput input)
        {
            if (string.IsNullOrWhiteSpace(input.Title) && string.IsNullOrWhiteSpace(input.Description))
                return BadRequest("title or description required");
            var text = $"{input.Title} {input.Description}".ToLowerInvariant();
            string category = "General";
            if (text.Contains("permit")) category = "Permit";
            else if (text.Contains("announcement")) category = "Announcement";
            else if (text.Contains("event")) category = "Event";
            else if (text.Contains("health")) category = "Health";
            else if (text.Contains("incident") || text.Contains("complaint")) category = "Incident";

            string priority = "Medium";
            if (text.Contains("urgent") || text.Contains("asap") || text.Contains("today")) priority = "Urgent";
            else if (text.Contains("high") || text.Contains("deadline")) priority = "High";
            else if (text.Contains("low")) priority = "Low";

            return Ok(new SuggestTaskOutput(priority, category));
        }
    }
}
