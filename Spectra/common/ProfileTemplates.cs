using System.Collections.Generic;
using Spectra.Localization;

namespace Spectra.common
{
    public sealed class ProfileTemplate
    {
        public string LocalizationKey { get; }
        public string FallbackName    { get; }
        public string Description     { get; }
        public int    Percent         { get; }

        public ProfileTemplate(string locKey, string fallbackName, string description, int percent)
        {
            LocalizationKey = locKey;
            FallbackName    = fallbackName;
            Description     = description;
            Percent         = percent;
        }

        public string GetLocalizedName()
        {
            var n = LocalizationManager.Get(LocalizationKey);
            return string.IsNullOrEmpty(n) || n == LocalizationKey ? FallbackName : n;
        }

        public int ToLevel(int minLevel, int maxLevel)
            => minLevel + (int)((maxLevel - minLevel) * Percent / 100.0 + 0.5);
    }

    public static class ProfileTemplates
    {
        public static readonly IReadOnlyList<ProfileTemplate> All = new[]
        {
            new ProfileTemplate("TplCompetitiveFps", "Competitive FPS",      "Maximum vibrance for competitive shooters",  100),
            new ProfileTemplate("TplCasualFps",      "Casual FPS",           "High vibrance, still looks natural",          70),
            new ProfileTemplate("TplStrategy",       "Strategy / MOBA",      "Moderate vibrance for strategy games",        55),
            new ProfileTemplate("TplRpg",            "RPG / Adventure",      "Slightly enhanced for story-driven games",    40),
            new ProfileTemplate("TplMovie",          "Movie / Video",         "Near-neutral for accurate color playback",    15),
            new ProfileTemplate("TplOffice",         "Office / Productivity", "Default display — no extra vibrance",         0),
        };
    }
}
