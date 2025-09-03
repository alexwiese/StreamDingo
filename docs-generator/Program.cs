using Statiq.App;
using Statiq.Docs;

return await Bootstrapper
    .Factory
    .CreateDocs(args)
    .AddSetting("Theme", "Samson")
    .RunAsync();
