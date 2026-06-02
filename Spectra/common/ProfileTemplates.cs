using System.Collections.Generic;

namespace Spectra.common
{
    public sealed class ProfileTemplate
    {
        public string Name        { get; }
        public string Description { get; }
        public int    Percent     { get; }

        public ProfileTemplate(string name, string description, int percent)
        {
            Name        = name;
            Description = description;
            Percent     = percent;
        }

        public int ToLevel(int minLevel, int maxLevel)
            => minLevel + (int)((maxLevel - minLevel) * Percent / 100.0 + 0.5);
    }

    public static class ProfileTemplates
    {
        public static readonly IReadOnlyList<ProfileTemplate> All = new[]
        {
            new ProfileTemplate("Competitive FPS",      "Maximum vibrance for competitive shooters",  100),
            new ProfileTemplate("Casual FPS",           "High vibrance, still looks natural",          70),
            new ProfileTemplate("Strategy / MOBA",      "Moderate vibrance for strategy games",        55),
            new ProfileTemplate("RPG / Adventure",      "Slightly enhanced for story-driven games",    40),
            new ProfileTemplate("Movie / Video",         "Near-neutral for accurate color playback",    15),
            new ProfileTemplate("Office / Productivity", "Default display — no extra vibrance",         0),
        };
    }
}
